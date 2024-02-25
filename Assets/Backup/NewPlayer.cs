using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class NewPlayer : MonoBehaviour
{
    #region State Variables
    [SerializeField] private PlayerStateMachine stateMachine;
    [SerializeField] private PlayerIdleState idleState;
    [SerializeField] private PlayerMoveState moveState;


    [SerializeField]
    private PlayerData playerData;
    #endregion


    [SerializeField] private Core core;
    [SerializeField] private PlayerInputHandler inputHandler;
    [SerializeField] private Animator anim;
    [SerializeField] private Rigidbody2D rigidbodyPlayer;
    #region Components

    public Core Core { get => core;}
    public Animator Anim { get => anim;}
    public Rigidbody2D RigidbodyPlayer { get => rigidbodyPlayer;}
    public PlayerInputHandler InputHandler { get => inputHandler;}
    public PlayerStateMachine StateMachine { get => stateMachine;}
    public PlayerIdleState IdleState { get => idleState;}
    public PlayerMoveState MoveState { get => moveState;}
    #endregion

    #region Unity Callback Functions
    private void Awake()
    {
        stateMachine = new PlayerStateMachine();
        core = GetComponentInChildren<Core>();
        inputHandler = GetComponentInChildren<PlayerInputHandler>();


        idleState = new PlayerIdleState(this,stateMachine , playerData, "Idle");
        moveState = new PlayerMoveState(this,stateMachine , playerData, "Move");
    }

    private void Start()
    {
        anim = GetComponent<Animator>();
        rigidbodyPlayer = GetComponent<Rigidbody2D>();
        stateMachine.Initialize(idleState);
    }

    private void Update()
    {
        core.LogicUpdate();
        stateMachine.CurrentState.LogicUpdate();
    }

    private void FixedUpdate()
    {
        stateMachine.CurrentState.PhysicsUpdate();
    }
    #endregion

    #region Other Functions

    private void AnimationTrigger() => stateMachine.CurrentState.AnimationTrigger();

    private void AnimtionFinishTrigger() => stateMachine.CurrentState.AnimationFinishTrigger();


    #endregion
}