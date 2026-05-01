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
        if (player.InputHandler.IsEquip_Unequip)
        {
            if (player.Core.WeaponHolder.FindInteraction())
            {
                stateMachine.ChangeState(player.EquidUnequidState);
            }
        }
        else if (player.InputHandler.IsInteractor)
        {
            if (player.Core.Interactor.FindInteraction())
            {
                stateMachine.ChangeState(player.IntertorState);
            }
        }
        else if(player.Core.WeaponHolder.Weapon != null)
        {
            if (player.InputHandler.IsAttack && player.Core.WeaponHolder.Weapon.CheckCanAttack(player))
            {
                stateMachine.ChangeState(player.AttackState);
            }
            else if(player.InputHandler.IsSkill && player.Core.AbilityHolder.CanUseAbility)
            {
                stateMachine.ChangeState(player.AbilityState);
            }
        }
        if(player.InputHandler.IsTakeDamage)
        {
            stateMachine.ChangeState(player.TakeDamageState);
        }
    }
    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
