using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMoveState : State
{
    [SerializeField] private IdleState idleState;
    [SerializeField] private MovementEnemy movement;

    [SerializeField] private int moveTime;
    [SerializeField] private float endMoveTime;
    [SerializeField] private float startMoveTime;
    [SerializeField] private bool isMove;

    //[SerializeField] private State state;

    private void Awake()
    {
        base.LoadState();
        movement = stateManager.GetComponent<MovementEnemy>();
        idleState = this.transform.parent.GetComponentInChildren<IdleState>();
    }
    void Start()
    {
        StartCurrentState();
    }
    public override void StartCurrentState()
    {
        startMoveTime =Time.time;
        endMoveTime = Time.time+ 1f;
        movement.DirectionVector = new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));
        movement.AngleCalculate(Vector2.zero);
    }
    public override State RunCurrentState()
    {
        stateManager.EnemyCtrl.AnimationManager.Animation_2_Run();
        movement.Move();
        if (stateManager.IsChase)
        {
            startMoveTime += Time.deltaTime;
        }
        return CheckState();
    }
    public override State CheckState()
    {
        if (startMoveTime >=endMoveTime)
        {
            movement.DirectionVector = Vector2.zero;
            return idleState;
        }
        return this;
    }
}
