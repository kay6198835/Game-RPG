using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class EntityAttackState : EntityBasicState
{
    float startAttackTime;
    public EntityAttackState(Entity etity, EntityStateMachine stateMachine, EntityData entityData, string animBoolName) : base(etity, stateMachine, entityData, animBoolName)
    {

    }
    public override void Enter()
    {
        base.Enter();
        startAttackTime = startTime;

    }
    public override void LogicUpdate()
    {
        entity.Anim.SetFloat(GameConstants.AnimationName.Parameter.DIRECTION, entityInput.DirectionLook);

        switch (Status)
        {
            case StatusAnimation.Start:
                entityInput.SetTarget(entityInput.TargetTransform.position);
                break;
            case StatusAnimation.StartRangeTrigger:
                break;
            case StatusAnimation.OnActivate:
                entityAttack.Attack();
                Status = StatusAnimation.OffActivate;
                break;
            case StatusAnimation.OffActivate:
                break;
            case StatusAnimation.EndRangeTrigger:
                Status = StatusAnimation.End;
                break;
            case StatusAnimation.End:
                entityAttack.SetRecovery();
                if (entityFindTarget.IsInRangeAttack())
                {
                    stateMachine.ChangeState(entity.IdleState);
                    return;
                }
                else
                {
                    stateMachine.ChangeState(entity.MoveState);
                    return;
                }
                break;
            default:
                break;
        }
    }

    public override void Exit()
    {
        entityAttack.Exit();
        base.Exit();
    }
}
