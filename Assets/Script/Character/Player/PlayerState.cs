using UnityEngine;
[System.Serializable]
public class PlayerState
{
    //protected Core core;

    protected Player player;
    protected PlayerStateMachine stateMachine;
    protected PlayerData playerData;

    protected bool isAnimationTrigger;
    protected bool isAnimationAction;
    protected bool isAnimationFinished;
    protected bool isAnimationExitingState;
    protected bool isExitingState;
    protected float startTime;
    protected string animBoolName;
    //protected StateStyle stateStyle;
    public enum StateStyle
    {
        Freeze,
        Motion
    }
    protected PlayerInputHandler inputHandler;
    protected WeaponHolder weaponHolder;
    protected Interactor interactor;
    protected AbilityHolder abilityHolder;
    protected PlayerMovement playerMovement;
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
        player.Core.GetCoreComponent(out inputHandler);
        player.Core.GetCoreComponent(out weaponHolder);
        player.Core.GetCoreComponent(out interactor);
        player.Core.GetCoreComponent(out abilityHolder);
        player.Core.GetCoreComponent(out playerMovement);
        player.Anim.SetBool(animBoolName, true);
        startTime = Time.time;
        //Debug.Log("Start" + animBoolName);
        isAnimationTrigger = false;
        isAnimationFinished = false;
        isExitingState = false;

    }
    public virtual void Exit()
    {
        player.Anim.SetBool(animBoolName, false);
        isExitingState = true;
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

    public virtual void AnimationTrigger()
    {
        isAnimationTrigger = true;
    }

    public virtual void AnimationAction()
    {
        isAnimationAction = true;
    }

    public virtual void AnimationFinishTrigger()
    {
        isAnimationFinished = true;
    }

    public virtual void AnimationExitingState()
    {
        isAnimationExitingState = true;
    }


}