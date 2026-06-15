using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveState : PlayerBasicState
{
    PlayerMovement playerMovement;
    public PlayerMoveState(Player player, string animBoolName) : base(player, animBoolName)
    {
    }
    public override void Enter()
    {
        base.Enter();
        //stateStyle = StateStyle.Motion;
        player.Anim.SetFloat(GameConstants.AnimationName.Parameter.DIRECTION, player.InputHandler.DirectionKeyboard);
        playerMovement = player.Core.GetCoreComponent<PlayerMovement>();
    }
    public override void LogicUpdate()
    {
        player.Anim.SetFloat(GameConstants.AnimationName.Parameter.DIRECTION, player.InputHandler.DirectionKeyboard);
        playerMovement.SetVeclocity(player.InputHandler.MoveVector * playerData.movementVelocities);
        if (player.InputHandler.MoveVector == Vector2.zero)
        {
            stateMachine.ChangeState(player.IdleState);
        }
        base.LogicUpdate();
    }
    public override void Exit()
    {
        base.Exit();
        //player.Core.Movement.SetVeclocity(Vector2.zero);
    }
}
