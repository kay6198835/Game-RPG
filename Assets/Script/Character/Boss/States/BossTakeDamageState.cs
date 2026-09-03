using UnityEngine;

/// <summary>
/// Boss flinch. Returns to <see cref="BossExecuteCommandState"/> rather than MoveState, and carries
/// its own timeout so a missing EndRangeTrigger animation event cannot freeze the fight.
/// </summary>
public class BossTakeDamageState : EntityTakeDamageState
{
    private readonly BossEntity boss;
    private readonly float maxStaggerTime;

    public BossTakeDamageState(BossEntity boss, EntityStateMachine stateMachine, EntityData entityData, string animBoolName, float maxStaggerTime)
        : base(boss, stateMachine, entityData, animBoolName)
    {
        this.boss = boss;
        this.maxStaggerTime = maxStaggerTime;
    }

    public override void LogicUpdate()
    {
        if (Status == StatusAnimation.EndRangeTrigger || Time.time - startTime >= maxStaggerTime)
        {
            stateMachine.ChangeState(boss.ExecuteCommandState);
        }
    }
}
