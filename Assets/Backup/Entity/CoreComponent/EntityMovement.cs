using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityMovement : EntityCoreComponent
{
    [SerializeField] protected Rigidbody2D rb;

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponentInParent<Rigidbody2D>();
    }

    public void MoveForwardTarget(Vector2 velocity)
    {
        rb.velocity = velocity;
    }

    
}
