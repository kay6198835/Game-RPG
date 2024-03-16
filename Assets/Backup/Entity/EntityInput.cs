using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EntityInput : MonoBehaviour
{
    [SerializeField] private Entity entity;
    [SerializeField] private Transform target;
    [SerializeField] private Vector2 directionMoveVector;
    [SerializeField] private int direction;
    [SerializeField] private float angleSin;
    [SerializeField] private float angleDirection;
    #region Read_Value 
    public Transform Target { get => target; }
    public Vector2 DirectionMoveVector { get => directionMoveVector; }
    public Entity Entity { get => entity;}
    public float AngleSin { get => angleSin;}
    public float AngleDirection { get => angleDirection;}
    public int Direction { get => direction;}
    #endregion

    private void Awake()
    {
        entity = GetComponentInParent<Entity>();
    }
    private void Update()
    {
        DirectionMehod();
        if(target == null)
        {
            target = entity.EntityCore.FindTarget.FindTargetMethod(entity.EntityData.RangeCheckFieldOfView);
        }
    }

    private void AngleCalculate(Vector2 directionVector)
    {
        float angle;
        //directionVector = ((Vector2)targetTowards - (Vector2)this.transform.position).normalized;
        angle = Mathf.Atan2(directionVector.x, directionVector.y) * Mathf.Rad2Deg;
        angle += 180;
        DirectionCaculate(angle);
        angleDirection = angle;
        this.angleSin = Vector2.SignedAngle(transform.right, directionVector);
        this.angleSin = (this.angleSin + 360) % 360;
    }
    private void DirectionCaculate(float angle)
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
        //entity.Anim.SetFloat("Direction", direction);
    }
    public void DirectionMehod()
    {
        if (target != null)
        {
            directionMoveVector = (target.position - transform.position).normalized;
        }
        AngleCalculate(directionMoveVector);

    }
    public Vector2 DirectionRadom(EntityMoveRandomState entityState)
    {
        float angle = Random.Range(0f, 360f);
        angle = Mathf.Round(angle / 45f) * 45f; // Làm tròn góc để chia thành 8 hướng
        float radian = angle * Mathf.Deg2Rad;
        float x = Mathf.Cos(radian);
        float y = Mathf.Sin(radian);
        return directionMoveVector = new Vector2(x, y).normalized*100f - (Vector2)transform.position;
    }
}
