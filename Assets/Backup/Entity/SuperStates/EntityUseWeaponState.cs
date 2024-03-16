using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityUseWeaponState : EntityState
{
    public EntityUseWeaponState(Entity etity, EntityStateMachine stateMachine, EntityData entityData, string animBoolName) : base(etity, stateMachine, entityData, animBoolName)
    {
    }
    public override void Enter()
    {
        base.Enter();
        entity.Anim.SetFloat("Direction", entity.InputHandler.Direction);
    }
    public override void LogicUpdate()
    {
        base.LogicUpdate();
    }
}
