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
                return;
            }
        }
        else if (inputHandler.IsInteractor)
        {
            if (interactor.FindInteraction())
            {
                stateMachine.ChangeState(player.IntertorState);
                return;
            }
        }
        else if (weaponHolder.Weapon != null)
        {
            if (inputHandler.IsAttack && weaponHolder.CanAttack())
            {
                stateMachine.ChangeState(player.AttackState);
                return;
            }
            else if (inputHandler.IsSkill && abilityHolder.CanUseAbility)
            {
                stateMachine.ChangeState(player.AbilityState);
                return;
            }
        }
        if (inputHandler.IsTakeDamage)
        {
            if (entityVitalStats.GetCurrentStatValue(StatType.HP) <= 0)
            {
                stateMachine.ChangeState(player.DeathState);
                return;
            }
            else
            {
                stateMachine.ChangeState(player.TakeDamageState);
                return;
            }
        }
    }
    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    public override void Exit()
    {
        base.Exit();
        weaponHolder = null;
        interactor = null;
        abilityHolder = null;
        inputHandler = null;
        playerMovement = null;
    }
}
