using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackState : PlayerUseWeaponState
{
    private float startAttackTime;

    public PlayerAttackState(Player player, string animBoolName) : base(player, animBoolName)
    {
    }

    public float StartAttackTime { get => startAttackTime; }

    public override void Enter()
    {
        base.Enter();
        startAttackTime = startTime;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (isAnimationFinished && inputHandler.IsAttack && weaponHolder.Weapon.CheckCanAttack(player))
        {
            stateMachine.ChangeState(player.AttackState);
        }
        if (isAnimationTrigger)
        {
            weaponHolder.Weapon.Attack();
            isAnimationTrigger = false;
        }
    }
}
