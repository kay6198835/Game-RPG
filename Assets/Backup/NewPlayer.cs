using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class NewPlayer : MonoBehaviour
{
    #region State Variables

    [SerializeField] private PlayerIdleState idleState;
    [SerializeField] private PlayerMoveState moveState;
    [SerializeField] private PlayerAttackState attackState;
    [SerializeField] private PlayerSkillWeaponState abilityState;
    [SerializeField] private PlayerEquidUnequid equidUnequidState;
    [SerializeField] private PlayerIntertorState intertorState;
    [SerializeField] private PlayerTakeDamageState takeDamageState;
    #endregion

    [SerializeField] private PlayerStateMachine stateMachine;
    [SerializeField] private PlayerData data;
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
    public PlayerAttackState AttackState { get => attackState;}
    public PlayerEquidUnequid EquidUnequidState { get => equidUnequidState; }
    public PlayerIntertorState IntertorState { get => intertorState; }
    public PlayerSkillWeaponState AbilityState { get => abilityState;}
    public PlayerTakeDamageState TakeDamageState { get => takeDamageState;}
    public PlayerData Data { get => data; }

    #endregion

    #region Unity Callback Functions
    private void Awake()
    {
        Debug.Log("On");
        stateMachine = new PlayerStateMachine();
        core = GetComponentInChildren<Core>();
        inputHandler = GetComponentInChildren<PlayerInputHandler>();
        idleState = new PlayerIdleState(this,"Idle");
        moveState = new PlayerMoveState(this,"Move");
        attackState = new PlayerAttackState(this,"Attack");
        equidUnequidState = new PlayerEquidUnequid(this, "EquidUnequid");
        intertorState = new PlayerIntertorState(this, "Interactor");
        abilityState = new PlayerSkillWeaponState(this,"Ability");
        takeDamageState = new PlayerTakeDamageState(this, "TakeDamage");
    }

    private void Start()
    {
        anim = GetComponent<Animator>();
        rigidbodyPlayer = GetComponent<Rigidbody2D>();
        stateMachine.Initialize(idleState);
    }

    private void Update()
    {
        //core.LogicUpdate();
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
    private void AnimationExitingState() => stateMachine.CurrentState.AnimationExitingState();


    #endregion
}