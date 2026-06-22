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


                //player.Anim.SetBool(GameConstants.AnimationName.ATTACK, true);
                break;
            case StatusAnimation.EndRangeTrigger:
                if (inputHandler.BufferIsAttack || inputHandler.IsAttack)            // dòng 40
                {                                                                     // dòng 41 (anchor)
                    Debug.Log($"[ATK-END] advance buffer={inputHandler.BufferIsAttack} isAtk={inputHandler.IsAttack} normT={player.Anim.GetCurrentAnimatorStateInfo(0).normalizedTime:F2} t={Time.time:F3} f={Time.frameCount}");
                    inputHandler.SetBufferAttack(false);
                    weaponHolder.Weapon.SetAnimation(player);
                    int stateHash = player.Anim.GetCurrentAnimatorStateInfo(0).fullPathHash;
                    player.Anim.Play(stateHash, 0, 0f);
                    Status = StatusAnimation.Start;
                }
                else
                {
                    Debug.Log(Status);
                    Status = StatusAnimation.None;
                }
                break;
            case StatusAnimation.None:
                //player.Anim.SetBool(GameConstants.AnimationName.ATTACK, false);
                base.LogicUpdate();
                break;
            default:
                break;
        }
    }

    public override void SetAnimationStatus(StatusAnimation statusAnimation)
    {                                                                                  // dòng 64 (anchor)
        Debug.Log($"[ATK-EVT] {statusAnimation} normT={player.Anim.GetCurrentAnimatorStateInfo(0).normalizedTime:F2} t={Time.time:F3} f={Time.frameCount}");
        base.SetAnimationStatus(statusAnimation);                                      // dòng 65
    }
}
