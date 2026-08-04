using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityMoveState : EntityBasicState
{
    protected float moveTime;
    protected float moveDurationTime;
    protected float speed;

    private Path _path;
    private int _wpIndex;
    private float _nextRequestTime;

    public EntityMoveState(Entity etity, EntityStateMachine stateMachine, EntityData entityData, string animBoolName)
        : base(etity, stateMachine, entityData, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        moveDurationTime = entityData.MoveDurationTime;
        moveTime = startTime + moveDurationTime;
        _path = null;
        _wpIndex = 0;
        _nextRequestTime = 0f;
        if (entity.Input.Target == null)
        {
            entity.Input.SetDirectionRadom();
        }
    }

    public override void LogicUpdate()
    {
        speed = entityData.MovementVelocities;

        if (entity.Input.Target == null)
        {
            Wander();
            base.LogicUpdate();
            return;
        }

        PathfindingGrid grid = EnemyManager.Instance != null ? EnemyManager.Instance.Grid : null;
        if (grid == null)
        {
            ChaseStraight();
            base.LogicUpdate();
            return;
        }

        RequestPathThrottled();
        if (_path == null) ChaseStraight();
        else FollowPath();

        base.LogicUpdate();
    }

    private void RequestPathThrottled()
    {
        if (Time.time < _nextRequestTime) return;
        _nextRequestTime = Time.time + entityData.PathRequestInterval;
        Vector3 start = entity.transform.position;
        Vector3 target = entity.Input.Target.transform.position;
        EnemyManager.Instance.RequestPath(new PathRequest(start, target, OnPathReceived));
    }

    private void OnPathReceived(Path path)
    {
        if (!path.Success || path.Waypoints.Count == 0) return;
        _path = path;
        _wpIndex = path.Waypoints.Count > 1 ? 1 : 0;
    }

    private void FollowPath()
    {
        float reach = Mathf.Max(0.1f, speed * Time.fixedDeltaTime * 2f);
        reach = Mathf.Min(reach, entityData.WaypointReachDistance);

        Vector2 pos = entity.transform.position;
        Vector2 wp = _path.Waypoints[_wpIndex];

        if ((wp - pos).sqrMagnitude <= reach * reach)
        {
            _wpIndex++;
            if (_wpIndex >= _path.Waypoints.Count)
            {
                entityCore.EntityMovement.MoveForwardTarget(Vector2.zero);
                _path = null;
                return;
            }
            wp = _path.Waypoints[_wpIndex];
        }

        Vector2 dir = (wp - pos).normalized;
        entityCore.EntityMovement.MoveForwardTarget(dir * speed);
    }

    private void ChaseStraight()
    {
        Vector2 dir = ((Vector2)entity.Input.Target.transform.position - (Vector2)entity.transform.position).normalized;
        entityCore.EntityMovement.MoveForwardTarget(dir * speed);
    }

    private void Wander()
    {
        Vector2 dir = entity.Input.DirectionLookVector.normalized;
        entityCore.EntityMovement.MoveForwardTarget(dir * speed);
        if (entityCore.FindTarget.FindWall(dir, speed))
        {
            entity.Input.TurnLeftOrRight();
        }
        if (moveTime <= Time.time)
        {
            entity.StateMachine.ChangeState(entity.IdleState);
        }
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Exit()
    {
        base.Exit();
        entityCore.EntityMovement.MoveForwardTarget(Vector2.zero);
    }
}
