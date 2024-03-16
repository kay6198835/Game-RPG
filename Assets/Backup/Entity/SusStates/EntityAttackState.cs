using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityAttackState : EntityUseWeaponState
{
    public EntityAttackState(Entity etity, EntityStateMachine stateMachine, EntityData entityData, string animBoolName) : base(etity, stateMachine, entityData, animBoolName)
    {
    }
    public override void Enter()
    {
        base.Enter();
    }
    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if(isAnimationFinished)
        {
            entity.StateMachine.ChangeState(entity.MoveToTargetState);
        }
    }
}
