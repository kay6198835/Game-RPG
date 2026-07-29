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
        idleDurationTime = entityData.IdleDurationTime;
        entityMovement.StopMove();
        // Should set by entityData
        idleDurationTime = 3;
    }
    public override void LogicUpdate()
    {
        base.LogicUpdate();
        idleTime += Time.deltaTime;
        if (idleTime >= idleDurationTime || entityFindTarget.DistanceToPlayer() >= 0)
        {
            idleTime = 0;
            entity.StateMachine.ChangeState(entity.MoveState);
            return;
        }
    }
}
