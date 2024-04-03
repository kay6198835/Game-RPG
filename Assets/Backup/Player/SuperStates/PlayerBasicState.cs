using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBasicState : PlayerState
{
    public PlayerBasicState(NewPlayer player, string animBoolName) : base(player, animBoolName)
    {
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
<<<<<<< HEAD
        if (player.Core.WeaponHolder.FindWeapon())
=======
        moveVector = player.InputHandler.MoveVector;
        if (player.InputHandler.IsAttack && player.Core.Weapon.CheckCanAttack(player))
>>>>>>> parent of e9a7753 (add Combo for Attack of Entity,  Dash State for Player (30/03/2024))
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
