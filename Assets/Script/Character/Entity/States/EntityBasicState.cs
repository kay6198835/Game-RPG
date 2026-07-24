using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityBasicState : EntityState
{
    protected EntityMovement entityMovement;
    protected EntityInput entityInput;
    protected EntityWeaponHolder weaponHolder;
    public EntityBasicState(Entity etity, EntityStateMachine stateMachine, EntityData entityData, string animBoolName) : base(etity, stateMachine, entityData, animBoolName)
    {
    }
    public override void Enter()
    {
        base.Enter();
        entity.Core.GetCoreComponent(out entityMovement);
        entity.Core.GetCoreComponent(out entityInput);
        entity.Core.GetCoreComponent(out weaponHolder);

    }
    public override void LogicUpdate()
    {
        base.LogicUpdate();
        entity.Anim.SetFloat(GameConstants.AnimationName.Parameter.DIRECTION, entity.Input.DirectionLook);
        if (entityInput.IsTakeDamage)
        {
            if (entity.Data.StatsSO.Health <= 0)
            {
                stateMachine.ChangeState(entity.DeathState);
            }
            else
            {
                stateMachine.ChangeState(entity.TakeDamageState);
            }
        }
        if (entityInput.IsAttack && weaponHolder.Weapon.CheckCanAttack(entity, startTime))
        {
            entity.StateMachine.ChangeState(entity.AttackState);
        }
    }
    public override void Exit()
    {
        entityMovement = null;
        entityInput = null;
        weaponHolder = null;
    }

}
