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
        

        if (!entityInput.TargetTransform)
        {
            time += Time.deltaTime;
            if (time >= 10)
            {
                time = 0;
                entity.StateMachine.ChangeState(entity.IdleState);
                return;
            }
        }

        if (entityFindTarget.IsNearPlayer())
        {
            entity.StateMachine.ChangeState(entity.IdleState);
            return;
            //Back Foward 
            // 3-5 unit for range entity
            // 2-3 unit for melee entity
        }
        entityMovement.CheckMove();
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
