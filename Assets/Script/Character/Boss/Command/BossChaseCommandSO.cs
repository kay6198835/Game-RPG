using UnityEngine;

/// <summary>
/// Boss walks the A* path toward the player until it is inside <see cref="StopDistance"/> or the
/// chase times out. Reuses <see cref="EntityMovement"/> so the boss respects the same pathfinding
/// grid as every other enemy.
/// </summary>
[CreateAssetMenu(fileName = "BossChase", menuName = "Boss/Command/Chase")]
public class BossChaseCommandSO : BossCommandSO
{
    [Header("Chase")]
    [SerializeField][Range(0.5f, 15f)] private float chaseDuration = 3f;
    [SerializeField][Range(0.5f, 15f)] private float stopDistance = 2f;

    public float ChaseDuration => chaseDuration;
    public float StopDistance => stopDistance;

    public override IBossCommand CreateRuntime() => new BossChaseCommand(this);
}

public class BossChaseCommand : BossCommandRuntime<BossChaseCommandSO>
{
    public BossChaseCommand(BossChaseCommandSO data) : base(data) { }

    protected override void OnTick(float deltaTime)
    {
        if (ctx.Movement == null)
        {
            TryResolveEffect();
            return;
        }

        ctx.Movement.CheckMove();

        if (Elapsed >= data.ChaseDuration || ctx.DistanceToTarget <= data.StopDistance)
        {
            TryResolveEffect();
        }
    }

    public override void PhysicsTick()
    {
        if (!EffectResolved && ctx.Movement != null) ctx.Movement.MoveForwardToTarget();
    }

    protected override void OnExit()
    {
        if (ctx.Movement != null) ctx.Movement.StopMove();
    }
}
