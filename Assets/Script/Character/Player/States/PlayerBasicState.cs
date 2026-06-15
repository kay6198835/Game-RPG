using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBasicState : PlayerState
{
    WeaponHolder weaponHolder;
    Interactor interactor;
    AbilityHolder abilityHolder;
    public PlayerBasicState(Player player, string animBoolName) : base(player, animBoolName)
    {
    }
    public override void Enter()
    {
        base.Enter();
        weaponHolder = player.Core.GetCoreComponent<WeaponHolder>();
        weaponHolder = player.Core.GetCoreComponent<WeaponHolder>();
        abilityHolder = player.Core.GetCoreComponent<AbilityHolder>();
    }
    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (player.InputHandler.IsEquip_Unequip)
        {
            if (weaponHolder.FindInteraction())
            {
                stateMachine.ChangeState(player.EquidUnequidState);
            }
        }
        else if (player.InputHandler.IsInteractor)
        {
            if (interactor.FindInteraction())
            {
                stateMachine.ChangeState(player.IntertorState);
            }
        }
        else if(weaponHolder.Weapon != null)
        {
            if (player.InputHandler.IsAttack && weaponHolder.Weapon.CheckCanAttack(player))
            {
                stateMachine.ChangeState(player.AttackState);
            }
            else if(player.InputHandler.IsSkill && abilityHolder.CanUseAbility)
            {
                stateMachine.ChangeState(player.AbilityState);
            }
        }
        if (player.InputHandler.IsTakeDamage)
        {
            stateMachine.ChangeState(player.TakeDamageState);
        }
    }
    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
