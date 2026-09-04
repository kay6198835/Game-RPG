using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : BaseEntity
{
    [SerializeField] protected EntityCore core;
    [SerializeField] protected Animator anim;
    [SerializeField] protected Rigidbody2D rb;
    [SerializeField] protected ParticleSystem particle;
    [SerializeField] public EntityStateMachine stateMachine = new EntityStateMachine();
    [SerializeField] private EntityIdleState idleState;
    [SerializeField] private EntityMoveState moveState;
    [SerializeField] private EntityAttackState attackState;
    [SerializeField] private EntityTakeDamageState takeDamageState;
    [SerializeField] private EntityDeathState deathState;
    [SerializeField] private EntityData data;
    public Animator Anim { get => anim; }
    public Rigidbody2D Rb { get => rb; }
    public ParticleSystem Particle { get => particle; }
    public EntityCore Core { get => core; }
    public EntityStateMachine StateMachine { get => stateMachine; }
    public EntityIdleState IdleState { get => idleState; }
    public EntityMoveState MoveState { get => moveState; }
    public EntityAttackState AttackState { get => attackState; }
    public EntityTakeDamageState TakeDamageState { get => takeDamageState; }
    public EntityDeathState DeathState { get => deathState; }
    public EntityData Data { get => data; }

    public override void Awake()
    {
        LoadEntity();
        LoadState();
    }
    public override void Start()
    {
        base.Start();
        stateMachine.Initialize(idleState);
    }
    public void OnEnable()
    {
        //Reborn();
    }
    public virtual void Reborn()
    {
        stateMachine.Initialize(idleState);
    }
    protected override IState CurrentState => stateMachine.CurrentState;
    protected virtual void LoadEntity()
    {
        stateMachine = new EntityStateMachine();
        core = GetComponentInChildren<EntityCore>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        particle = GetComponentInChildren<ParticleSystem>();
        anim.runtimeAnimatorController = data.Aima;
    }
    protected virtual void LoadState()
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

    private void AnimationStart() => stateMachine.CurrentState.SetAnimationStatus(StatusAnimation.Start);
    private void AnimationTrigger() => stateMachine.CurrentState.SetAnimationStatus(StatusAnimation.StartRangeTrigger);
    private void AnimationOnAction() => stateMachine.CurrentState.SetAnimationStatus(StatusAnimation.OnActivate);
    private void AnimationOffAction() => stateMachine.CurrentState.SetAnimationStatus(StatusAnimation.OffActivate);
    private void AnimationFinishTrigger() => stateMachine.CurrentState.SetAnimationStatus(StatusAnimation.EndRangeTrigger);
    private void AnimationEnd() => stateMachine.CurrentState.SetAnimationStatus(StatusAnimation.End);

}
