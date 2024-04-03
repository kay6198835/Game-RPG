using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveState : PlayerBasicState
{
    public PlayerMoveState(NewPlayer player, string animBoolName) : base(player, animBoolName)
    {
    }
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
        {
            stateMachine.ChangeState(player.IdleState);
        }
        if(player.InputHandler.IsDash)
        {
            stateMachine.ChangeState(player.DashState);
        }
        base.LogicUpdate();
    }
    public override void Exit()
    {
        base.Exit();
        player.Core.Movement.SetVeclocity(Vector2.zero);
        //Debug.Log("Eixt Run");
    }
}
