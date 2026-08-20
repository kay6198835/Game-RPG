using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUseWeaponState : PlayerState
{
    protected PlayerMovement playerMovement;
    protected PlayerInputHandler inputHandler;
    protected WeaponHolder weaponHolder;
    public PlayerUseWeaponState(Player player, string animBoolName) : base(player, animBoolName)
    {
    }
    public override void Enter()
    {
        base.Enter();
        player.Core.GetCoreComponent(out playerMovement);
        player.Core.GetCoreComponent(out inputHandler);
        player.Core.GetCoreComponent(out weaponHolder);
        playerMovement.SetVeclocity(Vector2.zero);
        player.Anim.SetFloat(GameConstants.AnimationName.Parameter.DIRECTION, inputHandler.DirectionMouse);
    }
    public override void Exit()
    {
        base.Exit();
        playerMovement = null;
        inputHandler = null;
        weaponHolder = null;
    }
    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (Status == StatusAnimation.None)
        {
            if (inputHandler.MoveVector == Vector2.zero)
            {
                stateMachine.ChangeState(player.IdleState);
                return;
            }
            else
            {
                stateMachine.ChangeState(player.MoveState);
                return;
            }
        }
    }
}
