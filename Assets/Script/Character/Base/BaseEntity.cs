using UnityEngine;

public abstract class BaseEntity : MonoBehaviour
{
    protected abstract IState CurrentState { get; }
    public virtual void Awake()
    {

    }
    public virtual void Start()
    {

    }
    public virtual void Update()
    {
        IState state = CurrentState;
        if (state != null) state.LogicUpdate();
    }

    public virtual void FixedUpdate()
    {
        IState state = CurrentState;
        if (state != null) state.PhysicsUpdate();
    }
}
