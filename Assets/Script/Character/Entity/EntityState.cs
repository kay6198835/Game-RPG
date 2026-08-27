using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class EntityState : IState
{
    [NonSerialized] protected EntityStateMachine stateMachine;
    protected Entity entity;
    protected EntityData entityData;
    public StatusAnimation Status { get; protected set; } = StatusAnimation.None;
    protected float startTime;
    protected string animBoolName;

    public EntityState(Entity etity, EntityStateMachine stateMachine, EntityData entityData, string animBoolName)
    {
        this.entity = etity;
        this.stateMachine = stateMachine;
        this.animBoolName = animBoolName;
        this.entityData = entityData;
    }

    public virtual void Enter()
    {
        DoChecks();
        entity.Anim.SetBool(animBoolName, true);
        Debug.Log(animBoolName);
        startTime = Time.time;
        this.Status = StatusAnimation.Start;
    }

    public virtual void Exit()
    {
        entity.Anim.SetBool(animBoolName, false);
        this.Status = StatusAnimation.End;
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

    public virtual void SetAnimationStatus(StatusAnimation statusAnimation)
    {
        this.Status = statusAnimation;
    }
}
