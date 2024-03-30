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
        idleTime = startTime + idleDurationTime;
        entityCore.EntityMovement.MoveForwardTarget(Vector2.zero);
        //Debug.Log("Start Idle");
    }
    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (idleTime <= Time.time || entity.Input.Target != null)
        {
            //Debug.Log("Idle");
            entity.StateMachine.ChangeState(entity.MoveState);
        }
    }
}
