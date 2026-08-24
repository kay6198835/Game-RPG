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
        player.Anim.SetFloat(GameConstants.AnimationName.Parameter.DIRECTION,
         inputHandler.DirectionExternality);
        player.Core.GetCoreComponent(out weaponHolder);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (StatusAnimation.OnActivate <= Status && Status < StatusAnimation.OffActivate)
        {
            Debug.Log("Call PlayerEquidUnequid Trigger");
            weaponHolder.Intertion();
            Status = StatusAnimation.OffActivate;
        }
    }
}
