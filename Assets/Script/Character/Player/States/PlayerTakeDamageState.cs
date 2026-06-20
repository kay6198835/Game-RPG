using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTakeDamageState : PlayerDisadvantageState
{
    // PlayerMovement playerMovement;
    public PlayerTakeDamageState(Player player, string animBoolName) : base(player, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.Core.GetCoreComponent(out playerMovement);
        playerMovement.SetVeclocity(Vector2.zero);
        inputHandler.SetBufferAttack(false);
        if (weaponHolder.Weapon != null)
        {
            weaponHolder.Weapon.ResetCombo();
        }
    }
    public override void LogicUpdate()
    {
        base.LogicUpdate();
        player.Anim.SetFloat(GameConstants.AnimationName.Parameter.DIRECTION, inputHandler.DirectionExternality);
    }
    public override void Exit()
    {
        base.Exit();
        // playerMovement = null;
    }
}
