using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : BasicState
{
    public PlayerIdleState(NewPlayer player, PlayerStateMachine playerStateMachine, PlayerData playerData, string animBoolName) : base(player, playerStateMachine, playerData, animBoolName)
    {
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (moveVector != Vector2.zero)
        {
            stateMachine.ChangeState(player.MoveState);
        }
        //Debug.Log("Idle "+mouseVector);
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    public override string ToString()
    {
        return base.ToString();
    }
}
