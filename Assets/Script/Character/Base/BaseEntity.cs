using UnityEngine;

public abstract class BaseEntity : MonoBehaviour
{
    protected abstract IState CurrentState { get; }

    protected virtual void Update()
    {
        IState state = CurrentState;
        if (state != null) state.LogicUpdate();
    }

    protected virtual void FixedUpdate()
    {
        IState state = CurrentState;
        if (state != null) state.PhysicsUpdate();
    }
}
