using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicState : PlayerState
{
    protected Vector2 mouseVector;
    protected Vector2 moveVector;
    public BasicState(NewPlayer player, PlayerStateMachine playerStateMachine, PlayerData playerData, string animBoolName) : base(player, playerStateMachine , playerData, animBoolName)
    {

    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        moveVector = player.InputHandler.MoveVector;
        core.Movement.AngleCalculate(mouseVector);
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
