using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : State
{
    [SerializeField] private AttackState attackState;
    [SerializeField] private CheckIsInRange checkInRangeAttack;
    [SerializeField] private MovementEnemy movement;
    public CheckIsInRange CheckInRangeAttack { get => checkInRangeAttack;}
    private void Awake()
    {
        LoadState();
        //idleState =this.transform.parent.GetComponentInChildren<IdleState>();
        attackState = this.transform.parent.GetComponentInChildren<AttackState>();
        checkInRangeAttack = stateManager.CheckInRange;
        movement = stateManager.Movement;
    }
    private void Start()
    {
        StartCurrentState();
    }
    public override void StartCurrentState()
    {
        checkInRangeAttack.TargetMask = stateManager.Stats.layerMask;
        checkInRangeAttack.Range = stateManager.Stats.attackRange;
        movement.Speed = stateManager.Stats.speedMove;
        movement.Player = stateManager.Target;
    }
    public override State RunCurrentState()
    {
        stateManager.EnemyCtrl.AnimationManager.Animation_2_Run();
        movement.Move();
        checkInRangeAttack.Check();
        stateManager.IsInAttackRange = checkInRangeAttack.IsInRange;
        return CheckState();
    }
    public override State CheckState()
    {
        if (stateManager.IsInAttackRange)
        {
            return attackState;
        }
        else
        {
            return this;
        }
    }
}
