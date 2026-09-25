using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityBasicState : EntityState
{
    protected EntityMovement entityMovement;
    protected EntityInput entityInput;
    protected EntityWeaponHolder weaponHolder;
    protected EntityFindTarget entityFindTarget;
    protected EntityVitalStats entityVitalStats;
    protected EntityAbilityHolder abilityHolder;
    public EntityBasicState(Entity etity, EntityStateMachine stateMachine, EntityData entityData, string animBoolName) : base(etity, stateMachine, entityData, animBoolName)
    {
    }
    public override void Enter()
    {
        base.Enter();
        entity.Core.GetCoreComponent(out entityMovement);
        entity.Core.GetCoreComponent(out entityInput);
        entity.Core.GetCoreComponent(out weaponHolder);
        entity.Core.GetCoreComponent(out entityFindTarget);
        entity.Core.GetCoreComponent(out entityVitalStats);
        entity.Core.GetCoreComponent(out abilityHolder);
    }
    public override void LogicUpdate()
    {
        base.LogicUpdate();
        entityInput.DirectionMethod();
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
        // Optional component: enemies without EntityAbilityHolder behave exactly as before.
        if (abilityHolder != null)
        {
            abilityHolder.Processing();
            if (entityFindTarget.HasTarget && entityFindTarget.IsInRangeAttack() && abilityHolder.TryDoAnyAbility())
            {
                stateMachine.ChangeState(entity.AbilityState);
                return;
            }
        }
        // No weapon, no attack: CallAttack() is false while nothing is equipped.
        if (weaponHolder != null && weaponHolder.CallAttack() && entityFindTarget.IsInRangeAttack())
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
        entityFindTarget = null;
        entityVitalStats = null;
        abilityHolder = null;
    }

}
