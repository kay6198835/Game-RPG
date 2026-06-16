using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBasicState : PlayerState
{
    protected WeaponHolder weaponHolder;
    protected Interactor interactor;
    protected AbilityHolder abilityHolder;
    protected PlayerInputHandler inputHandler;
    protected PlayerMovement playerMovement;
    public PlayerBasicState(Player player, string animBoolName) : base(player, animBoolName)
    {
    }
    public override void Enter()
    {
        base.Enter();
        player.Core.GetCoreComponent(out weaponHolder);
        player.Core.GetCoreComponent(out interactor);
        player.Core.GetCoreComponent(out abilityHolder);
        player.Core.GetCoreComponent(out inputHandler);
        player.Core.GetCoreComponent(out playerMovement);
    }
    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (inputHandler.IsEquip_Unequip)
        {
            if (weaponHolder.FindInteraction())
            {
                stateMachine.ChangeState(player.EquidUnequidState);
            }
        }
        else if (inputHandler.IsInteractor)
        {
            if (interactor.FindInteraction())
            {
                stateMachine.ChangeState(player.IntertorState);
            }
        }
        else if (weaponHolder.Weapon != null)
        {
            if (inputHandler.IsAttack && weaponHolder.Weapon.CheckCanAttack(player))
            {
                stateMachine.ChangeState(player.AttackState);
            }
            else if (inputHandler.IsSkill && abilityHolder.CanUseAbility)
            {
                stateMachine.ChangeState(player.AbilityState);
            }
        }
        if (inputHandler.IsTakeDamage)
        {
            stateMachine.ChangeState(player.TakeDamageState);
        }
    }
    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
