using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUseWeaponState : PlayerState
{
    public PlayerUseWeaponState(NewPlayer player, string animBoolName) : base(player, animBoolName)
    {
    }
    public override void Enter()
    {
        base.Enter();
        player.Core.Movement.SetVeclocity(Vector2.zero);
        player.Anim.SetFloat("Direction", player.InputHandler.DirectionMouse);
    }
    public override void Exit()
    {
        base.Exit();
    }
    public override void LogicUpdate()
    {
        base.LogicUpdate();
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
