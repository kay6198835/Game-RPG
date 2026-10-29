using UnityEngine;
using VContainer;

// Spawns one real enemy prefab for the CombatTest scene through the pool, exactly as EnemySpawner does
// (so Pool.Spawn injects EntityInput and the full prefab runs unchanged), pins it in place with a speed
// multiplier, and respawns it after death because the scene has no EnemySpawner to release it.
public class CombatTestEnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform spawnPoint;
    [Tooltip("Applied through IMovement.AddSpeedMultiplier: 0 = stands still, 1 = normal speed. Knockback is unaffected.")]
    [SerializeField] private float moveSpeedMultiplier;
    [SerializeField] private float respawnDelay = 2f;

    private IObjectPoolService pool;
    private GameObject current;

    [Inject]
    public void Construct(IObjectPoolService pool)
    {
        this.pool = pool;
    }

    private void OnEnable() => EventManager.Resgister(EventID.ON_ENEMY_DEATH, OnEnemyDeath);
    private void OnDisable() => EventManager.UnResgister(EventID.ON_ENEMY_DEATH, OnEnemyDeath);

    private void Start()
    {
        // EntityMovement.Start reads EnemyManager.Instance.Grid. An empty grid answers every query with
        // null / Path.Failure, which the movement code already handles; there is no tilemap to build one from.
        if (EnemyManager.Instance != null && EnemyManager.Instance.Grid == null)
            EnemyManager.Instance.SetPathfindingGrid(new PathfindingGrid());
        Spawn();
    }

    private void Spawn()
    {
        if (enemyPrefab == null || pool == null) return;
        Vector2 position = spawnPoint != null ? spawnPoint.position : transform.position;
        current = pool.Spawn(enemyPrefab, position, Quaternion.identity);
        IMovement movement = current != null ? current.GetComponentInChildren<IMovement>(true) : null;
        movement?.AddSpeedMultiplier(this, moveSpeedMultiplier);
    }

    private void OnEnemyDeath(object payload)
    {
        if (payload is not GameObject dead || dead != current) return;
        pool.Release(dead);
        current = null;
        Invoke(nameof(Spawn), respawnDelay);
    }
}
