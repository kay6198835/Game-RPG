using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDisadvantageState : PlayerState
{
    protected PlayerInputHandler playerInputHandler;
    public PlayerDisadvantageState(Player player, string animBoolName) : base(player, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.Core.GetCoreComponent(out playerInputHandler);
    }
    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (isAnimationFinished)
        {
            if (playerInputHandler.MoveVector == Vector2.zero)
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
