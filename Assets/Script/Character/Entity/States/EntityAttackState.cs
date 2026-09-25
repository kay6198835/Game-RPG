using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class EntityAttackState : EntityBasicState
{
    float startAttackTime;
    bool usesWeapon;
    public EntityAttackState(Entity etity, EntityStateMachine stateMachine, EntityData entityData, string animBoolName) : base(etity, stateMachine, entityData, animBoolName)
    {

    }
    public override void Enter()
    {
        base.Enter();
        startAttackTime = startTime;
        // Same lifecycle as PlayerAttackState when a Weapon is equipped; EntityAttack otherwise.
        usesWeapon = weaponHolder != null && weaponHolder.CanAttack();
        if (usesWeapon) weaponHolder.Attack();

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
                if (usesWeapon) weaponHolder.MakeDamage();
                else entityAttack.Attack();
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
        if (usesWeapon)
        {
            weaponHolder.EndDamage();
            // The weapon stage swapped the animator controller; restore the enemy's own.
            entity.Anim.runtimeAnimatorController = entityData.Aima;
        }
        entityAttack.Exit();
        base.Exit();
    }
}
