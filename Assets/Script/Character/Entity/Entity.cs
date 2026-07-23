using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : BaseEntity
{
    [SerializeField] protected EntityInput input;
    [SerializeField] protected EntityCore core;


    [SerializeField] protected Animator anim;
    [SerializeField] protected Rigidbody2D rb;
    [SerializeField] protected ParticleSystem particle;
    [SerializeField] protected EntityStateMachine stateMachine;
    [SerializeField] protected EntityData data;
    [SerializeField] private EntityIdleState idleState;
    [SerializeField] private EntityMoveState moveState;
    [SerializeField] private EntityAttackState attackState;
    [SerializeField] private EntityTakeDamageState takeDamageState;
    [SerializeField] private EntityDeathState deathState;
    public EntityInput Input { get => input;}
    public Animator Anim { get => anim;}
    public Rigidbody2D Rb { get => rb; }
    public ParticleSystem Particle { get => particle; }
    public EntityCore Core { get => core;}
    public EntityStateMachine StateMachine { get => stateMachine;}
    public EntityIdleState IdleState { get => idleState;}
    public EntityMoveState MoveState { get => moveState; }
    public EntityAttackState AttackState { get => attackState; }
    public EntityTakeDamageState TakeDamageState { get => takeDamageState; }
    public EntityDeathState DeathState { get => deathState; }
    public EntityData Data { get => data;}

    private void Awake()
    {
        LoadEntity();
        LoadState();
    }
    private void Start()
    {
        stateMachine.Initialize(idleState);
    }
    protected override IState CurrentState => stateMachine.CurrentState;
    private void LoadEntity()
    {
        stateMachine = new EntityStateMachine();
        core = GetComponentInChildren<EntityCore>();
        input = GetComponentInChildren<EntityInput>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        particle = GetComponentInChildren<ParticleSystem>();
        anim.runtimeAnimatorController = data.Aima;
    }
    private void LoadState()
    {
        idleState = new EntityIdleState(this, stateMachine, data, "Idle");
        moveState = new EntityMoveState(this, stateMachine, data, "Move");
        attackState = new EntityAttackState(this, stateMachine, data, "Attack");
        takeDamageState = new EntityTakeDamageState(this, stateMachine, data, "TakeDamage");
        deathState = new EntityDeathState(this, stateMachine, data, "Death");
    }

    public void SetDataEntity(EntityData data)
    {
        this.data = data;
        gameObject.name = data.name;
    }

    protected void AnimationTrigger() => stateMachine.CurrentState.SetAnimationStatus(StatusAnimation.StartRangeTrigger);

    protected void AnimtionFinishTrigger() => stateMachine.CurrentState.SetAnimationStatus(StatusAnimation.EndRangeTrigger);
}
