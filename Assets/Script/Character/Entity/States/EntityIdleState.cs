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
    }
    public override void LogicUpdate()
    {
        idleTime += Time.deltaTime;
        bool leave;
        if (entityFindTarget.HasTarget)
        {
            // Locked: hold position while the target stays in the attack band; face it so the
            // next attack and the view cone point at the player, not at the last waypoint.
            entityInput.SetTarget(entityInput.TargetPosition());
            leave = !entityFindTarget.IsInRangeAttack();
        }
        else
        {
            leave = idleTime >= idleDurationTime;
        }
        if (leave)
        {
            idleTime = 0;
            entity.StateMachine.ChangeState(entity.MoveState);
            return;
        }
        base.LogicUpdate();
    }
}
