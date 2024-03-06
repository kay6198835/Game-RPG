using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayeSkillWeaponState : UseWeaponState
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
    }
    public override void LogicUpdate()
    {
        //if(player.InputHandler.PlayerInput.Control.UseSkill.)
        base.LogicUpdate();
        player.Anim.SetFloat("StateSkill", stateIndex);
        if (onAnimationActivate)
        {
            if (stateIndex == 0)
            {
                //skill.Activate(player);
                stateIndex = 1;
                //player.Anim.SetFloat("StateSkill", stateIndex);
            }
            else if (stateIndex == 1)
            {
                skill.CastSkill();
                Debug.Log("Do Ability when hold cast Skill");
                if (player.InputHandler.State == PlayerInputHandler.SkillState.Do)
                {

                    stateIndex = 2;

                }
            }
            else if (stateIndex == 2)
            {
                skill.DoAbility();
                Debug.Log("Do Ability Skill");
            }
            onAnimationActivate = false;
        }
    }
}
