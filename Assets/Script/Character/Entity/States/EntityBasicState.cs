using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityBasicState : EntityState
{
    public EntityBasicState(Entity etity, EntityStateMachine stateMachine, EntityData entityData, string animBoolName) : base(etity, stateMachine, entityData, animBoolName)
    {
    }
    public override void Enter()
    {
        base.Enter();
    }
    public override void LogicUpdate()
    {
        base.LogicUpdate();
        entity.Anim.SetFloat(GameConstants.AnimationName.Parameter.DIRECTION, entity.Input.DirectionLook);
        if (entity.Input.IsTakeDamage)
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
        if (entity.Input.IsAttack && entity.Core.WeaponHolder.Weapon.CheckCanAttack(entity, startTime))
        {
            entity.StateMachine.ChangeState(entity.AttackState);
        }
    }

}
