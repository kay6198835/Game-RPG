using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    [SerializeField] private Vector2 moveVector;
    [SerializeField] private Vector2 mouseVector;

    [SerializeField] private Vector2 directionVector;
    [SerializeField] private int direction;

    public Vector2 MoveVector { get => moveVector;}
    public Vector2 MouseVector { get => mouseVector;}
    public Vector2 DirectionVector { get => directionVector;}
    public int Direction { get => direction;}

    private void Update()
    {
        
    }
    private void FixedUpdate()
    {
        moveVector.x = Input.GetAxisRaw("Horizontal");
        moveVector.y = Input.GetAxisRaw("Vertical");
        mouseVector = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        AngleCalculate(mouseVector);
    }
    protected void DirectionCaculate(float angle)
    {

        if ((angle > 22 && angle <= 67))
        {
            direction = 0;
        }
        else if (angle > 67 && angle <= 112)
        {
            direction = 1;
        }
        else if (angle > 112 && angle <= 157)
        {
            direction = 2;

        }
        else if (angle > 157 && angle <= 202)
        {
            direction = 3;
        }
        else if (angle > 202 && angle <= 247)
        {
            direction = 4;
        }
        else if (angle > 247 && angle <= 292)
        {
            direction = 5;
        }
        else if (angle > 292 && angle <= 337)
        {
            direction = 6;
        }
        else if (angle > 337 || angle < 22)
        {
            direction = 7;
        }
    }
    public void AngleCalculate(Vector2 targetTowards)
    {
        float angle;
        directionVector = (Vector2)targetTowards - (Vector2)this.transform.position;
        angle = Mathf.Atan2(directionVector.x, directionVector.y) * Mathf.Rad2Deg;
        angle += 180;
        DirectionCaculate(angle);
    }
}