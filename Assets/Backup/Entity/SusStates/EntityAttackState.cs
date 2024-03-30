using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityAttackState : EntityUseWeaponState
{
    private float startAttackTime;
    public float StartAttackTime { get => startAttackTime; }
    public EntityAttackState(Entity etity, EntityStateMachine stateMachine, EntityData entityData, string animBoolName) : base(etity, stateMachine, entityData, animBoolName)
    {
    }
    public override void Enter()
    {
        base.Enter();
        startAttackTime = startTime;
    }
    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (isAnimationTrigger)
        {
            entity.Core.Weapon.Attack();
            isAnimationTrigger = false;
        }
    }
}
