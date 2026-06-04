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
    public string filePath;
    public RoomType roomType;
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