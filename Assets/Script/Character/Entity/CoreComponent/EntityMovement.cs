using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class EntityMovement : EntityCoreComponent
{
    [SerializeField] protected Rigidbody2D rb;
    [SerializeField] private Transform target;
    [SerializeField] protected List<Vector2> Waypoints;
    [SerializeField] protected Vector2 targetPosition;
    [SerializeField] protected int indexWaypoints;
    [SerializeField] protected float speed;

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
        transform.DOMove(pointB.position, speed);
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
        if (target == null) return;
        PathRequest request = new PathRequest(transform.position, target.transform.position, GetPath);
        EnemyManager.Instance.RequestPath(request);
    }

    public void StopMove()
    {
        Waypoints.Clear();
        indexWaypoints = 0;
        targetPosition = null;
    }
}
