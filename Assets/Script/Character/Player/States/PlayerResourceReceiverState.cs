using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerResourceReceiverState : PlayerUseWeaponState
{
    ResourceReceiver resourceReceiver;
    public PlayerResourceReceiverState(Player player, string animBoolName) : base(player, animBoolName)
    {
    }
    public override void Enter()
    {
        base.Enter();
        player.Anim.SetFloat(GameConstants.AnimationName.Parameter.DIRECTION,
        inputHandler.DirectionExternality);
        player.Core.GetCoreComponent(out resourceReceiver);
    }
    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (StatusAnimation.OnActivate <= Status && Status < StatusAnimation.OffActivate)
        {
            Debug.Log("Call PlayerResourceReceiverState Trigger");
            resourceReceiver.Intertion();
            Status = StatusAnimation.OffActivate;
        }
    }


    public override void Exit()
    {
        base.Exit();
        resourceReceiver = null;
        Status = StatusAnimation.None;
    }
}
