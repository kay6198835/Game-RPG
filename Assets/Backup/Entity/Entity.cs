using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [SerializeField] private EntityInput inputHandler;
    [SerializeField] private EntityCore entityCore;
    [SerializeField] private Animator anim;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private EntityStateMachine stateMachine;
    [SerializeField] private EntityData entityData;

    [SerializeField] private EntityIdleState idleState;
    [SerializeField] private EntityMoveRandomState moveRandomState;
    [SerializeField] private EntityMoveToTargetState moveToTargetState;
    [SerializeField] private EntityAttackState attackState;
    public EntityInput InputHandler { get => inputHandler;}
    public Animator Anim { get => anim;}
    public Rigidbody2D Rb { get => rb; }
    public EntityCore EntityCore { get => entityCore;}
    public EntityStateMachine StateMachine { get => stateMachine;}
    public EntityIdleState IdleState { get => idleState;}
    public EntityMoveRandomState MoveRandomState { get => moveRandomState; }
    public EntityMoveToTargetState MoveToTargetState { get => moveToTargetState; }
    public EntityAttackState AttackState { get => attackState; }
    public EntityData EntityData { get => entityData;}

    private void Awake()
    {
        stateMachine = new EntityStateMachine();
        entityCore = GetComponentInChildren<EntityCore>();
        inputHandler = GetComponentInChildren<EntityInput>();
        idleState = new EntityIdleState(this,stateMachine,entityData,"Idle");
        moveRandomState = new EntityMoveRandomState(this,stateMachine,entityData,"Move");
        moveToTargetState = new EntityMoveToTargetState(this,stateMachine,entityData,"Move");
        attackState = new EntityAttackState(this,stateMachine,entityData,"Attack");
        anim =  GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        anim.runtimeAnimatorController = entityData.Aima;
        stateMachine.Initialize(idleState);
    }
    private void Update()
    {
        stateMachine.CurrentState.LogicUpdate();
    }

    protected void AnimationTrigger() => stateMachine.CurrentState.AnimationTrigger();

    protected void AnimtionFinishTrigger() => stateMachine.CurrentState.AnimationFinishTrigger();
}
