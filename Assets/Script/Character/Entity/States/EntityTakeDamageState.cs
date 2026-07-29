using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityTakeDamageState : EntityDisadvantageState
{
    EntityInput entityInput;
    EntityMovement entityMovement;
    public override void Enter()
    {
        base.Enter();
        entity.Core.GetCoreComponent(out entityMovement);
        entity.Core.GetCoreComponent(out entityInput);
        entityMovement.StopMove();
        entity.Anim.SetFloat(GameConstants.AnimationName.Parameter.DIRECTION, entityInput.DirectionIsAttaked);
    }
    public EntityTakeDamageState(Entity etity, EntityStateMachine stateMachine, EntityData entityData, string animBoolName) : base(etity, stateMachine, entityData, animBoolName)
    {
    }
    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (Status == StatusAnimation.EndRangeTrigger)
        {
            stateMachine.ChangeState(entity.MoveState);
        }
    }
    public override void Exit()
    {
        base.Exit();
        entityMovement = null;
        entityInput = null;
    }
}
