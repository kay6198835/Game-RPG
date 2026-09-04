using UnityEngine;

/// <summary>
/// Invulnerable beat between two phases. Exists so a phase change is a readable moment for the
/// player rather than a silent stat swap mid-combo.
/// </summary>
public class BossPhaseTransitionState : EntityBasicState
{
    private readonly BossEntity boss;
    private BossCommander commander;
    private float duration;

    public BossPhaseTransitionState(BossEntity boss, EntityStateMachine stateMachine, EntityData entityData, string animBoolName)
        : base(boss, stateMachine, entityData, animBoolName)
    {
        this.boss = boss;
    }

    public override void Enter()
    {
        base.Enter();
        entity.Core.GetCoreComponent(out commander);

        if (entityMovement != null) entityMovement.StopMove();

        commander.BeginPhaseTransition();
        duration = commander.PendingTransitionDuration;
    }

    public override void LogicUpdate()
    {
        entity.Anim.SetFloat(GameConstants.AnimationName.Parameter.DIRECTION, entityInput.DirectionLook);

        if (entityVitalStats != null && entityVitalStats.GetCurrentStatValue(StatType.HP) <= 0)
        {
            commander.CommitPhaseChange();
            stateMachine.ChangeState(entity.DeathState);
            return;
        }

        if (Time.time - startTime < duration) return;

        commander.CommitPhaseChange();
        stateMachine.ChangeState(boss.ExecuteCommandState);
    }
}
