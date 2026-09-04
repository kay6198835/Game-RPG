using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;
public class EntityMovement : EntityCoreComponent<EntityCore>
{
    [SerializeField] protected Rigidbody2D rb;
    [SerializeField] protected List<Vector2> Waypoints;
    [SerializeField] protected Vector2 targetPosition;
    [SerializeField] protected Vector2 endPosition;
    [SerializeField] protected int indexWaypoints;
    [SerializeField] protected float speed;
    [SerializeField] protected int maxRadiusSpawnPoint;
    [SerializeField] protected float distance;
    [SerializeField] protected Vector2 forwardDirection;
    [SerializeField] protected Vector2 _lastPlayerNodePosition;
    [SerializeField] protected Vector2 currentPlayerNodePosition;
    [SerializeField] protected PathfindingGrid grid;
    [SerializeField] protected Vector2[] allDirection;
    [SerializeField] protected EntityInput entityInput;
    [SerializeField] protected EntityFindTarget entityFindTarget;

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
    protected override void Awake()
    {
        base.Awake();
        rb = GetComponentInParent<Rigidbody2D>();
    }
    public void CheckMove()
    {
        if (entityInput.TargetTransform)
        {
            if (entityFindTarget.IsNearPlayer())
            {
                FleeTarget();
            }
            else
            {
                ChaseToTarget();
            }
        }
        else
        {
            ToRandomPosition();
        }
        MoveToNodeTarget();
    }

    public void SendResquestPath()
    {
        if (endPosition == Vector2.zero) return;
        PathRequest request = new PathRequest(transform.position, endPosition, GetPath);
        EnemyManager.Instance.RequestPath(request);
    }

    private void FleeTarget()
    {
        if (!entityFindTarget.IsNearPlayer()) return;
        var fleePosition = Core.Entity.transform.position +
        ((Core.Entity.transform.position - entityInput.TargetTransform.position).normalized * 3);
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

    }
    private void ChaseToTarget()
    {
        if (CheckPlayerPosition())
        {
            endPosition = entityInput.TargetTransform.position;
            SendResquestPath();
        }
    }

    private bool CheckNearPostion(Vector2 positionCheck)
    {
        float distance = Vector2.Distance(transform.position, positionCheck);
        return distance < GameConstants.SettingStats.PADDING_NODE_VALUE ? true : false;
    }

    private void MoveToNodeTarget()
    {
        if (Waypoints.Count == 0)
        {
            SendResquestPath();
            return;
        }
        if (CheckNearPostion(targetPosition)) indexWaypoints++;

        if (indexWaypoints > Waypoints.Count - 1)
        {
            SendResquestPath();
            return;
        }
        SetPointToForward(Waypoints[indexWaypoints]);
    }

    private bool CheckPlayerPosition()
    {
        SearchNode node = grid.GetNodeFromWorld(entityInput.TargetTransform.position);
        if (node == null) return false;
        currentPlayerNodePosition = node.TileMapPosition;
        //player changed grid cell → need new path
        return Vector2.Distance(entityInput.TargetTransform.position, currentPlayerNodePosition) > 0.1f;
    }

    public void MoveForwardToTarget()
    {
        if (targetPosition == Vector2.zero) return;
        forwardDirection = (targetPosition - (Vector2)Core.Entity.transform.position).normalized;
        rb.MovePosition(rb.position + forwardDirection * speed * Time.fixedDeltaTime);
        entityInput.SetTarget(targetPosition);
    }

    private void SetPointToForward(Vector2 targetPosition)
    {
        this.targetPosition = targetPosition;
    }

    private void GetPath(Path path)
    {
        if (!path.Success || path.Waypoints.Count == 0) return;
        Waypoints.Clear();
        Waypoints.AddRange(path.Waypoints);
        indexWaypoints = path.Waypoints.Count > 1 ? 1 : 0;
        if (Waypoints.Count != 0) SetPointToForward(Waypoints[indexWaypoints]);
    }


    private const int MaxSetNodeAttempts = 10;

    private void ToRandomPosition()
    {
        if (CheckNearPostion(endPosition) || endPosition == Vector2.zero)
        {
            Vector2 spawnPoint = entityInput.SpawnPoint;
            Node node = SetNodeRandom(spawnPoint);
            endPosition = node != null ? node.WorldPosition : spawnPoint;

        }
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


    public void StopMove()
    {
        Waypoints.Clear();
        indexWaypoints = 0;
        endPosition = Vector2.zero;
        targetPosition = Vector2.zero;
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
