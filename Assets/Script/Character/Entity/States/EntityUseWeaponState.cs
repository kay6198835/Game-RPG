using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityUseWeaponState : EntityState
{
    protected EntityMovement entityMovement;
    protected EntityWeaponHolder weaponHolder;
    protected EntityInput entityInput;
    public EntityUseWeaponState(Entity etity, EntityStateMachine stateMachine, EntityData entityData, string animBoolName) : base(etity, stateMachine, entityData, animBoolName)
    {
    }
    public override void Enter()
    {
        base.Enter();
        entity.Core.GetCoreComponent(out entityMovement);
        entity.Core.GetCoreComponent(out weaponHolder);
        entity.Core.GetCoreComponent(out entityInput);
        entityMovement.StopMove();
        entity.Anim.SetFloat(GameConstants.AnimationName.Parameter.DIRECTION, entityInput.DirectionLook);
    }
    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (Status == StatusAnimation.EndRangeTrigger)
        {
            entity.StateMachine.ChangeState(entity.IdleState);
            //Debug.Log("Attack");
        }
    }
    public override void Exit()
    {
        base.Exit();
        entityMovement = null;
        weaponHolder = null;
        entityInput = null;
    }
}
