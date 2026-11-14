using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;
public class EntityMovement : MovementBase<EntityCore>
{
    [SerializeField] protected List<Vector2> Waypoints;
    [SerializeField] protected Vector2 targetPosition;
    [SerializeField] protected Vector2 endPosition;
    [SerializeField] protected int indexWaypoints;
    // NEED CHECK: removed — was 0 on EnemyPrefab and assigned nowhere. Speed is MovementBase.MoveSpeed (StatType.MoveSpeed).
    //[SerializeField] protected float speed;
    [SerializeField] protected int maxRadiusSpawnPoint;
    [SerializeField] protected float distance;
    [SerializeField] protected Vector2 forwardDirection;
    [SerializeField] protected Vector2 _lastPlayerNodePosition;
    [SerializeField] protected Vector2 currentPlayerNodePosition;
    [SerializeField] protected PathfindingGrid grid;
    [SerializeField] protected Vector2[] allDirection;
    [SerializeField] protected EntityInput entityInput;
    [SerializeField] protected EntityFindTarget entityFindTarget;
    [Header("Path")]
    // Minimum time between two path requests, so a moving player does not trigger one per frame.
    [SerializeField, Range(0.05f, 1f)] protected float repathInterval = 0.25f;
    [SerializeField, Range(0.5f, 10f)] protected float fleeDistance = 3f;
    protected float nextRepathTime;
    protected bool hasPlayerNode;
    protected bool isFleeing;

    protected override void Start()
    {
        base.Start();
        Core.GetCoreComponent(out entityInput);
        Core.GetCoreComponent(out entityFindTarget);
        if (maxRadiusSpawnPoint == 0) maxRadiusSpawnPoint = 3;
        indexWaypoints = 0;
        grid = EnemyManager.Instance.Grid;
        allDirection = (Vector2[])GameConstants.Direction.Vector.ALL.Clone();
    }
    // public void CheckMove()
    // {
    //     if (entityInput.IsLockTarget)
    //     {
    //         if (entityFindTarget.IsNearPlayer())
    //         {
    //             FleeTarget();
    //         }
    //         else
    //         {
    //             ChaseToTarget();
    //         }
    //     }
    //     else
    //     {
    //         ToRandomPosition();
    //     }
    //     MoveToNodeTarget();
    // }

    public void SendResquestPath()
    {
        if (endPosition == Vector2.zero) return;
        if (Time.time < nextRepathTime) return;
        nextRepathTime = Time.time + repathInterval;
        PathRequest request = new PathRequest(transform.position, endPosition, GetPath);
        EnemyManager.Instance.RequestPath(request);
    }

    public void FleeTarget()
    {
        if (!entityFindTarget.IsNearPlayer()) return;
        // Keep the current flee point until it is reached; picking a new one every frame makes the path flicker.
        if (isFleeing && !CheckNearPostion(endPosition)) return;
        isFleeing = true;
        hasPlayerNode = false;
        var fleePosition = Core.Entity.transform.position +
        ((Core.Entity.transform.position - entityInput.TargetPosition()).normalized * fleeDistance);
        Node validNode = EnemyManager.Instance.GetNodeByPositionWorld((Vector2)fleePosition);

        if (validNode != null && validNode.Walkable)
        {
            endPosition = validNode.WorldPosition;
        }
        else
        {
            validNode = SetNodeRandom((Vector2)fleePosition);
            endPosition = validNode != null ? validNode.WorldPosition : Core.Entity.transform.position;
        }
        // The old chase path still points at the player: drop it and path to the flee point now.
        Waypoints.Clear();
        targetPosition = Vector2.zero;
        nextRepathTime = 0f;
        SendResquestPath();
    }

    /// <summary>Paths to a point at the preferred attack distance from the locked target; repaths only when the player changes grid cell.</summary>
    public void ChaseToTarget()
    {
        bool wasFleeing = isFleeing;
        isFleeing = false;
        if (!CheckPlayerPosition() && !wasFleeing) return;

        Vector2 playerPosition = entityInput.TargetPosition();
        Vector2 self = Core.Entity.transform.Position2D();
        Vector2 away = self - playerPosition;
        Vector2 standOff = away.sqrMagnitude > Mathf.Epsilon
            ? playerPosition + away.normalized * entityFindTarget.PreferredDistance
            : playerPosition;

        Node node = EnemyManager.Instance.GetNodeByPositionWorld(standOff);
        endPosition = node != null && node.Walkable ? node.WorldPosition : playerPosition;
        if (wasFleeing)
        {
            Waypoints.Clear();
            targetPosition = Vector2.zero;
            nextRepathTime = 0f;
        }
        SendResquestPath();
    }

    private bool CheckNearPostion(Vector2 positionCheck)
    {
        float distance = Vector2.Distance(transform.position, positionCheck);
        return distance < GameConstants.SettingStats.PADDING_NODE_VALUE ? true : false;
    }

