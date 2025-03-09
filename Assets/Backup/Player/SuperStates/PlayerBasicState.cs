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
        if (player.Data.StatsState.IsEquip_Unequip)
        {
            if (player.Core.WeaponHolder.FindInteraction())
            {
                stateMachine.ChangeState(player.EquidUnequidState);
            }
        }
        else if (player.Data.StatsState.IsInteractor)
        {
            if (player.Core.Interactor.FindInteraction())
            {
                stateMachine.ChangeState(player.IntertorState);
            }
        }
        else if(player.Core.WeaponHolder.Weapon != null)
        {
            if (player.Data.StatsState.IsAttack && player.Core.WeaponHolder.Weapon.CheckCanAttack(player))
            {
                stateMachine.ChangeState(player.AttackState);
            }
            else if(player.Data.StatsState.IsSkill && player.Core.AbilityHolder.CanUseAbility)
            {
                stateMachine.ChangeState(player.AbilityState);
            }
        }
        if(player.Data.StatsState.IsTakeDamage)
        {
            stateMachine.ChangeState(player.TakeDamageState);
        }
    }
    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
