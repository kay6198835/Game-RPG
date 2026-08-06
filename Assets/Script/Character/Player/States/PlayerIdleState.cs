using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerBasicState
{
    public PlayerIdleState(Player player, string animBoolName) : base(player, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        playerMovement.SetVeclocity(Vector2.zero);
    }

    public override void LogicUpdate()
    {

        player.Anim.SetFloat(GameConstants.AnimationName.Parameter.DIRECTION,
         inputHandler.DirectionMouse);
        if (inputHandler.MoveVector != Vector2.zero)
        {
            stateMachine.ChangeState(player.MoveState);
            return;
        }
        base.LogicUpdate();
    }
}
