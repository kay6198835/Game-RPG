using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseWeaponState : PlayerState
{
    public UseWeaponState(NewPlayer player, PlayerStateMachine playerStateMachine, PlayerData playerData, string animBoolName) : base(player, playerStateMachine, playerData, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.Anim.SetFloat("Direction", player.InputHandler.Direction);
    }

    public override void Exit()
    {
        base.Exit();

    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        player.Core.Movement.SetVeclocity(Vector2.zero);
        if (isAnimationFinished)
        {
            if (player.InputHandler.MoveVector == Vector2.zero)
            {
                stateMachine.ChangeState(player.IdleState);
            }
            else
            {
                stateMachine.ChangeState(player.MoveState);
            }
        }
        
    }
}
