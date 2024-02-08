using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : State
{
    [SerializeField] private ChaseState chaseState;
    [SerializeField] private EnemyAttack enemyAttack;
    [SerializeField] private CheckIsInRange checkInRangeAttack;

    [SerializeField] protected float lastAttack = 0;

    private void Awake()
    {
        LoadState();
        chaseState = this.transform.parent.GetComponentInChildren<ChaseState>();
        checkInRangeAttack = stateManager.CheckInRange;
        enemyAttack = stateManager.EnemyAttack;
    }
    private void Start()
    {
        StartCurrentState();
    }

    public override void StartCurrentState()
    {
        checkInRangeAttack.TargetMask = stateManager.Stats.layerMask;
        checkInRangeAttack.Range = stateManager.Stats.attackRange;
        enemyAttack.AttackRange = stateManager.Stats.attackRange;
        enemyAttack.PlayerMask = stateManager.Stats.layerMask;
        enemyAttack.AttackDamage = stateManager.Stats.damage;
        enemyAttack.AttackRate = stateManager.Stats.rateAttack;
    }
    public override State RunCurrentState()
    {
        if (lastAttack + enemyAttack.AttackRate <= Time.time)
        {
            stateManager.EnemyCtrl.AnimationManager.Animation_6_Attack();
            lastAttack = Time.time;
        }
        if (stateManager.IsAttack)
        {
            enemyAttack.Attack();
        }
        checkInRangeAttack.Check();
        stateManager.IsInAttackRange = checkInRangeAttack.IsInRange;
        stateManager.IsAttack = false;
        return CheckState();
    }
    public override State CheckState()
    {
        if(stateManager.IsInAttackRange) 
        {
            return this;
        }
        else
        {
            return chaseState;
        }
    }
}
