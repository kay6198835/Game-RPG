using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashState : PlayerBasicState
{
    public PlayerDashState(NewPlayer player, string animBoolName) : base(player, animBoolName)
    {
    }
    public override void Enter()
    {
        base.Enter();
    }
    public override void LogicUpdate()
    {
        base.LogicUpdate();
        player.Core.Movement.SetVeclocity(player.InputHandler.MoveVector * 30);
        if (!player.InputHandler.IsDash)
        {
            stateMachine.ChangeState(player.IdleState);
        }
    }
}
