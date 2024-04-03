using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillWeaponState : PlayerUseWeaponState
{
    private int stateIndex;
    private AbilitySO skill;
    private int currentLoopCount = 0;
    private float previousNormalizedTime = 0.0f;
    public PlayerSkillWeaponState(NewPlayer player, string animBoolName) : base(player, animBoolName)
    {

    }
    public override void Enter()
    {
        base.Enter();
        skill = player.Core.WeaponHolder.Weapon.CurrentAbilitySO;
        player.Anim.runtimeAnimatorController = skill.Animator;
        skill.Enter(player);
    }
    public override void LogicUpdate()
    {
        base.LogicUpdate();
        AnimatorStateInfo stateInfo = player.Anim.GetCurrentAnimatorStateInfo(0);
        if (!player.Anim.IsInTransition(0))
        {
            int normalizedTime = (int)stateInfo.normalizedTime;
            if (normalizedTime > currentLoopCount)
            {
                currentLoopCount = normalizedTime;
                // Animation đã bắt đầu một vòng lặp mới, thêm code của bạn ở đây
                Debug.Log("Start");
                player.Anim.SetFloat("StateSkill", stateIndex);
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
                case 0:
                    stateIndex = 1;
                    break;
                case 1:
                    if (player.InputHandler.State == PlayerInputHandler.SkillState.Do)
                    {
                        stateIndex = 2;
                    }
                    break;
                case 2:
                    stateIndex = 0;
                    break;
            }
        }
        previousNormalizedTime = currentNormalizedTime;
    }
}
