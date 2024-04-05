using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillWeaponState : PlayerUseWeaponState
{
    private int stateIndex;
    private AbilitySO skill;
    bool isCanUseSkills;
    public PlayerSkillWeaponState(NewPlayer player, string animBoolName) : base(player, animBoolName)
    {

    }
    public override void Enter()
    {
        base.Enter();
        skill = player.Core.WeaponHolder.Weapon.CurrentAbilitySO;
        player.Anim.runtimeAnimatorController = skill.Animator;
        skill.Enter(player);
        stateIndex = 0;
        player.Anim.SetFloat("StateSkill", stateIndex);


    }
    public override void LogicUpdate()
    {
        base.LogicUpdate();
        Debug.Log(isAnimationTrigger + " " + isCanUseSkills+" "+ player.InputHandler.State);
        if (isAnimationTrigger)
        {
            if (stateIndex == 0)
            {
                skill.Activate();
                stateIndex = 1;
            }
            else if (stateIndex == 1)
            {
                skill.CastSkill();

                if ((player.InputHandler.State == PlayerInputHandler.SkillState.Do ||skill.Type == AbilitySO.SkillType.DoNonCast)&&isAnimationExitingState)
                {
                    stateIndex = 2;
                    isAnimationExitingState = false;
                }
            }
            else if (stateIndex == 2)
            {
                skill.DoAbility();
                isCanUseSkills = false;
            }
            player.Anim.SetFloat("StateSkill", stateIndex);
            isAnimationTrigger = false;
        }
    }
}
