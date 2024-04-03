using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayeSkillWeaponState : PlayerUseWeaponState
{
    private int stateIndex;
    private AbilitySO skill;
<<<<<<< HEAD:Assets/Backup/Player/SubStates/PlayerSkillWeaponState.cs
    private int currentLoopCount = 0;
    private float previousNormalizedTime = 0.0f;
    public PlayerSkillWeaponState(NewPlayer player, string animBoolName) : base(player, animBoolName)
=======
    public PlayeSkillWeaponState(NewPlayer player, string animBoolName) : base(player, animBoolName)
>>>>>>> parent of e9a7753 (add Combo for Attack of Entity,  Dash State for Player (30/03/2024)):Assets/Backup/Player/SubStates/PlayeSkillWeaponState.cs
    {
        //player.InputHandler.
    }
    public override void Enter()
    {
        base.Enter();
       
        skill = player.Core.Weapon.CurrentAbilitySO;
        player.Anim.runtimeAnimatorController = skill.Animator;
        skill.Enter(player);
    }
    public override void LogicUpdate()
    {
        base.LogicUpdate();
        AnimatorStateInfo stateInfo = player.Anim.GetCurrentAnimatorStateInfo(0);
        if (!player.Anim.IsInTransition(0))
        {
<<<<<<< HEAD:Assets/Backup/Player/SubStates/PlayerSkillWeaponState.cs
            int normalizedTime = (int)stateInfo.normalizedTime;
            if (normalizedTime > currentLoopCount)
            {
                currentLoopCount = normalizedTime;
                // Animation đã bắt đầu một vòng lặp mới, thêm code của bạn ở đây
                Debug.Log("Start");
                player.Anim.SetFloat("StateSkill", stateIndex);
=======
            //Debug.Log("On");
            if (stateIndex == 0)
            {
                //if (isAnimationExitingState)
                //{
                //    stateIndex = 1;
                //    isAnimationExitingState = false;
                //}
                stateIndex = 1;
                //Debug.Log("Start Ability Skill");
>>>>>>> parent of e9a7753 (add Combo for Attack of Entity,  Dash State for Player (30/03/2024)):Assets/Backup/Player/SubStates/PlayeSkillWeaponState.cs
            }
        }
        float currentNormalizedTime = stateInfo.normalizedTime % 1.0f;
        // Kiểm tra xem normalizedTime hiện tại có nhỏ hơn normalizedTime trước đó không
        // Nếu có, điều này cho thấy rằng animation vừa bắt đầu một vòng lặp mới
        if (currentNormalizedTime < previousNormalizedTime)
        {
            // Animation đã kết thúc một vòng lặp, thực hiện hành động tại đây
            Debug.Log("End");
            switch (stateIndex)
            {
<<<<<<< HEAD:Assets/Backup/Player/SubStates/PlayerSkillWeaponState.cs
                case 0:
                    stateIndex = 1;
                    break;
                case 1:
                    if (player.InputHandler.State == PlayerInputHandler.SkillState.Do)
=======
                skill.CastSkill();

                if (player.InputHandler.State == PlayerInputHandler.SkillState.Do)
                {
                    //Debug.Log("Do Ability when hold cast Skill" + player.InputHandler.State);
                    if (isAnimationExitingState)
>>>>>>> parent of e9a7753 (add Combo for Attack of Entity,  Dash State for Player (30/03/2024)):Assets/Backup/Player/SubStates/PlayeSkillWeaponState.cs
                    {
                        stateIndex = 2;

                    }
<<<<<<< HEAD:Assets/Backup/Player/SubStates/PlayerSkillWeaponState.cs
                    break;
                case 2:
                    stateIndex = 0;
                    break;
            }
        }
        previousNormalizedTime = currentNormalizedTime;
=======
                    isAnimationExitingState = false;
                }
                //Debug.Log(stateIndex);
            }
            else if (stateIndex == 2)
            {
                skill.DoAbility();
                //Debug.Log("Do Ability Skill");
            }
            player.Anim.SetFloat("StateSkill", stateIndex);
            //Debug.Log(stateIndex);
            isAnimationTrigger = false;
        }
        //Debug.Log("Start Ability Skill");
        //Debug.Log("Exit");
>>>>>>> parent of e9a7753 (add Combo for Attack of Entity,  Dash State for Player (30/03/2024)):Assets/Backup/Player/SubStates/PlayeSkillWeaponState.cs
    }
}
