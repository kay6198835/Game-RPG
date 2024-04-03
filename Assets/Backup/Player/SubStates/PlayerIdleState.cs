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
<<<<<<< HEAD
        stateStyle = StateStyle.Motion;
        //Debug.Log("Idle");
=======
>>>>>>> parent of e9a7753 (add Combo for Attack of Entity,  Dash State for Player (30/03/2024))
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (player.InputHandler.MoveVector != Vector2.zero)
        {
            stateMachine.ChangeState(player.MoveState);
        }
    }
    public override void Exit()
    {
        base.Exit();

        //Debug.Log("Exit Idle");
    }
}
