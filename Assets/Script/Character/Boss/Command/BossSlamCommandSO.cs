using UnityEngine;

/// <summary>
/// Telegraphed area attack centred on the boss. The wind-up is the whole point: the player must be
/// able to read it and leave the circle, so <see cref="TelegraphTime"/> is a fairness knob, not a
/// pacing one.
/// </summary>
[CreateAssetMenu(fileName = "BossSlam", menuName = "Boss/Command/Slam")]
public class BossSlamCommandSO : BossCommandSO
{
    [Header("Telegraph")]
    [Tooltip("Seconds between the wind-up starting and the hit landing.")]
    [SerializeField][Range(0.05f, 4f)] private float telegraphTime = 0.7f;

    [Tooltip("Drive the hit frame from the Animator's OnActivate event instead of telegraphTime. " +
             "Leave off until the boss clips actually carry animation events.")]
    [SerializeField] private bool useAnimationEvent;

    [Header("Hit")]
    [SerializeField][Range(0.5f, 12f)] private float radius = 3f;
    [SerializeField][Range(0.1f, 10f)] private float damageMultiplier = 1f;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField][Range(1, 16)] private int maxTargets = 4;

    [Header("Offset")]
    [Tooltip("Push the circle this far toward the target. 0 centres it on the boss.")]
    [SerializeField][Range(0f, 8f)] private float forwardOffset;

    public float TelegraphTime => telegraphTime;
    public bool UseAnimationEvent => useAnimationEvent;
    public float Radius => radius;
    public float DamageMultiplier => damageMultiplier;
    public LayerMask TargetLayer => targetLayer;
    public int MaxTargets => maxTargets;
    public float ForwardOffset => forwardOffset;

    public override IBossCommand CreateRuntime() => new BossSlamCommand(this);
}

public class BossSlamCommand : BossCommandRuntime<BossSlamCommandSO>
{
    private Vector2 impactCenter;

    public BossSlamCommand(BossSlamCommandSO data) : base(data)
    {
        AllocateHitBuffer(data.MaxTargets);
    }

    protected override void OnEnter()
    {
        if (ctx.Movement != null) ctx.Movement.StopMove();
        FaceTarget();
        impactCenter = ResolveImpactCenter();
    }

    protected override void OnTick(float deltaTime)
    {
        if (EffectResolved) return;

        // The circle is locked in at wind-up so the telegraph the player read is the one that hits.
        if (!data.UseAnimationEvent && Elapsed >= data.TelegraphTime) Impact();
    }

    public override void OnAnimationStatus(StatusAnimation status)
    {
        if (data.UseAnimationEvent && status == StatusAnimation.OnActivate) Impact();
    }

    public override bool TryGetDangerZone(out Vector2 center, out float radius)
    {
        center = impactCenter;
        radius = data.Radius;
        return !EffectResolved;
    }

    private void Impact()
    {
        if (!TryResolveEffect()) return;
        ApplyAreaDamage(impactCenter, data.Radius, data.TargetLayer, ResolveDamage(data.DamageMultiplier));
    }

    private Vector2 ResolveImpactCenter()
    {
        if (data.ForwardOffset <= 0f) return ctx.Position;
        return ctx.Position + ctx.DirectionToTarget * data.ForwardOffset;
    }
}
