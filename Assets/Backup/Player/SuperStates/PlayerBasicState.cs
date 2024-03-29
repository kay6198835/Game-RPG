using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBasicState : PlayerState
{
    protected Vector2 mouseVector;
    protected Vector2 moveVector;

    public PlayerBasicState(NewPlayer player, string animBoolName) : base(player, animBoolName)
    {
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        moveVector = player.InputHandler.MoveVector;
        if (player.InputHandler.IsAttack && player.Core.Weapon.CheckCanAttack(player))
        {
            stateMachine.ChangeState(player.AttackState);
        }
        if (player.InputHandler.IsSkill&& player.Core.Weapon.SetAbility()!=null)
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
