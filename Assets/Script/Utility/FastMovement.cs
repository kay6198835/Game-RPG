using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
public class FastMovement : MonoBehaviour
{
    [SerializeField] protected Rigidbody2D rb;
    [SerializeField] protected Vector2 input;
    protected void Awake()
    {
        rb = GetComponentInParent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }
    protected void Update()
    {
        input.x = Input.GetAxisRaw(GameConstants.Input.HORIZONTAL);
        input.y = Input.GetAxisRaw(GameConstants.Input.VERTICAL);
        input.Normalize();
    }

    public void FixedUpdate()
    {
        rb.velocity = input * 10f;
    }
    public void SetVeclocity(Vector2 velocity)
    {
        rb.velocity = velocity;
    }
}
