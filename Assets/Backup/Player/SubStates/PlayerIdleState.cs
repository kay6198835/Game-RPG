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
        //stateStyle = StateStyle.Motion;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        player.Anim.SetFloat("Direction", player.Data.StatsBehavior.DirectionMouse);
        if (player.Data.StatsBehavior.MoveVector != Vector2.zero)
        {
            stateMachine.ChangeState(player.MoveState);
        }
    }
}
