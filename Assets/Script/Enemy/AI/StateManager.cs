using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
[RequireComponent(typeof(CheckIsInRange))]
[RequireComponent(typeof(MovementEnemy))]
public class StateManager : MonoBehaviour
{
    [Header("Controll")]
    [SerializeField] private Enemy enemyCtrl;
    [SerializeField] private State currentState;
    [SerializeField] private Transform target;
    [SerializeField] private EnemySO enemySO;
    public Enemy EnemyCtrl { get => enemyCtrl; }
    public State CurrentState { get => currentState; }
    public Transform Target { get => target; set => target = value; }
    public EnemySO Stats { get => enemySO;}

    [Header ("State")]
    [SerializeField] private bool isInFieldOfView;
    [SerializeField] private bool isInAttackRange;
    [SerializeField] private bool isChase;
    [SerializeField] private bool isIdle;
    [SerializeField] private bool isAttack;
    [SerializeField] private bool isInRoom  = true;

    public bool IsInFieldOfView { get => isInFieldOfView; set => isInFieldOfView = value; }
    public bool IsInAttackRange { get => isInAttackRange; set => isInAttackRange = value; }
    public bool IsChase { get => isChase; set => isChase = value; }
    public bool IsIdle { get => isIdle; set => isIdle = value; }
    public bool IsAttack { get => isAttack; set => isAttack = value; }
    public bool IsInRoom { get => isInRoom; set => isInRoom = value; }

    [Header("Behaviour")]
    [SerializeField] private CheckIsInRange checkInRange;
    [SerializeField] private MovementEnemy movement;
    [SerializeField] private EnemyAttack enemyAttack;

    public CheckIsInRange CheckInRange { get => checkInRange;}
    public MovementEnemy Movement { get => movement;}
    public EnemyAttack EnemyAttack { get => enemyAttack;}
    private void Awake()
    {
        checkInRange = GetComponent<CheckIsInRange>();
        movement = GetComponent<MovementEnemy>();
        enemyAttack = GetComponent<EnemyAttack>();
        enemyCtrl = GetComponentInParent<Enemy>();
        currentState = GetComponentInChildren<IdleState>();
        enemySO = enemyCtrl.EnemySO;
    }
    void Update()
    {
        RunStateMachine();
    }
    private void RunStateMachine()
    {
        State nextState = currentState?.RunCurrentState();

        if (nextState != null)
        {
            SwitchToTheNextState(nextState);
        }
    }
    private void SwitchToTheNextState(State nextState)
    {
        if (nextState == currentState)
        {
            return;
        }
        currentState = nextState;
        currentState.StartCurrentState();
    }
}
