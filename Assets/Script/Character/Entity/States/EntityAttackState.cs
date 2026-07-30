using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityAttackState : EntityBasicState
{
    EntityAttack entityAttack;
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
        if (StatusAnimation.OnActivate <= Status && Status <= StatusAnimation.OffActivate)
        {
            entityAttack.Attack();
        }

        switch (Status)
        {
            case StatusAnimation.StartRangeTrigger:
                // OnColider
                break;
            case StatusAnimation.EndRangeTrigger:
                // OffColider
                break;
            case StatusAnimation.None:
                //player.Anim.SetBool(GameConstants.AnimationName.ATTACK, false);
                base.LogicUpdate();
                break;
            default:
                break;
        }
        base.LogicUpdate();
    }
}
