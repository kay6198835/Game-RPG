using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Shared movement layer of Player and Entity: owns the Rigidbody2D and every external impact on
/// movement. Subclasses keep their own steering (input velocity / A* path following) and must
/// respect <see cref="CanMove"/> and <see cref="SpeedMultiplier"/>.
/// </summary>
public abstract class MovementBase<TCore> : CoreComponentBase<TCore>, IMovement where TCore : CoreBase
{
    [SerializeField] protected Rigidbody2D rb;

    private readonly Dictionary<object, float> speedMultipliers = new();
    private readonly HashSet<object> locks = new();
    private float speedMultiplier = 1f;
    private Vector2 knockbackVelocity;
    private float knockbackEndTime;

    public Vector2 Velocity => rb != null ? rb.velocity : Vector2.zero;
    public bool IsKnockedBack => Time.time < knockbackEndTime;
    public bool CanMove => locks.Count == 0 && !IsKnockedBack;
    public float SpeedMultiplier => speedMultiplier;

    // NEED CHECK: new 2026-11-04 — Player and Entity both move at StatType.MoveSpeed (was: PlayerMoveState
    // multiplied it in; EntityMovement used its own serialized `speed`, 0 on EnemyPrefab). 0 until Start().
    protected IStatService statService;
    /// <summary>Current MoveSpeed stat in world units per second; 0 when the character has no stat service.</summary>
    public float MoveSpeed => statService != null ? statService.GetStatValue(StatType.MoveSpeed) : 0f;

    protected override void Awake()
    {
        base.Awake();
    }

    public override void Setup()
    {
        base.Setup();
        rb = GetComponentInParent<Rigidbody2D>();
    }

    protected override void Start()
    {
        base.Start();
        Core.TryGetCapability(out statService);
    }

    protected virtual void FixedUpdate()
    {
        if (!IsKnockedBack) return;
        // MovePosition works for dynamic and kinematic bodies alike.
        rb.MovePosition(rb.position + knockbackVelocity * Time.fixedDeltaTime);
    }

    // NEED CHECK: 2026-11-08 — on respawn / Reborn the body keeps the previous life's velocity, and its
    // physics position only follows the transform at the next simulation step.
    public override void ReloadComponent()
    {
        base.ReloadComponent();
        if (rb == null) return;
        rb.velocity = Vector2.zero;
        rb.position = rb.transform.Position2D();
    }

    public virtual void SetVelocity(Vector2 velocity)
    {
        rb.velocity = CanMove ? velocity * speedMultiplier : Vector2.zero;
    }

    public virtual void Stop()
    {
        rb.velocity = Vector2.zero;
    }

    public void AddSpeedMultiplier(object source, float multiplier)
    {
        if (source == null) return;
        speedMultipliers[source] = multiplier;
        RecalculateSpeedMultiplier();
    }

    public void RemoveSpeedMultiplier(object source)
    {
        if (source == null || !speedMultipliers.Remove(source)) return;
        RecalculateSpeedMultiplier();
    }

    public void Lock(object source)
    {
        if (source == null) return;
        locks.Add(source);
        Stop();
    }

    public void Unlock(object source)
    {
        if (source == null) return;
        locks.Remove(source);
    }

    public void ApplyKnockback(Vector2 velocity, float duration)
    {
        if (duration <= 0f) return;
        knockbackVelocity = velocity;
        knockbackEndTime = Time.time + duration;
        rb.velocity = Vector2.zero;
    }

    public void ClearImpacts()
    {
        speedMultipliers.Clear();
        locks.Clear();
        speedMultiplier = 1f;
        knockbackEndTime = 0f;
        knockbackVelocity = Vector2.zero;
    }

    private void RecalculateSpeedMultiplier()
    {
        speedMultiplier = 1f;
        foreach (var multiplier in speedMultipliers.Values)
        {
            speedMultiplier *= multiplier;
        }
    }
}
