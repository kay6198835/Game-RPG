using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillWeaponState : PlayerUseWeaponState
{
    private int stateIndex;
    private AbilitySO skill;
    public PlayerSkillWeaponState(NewPlayer player, string animBoolName) : base(player, animBoolName)
    {

    }
    public override void Enter()
    {
        base.Enter();
        skill = player.Core.WeaponHolder.Weapon.CurrentAbilitySO;
        player.Anim.runtimeAnimatorController = skill.Animator;
        skill.Activate(player);
        stateIndex = 0;
        player.Anim.SetFloat("StateSkill", stateIndex);
    }
    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (isAnimationTrigger)
        {
            if (stateIndex == 0)
            {
                stateIndex = 1;
            }
            else if (stateIndex == 1)
            {
                skill.CastSkill();

                if (player.InputHandler.State == PlayerInputHandler.SkillState.Do)
                {
                    if (isAnimationExitingState)
                    {
                        stateIndex = 2;
                    }
                    isAnimationExitingState = false;
                }
            }
            else if (stateIndex == 2)
            {
                skill.DoAbility();
            }
            player.Anim.SetFloat("StateSkill", stateIndex);
            isAnimationTrigger = false;
        }
    }
}
