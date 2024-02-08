using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionTargetEnemy : DirectionTarget
{
    [SerializeField] StateManager stateManager;
    private void Awake()
    {
        stateManager = GetComponent<StateManager>();
    }
    private void Start()
    {
        
    }
    private void Update()
    {

        DirectionToTarget();
    }
    private void CheckTarget()
    {

    }
    public override void DirectionToTarget()
    {
        if (target == null)
        {
            if (stateManager.Target == null)
            {
                return;
            }
            target = stateManager.Target;
        }
        base.DirectionToTarget();
    }
}
