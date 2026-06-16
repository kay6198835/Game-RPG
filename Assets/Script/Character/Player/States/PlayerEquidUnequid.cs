using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEquidUnequid : PlayerUseWeaponState
{
    public PlayerEquidUnequid(Player player, string animBoolName) : base(player, animBoolName)
    {
    }
    public override void Enter()
    {
        base.Enter();
        player.Anim.SetFloat(GameConstants.AnimationName.Parameter.DIRECTION, playerInputHandler.DirectionExternality);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (isAnimationTrigger)
        {
            weaponHolder.Intertion();
            isAnimationTrigger = false;
        }
    }
}
