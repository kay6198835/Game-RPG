using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityDisadvantageState : EntityState
{
    public EntityDisadvantageState(Entity etity, EntityStateMachine stateMachine, EntityData entityData, string animBoolName) : base(etity, stateMachine, entityData, animBoolName)
    {
    }
}
