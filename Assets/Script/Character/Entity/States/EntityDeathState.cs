using UnityEngine;

public class EntityDeathState : EntityDisadvantageState
{
    private bool hasDied;

    public EntityDeathState(Entity etity, EntityStateMachine stateMachine, EntityData entityData, string animBoolName) : base(etity, stateMachine, entityData, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        hasDied = false;
        entityCore.EntityMovement.MoveForwardTarget(Vector2.zero);
        entity.Rb.simulated = false;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        // deathDurationTime fallback: enemies whose animator lacks a Death clip never
        // receive AnimationFinishTrigger and would linger forever
        if (isAnimationFinished || Time.time >= startTime + entityData.DeathDurationTime)
        {
            Die();
        }
    }

    private void Die()
    {
        if (hasDied) return;
        hasDied = true;
        EventManager.Emit(EventID.ON_ENEMY_DEATH);
        Object.Destroy(entity.gameObject);
    }
}
