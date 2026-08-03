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
    [SerializeField] protected int indexWaypoints;
    [SerializeField] protected float speed;
    [SerializeField] protected int maxRadiusSpawnPoint;
    [SerializeField] protected float distance;
    [SerializeField] protected Vector2 forwardDirection;
    [SerializeField] protected Vector2 _lastPlayerNodePosition;
    [SerializeField] protected Vector2 currentPlayerNode;
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
        PathRequest request = new PathRequest(transform.position, targetPosition, GetPath);
        EnemyManager.Instance.RequestPath(request);
    }

    private void FleeTarget()
    {
        Vector2 fleePosition = (Vector2)(Core.Entity.transform.position * 2f - entityInput.TargetTransform.position);
        Node node = SetNodeRandom(fleePosition);
        targetPosition = node != null ? node.WorldPosition : fleePosition;
    }
    private void ChaseToTarget()
    {
        if (CheckPlayerPosition())
        {
            targetPosition = entityInput.TargetTransform.position;
            _lastPlayerNodePosition = currentPlayerNode;
            SendResquestPath();
            return;
        }
    }

    private void MoveToNodeTarget()
    {
        distance = Vector2.Distance(transform.position, targetPosition);
        if (distance < 0.7f)
        {
            indexWaypoints++;
        }
        if (indexWaypoints > Waypoints.Count - 1 || Waypoints.Count == 0)
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
        currentPlayerNode = node.TileMapPosition;
        //player changed grid cell → need new path
        return currentPlayerNode != _lastPlayerNodePosition;
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
        this.targetPosition = targetPosition + Vector2.one * 0.5f;
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
        Vector2 spawnPoint = entityInput.SpawnPoint;
        Node node = SetNodeRandom(spawnPoint);
        targetPosition = node != null ? node.WorldPosition : spawnPoint;
    }

    private Node SetNodeRandom(Vector2 orginPosition)
    {
        for (int attempt = 0; attempt < MaxSetNodeAttempts; attempt++)
        {
            Vector2Int randomAddRangePosition = new Vector2Int(Random.Range(0, maxRadiusSpawnPoint), Random.Range(0, maxRadiusSpawnPoint));
            Utility.RandomShuffle(allDirection);
            foreach (var direction in allDirection)
            {
                var worldPosition = (Vector2)randomAddRangePosition * direction + orginPosition;
                Node validNode = EnemyManager.Instance.GetNodeByPositionWorld(worldPosition);
                if (validNode != null) return validNode;
            }
        }
        return null;
    }


    public void StopMove()
    {
        Waypoints.Clear();
        indexWaypoints = 0;
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
