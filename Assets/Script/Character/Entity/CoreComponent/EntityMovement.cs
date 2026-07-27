using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;
public class EntityMovement : EntityCoreComponent<EntityCore>
{
    [SerializeField] protected Rigidbody2D rb;
    [SerializeField] private Vector2 playerPosition;
    [SerializeField] protected List<Vector2> Waypoints;
    [SerializeField] protected Vector2 targetPosition;
    [SerializeField] protected int indexWaypoints;
    [SerializeField] protected float speed;
    [SerializeField] protected EntityInput entityInput;
    [SerializeField] protected int maxRadiusSpawnPoint;
    [SerializeField] protected GameObject enPoint;
    [SerializeField] protected float distance;
    [SerializeField] protected bool isSendRequest;


    protected override void Start()
    {
        base.Start();
        Core.GetCoreComponent(out entityInput);
        if (maxRadiusSpawnPoint == 0) maxRadiusSpawnPoint = 3;
        indexWaypoints = 0;
    }
    protected override void Awake()
    {
        base.Awake();
        rb = GetComponentInParent<Rigidbody2D>();
    }

    public void MoveToTarget()
    {
        if (isSendRequest) return;
        distance = Vector2.Distance(transform.position, targetPosition);
        if (distance <= 0.2f)
        {
            indexWaypoints++;
            if (indexWaypoints > Waypoints.Count - 1 || Waypoints.Count == 0)
            {
                SendResquestPath();
                isSendRequest = true;
            }
        }

        if (targetPosition == Vector2.zero || Waypoints.Count == 0 || indexWaypoints > Waypoints.Count - 1) return;
        SetPointToForward(Waypoints[indexWaypoints]);
        rb.MovePosition(rb.position + (targetPosition - (Vector2)transform.position).normalized
        * speed * Time.fixedDeltaTime);
        this.enPoint.transform.position = playerPosition;
        entityInput.SetTarget(targetPosition);
    }

    private void SetPointToForward(Vector2 targetPosition)
    {
        this.targetPosition = targetPosition;
        // + Vector2.one * 0.5f;
    }

    private void GetPath(Path path)
    {
        Waypoints.Clear();
        Waypoints.AddRange(path.Waypoints);
        indexWaypoints = 0;
        isSendRequest = false;
        if (path.Waypoints.Count > 0) SetPointToForward(Waypoints[indexWaypoints]);

    }

    public void SendResquestPath()
    {
        if (entityInput.TargetTransform != null)
        {
            this.playerPosition = entityInput.TargetTransform.position;
        }
        else
        {
            this.playerPosition = GetRandomNodePositionWorld();
        }
        PathRequest request = new PathRequest(transform.position, playerPosition, GetPath);
        EnemyManager.Instance.RequestPath(request);
        Debug.Log("SendResquestPath");
    }

    private const int MaxSetNodeAttempts = 10;

    public Vector2 GetRandomNodePositionWorld()
    {
        Vector2 SpawnPoint = entityInput.SpawnPoint;
        Node node = SetNode(SpawnPoint);
        return node != null ? node.WorldPosition : SpawnPoint;
    }

    private Node SetNode(Vector2 SpawnPoint)
    {
        for (int attempt = 0; attempt < MaxSetNodeAttempts; attempt++)
        {
            Vector2Int randomAddRangePosition = new Vector2Int(Random.Range(0, maxRadiusSpawnPoint), Random.Range(0, maxRadiusSpawnPoint));
            var allDirection = (Vector2[])GameConstants.Direction.Vector.ALL.Clone();
            Utility.RandomShuffle(allDirection);
            foreach (var direction in allDirection)
            {
                var worldPosition = (Vector2)randomAddRangePosition * direction + SpawnPoint;
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
    }
}
