using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveState : BasicState
{
    public PlayerMoveState(NewPlayer player, PlayerStateMachine playerStateMachine, PlayerData playerData, string animBoolName) : base(player, playerStateMachine, playerData, animBoolName)
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
