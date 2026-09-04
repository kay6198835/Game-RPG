using UnityEngine;

/// <summary>
/// The only state that runs boss combat. It holds no attack logic of its own — it asks
/// <see cref="BossCommander"/> for the next command and drives that command's lifecycle.
/// Adding a boss attack therefore never adds a state.
/// </summary>
public class BossExecuteCommandState : EntityBasicState
{
    private readonly BossEntity boss;
    private BossCommander commander;
    private IBossCommand current;

    public BossExecuteCommandState(BossEntity boss, EntityStateMachine stateMachine, EntityData entityData, string animBoolName)
        : base(boss, stateMachine, entityData, animBoolName)
    {
        this.boss = boss;
    }

    public override void Enter()
    {
        base.Enter();
        entity.Core.GetCoreComponent(out commander);
        current = null;
    }

    /// <summary>
    /// Deliberately does not call <c>base.LogicUpdate()</c>: EntityBasicState auto-transitions into
    /// EntityAttackState, and on a boss the commander is the only thing allowed to choose an attack.
    /// The interrupts it does provide are re-implemented below.
    /// </summary>
    public override void LogicUpdate()
    {
        entity.Anim.SetFloat(GameConstants.AnimationName.Parameter.DIRECTION, entityInput.DirectionLook);

        if (commander == null) return;
        commander.Tick();

        if (HandleInterrupts()) return;

        if (current == null && !BeginNext()) return;

        ForwardAnimationStatus();
        current.Tick(Time.deltaTime);

        if (current.IsDone) EndCurrent();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        current?.PhysicsTick();
    }

    public override void Exit()
    {
        EndCurrent();
        base.Exit();
    }

    private bool HandleInterrupts()
    {
        if (entityInput.IsTakeDamage)
        {
            if (entityVitalStats != null && entityVitalStats.GetCurrentStatValue(StatType.HP) <= 0)
            {
                EndCurrent();
                stateMachine.ChangeState(entity.DeathState);
                return true;
            }

            if (commander.StaggerOnHit)
            {
                EndCurrent();
                stateMachine.ChangeState(boss.BossTakeDamageState);
                return true;
            }
        }

        // A phase change waits for the current command to finish, so an attack is never cut mid-swing.
        if (commander.WantsPhaseChange && current == null)
        {
            stateMachine.ChangeState(boss.PhaseTransitionState);
            return true;
        }

        return false;
    }

    private bool BeginNext()
    {
        IBossCommand next = commander.DequeueNext();
        if (next == null) return false;

        current = next;
        entity.Anim.SetBool(animBoolName, false);
        entity.Anim.SetBool(current.AnimBoolName, true);
        current.Enter(commander.Context);
        commander.SetActiveCommand(current);
        Status = StatusAnimation.Animaing;
        return true;
    }

    private void EndCurrent()
    {
        if (current == null) return;

        entity.Anim.SetBool(current.AnimBoolName, false);
        current.Exit();
        current = null;
        commander.SetActiveCommand(null);
        entity.Anim.SetBool(animBoolName, true);
    }

    /// <summary>
    /// Relays Animator events to the running command and consumes them, mirroring
    /// <see cref="EntityAttackState"/> — Status is durable, so the reader must clear it.
    /// </summary>
    private void ForwardAnimationStatus()
    {
        switch (Status)
        {
            case StatusAnimation.OnActivate:
                current.OnAnimationStatus(StatusAnimation.OnActivate);
                Status = StatusAnimation.OffActivate;
                break;
            case StatusAnimation.EndRangeTrigger:
                current.OnAnimationStatus(StatusAnimation.EndRangeTrigger);
                Status = StatusAnimation.Animaing;
                break;
        }
    }
}