    public void MoveToNodeTarget()
    {
        if (Waypoints.Count == 0)
        {
            if (!CheckNearPostion(endPosition)) SendResquestPath();
            return;
        }
        if (CheckNearPostion(targetPosition)) indexWaypoints++;

        if (indexWaypoints > Waypoints.Count - 1)
        {
            // Arrived: stand still instead of re-requesting a path to the point already reached.
            Waypoints.Clear();
            indexWaypoints = 0;
            targetPosition = Vector2.zero;
            if (!CheckNearPostion(endPosition)) SendResquestPath();
            return;
        }
        SetPointToForward(Waypoints[indexWaypoints]);
    }

    private bool CheckPlayerPosition()
    {
        if (grid == null) return false;
        SearchNode node = grid.GetNodeFromWorld(entityInput.TargetPosition());
        if (node == null) return false;
        currentPlayerNodePosition = node.TileMapPosition;
        // Repath only when the player moved to another grid cell (or on the first chase frame).
        if (hasPlayerNode && currentPlayerNodePosition == _lastPlayerNodePosition) return false;
        hasPlayerNode = true;
        _lastPlayerNodePosition = currentPlayerNodePosition;
        return true;
    }

    public void MoveForwardToTarget()
    {
        if (targetPosition == Vector2.zero || !CanMove) return;
        Vector2 toTarget = targetPosition - rb.position;
        if (toTarget.sqrMagnitude <= Mathf.Epsilon) return;
        float step = MoveSpeed * SpeedMultiplier * Time.fixedDeltaTime;
        forwardDirection = toTarget.normalized;
        // Clamp the last step onto the point; a full step overshoots and the entity bounces back every frame.
        rb.MovePosition(toTarget.sqrMagnitude <= step * step
            ? targetPosition
            : rb.position + forwardDirection * step);
        entityInput.SetTarget(targetPosition);
    }

    private void SetPointToForward(Vector2 targetPosition)
    {
        this.targetPosition = targetPosition;
    }

    private void GetPath(NavPath path)
    {
        if (!path.Success || path.Waypoints.Count == 0) return;
        Waypoints.Clear();
        Waypoints.AddRange(path.Waypoints);
        indexWaypoints = path.Waypoints.Count > 1 ? 1 : 0;
        if (Waypoints.Count != 0) SetPointToForward(Waypoints[indexWaypoints]);
    }


    private const int MaxSetNodeAttempts = 10;

    public void ToRandomPosition()
    {
        if (CheckNearPostion(endPosition) || endPosition == Vector2.zero)
        {
            Vector2 spawnPoint = entityInput.SpawnPoint;
            Node node = SetNodeRandom(spawnPoint);
            endPosition = node != null ? node.WorldPosition : spawnPoint;

        }
    }

    public void SetPositionToCheck(Vector2 endPosition)
    {
        var rangeToCheck = Random.Range(30, 75)/100f;
        this.endPosition = Vector2.Lerp(transform.position, endPosition, rangeToCheck);
    }

    private Node SetNodeRandom(Vector2 orginPosition)
    {
        for (int attempt = 0; attempt < MaxSetNodeAttempts; attempt++)
        {
            Vector2Int randomAddRangePosition = Utility.RandomPaddingDistace(0, maxRadiusSpawnPoint);
            Utility.RandomShuffle(allDirection);
            foreach (var direction in allDirection)
            {
                var worldPosition = (Vector2)randomAddRangePosition * direction + orginPosition;
                Node validNode = EnemyManager.Instance.GetNodeByPositionWorld(worldPosition);
                if (validNode != null && validNode.Walkable) return validNode;
            }
        }
        return null;
    }


    public override void Stop()
    {
        base.Stop();
        StopMove();
    }

    public void StopMove()
    {
        Waypoints.Clear();
        indexWaypoints = 0;
        endPosition = Vector2.zero;
        targetPosition = Vector2.zero;
        hasPlayerNode = false;
        isFleeing = false;
        nextRepathTime = 0f;
    }

    private void OnDrawGizmosSelected()
    {
        if (Waypoints == null || Waypoints.Count == 0) return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, Waypoints[0]);
        for (int i = 0; i < Waypoints.Count - 1; i++)
        {
            Gizmos.DrawLine(Waypoints[i], Waypoints[i + 1]);
        }

        Gizmos.color = Color.yellow;
        for (int i = 0; i < Waypoints.Count; i++)
        {
            Gizmos.DrawSphere(Waypoints[i], 0.08f);
        }

        Gizmos.color = indexWaypoints < Waypoints.Count ? Color.green : Color.red;
        Gizmos.DrawWireSphere(targetPosition, 0.15f);

        if (targetPosition != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(targetPosition, 0.12f);
        }
    }
}
