using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDisadvantageState : PlayerState
{
    public PlayerDisadvantageState(NewPlayer player, string animBoolName) : base(player, animBoolName)
    {
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
