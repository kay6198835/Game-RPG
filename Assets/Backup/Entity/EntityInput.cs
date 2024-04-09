using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EntityInput : MonoBehaviour
{
    [SerializeField] protected Entity entity;
    [SerializeField] protected Transform target;
    [Header("State")]
    [SerializeField] protected bool isTakeDamage=false;
    [SerializeField] protected bool isAttack;
    [SerializeField] protected bool isSkill;
    [Header("Direction Look")]
    [SerializeField] protected Vector2 directionLookVector;
    [SerializeField] protected int directionLook;
    [SerializeField] protected float directionLookAngle;
    [Header("Direction TakeDamage")]
    [SerializeField] private Vector2 directionIsAttakedVector;
    [SerializeField] private int directionIsAttaked;
    [SerializeField] private float directionIsAttakedAngle;
    [Header("Skill")]
    [SerializeField] private SkillState state;
    [SerializeField] private SkillType skill;
    public enum SkillState
    {
        Start,
        Cast,
        Do,
    }
    public enum SkillType
    {
        Special,
        Ability
    }
    #region Read_Value 
    public bool IsTakeDamage { get => isTakeDamage; }
    public bool IsAttack { get => isAttack; }
    public bool IsSkill { get => isSkill; }


    public Transform Target { get => target; }
    public Vector2 DirectionLookVector { get => directionLookVector; }
    public Entity Entity { get => entity;}
    //public float AngleSin { get => angleSin;}
    public float DirectionLookAngle { get => directionLookAngle;}
    public int DirectionLook { get => directionLook;}
    public Vector2 DirectionIsAttakedVector { get => directionIsAttakedVector;}
    public int DirectionIsAttaked { get => directionIsAttaked;}
    public float DirectionIsAttakedAngle { get => directionIsAttakedAngle; }
    public SkillState State { get => state;}
    public SkillType Skill { get => skill;}
    #endregion

    private void Awake()
    {
        entity = GetComponentInParent<Entity>();
    }
    private void Update()
    {
        DirectionMehod();
        GetTargetInRange();
    }
    public void OnTakeDamage(Vector2 attackPosition)
    {
        ChangeIsTakeDamage();
        Invoke(nameof(ChangeIsTakeDamage), 0.1f);
        directionIsAttakedVector = ((attackPosition - (Vector2)this.transform.position)).normalized;
        AngleCalculate(directionIsAttakedVector,ref directionIsAttakedAngle , ref directionIsAttaked);
    }
    private void GetTargetInRange()
    {
        if (target == null)
        {
            target = entity.Core.FindTarget.FindTargetMethod(entity.Data.RangeCheckFieldOfView);
        }
        if (entity.Core.FindTarget.FindTargetMethod(entity.Data.RangeCheckAttack) != null)
        {
            isAttack = true;
        }
        else
        {
            isAttack = false;
        }
    }
    private void AngleCalculate(Vector2 directionVector,ref float angle, ref int direction)
    {
        angle = Mathf.Atan2(directionVector.x, directionVector.y) * Mathf.Rad2Deg;
        angle += 180;
        DirectionCaculate(angle,ref direction);
    }
    private void DirectionCaculate(float angle,ref int direction)
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
    private void DirectionMehod()
    {
        if (target != null)
        {
            directionLookVector = (target.position - transform.position).normalized;
        }
        AngleCalculate(directionLookVector,ref directionLookAngle ,ref directionLook);
    }
    public void SetDirectionRadom()
    {
        float angle = Random.Range(0f, 360f);
        angle = Mathf.Round(angle / 45f) * 45f;
        float radian = angle * Mathf.Deg2Rad;
        float x = Mathf.Cos(radian);
        float y = Mathf.Sin(radian);
        directionLookVector = new Vector2(x, y).normalized*100f - (Vector2)transform.position;
    }
    private void ChangeIsTakeDamage()
    {
        this.isTakeDamage = !this.isTakeDamage;
    }
}
