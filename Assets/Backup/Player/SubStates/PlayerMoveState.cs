using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveState : PlayerBasicState
{
    public PlayerMoveState(NewPlayer player, string animBoolName) : base(player, animBoolName)
    {
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        player.Core.Movement.SetVeclocity(moveVector*playerData.movementVelocities);
        if (moveVector == Vector2.zero)
        {
            stateMachine.ChangeState(player.IdleState);
        }
    }

    public override void Exit()
    {
        base.Exit();
        player.Core.Movement.SetVeclocity(Vector2.zero);
    }
}
