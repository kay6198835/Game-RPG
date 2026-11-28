// TileSO.cs - chỉ lưu thông tin tile
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
// DungeonRoomSO.cs - chỉ lưu danh sách room
[CreateAssetMenu(fileName = "DungeonRoomPrefab", menuName = "Data/Dungeon Room")]
public class DungeonRoomSO : ScriptableObject
{
    public List<RoomFile> room = new List<RoomFile>();
}

[System.Serializable]
public class RoomFile
{
    public string roomName;
    // Path under Assets/ (e.g. /Data/Json/Room/X.json). Editor tooling only (LevelEditorTool, QA, tests):
    // Assets/ does not exist in a Player build, so runtime code must never read this path.
    public string filePath;
    public RoomType roomType;
    // The room JSON as a serialized asset reference. This is what the runtime loads, and the reference is
    // what pulls the file into the Player build. LevelEditorTool fills it on Save / Import.
    public TextAsset roomJson;
}

public enum RoomType
{
    NormalRoom,
    StartRoom,
    BossRoom,
    CombatRoom,
    TreasureRoom,
    ShopRoom,
    RestRoom,
    PuzzleRoom,
    SecretRoom,
    ExitRoom
}