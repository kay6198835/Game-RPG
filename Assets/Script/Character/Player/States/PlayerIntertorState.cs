using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIntertorState : PlayerUseWeaponState
{
    Interactor interactor;
    public PlayerIntertorState(Player player, string animBoolName) : base(player, animBoolName)
    {
    }
    public override void Enter()
    {
        base.Enter();
        player.Anim.SetFloat(GameConstants.AnimationName.Parameter.DIRECTION,
        inputHandler.DirectionExternality);
        player.Core.GetCoreComponent(out interactor);
    }

    public override void AnimationOnAction()
    {
        base.AnimationOnAction();
        interactor.Intertion();
        Status = StatusAnimation.OffActivate;
    }

    public override void Exit()
    {
        interactor = null;
        base.Exit();
    }
}
