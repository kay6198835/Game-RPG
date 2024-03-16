using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayeSkillWeaponState : PlayerUseWeaponState
{
    private int stateIndex;
    private AbilitySO skill;
    public PlayeSkillWeaponState(NewPlayer player, PlayerStateMachine playerStateMachine, PlayerData playerData, string animBoolName) : base(player, playerStateMachine, playerData, animBoolName)
    {
        //player.InputHandler.
    }
    public override void Enter()
    {
        base.Enter();
       
        skill = player.Core.Weapon.CurrentAbilitySO;
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
            Debug.Log("On");
            if (stateIndex == 0)
            {
                if (isAnimationExitingState)
                {
                    stateIndex = 1;
                    isAnimationExitingState = false;
                }
                //stateIndex = 1;
                Debug.Log("Start Ability Skill");
            }
            else if (stateIndex == 1)
            {
                skill.CastSkill();

                if (player.InputHandler.State == PlayerInputHandler.SkillState.Do)
                {
                    Debug.Log("Do Ability when hold cast Skill" + player.InputHandler.State);
                    if (isAnimationExitingState)
                    {
                        stateIndex = 2;
                        isAnimationExitingState = false;
                    }
                }

                //Debug.Log(stateIndex);
            }
            else if (stateIndex == 2)
            {
                skill.DoAbility();
                Debug.Log("Do Ability Skill");

            }
            
            player.Anim.SetFloat("StateSkill", stateIndex);
            Debug.Log(stateIndex);
            isAnimationTrigger = false;
        }
        //Debug.Log("Start Ability Skill");
        Debug.Log("Exit");
    }
}
