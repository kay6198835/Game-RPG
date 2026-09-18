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
    public override void Launch(Vector2 dir, float lifetime,
                             AbilityContext context, Action<AbilityContext> execute)
    {
        base.Launch(dir, lifetime, context, execute);
        _rb.velocity = dir * speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        other.TryGetComponent<INegativeReceiver>(out INegativeReceiver negativeReciver);
        _context.Services.NegativeReceiver = negativeReciver;
        _callback.Invoke(_context);
    }
}
