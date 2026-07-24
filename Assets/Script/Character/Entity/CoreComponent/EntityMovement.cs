using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;
public class EntityMovement : EntityCoreComponent<EntityCore>
{
    [SerializeField] protected Rigidbody2D rb;
    [SerializeField] private Vector2 target;
    [SerializeField] protected List<Vector2> Waypoints;
    [SerializeField] protected Vector2 targetPosition;
    [SerializeField] protected int indexWaypoints = 0;
    [SerializeField] protected float speed;
    [SerializeField] protected EntityInput entityInput;
    [SerializeField] protected int maxRadiusSpawnPoint;

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponentInParent<Rigidbody2D>();
    }

    public void MoveToTarget()
    {
        float distance = Vector2.Distance(transform.position, targetPosition);

        if (distance <= 0.3f)
        {
            indexWaypoints++;
            SetPointToForward(Waypoints[indexWaypoints]);
        }
        if (targetPosition == Vector2.zero) return;

        rb.MovePosition(rb.position + (targetPosition - (Vector2)transform.position ) 
        * speed * Time.fixedDeltaTime);
    }

    private void SetPointToForward(Vector2 targetPosition)
    {
        this.targetPosition = targetPosition;
    }

    private void GetPath(Path path)
    {
        Waypoints.Clear();
        Waypoints.AddRange(path.Waypoints);
        indexWaypoints = 0;
        SetPointToForward(Waypoints[indexWaypoints]);
    }

    public void SendResquestPath()
    {
        if (entityInput.TargetTransform.position)
        {
            this.target = entityInput.TargetTransform.position;
        }
        else
        {
            this.target = GetRandomNodePositionWorld();
            entityInput.SetTarget(target);
        }
        PathRequest request = new PathRequest(transform.position, target, GetPath);
        EnemyManager.Instance.RequestPath(request);
    }

    public Vector2 GetRandomNodePositionWorld()
    {
        Vector2 SpawnPoint = entityInput.SpawnPoint;
        Vector2Int randomAddRangePosition = new Vector2Int(Random.Range(0, maxRadiusSpawnPoint), Random.Range(0, maxRadiusSpawnPoint));
        Node validNode = new Node();
        var allDirection = (Vector2[])GameConstants.Direction.Vector.ALL.Clone();
        Utility.RandomShuffle(allDirection);
        foreach (var direction in allDirection)
        {
            validNode = EnemyManager.Instance.GetNodeByPositionWorld(
                ((Vector2)randomAddRangePosition * direction) + SpawnPoint
            );
            if (validNode != null) break;
        }
        return validNode.WorldPosition;
    }

    public void StopMove()
    {
        Waypoints.Clear();
        indexWaypoints = 0;
        targetPosition = Vector2.zero;
    }
}
