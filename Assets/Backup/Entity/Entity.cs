using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [SerializeField] protected EntityInput input;
    [SerializeField] protected EntityCore core;


    [SerializeField] protected Animator anim;
    [SerializeField] protected Rigidbody2D rb;
    [SerializeField] protected EntityStateMachine stateMachine;
    [SerializeField] protected EntityData data;

    [SerializeField] private EntityIdleState idleState;
    [SerializeField] private EntityMoveState moveState;
    [SerializeField] private EntityAttackState attackState;
    [SerializeField] private EntityTakeDamageState takeDamageState;
    public EntityInput Input { get => input;}
    public Animator Anim { get => anim;}
    public Rigidbody2D Rb { get => rb; }
    public EntityCore Core { get => core;}
    public EntityStateMachine StateMachine { get => stateMachine;}
    public EntityIdleState IdleState { get => idleState;}
    public EntityMoveState MoveState { get => moveState; }
    public EntityAttackState AttackState { get => attackState; }
    public EntityTakeDamageState TakeDamageState { get => takeDamageState; }
    public EntityData Data { get => data;}

    private void Awake()
    {
        LoadState();
        LoadEntity();

        stateMachine = new EntityStateMachine();
        core = GetComponentInChildren<EntityCore>();
        input = GetComponentInChildren<EntityInput>();
        idleState = new EntityIdleState(this, stateMachine, data, "Idle");
        moveState = new EntityMoveState(this, stateMachine, data, "Move");
        attackState = new EntityAttackState(this, stateMachine, data, "Attack");
        takeDamageState = new EntityTakeDamageState(this, stateMachine, data, "TakeDamage");
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        anim.runtimeAnimatorController = data.Aima;
    }
    private void Start()
    {
        stateMachine.Initialize(idleState);
    }
    private void Update()
    {
        stateMachine.CurrentState.LogicUpdate();
    }
    private void LoadEntity()
    {

    }
    private void LoadState()
    {

    }

    protected void AnimationTrigger() => stateMachine.CurrentState.AnimationTrigger();

    protected void AnimtionFinishTrigger() => stateMachine.CurrentState.AnimationFinishTrigger();
}
