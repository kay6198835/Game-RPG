using UnityEngine;

/// <summary>Runtime half of a boss command. Created once per <see cref="BossCommandSO"/> and reused.</summary>
public interface IBossCommand
{
    /// <summary>Animator bool driven while this command runs.</summary>
    string AnimBoolName { get; }

    /// <summary>Designer-facing name, shown in the commander's debug fields.</summary>
    string DisplayName { get; }

    /// <summary>Resets runtime state and starts the command. Called every activation, not just the first.</summary>
    void Enter(BossCommandContext context);

    /// <summary>Per-frame logic. Never allocates.</summary>
    void Tick(float deltaTime);

    /// <summary>Physics-step movement. Mirrors <see cref="IState.PhysicsUpdate"/>.</summary>
    void PhysicsTick();

    /// <summary>Animation event relayed by <see cref="BossExecuteCommandState"/>.</summary>
    void OnAnimationStatus(StatusAnimation status);

    bool IsDone { get; }

    void Exit();

    /// <summary>
    /// The area this command is about to damage, in world space. False when the command has no
    /// hitbox or has already resolved. Feeds the boss gizmos today and a telegraph VFX later.
    /// </summary>
    bool TryGetDangerZone(out Vector2 center, out float radius);
}

/// <summary>
/// Shared references handed to every command. Owned by <see cref="BossCommander"/> and mutated in place,
/// so a command always reads the latest <see cref="Target"/> without re-resolving components.
/// </summary>
public class BossCommandContext
{
    public Entity Entity;
    public BossCommander Commander;
    public EntityInput Input;
    public EntityMovement Movement;
    public EntityFindTarget FindTarget;
    public EntityVitalStats VitalStats;

    public Transform Target;

    public bool HasTarget => Target != null;

    public Vector2 Position => Entity.transform.Position2D();

    public Vector2 TargetPosition => Target != null ? Target.Position2D() : Position;

    public Vector2 DirectionToTarget
    {
        get
        {
            Vector2 delta = TargetPosition - Position;
            return delta.sqrMagnitude > 0.0001f ? delta.normalized : Vector2.down;
        }
    }

    public float DistanceToTarget => Target != null ? Vector2.Distance(Position, TargetPosition) : float.MaxValue;
}
