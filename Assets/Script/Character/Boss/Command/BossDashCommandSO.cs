using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Wind up, lock a direction, then charge along it. The direction is locked at the end of the
/// wind-up and never re-aimed — a charge that tracks the player is unreadable and unfair.
/// </summary>
[CreateAssetMenu(fileName = "BossDash", menuName = "Boss/Command/Dash")]
public class BossDashCommandSO : BossCommandSO
{
    [Header("Wind-up")]
    [SerializeField][Range(0.05f, 3f)] private float windUpTime = 0.6f;

    [Header("Dash")]
    [SerializeField][Range(1f, 40f)] private float dashSpeed = 12f;
    [SerializeField][Range(0.1f, 4f)] private float dashDuration = 0.5f;
    [Tooltip("Stops the charge early when it runs into an obstacle layer. Leave empty to disable.")]
    [SerializeField] private LayerMask obstacleLayer;

    [Header("Hit")]
    [SerializeField][Range(0.2f, 6f)] private float hitRadius = 1.2f;
    [SerializeField][Range(0.1f, 10f)] private float damageMultiplier = 1.5f;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField][Range(1, 16)] private int maxTargets = 4;

    public float WindUpTime => windUpTime;
    public float DashSpeed => dashSpeed;
    public float DashDuration => dashDuration;
    public LayerMask ObstacleLayer => obstacleLayer;
    public float HitRadius => hitRadius;
    public float DamageMultiplier => damageMultiplier;
    public LayerMask TargetLayer => targetLayer;
    public int MaxTargets => maxTargets;

    public override IBossCommand CreateRuntime() => new BossDashCommand(this);
}

public class BossDashCommand : BossCommandRuntime<BossDashCommandSO>
{
    private readonly HashSet<int> hitThisDash = new HashSet<int>();
    private Vector2 lockedDirection;
    private float dashTimeLeft;
    private bool dashing;

    public BossDashCommand(BossDashCommandSO data) : base(data)
    {
        AllocateHitBuffer(data.MaxTargets);
    }

    protected override void OnEnter()
    {
        hitThisDash.Clear();
        dashing = false;
        dashTimeLeft = data.DashDuration;
        lockedDirection = Vector2.zero;
        if (ctx.Movement != null) ctx.Movement.StopMove();
        FaceTarget();
    }

    protected override void OnTick(float deltaTime)
    {
        if (EffectResolved) return;

        if (!dashing)
        {
            if (Elapsed < data.WindUpTime)
            {
                FaceTarget();
                return;
            }
            lockedDirection = ctx.DirectionToTarget;
            dashing = true;
            return;
        }

        dashTimeLeft -= deltaTime;
        if (dashTimeLeft <= 0f) TryResolveEffect();
    }

    public override void PhysicsTick()
    {
        if (!dashing || EffectResolved) return;

        Rigidbody2D rb = ctx.Entity.Rb;
        if (rb == null)
        {
            TryResolveEffect();
            return;
        }

        if (HitObstacle(rb.position))
        {
            TryResolveEffect();
            return;
        }

        rb.MovePosition(rb.position + lockedDirection * data.DashSpeed * Time.fixedDeltaTime);
        ctx.Input?.SetTarget(rb.position + lockedDirection);

        ApplyAreaDamage(rb.position, data.HitRadius, data.TargetLayer,
                        ResolveDamage(data.DamageMultiplier), hitThisDash);
    }

    public override bool TryGetDangerZone(out Vector2 center, out float radius)
    {
        center = ctx.Position + lockedDirection * (data.DashSpeed * data.DashDuration * 0.5f);
        radius = data.HitRadius;
        return !EffectResolved && dashing;
    }

    private bool HitObstacle(Vector2 from)
    {
        if (data.ObstacleLayer.value == 0) return false;
        float step = data.DashSpeed * Time.fixedDeltaTime + data.HitRadius;
        return Physics2D.Raycast(from, lockedDirection, step, data.ObstacleLayer).collider != null;
    }
}
