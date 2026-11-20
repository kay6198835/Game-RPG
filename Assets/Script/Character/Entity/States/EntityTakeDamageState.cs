using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityTakeDamageState : EntityDisadvantageState
{
    EntityInput entityInput;
    EntityMovement entityMovement;
    EntityFindTarget entityFindTarget;
    EntityStatsHandler entityStats;
    public override void Enter()
    {
        base.Enter();
        entity.Core.GetCoreComponent(out entityMovement);
        entity.Core.GetCoreComponent(out entityInput);
        entity.Core.GetCoreComponent(out entityFindTarget);
        entity.Core.GetCoreComponent(out entityStats);
        entityMovement.StopMove();
        entity.Anim.SetFloat(GameConstants.AnimationName.Parameter.DIRECTION, entityInput.DirectionIsAttaked);
    }
    public EntityTakeDamageState(Entity etity, EntityStateMachine stateMachine, EntityData entityData, string animBoolName) : base(etity, stateMachine, entityData, animBoolName)
    {
    }
    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (entityInput.IsDeath)
        {
            stateMachine.ChangeState(entity.DeathState);
            return;
        }
        if (Status == StatusAnimation.EndRangeTrigger)
        {
            stateMachine.ChangeState(NextStateAfterHit());
        }
    }

    // Already locked: keep chasing. Otherwise turn the view toward the hit and look:
    // seen → lock and chase; blocked by an obstacle → Idle; out of view → investigate along the hit
    // direction, or Idle when no straight walkable point exists (see EntityMovement.TryPickInvestigatePoint).
    private EntityState NextStateAfterHit()
    {
        if (entityInput.IsLockTarget || entityFindTarget == null) return entity.MoveState;

        entityInput.FaceHitDirection();
        entityFindTarget.DistanceToPlayer();
        switch (entityFindTarget.CheckView())
        {
            case EntityFindTarget.ViewResult.Seen:
                entityInput.SetLockTarget(true);
                return entity.MoveState;
            case EntityFindTarget.ViewResult.Blocked:
                return entity.IdleState;
        }

        float distance = entityData.InvestigateDistance(entityStats);
        // MinRange can be 0 (Assasin): never investigate less than one grid cell.
        float minDistance = Mathf.Max(entityFindTarget.MinRange, 1f);
        if (entityMovement.TryPickInvestigatePoint(entityInput.DirectionIsAttakedVector, distance, minDistance, out Vector2 point))
        {
            entityInput.SetInvestigatePoint(point);
            return entity.InvestigateState;
        }
        return entity.IdleState;
    }

    public override void Exit()
    {
        base.Exit();
        entityMovement = null;
        entityInput = null;
        entityFindTarget = null;
        entityStats = null;
    }
}
