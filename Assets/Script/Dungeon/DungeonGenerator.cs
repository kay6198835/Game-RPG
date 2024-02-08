using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    public DungeonGeneration dunData;

    private List<Vector2Int> dunRooms;

    private void Start()
    {
        dunRooms = DungeonCrawlerController.GenerateDungeon(dunData);
        SpawnRooms(dunRooms);
    }

    private void SpawnRooms(IEnumerable<Vector2Int> rooms)
    {
        RoomController.instance.LoadRoom("Start", 0, 0);

        foreach (Vector2Int roomLocation in rooms)
        {
            RoomController.instance.LoadRoom(RoomController.instance.GetRandomRoomName(), roomLocation.x, roomLocation.y);
        }
    }
}
