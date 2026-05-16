using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelManager : MonoBehaviour
{
    [SerializeField] public int index;
    public static LevelManager Instance;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }

    public List<Tilemap> tilemap;
    public List <Tilemap> genmap = new List<Tilemap>();
    public DungeonRoomSO dungeonRoomSO;
    public TileBase[] tilepalette; // kéo tất cả tile vào đây trong Inspector

    public void SaveLevel()
    {
        LevelData levelData = new LevelData();
        foreach (Tilemap tm in tilemap)
        {
            BoundsInt bounds = tm.cellBounds;

            for (int x = bounds.min.x; x < bounds.max.x; x++)
            {
                for (int y = bounds.min.y; y < bounds.max.y; y++)
                {
                    TileBase temp = tm.GetTile(new Vector3Int(x, y, 0));
                    if (temp != null)
                    {
                        levelData.tiles.Add(temp); // ✅ lưu string
                        levelData.poses.Add(new Vector3Int(x, y, 0));
                    }
                }
            }
        }
        string roomName = $"room_{dungeonRoomSO.room.Count}";
        string path = $"/Data/Json/Room/{roomName}.json"; // ✅ $ prefix

        string json = JsonUtility.ToJson(levelData, true);
        File.WriteAllText(Application.dataPath + path, json);

        dungeonRoomSO.room.Add(new RoomFile { roomName = roomName, filePath = path });

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(dungeonRoomSO);
        UnityEditor.AssetDatabase.SaveAssets();
#endif

        Debug.Log($"Saved: {roomName}");

    }

    public void LoadLevel()
    {
        string json = File.ReadAllText(Application.dataPath + dungeonRoomSO.room[index].filePath);
        LevelData data = JsonUtility.FromJson<LevelData>(json);
        foreach (Tilemap tm in tilemap)
        {
            tm.ClearAllTiles();
        }
        genmap[0].ClearAllTiles();
        for (int i = 0; i < data.poses.Count; i++)
        {
            genmap[0].SetTile(data.poses[i], data.tiles[i]);
        }
    }
}

[System.Serializable]
public class LevelData
{
    public List<TileBase> tiles = new List<TileBase>(); // ✅ string
    public List<Vector3Int> poses = new List<Vector3Int>();
}