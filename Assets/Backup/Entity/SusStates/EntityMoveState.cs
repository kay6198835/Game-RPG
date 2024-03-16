using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityMoveState : EntityBasicState
{
    protected float moveTime;
    protected float moveDurationTime;
    protected Vector2 directionMoveVector;
    protected bool isFindTargeted;
    protected bool isMove;

    public float MoveTime { get => moveTime;}

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
        base.LogicUpdate();
        entityCore.EntityMovement.MoveForwardTarget(directionMoveVector.normalized*entityData.MovementVelocities);
    }
    public override void Exit()
    {
        base.Exit();
        entityCore.EntityMovement.MoveForwardTarget(Vector2.zero);
    }
}
