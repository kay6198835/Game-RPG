using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveState : PlayerBasicState
{
    public PlayerMoveState(NewPlayer player, string animBoolName) : base(player, animBoolName)
    {
    }
    public override void Enter()
    {
        base.Enter();
        //stateStyle = StateStyle.Motion;
        player.Anim.SetFloat("Direction", player.Data.StatsBehavior.DirectionKeyboard);
    }
    public override void LogicUpdate()
    {
        player.Anim.SetFloat("Direction", player.Data.StatsBehavior.DirectionKeyboard);
        player.Core.Movement.SetVeclocity(player.Data.StatsBehavior.MoveVector * playerData.movementVelocities);
        if (player.Data.StatsBehavior.MoveVector == Vector2.zero)
        {
            stateMachine.ChangeState(player.IdleState);
        }
        base.LogicUpdate();
    }
    public override void Exit()
    {
        base.Exit();
        player.Core.Movement.SetVeclocity(Vector2.zero);
    }
}
