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

    // Check State hiện tại kích hoạt nhưng không thấy cập nhật trạng thái
    public override void Enter()
    {
        player.Core.GetCoreComponent(out abilityHolder);
        base.Enter();
        //fix later
        stateIndex = 0;
        player.Anim.SetFloat("StateSkill", stateIndex);
    }
    public override void AnimationFinishTrigger()
    {
        base.AnimationFinishTrigger();
        int stateHash = 0;
        switch (abilityHolder.State)
        {
            case SkillState.Start:
                break;
            case SkillState.Cast:
                if (!abilityHolder.IsHolding)
                {
                    Debug.Log("Finish Cast");
                    stateHash = player.Anim.GetCurrentAnimatorStateInfo(0).fullPathHash;
                    player.Anim.SetBool("DoAB", true);
                    player.Anim.Play(stateHash, 0, 0f);
                }
                break;
        }
    }

    public override void AnimationEnd()
    {
        base.AnimationEnd();
        switch (abilityHolder.State)
        {
            case SkillState.Do:
                // int stateHash = player.Anim.GetCurrentAnimatorStateInfo(0).fullPathHash;
                // player.Anim.Play(stateHash, 0, 0f);
                player.Anim.SetBool("DoAB", false);
                base.LogicUpdate();
                break;
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
    public override void Exit()
    {
        //fix later
        //abilityHolder.ExitAbility();
        base.Exit();
    }
}
