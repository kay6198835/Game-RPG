using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDisadvantageState : PlayerState
{
    protected PlayerInputHandler inputHandler;
    public PlayerDisadvantageState(Player player, string animBoolName) : base(player, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.Core.GetCoreComponent(out inputHandler);
    }
    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (Status == StatusAnimation.EndRangeTrigger)
        {
            if (inputHandler.MoveVector == Vector2.zero)
            {
                stateMachine.ChangeState(player.IdleState);
                return;
            }
            else
            {
                stateMachine.ChangeState(player.MoveState);
                return;
            }
        }
    }
}
