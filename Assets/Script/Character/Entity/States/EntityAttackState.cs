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
        entity.Core.GetCoreComponent(out entityAttack);
        startAttackTime = startTime;
    }
    public override void LogicUpdate()
    {
        entity.Anim.SetFloat(GameConstants.AnimationName.Parameter.DIRECTION, entityInput.DirectionLook);

        switch (Status)
        {
            case StatusAnimation.StartRangeTrigger:
                Status = StatusAnimation.OnActivate;
                entityAttack.Attack();
                // OnColider
                break;
            case StatusAnimation.EndRangeTrigger:
                // OffColider
                break;
            case StatusAnimation.None:
                //player.Anim.SetBool(GameConstants.AnimationName.ATTACK, false);
                if (entityAttack.IsInRangeAttack())
                {
                    if (entityFindTarget.DistanceToPlayer() > 0)
                    {
                        stateMachine.ChangeState(entity.MoveState);
                        return;
                    }
                    else
                    {
                        stateMachine.ChangeState(entity.IdleState);
                        return;
                    }
                }

                //Check why warning when change state
                break;
            default:
                break;
        }
    }
}
