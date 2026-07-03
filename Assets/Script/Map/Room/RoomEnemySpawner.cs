using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RoomGridController))]
[RequireComponent(typeof(RoomGeneraterController))]
public class RoomEnemySpawner : MonoBehaviour
{
    [SerializeField] private RoomGridController roomGridController;
    [SerializeField] private RoomGeneraterController roomGeneraterController;
    [SerializeField] private List<EncounterSO> encounters = new List<EncounterSO>();

    private readonly RoomEncounterTracker tracker = new RoomEncounterTracker();
    private RoomCell currentRoom;

    private void Awake()
    {
        roomGridController = GetComponent<RoomGridController>();
        roomGeneraterController = GetComponent<RoomGeneraterController>();
    }

    private void OnEnable()
    {
        EventManager.Resgister(EventID.ON_LOAD_MAP, OnRoomLoaded);
        EventManager.Resgister(EventID.ON_ENEMY_DEATH, OnEnemyDeath);
    }

    private void OnDisable()
    {
        EventManager.UnResgister(EventID.ON_LOAD_MAP, OnRoomLoaded);
        EventManager.UnResgister(EventID.ON_ENEMY_DEATH, OnEnemyDeath);
    }

    private void OnRoomLoaded(object obj)
    {
        tracker.Reset();
        currentRoom = roomGridController.Current;
        if (currentRoom == null || currentRoom.IsCleared) return;

        int roomIndex = (int)obj;
        EncounterSO encounter = FindEncounter(roomGeneraterController.GetRoomFile(roomIndex).roomType);
        List<Vector3> points = roomGeneraterController.SpawnPoints;
        if (encounter == null || points.Count == 0)
        {
            ClearCurrentRoom(false);
            return;
        }

        currentRoom.CloseDoor();
        int spawned = SpawnEnemies(encounter, points);
        if (spawned == 0)
        {
            ClearCurrentRoom(false);
            return;
        }
        tracker.StartEncounter(spawned);
    }

    private void OnEnemyDeath(object obj)
    {
        if (tracker.OnEnemyDeath())
        {
            ClearCurrentRoom(true);
        }
    }

    private void ClearCurrentRoom(bool isCombatClear)
    {
        currentRoom.MarkCleared();
        EventManager.Emit(EventID.ON_CLEAR_ENEMY);
        if (isCombatClear)
        {
            EventManager.Emit(EventID.ON_ROOM_CLEAR);
        }
    }

    private EncounterSO FindEncounter(RoomType roomType)
    {
        foreach (EncounterSO encounter in encounters)
        {
            if (encounter != null && encounter.RoomType == roomType) return encounter;
        }
        return null;
    }

    private int SpawnEnemies(EncounterSO encounter, List<Vector3> points)
    {
        int spawned = 0;
        int pointOffset = Random.Range(0, points.Count);
        foreach (EncounterEntry entry in encounter.Entries)
        {
            if (entry.prefab == null) continue;
            for (int i = 0; i < entry.count; i++)
            {
                Vector3 point = points[(pointOffset + spawned) % points.Count];
                Instantiate(entry.prefab, point, Quaternion.identity);
                spawned++;
            }
        }
        return spawned;
    }
}
