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
        if (player.InputHandler.Attack && player.Core.Weapon.CheckCanAttack(player))
        {
            stateMachine.ChangeState(player.AttackState);
        }
        if (player.InputHandler.Skill&& player.Core.Weapon.SetAbility()!=null)
        {
            stateMachine.ChangeState(player.AbilityState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        player.Anim.SetFloat("Direction", player.InputHandler.Direction);
    }
}
