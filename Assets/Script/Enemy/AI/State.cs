using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State : MonoBehaviour
{
    [SerializeField] protected StateManager stateManager;

    public StateManager StateManager { get => stateManager; }

    protected virtual void LoadState()
    {
        stateManager=GetComponentInParent<StateManager>();
    }
    public abstract void StartCurrentState();
    public abstract State RunCurrentState();
    public abstract State CheckState();
}
