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

    public override void Enter()
    {
        player.Core.GetCoreComponent(out abilityHolder);
        base.Enter();
        stateIndex = 0;
        player.Anim.SetFloat("StateSkill", stateIndex);
        player.Anim.SetBool("DoAB", false);
    }

    public override void AnimationStart()
    {
        base.AnimationStart();
    }

    public override void AnimationOnAction()
    {
        base.AnimationOnAction();
        // DoAB opens Cast -> Do; it must stay closed until Cast's own OnAction has run.
        switch (abilityHolder.CurrentAbilityState)
        {
            case AbilityState.Start: player.Anim.SetBool("DoAB", false); break;
            case AbilityState.Cast: player.Anim.SetBool("DoAB", !abilityHolder.IsHolding); break;
        }
        abilityHolder.HandleInput();
        Status = StatusAnimation.OffActivate;
    }

    public override void AnimationEnd()
    {
        base.AnimationEnd();
        switch (abilityHolder.CurrentAbilityState)
        {
            case AbilityState.Do:
                //base.LogicUpdate();
                break;
            case AbilityState.Exit:
                base.LogicUpdate();
                break;
        }
    }

    override public void LogicUpdate()
    {
        base.LogicUpdate();
        abilityHolder.Processing();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    public override void Exit()
    {
        base.Exit();
    }
}
