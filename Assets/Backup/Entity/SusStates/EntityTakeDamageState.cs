using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityTakeDamageState : EntityDisadvantageState
{
    public override void Enter()
    {
        base.Enter();
        entity.Anim.SetFloat("Direction", entity.Input.DirectionIsAttaked);
    }
    public EntityTakeDamageState(Entity etity, EntityStateMachine stateMachine, EntityData entityData, string animBoolName) : base(etity, stateMachine, entityData, animBoolName)
    {
    }
    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (isAnimationFinished)
        {
            stateMachine.ChangeState(entity.MoveState);
        }
    }
    public override void Exit()
    {
        base.Exit();
    }
}
