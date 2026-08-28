using System.Collections.Generic;
using UnityEngine;
using VContainer;
public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private List<Vector2Int> spawnPosition;
    [SerializeField] private MapModel mapModel;
    [SerializeField] private RoomModel roomModel;
    [SerializeField] private float paddingPosition;
    [SerializeField] private float maxPadding;
    [SerializeField] private Vector2 positionRandom;
    private IObjecPoolService objectPoolManager;
    [Inject]
    public void Construct(IObjecPoolService objectPoolManager)
    {
        this.objectPoolManager = objectPoolManager;
    }
    void Awake()
    {

    }
    public void OnEnable()
    {
        EventManager.Resgister(EventID.ON_GET_SPAWN_POSITIONS, OnGetSpawnPositions);
        EventManager.Resgister(EventID.ON_SPAWN_EXTRA_ENEMY, SpawnExtraEnemy);
        EventManager.Resgister(EventID.ON_ENEMY_DEATH, ReleaseEnemy);
    }
    public void OnDisable()
    {
        EventManager.UnResgister(EventID.ON_GET_SPAWN_POSITIONS, OnGetSpawnPositions);
        EventManager.UnResgister(EventID.ON_SPAWN_EXTRA_ENEMY, SpawnExtraEnemy);
        EventManager.Resgister(EventID.ON_ENEMY_DEATH, ReleaseEnemy);
    }
    public void Spawn()
    {

    }

    public void OnGetSpawnPositions(object obj = null)
    {
        spawnPosition = (List<Vector2Int>)obj;
        if (spawnPosition == null || spawnPosition.Count == 0)
        {
            Debug.LogWarning("OnGetSpawnPositions: spawnPosition is empty");
            return;
        }
        roomModel = mapModel.GetRandomRoom();
        if (roomModel == null)
        {
            Debug.LogWarning("OnGetSpawnPositions: roomModel is empty");
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

    public void ReleaseEnemy(object obj = null)
    {
        objectPoolManager.Release((GameObject)obj);
    }

    public void SpawnRoomEnemies(in List<Vector2Int> spawnPosition)
    {
        int enemyCount = 0;
        List<EnemySpawnEntry> set = GetRoomSpawnSet();
        if (set == null || set.Count == 0)
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
                positionRandom = Vector2Int.RoundToInt(spawnPosition[Random.Range(0, spawnPosition.Count)]);

                objectPoolManager.Spawn(positionRandom + Utility.RandomPaddingDistace(-maxPadding, maxPadding), Quaternion.identity, entry.enemy.Prefab);
                //entityInput.SetSpawnPoint(positionRandom);
                enemyCount++;
            }
        }
        Debug.Log($"[{nameof(EnemySpawner)}] Spawned {enemyCount} enemies in room {roomModel.name}.");
        EventManager.Emit(EventID.ON_DONE_SPAWN_ENEMY, enemyCount);
    }

    // it for behavior spawn extra enemy like ability enemy, trap, skill boss. Use RequestSpawnEnemy
    public void SpawnExtraEnemy(object obj = null)
    {
        RequestSpawnEnemy spawnEnemy = (RequestSpawnEnemy)obj;
        objectPoolManager.Spawn(spawnEnemy.positionSpawn, Quaternion.identity, spawnEnemy.prefab);
    }
}

public class RequestSpawnEnemy
{
    public Vector2 positionSpawn;
    public GameObject prefab;
}