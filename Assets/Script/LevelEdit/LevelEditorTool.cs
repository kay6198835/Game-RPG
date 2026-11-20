using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;

// Room CRUD tool for the Editor: Create (Save), Read (Load), Update (Edit), Delete, plus Import of
// existing JSON files. Tiles and tilemaps come from LevelManager; the room library is chosen here.
// No Unity lifecycle method and nothing in the runtime map flow references it. Sits on an EditorOnly object.
public class LevelEditorTool : MonoBehaviour
{
    private const string RoomFolder = "/Data/Json/Room";

    [SerializeField] private LevelManager _levelManager;
    [SerializeField] private DungeonRoomSO dungeonRoomSO;

    [Header("Create")]
    [SerializeField] private RoomType roomType = RoomType.NormalRoom;
    [SerializeField] private string customRoomName = "";

    [Header("Load / Edit / Delete")]
    [SerializeField] private int index;

    public void SaveLevel()
    {
        if (!HasSetup()) return;

        string roomName = string.IsNullOrWhiteSpace(customRoomName)
            ? $"{roomType}_{dungeonRoomSO.room.Count}"
            : customRoomName.Trim();
        string path = $"{RoomFolder}/{roomName}.json";
        int existing = dungeonRoomSO.room.FindIndex(r => r.filePath == path);

        if ((existing >= 0 || File.Exists(Application.dataPath + path))
            && !Confirm("Overwrite room", $"{roomName} already exists. Overwrite it?"))
            return;

        WriteRoom(path, CollectLevelData());
        if (existing >= 0)
            dungeonRoomSO.room[existing].roomType = roomType;
        else
            dungeonRoomSO.room.Add(new RoomFile { roomName = roomName, filePath = path, roomType = roomType });

        SaveLibrary();
        Debug.Log($"[{nameof(LevelEditorTool)}] Saved {roomName}");
    }

    public void LoadLevel()
    {
        if (!HasSetup() || !HasRoomAt(index)) return;

        LevelData data = JsonUtility.FromJson<LevelData>(File.ReadAllText(Application.dataPath + dungeonRoomSO.room[index].filePath));
        List<Tilemap> tilemaps = _levelManager.GetTilemaps();
        List<TileSO> tiles = _levelManager.GetTileSOs();

        foreach (Tilemap tm in tilemaps) tm.ClearAllTiles();

        bool hasLayerData = data.layerIndices != null && data.layerIndices.Count == data.poses.Count;
        for (int i = 0; i < data.poses.Count; i++)
        {
            int layerIdx = hasLayerData ? data.layerIndices[i] : 0;
            if (layerIdx < 0 || layerIdx >= tilemaps.Count) layerIdx = 0;

            TileSO tile = tiles.Find(t => t.name == data.tiles[i]);
            if (tile == null)
            {
                Debug.LogWarning($"[{nameof(LevelEditorTool)}] Tile '{data.tiles[i]}' is not in the LevelManager tile list; skipped.");
                continue;
            }
            tilemaps[layerIdx].SetTile(data.poses[i], tile.tile);
        }
    }

    public void UpdateRoom()
    {
        if (!HasSetup() || !HasRoomAt(index)) return;

        WriteRoom(dungeonRoomSO.room[index].filePath, CollectLevelData());
        Debug.Log($"[{nameof(LevelEditorTool)}] Updated {dungeonRoomSO.room[index].roomName}");
    }

    public void DeleteRoom()
    {
        if (!HasSetup() || !HasRoomAt(index)) return;

        RoomFile room = dungeonRoomSO.room[index];
        string warning = index == 0 || index == dungeonRoomSO.room.Count - 1
            ? "\n\nThis is the first/last room of the library, which the game uses as the start/end room."
            : "";
        if (!Confirm("Delete room", $"Delete {room.roomName} from {dungeonRoomSO.name} and delete {room.filePath}?{warning}"))
            return;

        dungeonRoomSO.room.RemoveAt(index);
#if UNITY_EDITOR
        UnityEditor.AssetDatabase.DeleteAsset("Assets" + room.filePath);
#endif
        SaveLibrary();
        Debug.Log($"[{nameof(LevelEditorTool)}] Deleted {room.roomName}");
    }

