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
        //skill = player.Core.AbilityHolder.Ability;
        //player.Anim.runtimeAnimatorController = skill.Animator;
        //skill.Enter(player);
        stateIndex = 0;
        player.Anim.SetFloat("StateSkill", stateIndex);
    }
    public override void LogicUpdate()
    {
        base.LogicUpdate();
        
        /*if (isAnimationTrigger)
        {
            if (stateIndex == 0)
            {
                skill.Activate();
                stateIndex = 1;
            }
            else if (stateIndex == 1)
            {
                skill.Cast();

                if ((player.InputHandler.State == PlayerInputHandler.SkillState.Do ||skill.Type == AbilitySO.SkillType.DoNonCast)&&isAnimationExitingState)
                {
                    stateIndex = 2;
                    isAnimationExitingState = false;
                }
            }
            else if (stateIndex == 2)
            {
                skill.Do();
                isCanUseSkills = false;
            }
            player.Anim.SetFloat("StateSkill", stateIndex);
            isAnimationTrigger = false;
        }*/
        player.Core.AbilityHolder.SetStateAbility(ref isAnimationTrigger);
        
    }
    public override void Exit()
    {
        base.Exit();
        player.Core.AbilityHolder.ExitAbility();
    }
}
