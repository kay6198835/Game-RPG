using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class PlayerState
{
    //protected Core core;

    protected NewPlayer player;
    protected PlayerStateMachine stateMachine;
    protected PlayerData playerData;
    protected bool isAnimationTrigger;
    protected bool isAnimationFinished;
    protected bool isAnimationExitingState;
    protected bool isExitingState;
    protected float startTime;
    private string animBoolName;
    protected StateStyle stateStyle;
    public enum StateStyle
    {
        Freeze,
        Motion
    }
    public PlayerState(NewPlayer player, string animBoolName)
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
        //Debug.Log("Start" + animBoolName);
        isAnimationTrigger = false;
        isAnimationFinished = false;
        isExitingState = false;
        player.Anim.SetFloat("Direction", player.InputHandler.Direction);
    }
    public virtual void Exit()
    {
        //Debug.Log("End" + animBoolName);
        player.Anim.SetBool(animBoolName, false);
        isExitingState = true;
    }

    public virtual void LogicUpdate()
    {
        if(stateStyle == StateStyle.Motion)
        {
            player.Anim.SetFloat("Direction", player.InputHandler.Direction);
        }
    }
    public virtual void PhysicsUpdate()
    {
        DoChecks();
    }

    public virtual void DoChecks() { }

    public virtual void AnimationTrigger()
    {
        isAnimationTrigger = true;
        //Debug.Log("isAnimationTrigger "+isAnimationTrigger);
    }
    public virtual void AnimationFinishTrigger() => isAnimationFinished = true;
    public virtual void AnimationExitingState()
    {
        isAnimationExitingState = true;
        //Debug.Log("isAnimationExitingState: " + isAnimationExitingState);
    }


}