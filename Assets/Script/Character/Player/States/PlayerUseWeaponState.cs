using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUseWeaponState : PlayerState
{
    protected PlayerMovement playerMovement;
    protected PlayerInputHandle inputHandler;
    public PlayerUseWeaponState(Player player, string animBoolName) : base(player, animBoolName)
    {
    }
    public override void Enter()
    {
        base.Enter();
        player.Core.GetCoreComponent(out playerMovement);
        player.Core.GetCoreComponent(out inputHandler);
        playerMovement.SetVeclocity(Vector2.zero);
        player.Anim.SetFloat(GameConstants.AnimationName.Parameter.DIRECTION, inputHandler.DirectionMouse);
    }
    public override void Exit()
    {
        base.Exit();
        playerMovement = null;
        inputHandler = null;
    }
    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (Status == StatusAnimation.None)
        {
            if (inputHandler.MoveVector == Vector2.zero)
            {
                stateMachine.ChangeState(player.IdleState);
            }
            else
            {
                stateMachine.ChangeState(player.MoveState);
            }
        }
    }
}
