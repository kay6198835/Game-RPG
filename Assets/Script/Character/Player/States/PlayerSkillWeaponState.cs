using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillWeaponState : PlayerUseWeaponState
{
    private int stateIndex;
    public PlayerSkillWeaponState(Player player, string animBoolName) : base(player, animBoolName)
    {

    }
    public override void Enter()
    {
        base.Enter();
        abilityHolder.EnterAbility();
        stateIndex = 0;
        player.Anim.SetFloat("StateSkill", stateIndex);
        //stateStyle = StateStyle.Freeze;
    }
    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (isAnimationTrigger)
        {
            abilityHolder.SetStateAbility();
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
        abilityHolder.ExitAbility();
        abilityHolder = null;
    }
}
