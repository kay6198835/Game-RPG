using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackState : UseWeaponState
{
    private float startAttackTime;
    public PlayerAttackState(NewPlayer player, PlayerStateMachine playerStateMachine, PlayerData playerData, string animBoolName) : base(player, playerStateMachine, playerData, animBoolName)
    {
    }

    public float StartAttackTime { get => startAttackTime;}

    public override void Enter()
    {
        base.Enter();
        startAttackTime = startTime;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
    }
}
