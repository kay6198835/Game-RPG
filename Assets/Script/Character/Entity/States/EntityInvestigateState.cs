using UnityEngine;

/// <summary>
/// Hit from out of view: walk straight to EntityInput.InvestigatePoint (picked by EntityTakeDamageState along
/// the hit direction). Sees the player on the way → lock and chase (MoveState); arrives or the path fails → Idle.
/// Plays the Move animation.
/// </summary>
public class EntityInvestigateState : EntityBasicState
{
    public EntityInvestigateState(Entity etity, EntityStateMachine stateMachine, EntityData entityData, string animBoolName) : base(etity, stateMachine, entityData, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        entityMovement.StopMove();
        entityMovement.MoveTo(entityInput.InvestigatePoint);
    }

    public override void LogicUpdate()
    {
        if (entityInput.IsLockTarget)
        {
            entity.StateMachine.ChangeState(entity.MoveState);
            return;
        }
        if (entityMovement.PathFailed || entityMovement.HasReachedEnd)
        {
            entity.StateMachine.ChangeState(entity.IdleState);
            return;
        }
        entityMovement.MoveToNodeTarget();
        // Death, take-damage and attack transitions, facing and the per-frame target search.
        base.LogicUpdate();
    }

    public override void PhysicsUpdate()
    {
        if (entityMovement != null) entityMovement.MoveForwardToTarget();
    }

    public override void Exit()
    {
        entityMovement.StopMove();
        entityInput.ClearInvestigatePoint();
        base.Exit();
    }
}
