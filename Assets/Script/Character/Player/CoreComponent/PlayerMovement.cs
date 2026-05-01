using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : CoreCompoment
{
    [SerializeField] protected Rigidbody2D rb;
    protected override void Awake()
    {
        base.Awake();
        rb = GetComponentInParent<Rigidbody2D>();
    }
    protected void Start()
    {   
    }
    public void SetVeclocity(Vector2 velocity)
    {
        rb.velocity = velocity;
    }
}
