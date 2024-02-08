using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
public class IdleState : State
{
    [SerializeField] private ChaseState chaseState;
    [SerializeField] private RandomMoveState randomMoveState;
    [SerializeField] private CheckIsInRange CIR_FieldOfView;
    [SerializeField] private State state;
    [SerializeField] private int moveTime;
    private void Awake()
    {
        LoadState();
        CIR_FieldOfView = stateManager.CheckInRange;
        chaseState = this.transform.parent.GetComponentInChildren<ChaseState>();
        randomMoveState = this.transform.parent.GetComponentInChildren<RandomMoveState>();
    }
    private void Start()
    {
        StartCurrentState();
    }
    public override void StartCurrentState()
    {
        CIR_FieldOfView.TargetMask = stateManager.Stats.layerMask;
        CIR_FieldOfView.Range = stateManager.Stats.fieldOfViewRange;
        state = this;
    }
    public override State RunCurrentState()
    {
        stateManager.EnemyCtrl.AnimationManager.Animation_1_Idle();
        if (!stateManager.EnemyCtrl.StateManager.IsInRoom)
        {
            return state;
        }
        stateManager.IsChase = false;
        CIR_FieldOfView.Check();
        stateManager.IsInFieldOfView = CIR_FieldOfView.IsInRange;
        return CheckState();
    }
    public override State CheckState()
    {
        if (stateManager.IsInFieldOfView)
        {
            stateManager.Target = CIR_FieldOfView.Target;
            state = chaseState;
        }
        else
        {
            StartCoroutine(IdleTime());
        }
        return state;
    }
    IEnumerator IdleTime()
    {
        yield return new WaitForSecondsRealtime(1f);
        state = randomMoveState;
    }
}
