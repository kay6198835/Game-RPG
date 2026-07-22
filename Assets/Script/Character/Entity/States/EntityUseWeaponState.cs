using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityUseWeaponState : EntityState
{
    public EntityUseWeaponState(Entity etity, EntityStateMachine stateMachine, EntityData entityData, string animBoolName) : base(etity, stateMachine, entityData, animBoolName)
    {
    }
    public override void Enter()
    {
        base.Enter();
        //move
        entity.Anim.SetFloat(GameConstants.AnimationName.Parameter.DIRECTION, entity.Input.DirectionLook);
    }
    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (Status == StatusAnimation.EndRangeTrigger)
        {
            entity.StateMachine.ChangeState(entity.IdleState);
            //Debug.Log("Attack");
        }
    }
}
