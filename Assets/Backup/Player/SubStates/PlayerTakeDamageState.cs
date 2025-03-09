using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTakeDamageState : PlayerDisadvantageState
{
    public PlayerTakeDamageState(NewPlayer player, string animBoolName) : base(player, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.Core.Movement.SetVeclocity(Vector2.zero);

    }
    public override void LogicUpdate()
    {
        base.LogicUpdate();
        player.Anim.SetFloat("Direction", player.Data.StatsBehavior.DirectionExternality);
    }
    public override void Exit()
    {
        base.Exit();
    }
}
