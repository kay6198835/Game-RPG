using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashState : PlayerBasicState
{
    INegativeReceiver<Core> negativeReceiver;
    public PlayerDashState(Player player, string animBoolName) : base(player, animBoolName)
    {
    }
    public override void Enter()
    {
        base.Enter();
        //stateStyle = StateStyle.Motion;
        player.Anim.SetFloat(GameConstants.AnimationName.Parameter.DIRECTION, inputHandler.DirectionKeyboard);
        player.Anim.SetBool(GameConstants.AnimationName.Parameter.IS_DASH, inputHandler.IsDash);
        player.Core.TryGetCapability(out negativeReceiver);
    }
    public override void AnimationOnAction()
    {
        playerMovement.Dash(inputHandler.MoveVector);
        negativeReceiver.InvicisibleHurtbox(true);
    }
    public override void AnimationOffAction()
    {
        playerMovement.ExitDash();
        negativeReceiver.InvicisibleHurtbox(false);
    }
    public override void AnimationEnd()
    {
        base.AnimationEnd();
        if (inputHandler.MoveVector == Vector2.zero)
        {
            stateMachine.ChangeState(player.IdleState);
            return;
        }else
        {
            stateMachine.ChangeState(player.MoveState);
            return;
        }
    }

    public override void Exit()
    {
        player.Anim.SetBool(GameConstants.AnimationName.Parameter.IS_DASH, inputHandler.IsDash);
        playerMovement.ExitDash();
        base.Exit();
    }
}
