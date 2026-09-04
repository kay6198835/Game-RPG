using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base runtime for every boss command. Owns the lifetime bookkeeping (elapsed time, timeout,
/// recovery tail) so concrete commands only describe their effect.
/// </summary>
/// <typeparam name="TData">The authoring SO this runtime reads from.</typeparam>
public abstract class BossCommandRuntime<TData> : IBossCommand where TData : BossCommandSO
{
    protected readonly TData data;
    protected BossCommandContext ctx;

    private float elapsed;
    private float recoveryLeft;
    private bool effectResolved;

    protected BossCommandRuntime(TData data)
    {
        this.data = data;
    }

    public string AnimBoolName => data.AnimBoolName;

    public string DisplayName => data.DisplayName;

    /// <summary>Seconds since this activation started.</summary>
    protected float Elapsed => elapsed;

    /// <summary>True once <see cref="TryResolveEffect"/> has succeeded for this activation.</summary>
    protected bool EffectResolved => effectResolved;

    public bool IsDone => (effectResolved && recoveryLeft <= 0f) || elapsed >= data.MaxDuration;

    public void Enter(BossCommandContext context)
    {
        ctx = context;
        elapsed = 0f;
        recoveryLeft = data.RecoveryTime;
        effectResolved = false;
        OnEnter();
    }

    public void Tick(float deltaTime)
    {
        elapsed += deltaTime;
        OnTick(deltaTime);
        if (effectResolved && recoveryLeft > 0f) recoveryLeft -= deltaTime;
    }

    public virtual void PhysicsTick() { }

    public virtual void OnAnimationStatus(StatusAnimation status) { }

    public virtual bool TryGetDangerZone(out Vector2 center, out float radius)
    {
        center = Vector2.zero;
        radius = 0f;
        return false;
    }

    public void Exit()
    {
        OnExit();
        ctx = null;
    }

    /// <summary>
    /// Marks the command's effect as done and starts the recovery tail. Idempotent — a command
    /// driven by both a timer and an animation event must not fire its hit twice.
    /// </summary>
    protected bool TryResolveEffect()
    {
        if (effectResolved) return false;
        effectResolved = true;
        return true;
    }

    /// <summary>Faces the boss at its current target so the Animator "Direction" float tracks it.</summary>
    protected void FaceTarget()
    {
        if (ctx.Input != null && ctx.HasTarget) ctx.Input.SetTarget(ctx.TargetPosition);
    }

    /// <summary>
    /// Physical damage from the boss's own stats, scaled by a per-command multiplier.
    /// Deliberately unguarded against a missing PhysicalDamage entry: a boss stat asset without
    /// it is a wiring error that should fail loudly rather than silently deal zero.
    /// </summary>
    protected float ResolveDamage(float multiplier)
    {
        if (ctx.VitalStats == null) return 0f;
        float physical = ctx.VitalStats.GetCurrentStatValue(StatType.PhysicalDamage);
        return Mathf.Max(1f, physical * multiplier);
    }

    private Collider2D[] hitBuffer;

    /// <summary>Allocates the overlap buffer once, at construction — never inside a tick.</summary>
    protected void AllocateHitBuffer(int maxTargets)
    {
        hitBuffer = new Collider2D[Mathf.Max(1, maxTargets)];
    }

    /// <summary>
    /// Damages every <see cref="INegativeReceiver"/> inside the circle. Returns how many were hit.
    /// Requires <see cref="AllocateHitBuffer"/> to have run.
    /// </summary>
    protected int ApplyAreaDamage(Vector2 center, float radius, LayerMask mask, float damage,
                                  HashSet<int> alreadyHit = null)
    {
        if (hitBuffer == null) return 0;

        int count = Physics2D.OverlapCircleNonAlloc(center, radius, hitBuffer, mask);
        int hits = 0;
        for (int i = 0; i < count; i++)
        {
            if (hitBuffer[i] == null) continue;
            if (alreadyHit != null && !alreadyHit.Add(hitBuffer[i].GetInstanceID())) continue;
            if (!hitBuffer[i].TryGetComponent(out INegativeReceiver receiver)) continue;
            receiver.TakeDamage(damage, ctx.Position);
            hits++;
        }
        return hits;
    }

    protected virtual void OnEnter() { }
    protected virtual void OnTick(float deltaTime) { }
    protected virtual void OnExit() { }
}
