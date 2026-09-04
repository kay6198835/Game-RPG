using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class EntityMoveState : EntityBasicState
{
    protected float moveTime;
    protected float moveDurationTime;
    protected Vector2 directionMoveVector;
    private float time;
    private float distance;

    public EntityMoveState(Entity etity, EntityStateMachine stateMachine, EntityData entityData, string animBoolName) : base(etity, stateMachine, entityData, animBoolName)
    {

    }
    public override void Enter()
    {
        base.Enter();
        moveDurationTime = entityData.MoveDurationTime;
        moveTime = startTime + moveDurationTime;
    }
    public override void LogicUpdate()
    {
        if (!entityInput.HasTarget)
        {
            time += Time.deltaTime;
            if (time >= 10)
            {
                time = 0;
                entity.StateMachine.ChangeState(entity.IdleState);
                return;
            }
        }
        if (entityInput.IsLockTarget)
        {
            if (entityFindTarget.IsNearPlayer())
            {
                entityMovement.FleeTarget();
            }
            else
            {
                entityMovement.ChaseToTarget();
            }
        }
        else
        {
            entityMovement.ToRandomPosition();
        }
        entityMovement.MoveToNodeTarget();
        base.LogicUpdate();
    }
    public override void DoChecks()
    {
        base.DoChecks();
    }
    public override void PhysicsUpdate()
    {
        if (entityMovement != null) entityMovement.MoveForwardToTarget();
    }
    public override void Exit()
    {
        entityMovement.StopMove();
        base.Exit();
    }
}
