using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityState
{
    protected EntityStateMachine stateMachine;
    protected Entity entity;
    protected EntityCore entityCore;
    protected EntityData entityData;
    protected bool isAnimationTrigger;
    protected bool isAnimationFinished;
    protected bool isExitingState;
    protected float startTime;
    protected string animBoolName;
    //protected StateStyle stateStyle;
    //public enum StateStyle
    //{
    //    Freeze,
    //    Motion
    //}
    public EntityState(Entity etity, EntityStateMachine stateMachine, EntityData entityData , string animBoolName)
    {
        this.entity = etity;
        this.stateMachine = stateMachine;
        this.animBoolName = animBoolName;
        this.entityData = entityData;
        entityCore = entity.Core;
    }

    public virtual void Enter()
    {
        DoChecks();
        entity.Anim.SetBool(animBoolName, true);
        startTime = Time.time;
        isAnimationTrigger = false;
        isAnimationFinished = false;
        isExitingState = false;
    }

    public virtual void Exit()
    {
        entity.Anim.SetBool(animBoolName, false);
    }

    public virtual void LogicUpdate()
    {

    }

    public virtual void PhysicsUpdate()
    {
        DoChecks();
    }

    public virtual void DoChecks()
    {

    }
    public virtual void AnimationTrigger() => isAnimationTrigger = true;

    public virtual void AnimationFinishTrigger() => isAnimationFinished = true;
}
