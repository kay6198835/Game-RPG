using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityStateMachine
{
    protected EntityState currentState;
    public EntityState CurrentState { get => currentState;}

    public void Initialize(EntityState startingState)
    {
        currentState = startingState;
        currentState.Enter();
    }

    public void ChangeState(EntityState newState)
    {
        currentState.Exit();
        currentState = newState;
        currentState.Enter();
    }
}
