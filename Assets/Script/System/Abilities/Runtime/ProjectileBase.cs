using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]

public class SlashProjectile : SpawnMono
{
    protected Rigidbody2D _rb;
    protected CircleCollider2D _col;
    [Range(1, 30)]
    protected float speed;
    protected override void Awake()
    {
        base.Awake();
        _rb = GetComponent<Rigidbody2D>();
        _rb.gravityScale = 0f;

        _col = GetComponent<CircleCollider2D>();
        _col.isTrigger = true;
    }
    public override void Launch(Vector2 target, float lifetime,
                             AbilityContext context)
    {
        base.Launch(target, lifetime, context);
        var direction = target - transform.position;
        _rb.velocity = direction * speed;
    }
}
