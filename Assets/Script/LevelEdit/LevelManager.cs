using System.Collections.Generic;
using System.IO;
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
    [SerializeField] public List <Tilemap> genmap = new List<Tilemap>();
    [SerializeField] public DungeonRoomSO dungeonRoomSO;
    [SerializeField] public int index;
    [SerializeField] public int amount;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }
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
                    if (temp != null)
                    {
                        levelData.tiles.Add(temp);
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

    public void LoadLevel()
    {
        string json = File.ReadAllText(Application.dataPath + dungeonRoomSO.room[index].filePath);
        LevelData data = JsonUtility.FromJson<LevelData>(json);

        foreach (Tilemap tm in tilemap) tm.ClearAllTiles();
        foreach (Tilemap gm in genmap)  gm.ClearAllTiles();

        bool hasLayerData = data.layerIndices != null && data.layerIndices.Count == data.poses.Count;

        for (int i = 0; i < data.poses.Count; i++)
        {
            int layerIdx = hasLayerData ? data.layerIndices[i] : 0;
            if (layerIdx < 0 || layerIdx >= genmap.Count) layerIdx = 0;
            genmap[layerIdx].SetTile(data.poses[i], data.tiles[i]);
        }
    }
}

[System.Serializable]
public class LevelData
{
    public List<TileBase> tiles      = new List<TileBase>();
    public List<Vector3Int> poses    = new List<Vector3Int>();
    public List<int> layerIndices    = new List<int>();
}