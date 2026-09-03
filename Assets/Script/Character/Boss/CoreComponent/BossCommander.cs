using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The boss "brain". Owns the phase the fight is in, decides which command runs next, and holds
/// the queue that lets a phase play a scripted combo. It never executes anything itself —
/// <see cref="BossExecuteCommandState"/> drives whatever this hands it.
/// </summary>
public class BossCommander : EntityCoreComponent<EntityCore>
{
    [Header("Phases")]
    [Tooltip("Ordered by descending health ratio, e.g. 1.0 / 0.66 / 0.33.")]
    [SerializeField] private List<BossPhaseSO> phases = new List<BossPhaseSO>();

    [Header("Targeting")]
    [Tooltip("Seconds between target scans. FindTargetMethod allocates, so this must not run per frame.")]
    [SerializeField][Range(0.05f, 2f)] private float targetScanInterval = 0.2f;

    [Tooltip("Scan radius. 0 falls back to EntityData.RangeCheckFieldOfView.")]
    [SerializeField][Range(0f, 60f)] private float targetScanRange;

    [Header("Reactions")]
    [Tooltip("Off by default: a boss that flinches on every hit can be stun-locked out of the fight.")]
    [SerializeField] private bool staggerOnHit;

    [Header("Debug (read only)")]
    [SerializeField] private string debugPhase;
    [SerializeField] private string debugCommand;
    [SerializeField] private int debugQueueDepth;

    private readonly Dictionary<BossCommandSO, IBossCommand> runtimes = new Dictionary<BossCommandSO, IBossCommand>();
    private readonly Queue<IBossCommand> queue = new Queue<IBossCommand>();
    private readonly List<int> candidates = new List<int>();

    private float[][] nextReadyTime;
    private bool initialized;
    private int currentPhaseIndex = -1;
    private int pendingPhaseIndex = -1;
    private int sequenceIndex;
    private float nextScanTime;

    private BossCommandContext context;
    private EntityInput entityInput;
    private EntityMovement entityMovement;
    private EntityFindTarget entityFindTarget;
    private EntityVitalStats entityVitalStats;
    private EntityStatsHandler entityStatsHandler;
    private IBossCommand activeCommand;

    public bool StaggerOnHit => staggerOnHit;

    /// <summary>True during a phase transition. Damage receivers should read this once BUG-053 is fixed.</summary>
    public bool IsInvulnerable { get; private set; }

    public bool WantsPhaseChange => pendingPhaseIndex >= 0 && pendingPhaseIndex != currentPhaseIndex;

    public float PendingTransitionDuration =>
        pendingPhaseIndex >= 0 && pendingPhaseIndex < phases.Count
            ? phases[pendingPhaseIndex]?.TransitionDuration ?? 0f
            : 0f;

    public BossPhaseSO CurrentPhase =>
        currentPhaseIndex >= 0 && currentPhaseIndex < phases.Count ? phases[currentPhaseIndex] : null;

    protected override void Start()
    {
        base.Start();
        Core.GetCoreComponent(out entityInput);
        Core.GetCoreComponent(out entityMovement);
        Core.GetCoreComponent(out entityFindTarget);
        Core.GetCoreComponent(out entityVitalStats);
        Core.GetCoreComponent(out entityStatsHandler);

        context = new BossCommandContext
        {
            Entity = Core.Entity,
            Commander = this,
            Input = entityInput,
            Movement = entityMovement,
            FindTarget = entityFindTarget,
            VitalStats = entityVitalStats
        };

        BuildRuntimes();
        initialized = true;
        ResetFight();
    }

    private void OnEnable()
    {
        // Pooled bosses are reused, and Start() only runs on the first spawn: a second fight must
        // not inherit the first fight's phase or cooldowns.
        if (initialized) ResetFight();
    }

    private void ResetFight()
    {
        pendingPhaseIndex = -1;
        IsInvulnerable = false;
        activeCommand = null;
        queue.Clear();

        for (int p = 0; p < nextReadyTime.Length; p++)
        {
            float[] row = nextReadyTime[p];
            for (int c = 0; c < row.Length; c++) row[c] = 0f;
        }

        EnterPhase(ResolvePhaseIndex(1f));
    }

    /// <summary>
    /// Instantiates one runtime per distinct command asset, once. Runtimes are reused across
    /// activations so no command allocates mid-fight.
    /// </summary>
    private void BuildRuntimes()
    {
        nextReadyTime = new float[phases.Count][];

        for (int p = 0; p < phases.Count; p++)
        {
            BossPhaseSO phase = phases[p];
            if (phase == null)
            {
                nextReadyTime[p] = new float[0];
                continue;
            }

            nextReadyTime[p] = new float[phase.Commands.Count];

            for (int c = 0; c < phase.Commands.Count; c++) Register(phase.Commands[c]?.Command);
            for (int o = 0; o < phase.OpeningSequence.Count; o++) Register(phase.OpeningSequence[o]);
        }
    }

    private void Register(BossCommandSO command)
    {
        if (command == null || runtimes.ContainsKey(command)) return;
        runtimes.Add(command, command.CreateRuntime());
    }

    public void Tick()
    {
        ScanForTarget();
        EvaluatePhase();
        debugQueueDepth = queue.Count;
    }

