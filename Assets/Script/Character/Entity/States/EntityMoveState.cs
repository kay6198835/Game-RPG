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
        entityMovement.SendResquestPath();
    }
    public override void LogicUpdate()
    {
        // base.LogicUpdate();
        entity.Anim.SetFloat(GameConstants.AnimationName.Parameter.DIRECTION, entity.Input.DirectionLook);
        time += Time.deltaTime;
        // if (!entityInput.TargetTransform)
        // {
        //     if (time >= 5)
        //     {
        //         time = 0;
        //         //Change to Idle State
        //     }
        // }
        // else
        // {
        //     // if (time >= 1)
        //     // {
        //     //     time = 0;
        //     //     entityMovement.SendResquestPath();
        //     // }
        // }

        entityMovement.CheckMove();
    }
    public override void DoChecks()
    {
        base.DoChecks();

    }
    public override void PhysicsUpdate()
    {
        entityMovement.MoveToTarget();
    }
    public override void Exit()
    {
        base.Exit();
        entityMovement.StopMove();
    }
}
