using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityIdleState : EntityBasicState
{
    private float idleTime;
    private float idleDurationTime;

    public EntityIdleState(Entity etity, EntityStateMachine stateMachine, EntityData entityData, string animBoolName) : base(etity, stateMachine, entityData, animBoolName)
    {
    }
    public override void Enter()
    {
        base.Enter();
        //entityCore.EntityMovement.MoveForwardTarget(Vector2.zero);
        idleDurationTime = entityData.IdleDurationTime;
        idleTime = startTime + idleDurationTime;
    }
    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if(idleTime <= Time.time)
        {
            entity.StateMachine.ChangeState(entity.MoveRandomState);
        }
        if (entity.InputHandler.Target!=null)
        {
            entity.StateMachine.ChangeState(entity.MoveToTargetState);
        }
    }
}
