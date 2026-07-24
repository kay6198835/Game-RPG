using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityMoveState : EntityBasicState
{
    protected float moveTime;
    protected float moveDurationTime;
    protected Vector2 directionMoveVector;

    public EntityMoveState(Entity etity, EntityStateMachine stateMachine, EntityData entityData, string animBoolName) : base(etity, stateMachine, entityData, animBoolName)
    {

    }
    public override void Enter()
    {
        base.Enter();
        moveDurationTime = entityData.MoveDurationTime;
        moveTime = startTime + moveDurationTime;
        entityMovement.SendResquestPath();
    }
    public override void LogicUpdate()
    {
        directionMoveVector = entity.Input.DirectionLookVector.normalized;
        entity.Anim.SetFloat(GameConstants.AnimationName.Parameter.DIRECTION, directionMoveVector);
        base.LogicUpdate();
    }
    public override void DoChecks()
    {
        base.DoChecks();

    }
    public override void FixUpdate()
    {
        entityMovement.MoveToTarget();
    }
    public override void Exit()
    {
        base.Exit();
        entityMovement.StopMove();
    }
}
