using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityMoveToTargetState : EntityMoveState
{
    public EntityMoveToTargetState(Entity etity, EntityStateMachine stateMachine, EntityData entityData, string animBoolName) : base(etity, stateMachine, entityData, animBoolName)
    {
    }
    public override void LogicUpdate()
    {
        base.LogicUpdate();
        directionMoveVector = entity.InputHandler.DirectionMoveVector;
        if (entity.EntityCore.FindTarget.FindTargetMethod(entity.EntityData.RangeCheckAttack)!=null )
        {
            entity.StateMachine.ChangeState(entity.AttackState);
        }
    }
}
