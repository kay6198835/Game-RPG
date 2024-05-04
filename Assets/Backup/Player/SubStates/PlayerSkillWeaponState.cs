using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillWeaponState : PlayerUseWeaponState
{
    private int stateIndex;
    //private AbilitySO ability;
    //bool isCanUseSkills;
    public PlayerSkillWeaponState(NewPlayer player, string animBoolName) : base(player, animBoolName)
    {

    }
    public override void Enter()
    {
        base.Enter();
        player.Core.AbilityHolder.EnterAbility();
        stateIndex = 0;
        player.Anim.SetFloat("StateSkill", stateIndex);
        stateStyle = StateStyle.Freeze;
    }
    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (isAnimationTrigger)
        {
            player.Core.AbilityHolder.SetStateAbility();
            isAnimationTrigger = false;
        }
    }
    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
    public override void Exit()
    {
        base.Exit();
        player.Core.AbilityHolder.ExitAbility();
    }
}
