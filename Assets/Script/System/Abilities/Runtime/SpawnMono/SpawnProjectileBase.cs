using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]

public class SpawnProjectileBase : SpawnMono
{
    protected Rigidbody2D _rb;
    protected CircleCollider2D _col;
    [Range(1, 30)]
    [SerializeField] protected float speed;
    protected virtual void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.gravityScale = 0f;

        _col = GetComponent<CircleCollider2D>();
        _col.isTrigger = true;
    }
    public override void Launch(float lifetime,
                             AbilityContext context, Action<AbilityContext> execute)
    {
        base.Launch(lifetime, context, execute);
        _rb.velocity = context.Forward * speed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Set-then-invoke: always assign (null for a non-hurtbox) so a stale target is never reused.
        other.TryGetCharacter(out ICharacter target);
        _context.Target = target;
        _callback.Invoke(_context);
    }
}
