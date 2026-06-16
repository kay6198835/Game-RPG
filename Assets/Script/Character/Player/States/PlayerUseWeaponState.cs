using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUseWeaponState : PlayerState
{
    protected PlayerMovement playerMovement;
    protected WeaponHolder weaponHolder;
    protected AbilityHolder abilityHolder;
    protected PlayerInputHandler playerInputHandler;
    public PlayerUseWeaponState(Player player, string animBoolName) : base(player, animBoolName)
    {
    }
    public override void Enter()
    {
        base.Enter();
        player.Core.GetCoreComponent(out playerMovement);
        player.Core.GetCoreComponent(out weaponHolder);
        player.Core.GetCoreComponent(out abilityHolder);
        player.Core.GetCoreComponent(out playerInputHandler);
        playerMovement.SetVeclocity(Vector2.zero);
        player.Anim.SetFloat(GameConstants.AnimationName.Parameter.DIRECTION, playerInputHandler.DirectionMouse);
    }
    public override void Exit()
    {
        base.Exit();
    }
    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (isAnimationFinished)
        {
            if (playerInputHandler.MoveVector == Vector2.zero)
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