    public void ImportRoomJsonFiles()
    {
        if (dungeonRoomSO == null)
        {
            Debug.LogError($"[{nameof(LevelEditorTool)}] Assign a DungeonRoomSO first.");
            return;
        }

        string roomFolder = Application.dataPath + RoomFolder;
        if (!Directory.Exists(roomFolder))
        {
            Debug.LogWarning($"[{nameof(LevelEditorTool)}] Import failed: folder not found {roomFolder}");
            return;
        }

        int importedCount = 0;
        foreach (string file in Directory.GetFiles(roomFolder, "*.json", SearchOption.TopDirectoryOnly))
        {
            string relativePath = file.Substring(Application.dataPath.Length).Replace("\\", "/");
            if (!relativePath.StartsWith("/")) relativePath = "/" + relativePath;
            if (dungeonRoomSO.room.Exists(r => r.filePath == relativePath)) continue;

            string roomName = Path.GetFileNameWithoutExtension(file);
            dungeonRoomSO.room.Add(new RoomFile
            {
                roomName = roomName,
                filePath = relativePath,
                roomType = InferRoomTypeFromName(roomName)
            });
            importedCount++;
        }

        if (importedCount > 0) SaveLibrary();
        Debug.Log($"[{nameof(LevelEditorTool)}] Imported {importedCount} room json files into {dungeonRoomSO.name}");
    }

    private LevelData CollectLevelData()
    {
        LevelData levelData = new LevelData();
        List<Tilemap> tilemaps = _levelManager.GetTilemaps();
        List<TileSO> tiles = _levelManager.GetTileSOs();

        for (int tmIndex = 0; tmIndex < tilemaps.Count; tmIndex++)
        {
            Tilemap tm = tilemaps[tmIndex];
            BoundsInt bounds = tm.cellBounds;
            for (int x = bounds.min.x; x < bounds.max.x; x++)
            {
                for (int y = bounds.min.y; y < bounds.max.y; y++)
                {
                    TileBase temp = tm.GetTile(new Vector3Int(x, y, 0));
                    if (temp == null) continue;

                    TileSO tile = tiles.Find(t => t.tile == temp);
                    if (tile == null)
                    {
                        Debug.LogWarning($"[{nameof(LevelEditorTool)}] Tile at ({x}, {y}) in layer {tmIndex} is not in the LevelManager tile list; not saved.");
                        continue;
                    }
                    levelData.tiles.Add(tile.id);
                    levelData.poses.Add(new Vector3Int(x, y, 0));
                    levelData.layerIndices.Add(tmIndex);
                }
            }
        }
        return levelData;
    }

    private static void WriteRoom(string path, LevelData data)
    {
        File.WriteAllText(Application.dataPath + path, JsonUtility.ToJson(data, true));
#if UNITY_EDITOR
        UnityEditor.AssetDatabase.ImportAsset("Assets" + path);
#endif
    }

    private static RoomType InferRoomTypeFromName(string roomName)
    {
        foreach (RoomType type in System.Enum.GetValues(typeof(RoomType)))
        {
            if (roomName.StartsWith(type.ToString())) return type;
        }
        return RoomType.NormalRoom;
    }

    private bool HasSetup()
    {
        if (_levelManager == null) _levelManager = GetComponentInParent<LevelManager>();
        if (_levelManager != null && dungeonRoomSO != null) return true;
        Debug.LogError($"[{nameof(LevelEditorTool)}] Needs a LevelManager and a DungeonRoomSO.");
        return false;
    }

    private bool HasRoomAt(int roomIndex)
    {
        if (roomIndex >= 0 && roomIndex < dungeonRoomSO.room.Count) return true;
        Debug.LogError($"[{nameof(LevelEditorTool)}] Index {roomIndex} is outside {dungeonRoomSO.name} (0..{dungeonRoomSO.room.Count - 1}).");
        return false;
    }

    private void SaveLibrary()
    {
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(dungeonRoomSO);
        UnityEditor.AssetDatabase.SaveAssets();
#endif
    }

    private static bool Confirm(string title, string message)
    {
#if UNITY_EDITOR
        return UnityEditor.EditorUtility.DisplayDialog(title, message, "Yes", "Cancel");
#else
        return false;
#endif
    }
}
