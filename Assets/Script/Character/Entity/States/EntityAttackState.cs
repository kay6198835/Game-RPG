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
        // Same lifecycle as PlayerAttackState. EntityBasicState only enters here when CallAttack() passed,
        // i.e. a weapon is equipped.
        weaponHolder.Attack();
    }
    public override void LogicUpdate()
    {
        entity.Anim.SetFloat(GameConstants.AnimationName.Parameter.DIRECTION, entityInput.DirectionLook);

        switch (Status)
        {
            case StatusAnimation.Start:
                entityInput.SetTarget(entityInput.TargetPosition());
                break;
            case StatusAnimation.StartRangeTrigger:
                break;
            case StatusAnimation.OnActivate:
                weaponHolder.MakeDamage();
                Status = StatusAnimation.OffActivate;
                break;
            case StatusAnimation.OffActivate:
                break;
            case StatusAnimation.EndRangeTrigger:
                Status = StatusAnimation.End;
                break;
            case StatusAnimation.End:
                weaponHolder.SetRecovery();
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
            default:
                break;
        }
    }

    public override void Exit()
    {
        weaponHolder.EndDamage();
        // The weapon stage swapped the animator controller; restore the enemy's own.
        entity.Anim.runtimeAnimatorController = entityData.Aima;
        base.Exit();
    }
}
