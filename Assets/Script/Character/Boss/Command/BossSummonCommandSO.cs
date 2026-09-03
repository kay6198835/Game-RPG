using UnityEngine;

/// <summary>
/// Spawns adds around the boss through the existing spawn pipeline. Emitting
/// <see cref="EventID.ON_SPAWN_EXTRA_ENEMY"/> rather than pooling directly is what keeps the room
/// clear counter correct: <c>EnemySpawner</c> does the pooling and <c>RoomCell</c> increments its
/// alive count off the same event.
/// </summary>
[CreateAssetMenu(fileName = "BossSummon", menuName = "Boss/Command/Summon")]
public class BossSummonCommandSO : BossCommandSO
{
    [Header("Summon")]
    [SerializeField] private GameObject[] addPrefabs;
    [SerializeField][Range(1, 8)] private int count = 2;
    [SerializeField][Range(0.5f, 10f)] private float spawnRadius = 3f;

    [Header("Timing")]
    [Tooltip("Seconds before the adds appear. Gives the player a window to reposition.")]
    [SerializeField][Range(0.05f, 4f)] private float castTime = 0.8f;
    [SerializeField] private bool useAnimationEvent;

    public GameObject[] AddPrefabs => addPrefabs;
    public int Count => count;
    public float SpawnRadius => spawnRadius;
    public float CastTime => castTime;
    public bool UseAnimationEvent => useAnimationEvent;

    public override IBossCommand CreateRuntime() => new BossSummonCommand(this);
}

public class BossSummonCommand : BossCommandRuntime<BossSummonCommandSO>
{
    public BossSummonCommand(BossSummonCommandSO data) : base(data) { }

    protected override void OnEnter()
    {
        if (ctx.Movement != null) ctx.Movement.StopMove();
        FaceTarget();
    }

    protected override void OnTick(float deltaTime)
    {
        if (EffectResolved) return;
        if (!data.UseAnimationEvent && Elapsed >= data.CastTime) Summon();
    }

    public override void OnAnimationStatus(StatusAnimation status)
    {
        if (data.UseAnimationEvent && status == StatusAnimation.OnActivate) Summon();
    }

    private void Summon()
    {
        if (!TryResolveEffect()) return;

        GameObject[] prefabs = data.AddPrefabs;
        if (prefabs == null || prefabs.Length == 0)
        {
            Debug.LogWarning($"[{nameof(BossSummonCommand)}] {data.DisplayName} has no add prefabs assigned.");
            return;
        }

        for (int i = 0; i < data.Count; i++)
        {
            GameObject prefab = prefabs[Random.Range(0, prefabs.Length)];
            if (prefab == null) continue;

            EventManager.Emit(EventID.ON_SPAWN_EXTRA_ENEMY, new RequestSpawnEnemy
            {
                prefab = prefab,
                positionSpawn = ResolveSpawnPosition()
            });
        }
    }

    /// <summary>Nearest walkable node to a random point around the boss, so adds never spawn inside a wall.</summary>
    private Vector2 ResolveSpawnPosition()
    {
        Vector2 candidate = ctx.Position + Random.insideUnitCircle.normalized * data.SpawnRadius;

        if (EnemyManager.Instance == null) return candidate;

        Node node = EnemyManager.Instance.GetNodeByPositionWorld(candidate);
        return node != null && node.Walkable ? node.WorldPosition : ctx.Position;
    }
}
