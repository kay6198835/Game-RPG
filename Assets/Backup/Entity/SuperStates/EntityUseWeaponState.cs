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
        entity.Core.EntityMovement.MoveForwardTarget(Vector2.zero);
        entity.Anim.SetFloat("Direction", entity.Input.DirectionLook);

    }
    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (isAnimationFinished)
        {
            entity.StateMachine.ChangeState(entity.MoveState);
            Debug.Log("Attack");
        }
    }
}
