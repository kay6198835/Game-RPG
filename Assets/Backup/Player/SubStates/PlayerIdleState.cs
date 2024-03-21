using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerBasicState
{
    public PlayerIdleState(NewPlayer player, string animBoolName) : base(player, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.Core.Movement.SetVeclocity(Vector2.zero);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (moveVector != Vector2.zero)
        {
            stateMachine.ChangeState(player.MoveState);
        }
    }
}
