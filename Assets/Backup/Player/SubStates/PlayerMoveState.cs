using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveState : PlayerBasicState
{
    public PlayerMoveState(NewPlayer player, string animBoolName) : base(player, animBoolName)
    {
    }
<<<<<<< HEAD
    public override void Enter()
    {
        base.Enter();
        stateStyle = StateStyle.Motion;
        //Debug.Log("Run");
    }
    public override void LogicUpdate()
    {
        player.Core.Movement.SetVeclocity(player.InputHandler.MoveVector * playerData.movementVelocities);
        if (player.InputHandler.MoveVector == Vector2.zero)
=======

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        player.Core.Movement.SetVeclocity(moveVector*playerData.movementVelocities);
        if (moveVector == Vector2.zero)
>>>>>>> parent of e9a7753 (add Combo for Attack of Entity,  Dash State for Player (30/03/2024))
        {
            stateMachine.ChangeState(player.IdleState);
        }
    }

    public override void Exit()
    {
        base.Exit();
        player.Core.Movement.SetVeclocity(Vector2.zero);
        //Debug.Log("Eixt Run");
    }
}
