using System;
using UnityEngine;
[System.Serializable]
public class PlayerState : IState
{
    //protected Core core;

    protected Player player;
    [NonSerialized] protected PlayerStateMachine stateMachine;
    protected PlayerData playerData;
    public StatusAnimation Status = StatusAnimation.None;
    protected float startTime;
    protected string animBoolName;
    //protected StateStyle stateStyle;
    public enum StateStyle
    {
        Freeze,
        Motion
    }
    public PlayerState(Player player, string animBoolName)
    {
        this.player = player;
        this.stateMachine = player.StateMachine;
        this.playerData = player.Data;
        this.animBoolName = animBoolName;
        //core = player.Core;
    }

    public virtual void Enter()
    {
        DoChecks();
        player.Anim.SetBool(animBoolName, true);
        startTime = Time.time;
        //this.Status = StatusAnimation.Start;

    }
    public virtual void Exit()
    {
        player.Anim.SetBool(animBoolName, false);
        this.Status = StatusAnimation.End;
    }

    public virtual void LogicUpdate()
    {
        //if(stateStyle == StateStyle.Motion)
        //{
        //    player.Anim.SetFloat(GameConstants.AnimationName.Parameter.DIRECTION, player.InputHandler.DirectionExtra);
        //}
    }
    public virtual void PhysicsUpdate()
    {
        DoChecks();
    }

    public virtual void DoChecks() { }

    public virtual void SetAnimationStatus(StatusAnimation statusAnimation)
    {
        this.Status = statusAnimation;
    }
}