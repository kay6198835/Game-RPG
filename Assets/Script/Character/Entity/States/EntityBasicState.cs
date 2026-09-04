using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityBasicState : EntityState
{
    protected EntityMovement entityMovement;
    protected EntityInput entityInput;
    protected EntityWeaponHolder weaponHolder;
    protected EntityAttack entityAttack;
    protected EntityFindTarget entityFindTarget;
    protected EntityVitalStats entityVitalStats;
    public EntityBasicState(Entity etity, EntityStateMachine stateMachine, EntityData entityData, string animBoolName) : base(etity, stateMachine, entityData, animBoolName)
    {
    }
    public override void Enter()
    {
        base.Enter();
        entity.Core.GetCoreComponent(out entityMovement);
        entity.Core.GetCoreComponent(out entityInput);
        entity.Core.GetCoreComponent(out weaponHolder);
        entity.Core.GetCoreComponent(out entityAttack);
        entity.Core.GetCoreComponent(out entityFindTarget);
        entity.Core.GetCoreComponent(out entityVitalStats);
    }
    public override void LogicUpdate()
    {
        base.LogicUpdate();
        entityInput.DirectionMehod();
        entity.Anim.SetFloat(GameConstants.AnimationName.Parameter.DIRECTION, entityInput.DirectionLook);
        if (entityInput.IsTakeDamage)
        {
            if (entityVitalStats.GetCurrentStatValue(StatType.HP) <= 0)
            {
                stateMachine.ChangeState(entity.DeathState);
                return;
            }
            else
            {
                stateMachine.ChangeState(entity.TakeDamageState);
                return;
            }
        }
        if (entityAttack.CallAttack())
        {
            entity.StateMachine.ChangeState(entity.AttackState);
            return;
        }
        entityFindTarget.DistanceToPlayer();
        entityFindTarget.FindTargetMethod();
    }
    public override void Exit()
    {
        base.Exit();
        entityMovement = null;
        entityInput = null;
        weaponHolder = null;
        entityAttack = null;
        entityFindTarget = null;
        entityVitalStats = null;
    }

}
