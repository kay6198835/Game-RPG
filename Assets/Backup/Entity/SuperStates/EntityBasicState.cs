using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityBasicState : EntityState
{
    public EntityBasicState(Entity etity, EntityStateMachine stateMachine, EntityData entityData, string animBoolName) : base(etity, stateMachine, entityData, animBoolName)
    {
    }
    public override void Enter()
    {
        base.Enter();
    }
    public override void LogicUpdate()
    {
        base.LogicUpdate();
        entity.Anim.SetFloat("Direction", entity.InputHandler.Direction);
    }
}