    /// <summary>
    /// Throttled because <see cref="EntityFindTarget.FindTargetMethod"/> uses the allocating
    /// Physics2D.OverlapCircle. Also pushes the target onto EntityInput so EntityMovement can chase.
    /// </summary>
    private void ScanForTarget()
    {
        if (Time.time < nextScanTime) return;
        nextScanTime = Time.time + targetScanInterval;

        if (entityFindTarget == null) return;

        float range = targetScanRange > 0f ? targetScanRange : Core.Entity.Data.RangeCheckFieldOfView;
        Transform found = entityFindTarget.FindTargetMethod(range);

        context.Target = found;
        if (entityInput != null) entityInput.SetTargetTransform(found);
    }

    private void EvaluatePhase()
    {
        if (WantsPhaseChange) return;

        // Forward only. A boss healed back over a threshold must not replay an earlier phase, and
        // health sitting exactly on a threshold must not thrash between two phases.
        int resolved = ResolvePhaseIndex(HealthRatio());
        if (resolved > currentPhaseIndex) pendingPhaseIndex = resolved;
    }

    private float HealthRatio()
    {
        if (entityVitalStats == null || entityStatsHandler == null) return 1f;

        float max = entityStatsHandler.GetStatValue(StatType.HP);
        if (max <= 0f) return 1f;

        return Mathf.Clamp01(entityVitalStats.GetCurrentStatValue(StatType.HP) / max);
    }

    /// <summary>Last phase whose threshold the current health ratio has fallen to or below.</summary>
    private int ResolvePhaseIndex(float healthRatio)
    {
        int index = 0;
        for (int i = 0; i < phases.Count; i++)
        {
            if (phases[i] != null && healthRatio <= phases[i].EnterAtHealthRatio) index = i;
        }
        return index;
    }

    public void BeginPhaseTransition()
    {
        IsInvulnerable = true;
    }

    public void CommitPhaseChange()
    {
        IsInvulnerable = false;
        if (pendingPhaseIndex >= 0) EnterPhase(pendingPhaseIndex);
        pendingPhaseIndex = -1;
    }

    private void EnterPhase(int index)
    {
        currentPhaseIndex = Mathf.Clamp(index, 0, Mathf.Max(0, phases.Count - 1));
        sequenceIndex = 0;
        queue.Clear();

        BossPhaseSO phase = CurrentPhase;
        if (phase == null) return;

        debugPhase = phase.PhaseName;

        for (int i = 0; i < phase.OpeningSequence.Count; i++)
        {
            if (TryGetRuntime(phase.OpeningSequence[i], out IBossCommand runtime)) queue.Enqueue(runtime);
        }
    }

    /// <summary>Next command to run: whatever is queued, otherwise a fresh pick from the current phase.</summary>
    public IBossCommand DequeueNext()
    {
        if (queue.Count > 0) return queue.Dequeue();
        return SelectFromPhase();
    }

    /// <summary>Reads the next command without consuming it. Feeds telegraphs and gizmos.</summary>
    public IBossCommand Peek() => queue.Count > 0 ? queue.Peek() : null;

    private IBossCommand SelectFromPhase()
    {
        BossPhaseSO phase = CurrentPhase;
        if (phase == null || phase.Commands.Count == 0) return null;

        return phase.Selection == BossPhaseSO.SelectionMode.Sequence
            ? SelectSequential(phase)
            : SelectWeighted(phase);
    }

    private IBossCommand SelectSequential(BossPhaseSO phase)
    {
        for (int attempt = 0; attempt < phase.Commands.Count; attempt++)
        {
            int index = sequenceIndex % phase.Commands.Count;
            sequenceIndex++;

            if (IsUsable(phase, index) && TryGetRuntime(phase.Commands[index].Command, out IBossCommand runtime))
            {
                StampCooldown(phase, index);
                return runtime;
            }
        }
        return null;
    }

    private IBossCommand SelectWeighted(BossPhaseSO phase)
    {
        candidates.Clear();
        float totalWeight = 0f;

        for (int i = 0; i < phase.Commands.Count; i++)
        {
            if (!IsUsable(phase, i)) continue;
            candidates.Add(i);
            totalWeight += phase.Commands[i].Weight;
        }

        if (candidates.Count == 0 || totalWeight <= 0f) return null;

        float roll = Random.Range(0f, totalWeight);
        for (int i = 0; i < candidates.Count; i++)
        {
            int index = candidates[i];
            roll -= phase.Commands[index].Weight;
            if (roll > 0f) continue;

            if (!TryGetRuntime(phase.Commands[index].Command, out IBossCommand runtime)) return null;
            StampCooldown(phase, index);
            return runtime;
        }
        return null;
    }

    private bool IsUsable(BossPhaseSO phase, int index)
    {
        BossCommandEntry entry = phase.Commands[index];
        if (entry == null || entry.Command == null) return false;
        if (Time.time < nextReadyTime[currentPhaseIndex][index]) return false;

        float distance = context.HasTarget ? context.DistanceToTarget : 0f;
        return entry.IsInRange(distance);
    }

    private void StampCooldown(BossPhaseSO phase, int index)
    {
        nextReadyTime[currentPhaseIndex][index] = Time.time + phase.Commands[index].Cooldown;
    }

    private bool TryGetRuntime(BossCommandSO command, out IBossCommand runtime)
    {
        runtime = null;
        return command != null && runtimes.TryGetValue(command, out runtime);
    }

    public BossCommandContext Context => context;

    public void SetActiveCommand(IBossCommand command)
    {
        activeCommand = command;
        debugCommand = command == null ? "-" : command.DisplayName;
    }

    private void OnDrawGizmosSelected()
    {
        if (activeCommand == null) return;

        if (activeCommand.TryGetDangerZone(out Vector2 center, out float radius))
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(center, radius);
        }

        if (context != null && context.HasTarget)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, context.Target.position);
        }
    }
}
