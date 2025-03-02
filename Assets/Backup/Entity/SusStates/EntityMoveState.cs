using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityMoveState : EntityBasicState
{
    protected float moveTime;
    protected float moveDurationTime;
    protected Vector2 directionMoveVector;
    protected float speed;

    public EntityMoveState(Entity etity, EntityStateMachine stateMachine, EntityData entityData, string animBoolName) : base(etity, stateMachine, entityData, animBoolName)
    {

    }
    public override void Enter()
    {
        base.Enter();
        moveDurationTime = entityData.MoveDurationTime;
        moveTime = startTime + moveDurationTime;
        if (entity.Input.Target == null)
        {
            entity.Input.SetDirectionRadom();
        }
    }
    public override void LogicUpdate()
    {
        directionMoveVector = entity.Input.DirectionLookVector.normalized;
        speed = entityData.MovementVelocities;
        if(Vector2.Distance(entity.transform.position,entity.Input.Target.transform.position)<=10f)
        entityCore.EntityMovement.MoveForwardTarget(
        directionMoveVector * speed
        );
        if (entity.Input.Target == null)
        {
            if (entityCore.FindTarget.FindWall(directionMoveVector, speed))
            {
                Debug.Log("Turn");
                entity.Input.TurnLeftOrRight();
            }
            if (moveTime <= Time.time)
            {
                entity.StateMachine.ChangeState(entity.IdleState);
            }
        }


        base.LogicUpdate();
    }
    public override void DoChecks()
    {
        base.DoChecks();
    }
    public override void Exit()
    {
        base.Exit();
        entityCore.EntityMovement.MoveForwardTarget(Vector2.zero);
    }
}
