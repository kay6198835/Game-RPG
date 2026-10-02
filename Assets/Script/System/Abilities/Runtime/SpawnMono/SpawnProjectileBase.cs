using System;
using UnityEngine;

[RequireComponent(typeof(ProjectileBody))]
public class SpawnProjectileBase : SpawnMono, IProjectilePayload
{
    [Range(1, 30)]
    [SerializeField] protected float speed;
    [SerializeField] protected LayerMask targetMask;
    [SerializeField] protected LayerMask blockMask;
    [SerializeField, Range(0, 10)] protected int pierceCount;

    protected ProjectileBody _body;

    protected virtual void Awake()
    {
        _body = GetComponent<ProjectileBody>();
    }

    public override void Launch(float lifetime,
                                AbilityContext context, Action<AbilityContext> execute)
    {
        // Lifetime is owned by the body; 0 stops SpawnMono from starting a second despawn coroutine.
        base.Launch(0f, context, execute);

        var config = new ProjectileConfig
        {
            speed = speed,
            lifetime = lifetime,
            targetMask = targetMask,
            blockMask = blockMask,
        };
        _body.Launch(context.Forward, config, this);
    }

    public void OnHit(Collider2D target, Vector2 hitPos)
    {
        if (_context == null || _callback == null) return;
        // Set-then-invoke: always assign (null for a non-hurtbox) so a stale target is never reused.
        _context.Target = target.TryGetComponent(out INegativeReceiver _) ? target : null;
        _callback.Invoke(_context);
    }
}