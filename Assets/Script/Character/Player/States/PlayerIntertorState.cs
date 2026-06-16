using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIntertorState : PlayerUseWeaponState
{
    public PlayerIntertorState(Player player, string animBoolName) : base(player, animBoolName)
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
            //player.Core.Interactor.Intertion();
            isAnimationTrigger = false;
        }
    }
}
