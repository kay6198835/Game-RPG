using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
[RequireComponent(typeof(ObjectPoolManager))]
public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private List<Vector2Int> spawnPosition;
    [SerializeField] private MapModel mapModel;
    [SerializeField] private RoomModel roomModel;
    [SerializeField] private float paddingPosition;
    [SerializeField] private float maxPadding;
    [SerializeField] private Vector2 positionRandom;
    [SerializeField] private ObjectPoolManager objectPoolManager;
    void Awake()
    {
        objectPoolManager = GetComponent<ObjectPoolManager>();
    }
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
        if (spawnPosition == null || spawnPosition.Count == 0)
        {
            Debug.LogWarning("OnDoneLoadRoomGrid: spawnPosition is empty");
            return;
        }
        roomModel = mapModel.GetRandomRoom();
        if (roomModel == null)
        {
            Debug.LogWarning("OnDoneLoadRoomGrid: roomModel is empty");
            return;
        }
        SpawnRoomEnemies(in spawnPosition);
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

    public void SpawnRoomEnemies(in List<Vector2Int> spawnPosition)
    {
        List<EnemySpawnEntry> set = GetRoomSpawnSet();
        if (set.Count == 0 || set == null)
        {
            Debug.LogWarning("SpawnRoomEnemies: nothing to spawn");
            return;
        }
        if (spawnPosition == null || spawnPosition.Count == 0)
        {
            Debug.LogWarning("SpawnRoomEnemies: spawnPosition is empty");
            return;
        }
        foreach (var entry in set)
        {
            if (entry.enemy == null || entry.enemy.Prefab == null) continue;
            for (int i = 0; i < entry.count; i++)
            {
                positionRandom = spawnPosition[Random.Range(0, spawnPosition.Count)].ConvertTo<Vector2>();
                paddingPosition = Random.Range(-maxPadding, maxPadding);
                positionRandom.y += paddingPosition;
                paddingPosition = Random.Range(-maxPadding, maxPadding);
                positionRandom.x += paddingPosition;
                objectPoolManager.Spawn(positionRandom, entry.enemy.Prefab);
            }
        }
    }
}