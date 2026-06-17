using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUseWeaponState : PlayerState
{
    // protected PlayerMovement playerMovement;
    // protected WeaponHolder weaponHolder;
    // protected AbilityHolder abilityHolder;
    public PlayerUseWeaponState(Player player, string animBoolName) : base(player, animBoolName)
    {
    }
    public override void Enter()
    {
        base.Enter();
        // player.Core.GetCoreComponent(out playerMovement);
        // player.Core.GetCoreComponent(out weaponHolder);
        // player.Core.GetCoreComponent(out abilityHolder);
        playerMovement.SetVeclocity(Vector2.zero);
        player.Anim.SetFloat(GameConstants.AnimationName.Parameter.DIRECTION, inputHandler.DirectionMouse);
    }
    public override void Exit()
    {
        base.Exit();
        // weaponHolder = null;
        // abilityHolder = null;
        // playerMovement = null;
    }
    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (isAnimationFinished)
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
