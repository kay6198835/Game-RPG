using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackState : PlayerUseWeaponState
{
    private float startAttackTime;

    public PlayerAttackState(Player player, string animBoolName) : base(player, animBoolName)
    {
    }

    public float StartAttackTime { get => startAttackTime; }

    public override void Enter()
    {
        base.Enter();
        startAttackTime = startTime;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        // if (isAnimationFinishedTrigger && inputHandler.IsAttack)
        // {
        //     stateMachine.ChangeState(player.AttackState);
        // }
        switch (Status)
        {
            case StatusAnimation.Start:
                player.Anim.SetBool(GameConstants.AnimationName.ATTACK, true);
                break;
            case StatusAnimation.EndRangeTrigger:
                if (inputHandler.BufferIsAttack)
                {
                    Debug.Log("On Check Attack");
                    weaponHolder.Weapon.SetAnimation(player);
                    inputHandler.SetBufferAttack(false);
                }
                Status = StatusAnimation.None;
                break;
            case StatusAnimation.None:
                player.Anim.SetBool(GameConstants.AnimationName.ATTACK, false);
                base.LogicUpdate();
                break;
            default:
                break;
        }
    }

    public override void SetAnimationStatus(StatusAnimation statusAnimation)
    {
        base.SetAnimationStatus(statusAnimation);
        Debug.Log("Event Call: " + statusAnimation);
    }
}
