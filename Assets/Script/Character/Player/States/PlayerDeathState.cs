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
    }
    public override void LogicUpdate()
    {
        base.LogicUpdate();
        // if (StatusAnimation.Start)
        // {
        //     EventManager.Emit(EventID.ON_PLAYER_DEATH);
        // }
        // if (StatusAnimation.End)
        // {
        //     EventManager.Emit(EventID.ON_REALOAD_GAME);
        // }
    }
}