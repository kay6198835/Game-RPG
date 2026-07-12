using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private List<Vector2Int> spawnPosition;
    [SerializeField] private MapModel mapModel;
    [SerializeField] private RoomModel roomModel;
    [SerializeField] private float paddingPosition;
    [SerializeField] private float maxPadding = 5f;
    [SerializeField] private Vector2 positionRandom;
    public void OnEnable()
    {
        EventManager.Resgister(EventID.ON_GET_SPAWN_POSITIONS, OnDoneLoadRoomGrid);
    }
    public void OnDisable()
    {
        EventManager.UnResgister(EventID.ON_GET_SPAWN_POSITIONS, OnDoneLoadRoomGrid);
    }
    public void Spawn()
    {

    }

    public void OnDoneLoadRoomGrid(object obj = null)
    {
        spawnPosition = (List<Vector2Int>)obj;
        roomModel = mapModel.GetRandomRoom();
        SpawnRoomEnemies();
    }
    private List<EnemySpawnEntry> GetRoomSpawnSet()
    {
        if (roomModel == null)
        {
            Debug.LogWarning("GetRoomSpawnSet: roomModel is null");
            return new List<EnemySpawnEntry>();
        }
        return roomModel.GetSpawnSet();
    }

    /// <summary>
    /// Spawn quái cho phòng: Instantiate prefab từ EnemySpawnEntry tại vị trí ngẫu nhiên
    /// quanh object (transform), bán kính ≤ spawnRadius. Gọi SAU LoadRoom (SetPosition đã xong).
    /// </summary>
    public void SpawnRoomEnemies()
    {
        List<EnemySpawnEntry> set = GetRoomSpawnSet();
        if (set.Count == 0)
        {
            Debug.LogWarning("SpawnRoomEnemies: nothing to spawn");
            return;
        }

        foreach (var entry in set)
        {
            if (entry.enemy == null || entry.enemy.prefab == null) continue;
            for (int i = 0; i < entry.count; i++)
            {
                positionRandom = spawnPosition[Random.Range(0, spawnPosition.Count)].ConvertTo<Vector2>();
                paddingPosition = Random.Range(-maxPadding, maxPadding);
                positionRandom.y += paddingPosition;
                paddingPosition = Random.Range(-maxPadding, maxPadding);
                positionRandom.x += paddingPosition;
                Instantiate(entry.enemy.prefab, positionRandom,
                Quaternion.identity, transform);
            }
        }
    }
}