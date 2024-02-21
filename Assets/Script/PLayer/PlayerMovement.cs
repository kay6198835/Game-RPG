using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : CoreCompoment
{
    [SerializeField] protected Rigidbody2D rb;
    [SerializeField] protected float moveSpeed;

    [Header("Direction")]
    [SerializeField] protected float angle;
    [SerializeField] protected int direction;
    [SerializeField] protected Vector2 directionVector;
    [SerializeField] protected Vector2 targetTowards;
    public float MoveSpeed { get => moveSpeed; }
    private void Awake()
    {
        rb = GetComponentInParent<Rigidbody2D>();
    }
    protected void Start()
    {   
    }
    public void Move(Vector2 moveVector, float moveSpeed)
    {
        rb.velocity = moveVector.normalized * moveSpeed * Time.deltaTime;
    }
    protected void DirectionCharacter()
    {
        if ((angle > 22 && angle <= 67))
        {
            direction = 0;
        }
        //Direction 2
        else if (angle > 67 && angle <= 112)
        {
            direction = 1;
        }
        //Direction 3
        else if (angle > 112 && angle <= 157)
        {
            direction = 2;

        }
        //Direction 4
        else if (angle > 157 && angle <= 202)
        {
            direction = 3;
        }
        //Direction 5
        else if (angle > 202 && angle <= 247)
        {
            direction = 4;
        }
        //Direction 6
        else if (angle > 247 && angle <= 292)
        {
            direction = 5;
        }
        //Direction 7
        else if (angle > 292 && angle <= 337)
        {
            direction = 6;
        }
        //Direction 8
        else if (angle > 337 || angle < 22)
        {
            direction = 7;
        }
    }
    public void AngleCalculate(Vector2 targetTowards)
    {
        this.targetTowards = targetTowards;
        if (this.targetTowards != Vector2.zero)
        {
            directionVector = (Vector2)this.targetTowards - (Vector2)this.transform.position;
        }
        angle = Mathf.Atan2(directionVector.x, directionVector.y) * Mathf.Rad2Deg;
        angle += 180;
    }
}
