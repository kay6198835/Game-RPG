using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveState : BasicState
{
    public PlayerMoveState(NewPlayer player, PlayerStateMachine playerStateMachine, PlayerData playerData, string animBoolName) : base(player, playerStateMachine, playerData, animBoolName)
    {

    }

    public override void AnimationFinishTrigger()
    {
        base.AnimationFinishTrigger();
    }

    public override void AnimationTrigger()
    {
        base.AnimationTrigger();
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (moveVector == Vector2.zero)
        {
            stateMachine.ChangeState(player.IdleState);
        }
        //Debug.Log("Move");
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
