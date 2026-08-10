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
        weaponHolder.Attack();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        switch (Status)
        {
            case StatusAnimation.Start:
                if (inputHandler.BufferIsAttack)
                {
                    inputHandler.SetBufferAttack(false);
                }
                //player.Anim.SetBool(GameConstants.AnimationName.ATTACK, true);
                break;
            case StatusAnimation.OnActivate:
                weaponHolder.MakeDamage();
                Status = StatusAnimation.OffActivate;
                break;
            case StatusAnimation.OffActivate:

                break;
            case StatusAnimation.EndRangeTrigger:
                if (inputHandler.BufferIsAttack || inputHandler.IsAttack)
                {
                    weaponHolder.Attack();
                    int stateHash = player.Anim.GetCurrentAnimatorStateInfo(0).fullPathHash;
                    player.Anim.Play(stateHash, 0, 0f);
                    Status = StatusAnimation.Start;
                }
                break;
            case StatusAnimation.End:
                base.LogicUpdate();
                break;
            case StatusAnimation.None:
                break;
            default:
                inputHandler.SetStatusAnimation(Status);
                break;
        }

    }

    public override void SetAnimationStatus(StatusAnimation statusAnimation)
    {
        base.SetAnimationStatus(statusAnimation);
    }
}
