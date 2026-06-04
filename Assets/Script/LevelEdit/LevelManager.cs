using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

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

    [Header("Save Map")]
    [SerializeField] public List<Tilemap> tilemap;
    [SerializeField] public RoomType roomType = RoomType.NormalRoom;
    public string customRoomName = "";
    [Header("Load Map")]
    [SerializeField] private List<Tilemap> genmap = new List<Tilemap>();
    [SerializeField] private DungeonRoomSO dungeonRoomSO;
    [SerializeField] private int index;
    [SerializeField] private int amount;
    [SerializeField] private List<RoomFile> listRooms;
    [SerializeField] private List<TileSO> _listTiles;
    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        listRooms = GetRandomRooms();
    }
    #region Editor Funtion
    public void SaveLevel()
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
                }
            }
        }
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

    [ContextMenu("Import Room Json Files")]
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

            string roomName = Path.GetFileNameWithoutExtension(file);
            RoomType roomType = InferRoomTypeFromName(roomName);

            dungeonRoomSO.room.Add(new RoomFile
            {
                roomName = roomName,
                filePath = relativePath,
                roomType = roomType
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

#if UNITY_EDITOR
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
#endif

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
    #endregion

    #region public Funtion
    public List<RoomFile> GetRandomRooms()
    {
        List<RoomFile> shuffled = new List<RoomFile>(dungeonRoomSO.room);
        for (int i = shuffled.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            RoomFile temp = shuffled[i];
            shuffled[i] = shuffled[j];
            shuffled[j] = temp;
        }
        int count = Mathf.Min(amount, shuffled.Count);
        return shuffled.GetRange(0, count);
    }


    public void LoadRoom(int index, Vector3 positionLoadMap)
    {
        string filePath = "";
        index = index == null ? 0 : index;
        if (index == 0)
        {
            filePath = dungeonRoomSO.room[index].filePath;
        }
        else
        {
            filePath = listRooms[index].filePath;
        }
        string json = File.ReadAllText(Application.dataPath + filePath);
        LevelData data = JsonUtility.FromJson<LevelData>(json);

        foreach (Tilemap gm in genmap) gm.ClearAllTiles();

        bool hasLayerData = data.layerIndices != null && data.layerIndices.Count == data.poses.Count;

        for (int i = 0; i < data.poses.Count; i++)
        {
            int layerIdx = hasLayerData ? data.layerIndices[i] : 0;
            if (layerIdx < 0 || layerIdx >= genmap.Count) layerIdx = 0;

            var tilemap = data.tiles[i];
            //Refactor late
            if (tilemap == GameConstants.TileName.DOOR)
            {
                // get direction
                Utility.ToCardinalDirection((Vector3)data.poses[i]);
                // check include

                // true swap tile

                // false save tile data in lobal class
            }


            genmap[layerIdx].SetTile(data.poses[i], _listTiles.Find(t => t.name == data.tiles[i]).tile);
        }

        this.SetPosition(positionLoadMap);
    }

    private void SetPosition(Vector3 positionLoadMap)
    {
        this.transform.SetPositionAndRotation(positionLoadMap, Quaternion.identity);
    }

    /// <summary>
    /// Wrapper: seal tất cả Tile_Door có hướng không nằm trong <paramref name="directions"/>.
    /// Logic xử lý thực tế nằm ở <see cref="Utility.SealUnusedDoors"/>.
    /// </summary>
    public void SealUnusedDoors(Vector2[] directions)
    {
        TileBase roomTile = _listTiles.Find(t => t.name == GameConstants.TileName.ROOM)?.tile;
        TileBase doorTile = _listTiles.Find(t => t.name == GameConstants.TileName.DOOR)?.tile;
        if (roomTile == null || doorTile == null) return;

        Utility.SealUnusedDoors(directions, genmap, roomTile, doorTile);
    }

    /// <summary>
    /// Wrapper: trả về vị trí world của cụm Tile_Door nằm trên cạnh wall theo <paramref name="direction"/>.
    /// Logic xử lý thực tế nằm ở <see cref="Utility.GetDoorWorldPosition"/>.
    /// </summary>
    public Vector2 GetDoorWorldPosition(Vector2 direction)
    {
        TileBase roomTile = _listTiles.Find(t => t.name == GameConstants.TileName.ROOM)?.tile;
        TileBase doorTile = _listTiles.Find(t => t.name == GameConstants.TileName.DOOR)?.tile;
        if (roomTile == null || doorTile == null) return Vector2.zero;

        return Utility.GetDoorWorldPosition(direction, genmap, roomTile, doorTile);
    }
    #endregion

    public List<TileSO> GetTileSOs() => _listTiles;
    public List<Tilemap> GetTilemaps() => tilemap;
    public DungeonRoomSO GetDungeonRoomSO() => dungeonRoomSO;
}

[System.Serializable]
public class LevelData
{
    //Do not change parameter name or spell
    public List<string> tiles = new List<string>();
    public List<Vector3Int> poses = new List<Vector3Int>();
    public List<int> layerIndices = new List<int>();
    public LevelData() { }
    public LevelData(LevelData other)
    {
        this.tiles = new List<string>(other.tiles);
        this.poses = new List<Vector3Int>(other.poses);
        this.layerIndices = new List<int>(other.layerIndices);
    }

    public void CopyData(LevelData other)
    {
        this.tiles = new List<string>(other.tiles);
        this.poses = new List<Vector3Int>(other.poses);
        this.layerIndices = new List<int>(other.layerIndices);
    }

    public void Clear()
    {
        tiles.Clear();
        poses.Clear();
        layerIndices.Clear();
    }
}