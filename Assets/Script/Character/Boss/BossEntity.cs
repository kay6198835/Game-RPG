using UnityEngine;

/// <summary>
/// A boss is an <see cref="Entity"/> whose combat loop is data-driven. It keeps the shared
/// death and flinch states, replaces the Idle/Move/Attack loop with a single
/// <see cref="BossExecuteCommandState"/>, and adds a phase transition beat.
/// Attacks live in <see cref="BossCommandSO"/> assets — adding one never adds a state class.
/// </summary>
public class BossEntity : Entity
{
    [Header("Boss")]
    [Tooltip("Fallback flinch length if the take-damage clip has no EndRangeTrigger event.")]
    [SerializeField][Range(0.1f, 5f)] private float maxStaggerTime = 0.8f;

    private BossExecuteCommandState executeCommandState;
    private BossPhaseTransitionState phaseTransitionState;
    private BossTakeDamageState bossTakeDamageState;

    public BossExecuteCommandState ExecuteCommandState => executeCommandState;
    public BossPhaseTransitionState PhaseTransitionState => phaseTransitionState;
    public BossTakeDamageState BossTakeDamageState => bossTakeDamageState;

    protected override void LoadState()
    {
        base.LoadState();

        executeCommandState = new BossExecuteCommandState(
            this, stateMachine, Data, GameConstants.AnimationName.IDLE);

        phaseTransitionState = new BossPhaseTransitionState(
            this, stateMachine, Data, GameConstants.AnimationName.IDLE);

        bossTakeDamageState = new BossTakeDamageState(
            this, stateMachine, Data, GameConstants.AnimationName.TAKE_DAMAGE, maxStaggerTime);
    }

    /// <summary>
    /// Starts straight in the command loop. Entity.Start() is intentionally not called — it would
    /// initialise the generic IdleState, and the boss's idling is a command, not a state.
    /// </summary>
    public override void Start()
    {
        stateMachine.Initialize(executeCommandState);
    }

    public override void Reborn()
    {
        stateMachine.Initialize(executeCommandState);
    }
}
