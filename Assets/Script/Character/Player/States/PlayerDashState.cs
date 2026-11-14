using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashState : PlayerBasicState
{
    public PlayerDashState(Player player, string animBoolName) : base(player, animBoolName)
    {
    }
    public override void Enter()
    {
        base.Enter();
        //stateStyle = StateStyle.Motion;
        player.Anim.SetFloat(GameConstants.AnimationName.Parameter.DIRECTION, inputHandler.DirectionKeyboard);
    }
    public override void AnimationOnAction()
    {
        playerMovement.Dash(inputHandler.DirectionMouseVector);
    }
    public override void AnimationOffAction()
    {
        playerMovement.ExitDash();
    }
    public override void AnimationEnd()
    {
        base.AnimationEnd();
        playerMovement.Move(inputHandler.MoveVector);
        if (inputHandler.MoveVector == Vector2.zero)
        {
            stateMachine.ChangeState(player.IdleState);
            return;
        }
        base.LogicUpdate();
    }
}
