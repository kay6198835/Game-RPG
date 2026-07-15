public class StateMachine<TState> where TState : class, IState
{
    public TState CurrentState { get; private set; }

    public void Initialize(TState startingState)
    {
        CurrentState = startingState;
        CurrentState.Enter();
    }

    public void ChangeState(TState newState)
    {
        CurrentState.Exit();
        CurrentState = newState;
        CurrentState.Enter();
    }
}
