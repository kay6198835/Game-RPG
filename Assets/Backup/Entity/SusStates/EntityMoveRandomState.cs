using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityMoveRandomState : EntityMoveState
{
    public EntityMoveRandomState(Entity etity, EntityStateMachine stateMachine, EntityData entityData, string animBoolName) : base(etity, stateMachine, entityData, animBoolName)
    {


    }
    public override void Enter()
    {
        base.Enter();
        directionMoveVector = entity.InputHandler.DirectionRadom(this);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (moveTime <= Time.time)
        {
            entity.StateMachine.ChangeState(entity.IdleState);
        }
        if (entity.InputHandler.Target != null)
        {
            entity.StateMachine.ChangeState(entity.MoveToTargetState);
        }

    }
}
