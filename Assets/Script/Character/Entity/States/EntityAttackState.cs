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

    public override void Exit()
    {
        entityAttack.Exit();
        base.Exit();
    }
}
