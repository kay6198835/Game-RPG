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
            case StatusAnimation.OnActivate:
                Debug.Log("OnActivate");
                entityAttack.Attack();
                Status = StatusAnimation.OffActivate;
                break;
            case StatusAnimation.EndRangeTrigger:
                Status = StatusAnimation.None;
                break;
            case StatusAnimation.None:
                if (entityAttack.IsInRangeAttack())
                {
                    int stateHash = entity.Anim.GetCurrentAnimatorStateInfo(0).fullPathHash;
                    entity.Anim.Play(stateHash, 0, 0f);
                    Status = StatusAnimation.Start;
                    return;
                }
                else if (!entityFindTarget.IsNearPlayer() && entityFindTarget.OutOfRange())
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
