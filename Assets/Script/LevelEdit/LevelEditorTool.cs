using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;

// Room authoring tool (Save / Load / Edit / Import room JSON, test spawn). Editor-only by intent:
// it has no Unity lifecycle method and nothing in the runtime map flow references it — the in-game
// room generation reads LevelManager and RoomGeneraterController only. Put it on an EditorOnly object.
public class LevelEditorTool : MonoBehaviour
{
    private static readonly Dictionary<RoomType, string> RoomTypeNames = new Dictionary<RoomType, string>
    {
        { RoomType.NormalRoom,   "NormalRoom"   },
        { RoomType.StartRoom,    "StartRoom"    },
        { RoomType.BossRoom,     "BossRoom"     },
        { RoomType.CombatRoom,   "CombatRoom"   },
        { RoomType.TreasureRoom, "TreasureRoom" },
        { RoomType.ShopRoom,     "ShopRoom"     },
        { RoomType.RestRoom,     "RestRoom"     },
        { RoomType.PuzzleRoom,   "PuzzleRoom"   },
        { RoomType.SecretRoom,   "SecretRoom"   },
        { RoomType.ExitRoom,     "ExitRoom"     },
    };

    [Header("Room Library")]
    [SerializeField] private DungeonRoomSO dungeonRoomSO;
    [SerializeField] private List<TileSO> _listTiles;

    [Header("Save Map")]
    [SerializeField] public List<Tilemap> tilemap;
    [SerializeField] public RoomType roomType = RoomType.NormalRoom;
    public string customRoomName = "";

    [Header("Load Map")]
    [SerializeField] private List<Tilemap> genmap = new List<Tilemap>();
    [SerializeField] private int index;

    [Header("Test Spawn")]
    [SerializeField] private RoomModel roomModel;
    [SerializeField, Range(0f, 10f)] private float spawnRadius = 10f;

    public void SaveLevel()
    {
        LevelData levelData = CollectLevelData(false);
        string typeName = RoomTypeNames[roomType];
        string roomName = string.IsNullOrWhiteSpace(customRoomName)
            ? $"{typeName}_{dungeonRoomSO.room.Count}"
            : customRoomName;
        string path = $"/Data/Json/Room/{roomName}.json";

        string json = JsonUtility.ToJson(levelData, true);
        File.WriteAllText(Application.dataPath + path, json);

        dungeonRoomSO.room.Add(new RoomFile { roomName = roomName, filePath = path, roomType = roomType });

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(dungeonRoomSO);
        UnityEditor.AssetDatabase.SaveAssets();
#endif
        Debug.Log($"Saved: {roomName}");
    }

    public void ImportRoomJsonFiles()
    {
#if UNITY_EDITOR
        string roomFolder = Application.dataPath + "/Data/Json/Room";
        if (!Directory.Exists(roomFolder))
        {
            Debug.LogWarning($"Import failed: folder not found {roomFolder}");
            return;
        }

        string[] jsonFiles = Directory.GetFiles(roomFolder, "*.json", SearchOption.TopDirectoryOnly);
        int importedCount = 0;

        foreach (string file in jsonFiles)
        {
            string relativePath = file.Substring(Application.dataPath.Length).Replace("\\", "/");
            if (!relativePath.StartsWith("/"))
            {
                relativePath = "/" + relativePath;
            }

            if (dungeonRoomSO.room.Exists(r => r.filePath == relativePath))
            {
                continue;
            }

            string roomName = System.IO.Path.GetFileNameWithoutExtension(file);
            dungeonRoomSO.room.Add(new RoomFile
            {
                roomName = roomName,
                filePath = relativePath,
                roomType = InferRoomTypeFromName(roomName)
            });

            importedCount++;
        }

        if (importedCount > 0)
        {
            UnityEditor.EditorUtility.SetDirty(dungeonRoomSO);
            UnityEditor.AssetDatabase.SaveAssets();
        }

        Debug.Log($"Imported {importedCount} room json files into {dungeonRoomSO.name}");
#else
        Debug.LogWarning("ImportRoomJsonFiles is only supported in the Unity Editor.");
#endif
    }

    private RoomType InferRoomTypeFromName(string roomName)
    {
        foreach (RoomType type in System.Enum.GetValues(typeof(RoomType)))
        {
            if (roomName.StartsWith(type.ToString()))
            {
                return type;
            }
        }

        return RoomType.NormalRoom;
    }

    public void LoadLevel()
    {
        string json = File.ReadAllText(Application.dataPath + dungeonRoomSO.room[this.index].filePath);
        LevelData data = JsonUtility.FromJson<LevelData>(json);

        foreach (Tilemap tm in tilemap) tm.ClearAllTiles();
        foreach (Tilemap gm in genmap) gm.ClearAllTiles();

        bool hasLayerData = data.layerIndices != null && data.layerIndices.Count == data.poses.Count;

        for (int i = 0; i < data.poses.Count; i++)
        {
            int layerIdx = hasLayerData ? data.layerIndices[i] : 0;
            if (layerIdx < 0 || layerIdx >= genmap.Count) layerIdx = 0;
            genmap[layerIdx].SetTile(data.poses[i], _listTiles.Find(t => t.name == data.tiles[i]).tile);
        }
    }

    public void UpdateRoom()
    {
        if (dungeonRoomSO == null || index < 0 || index >= dungeonRoomSO.room.Count)
        {
            return;
        }

        LevelData levelData = CollectLevelData(true);
        string filePath = dungeonRoomSO.room[index].filePath;
        string json = JsonUtility.ToJson(levelData, true);
        File.WriteAllText(Application.dataPath + filePath, json);
    }

    private LevelData CollectLevelData(bool warnUnknownTiles)
    {
        LevelData levelData = new LevelData();
        for (int tmIndex = 0; tmIndex < tilemap.Count; tmIndex++)
        {
            Tilemap tm = tilemap[tmIndex];
            BoundsInt bounds = tm.cellBounds;

            for (int x = bounds.min.x; x < bounds.max.x; x++)
            {
                for (int y = bounds.min.y; y < bounds.max.y; y++)
                {
                    TileBase temp = tm.GetTile(new Vector3Int(x, y, 0));

                    TileSO tempTile = _listTiles.Find(t => t.tile == temp);
                    if (tempTile != null)
                    {
                        levelData.tiles.Add(tempTile.id);
                        levelData.poses.Add(new Vector3Int(x, y, 0));
                        levelData.layerIndices.Add(tmIndex);
                    }
                    else if (warnUnknownTiles)
                    {
                        Debug.LogWarning($"UpdateRoom: tile at ({x}, {y}) in layer {tmIndex} not found in _listTiles");
                    }
                }
            }
        }
        return levelData;
    }

    // Instantiates (not pooled) the RoomModel's spawn set around this object, for checking a set by eye.
    public void SpawnRoomEnemies()
    {
        if (roomModel == null) return;
        List<EnemySpawnEntry> set = roomModel.GetSpawnSet();

        foreach (var entry in set)
        {
            if (entry.enemy == null || entry.enemy.Prefab == null) continue;
            for (int i = 0; i < entry.count; i++)
            {
                Vector2 offset = Random.insideUnitCircle * spawnRadius;
                Vector3 pos = transform.position + new Vector3(offset.x, offset.y, 0f);
                Instantiate(entry.enemy.Prefab, pos, Quaternion.identity, transform.parent);
            }
        }
    }
}
