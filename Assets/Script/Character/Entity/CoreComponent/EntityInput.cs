using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EntityInput : EntityCoreComponent<EntityCore>, IAimProvider
{
    public Vector2 AimDirection => directionLookVector;

    [SerializeField] protected Vector2 spawnPoint;
    // [SerializeField] protected Entity entity;
    [SerializeField] protected Vector2 targetFowardPosition;
    [SerializeField] protected Transform targetTransform;
    [Header("State")]
    [SerializeField] protected bool isTakeDamage = false;
    [SerializeField] protected bool isAttack;
    [SerializeField] protected bool isSkill;
    [SerializeField] protected bool isLockTarget = false;
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
    public Transform IsLockTarget { get => isLockTarget; }
    public Vector2 DirectionLookVector { get => directionLookVector; }
    //public float AngleSin { get => angleSin;}
    public float DirectionLookAngle { get => directionLookAngle; }
    public int DirectionLook { get => directionLook; }
    public Vector2 DirectionIsAttakedVector { get => directionIsAttakedVector; }
    public int DirectionIsAttaked { get => directionIsAttaked; }
    public float DirectionIsAttakedAngle { get => directionIsAttakedAngle; }
    public Vector2 SpawnPoint { get => spawnPoint; }
    public SkillState State { get => state; }
    public SkillType Skill { get => skill; }
    #endregion

    private EntityFindTarget entityFind;
    [SerializeField] private IPlayerService _playerService;
    [Inject]
    public void Construct(IPlayerService playerService)
    {
        _playerService = playerService;
    }
    protected override void Start()
    {
        base.Start();
        Core.GetCoreComponent(out entityFind);
        this.spawnPoint = this.transform.position;
        targetTransform = _playerService.GetPlayerTransform();
    }
    public void OnTakeDamage(Vector2 attackPosition)
    {
        ChangeIsTakeDamage();
        Invoke(nameof(ChangeIsTakeDamage), 0.1f);
        directionIsAttakedVector = ((attackPosition - (Vector2)this.transform.position)).normalized;
        AngleCalculate(directionIsAttakedVector, ref directionIsAttakedAngle, ref directionIsAttaked);
    }
    private void GetTargetInRange()
    {

        // fix need refactor
        if (targetTransform == null)
        {
            targetTransform = entityFind.FindTargetMethod(Core.Entity.Data.RangeCheckFieldOfView);
        }
        if (entityFind.FindTargetMethod(Core.Entity.Data.RangeCheckAttack) != null)
        {
            isAttack = true;
        }
        else
        {
            isAttack = false;
        }
    }
    private void AngleCalculate(Vector2 directionVector, ref float angle, ref int direction)
    {
        DirectionResolver.Calculate(directionVector, ref angle, ref direction);
    }
    private void DirectionMehod()
    {
        // if (targetTransform != null)
        // {
        //     directionLookVector = (targetTransform.position - transform.position).normalized;
        // }
        // else
        // {

        // }
        directionLookVector = (targetFowardPosition - (Vector2)transform.position).normalized;

        AngleCalculate(directionLookVector, ref directionLookAngle, ref directionLook);
    }
    public void SetTarget(Vector2 targetPosition)
    {
        this.targetFowardPosition = targetPosition;
    }

    public void SetDirectionRadom()
    {
        directionLookAngle = Random.Range(0f, 360f);
        directionLookAngle = Mathf.Round(directionLookAngle / 45f) * 45f;
        float radian = directionLookAngle * Mathf.Deg2Rad;
        float x = Mathf.Cos(radian);
        float y = Mathf.Sin(radian);
        directionLookVector = new Vector2(x, y).normalized * 100f - (Vector2)transform.position;
    }
    public void TurnLeftOrRight()
    {
        bool isRight = Random.Range(0, 2) == 0;
        int bonusAngle = Random.Range(45, 90);
        if (isRight)
        {
            directionLookAngle += 90f;
        }
        else
        {
            directionLookAngle -= 90f;
        }
        //directionLookAngle = Mathf.Round(directionLookAngle / 45f) * 45f;
        float radian = directionLookAngle * Mathf.Deg2Rad;
        float x = Mathf.Cos(radian);
        float y = Mathf.Sin(radian);
        directionLookVector = new Vector2(x, y).normalized * 100f - (Vector2)transform.position;
    }
    private void ChangeIsTakeDamage()
    {
        this.isTakeDamage = !this.isTakeDamage;
    }

    public void SetLockTarget(bool isLockTarget)
    {
        this.isLockTarget = isLockTarget;
    }

    public Vector2 TargetPosition()
    {
        if (!isLockTarget) return Vector2.zero;
        return targetTransform.position;
    }
}