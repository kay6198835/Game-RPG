using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeathState : PlayerDisadvantageState
{
    public PlayerDeathState(Player player, string animBoolName) : base(player, animBoolName)
    {
    }
    public override void Enter()
    {
        base.Enter();
        entityMovement.StopMove();
    }
    public override void LogicUpdate()
    {
        if (Status == StatusAnimation.EndRangeTrigger)
        {
            EventManager.Emit(EventID.ON_PLAYER_DEATH);
        }
    }
}