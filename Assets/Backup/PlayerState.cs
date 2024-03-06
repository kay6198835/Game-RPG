using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class PlayerState
{
    protected Core core;

    protected NewPlayer player;
    protected PlayerStateMachine stateMachine;
    protected PlayerData playerData;

    protected bool onAnimationActivate;
    protected bool isAnimationFinished;
    protected bool isExitingState;

    protected float startTime;

    private string animBoolName;

    public PlayerState(NewPlayer player,PlayerStateMachine playerStateMachine , PlayerData playerData, string animBoolName)
    {
        this.player = player;
        this.stateMachine = player.StateMachine;
        this.playerData = playerData;
        this.animBoolName = animBoolName;
        core = player.Core;
    }

    public virtual void Enter()
    {
        DoChecks();
        player.Anim.SetBool(animBoolName, true);
        startTime = Time.time;
        Debug.Log("Start" + animBoolName);
        onAnimationActivate = false;
        isAnimationFinished = false;
        isExitingState = false;
    }

    public virtual void Exit()
    {
        Debug.Log("End" + animBoolName);
        player.Anim.SetBool(animBoolName, false);
        isExitingState = true;
    }

    public virtual void LogicUpdate()
    {

    }

    public virtual void PhysicsUpdate()
    {
        DoChecks();
    }

    public virtual void DoChecks() { }

    public virtual void AnimationOnAction() => onAnimationActivate = true;

    public virtual void AnimationFinishTrigger() => isAnimationFinished = true;


}