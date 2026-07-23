using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillWeaponState : PlayerUseWeaponState
{
    private int stateIndex;
    protected AbilityHolder abilityHolder;
    public PlayerSkillWeaponState(Player player, string animBoolName) : base(player, animBoolName)
    {

    }
    public override void Enter()
    {
        player.Core.GetCoreComponent(out abilityHolder);
        base.Enter();
        abilityHolder.EnterAbility();
        stateIndex = 0;
        player.Anim.SetFloat("StateSkill", stateIndex);
        //stateStyle = StateStyle.Freeze;
    }
    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (StatusAnimation.StartRangeTrigger <= Status && Status <= StatusAnimation.EndRangeTrigger)
        {
            abilityHolder.SetStateAbility();
        }
    }
    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
    public override void Exit()
    {
        abilityHolder.ExitAbility();
        base.Exit();
    }
}
