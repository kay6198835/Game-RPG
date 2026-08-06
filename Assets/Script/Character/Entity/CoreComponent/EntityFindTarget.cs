using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityFindTarget : EntityCoreComponent<EntityCore>
{
    [SerializeField] private Transform target;
    [SerializeField] private float range;
    [SerializeField] private LayerMask player, obstracles;
    //[SerializeField] private bool isFindTarget;
    [SerializeField] private Collider2D collider;
    [SerializeField] private float distanceToPlayer;
    [SerializeField] private EntityInput entityInput;
    public Transform Target { get => target; }
    [Range(0, 20)]
    [SerializeField] private float minRange, maxRange;

    protected override void Awake()
    {
        base.Awake();

    }
    protected override void Start()
    {
        base.Start();
        Debug.Log(Core);
        // minRange, maxRange set by entity data
        Core.GetCoreComponent(out entityInput);
    }

    public float DistanceToPlayer()
    {
        TryGetDistanceToPlayer();
        return distanceToPlayer;
    }
    public bool HasTarget => entityInput != null && entityInput.TargetTransform != null;

    /// <summary>Distance to the current target; false when no target — distance is only valid on true.</summary>
    public bool TryGetDistanceToPlayer()
    {
        if (!HasTarget)
        {
            distanceToPlayer = 0f;
            return false;
        }
        distanceToPlayer = Vector2.Distance(Core.Entity.transform.Position2D(), entityInput.TargetTransform.Position2D());;
        return true;
    }

    public bool IsNearPlayer()
    {
        var isNearPlayer = TryGetDistanceToPlayer() && distanceToPlayer < minRange;
        return isNearPlayer;
    }

    public bool OutOfRange()
    {
        var isOutOfRange = !TryGetDistanceToPlayer() || distanceToPlayer > maxRange;
        return isOutOfRange;
    }

    public bool IsInRangeAttack()
    {
        var isInRangeAttack = TryGetDistanceToPlayer()
                             && distanceToPlayer >= minRange
                             && distanceToPlayer <= maxRange;
        return isInRangeAttack;
    }

    public Transform FindTargetMethod(float range)
    {
        this.range = range;
        collider = Physics2D.OverlapCircle(
            this.transform.position,
            range,
            player);
        if (collider != null)
        {
            target = collider.transform;
        }
        else
        {
            target = null;
        }
        return target;
    }

    public bool FindWall(Vector2 direction, float speed)
    {
        bool isFindWall;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, speed * 0.5f, obstracles);
        if (hit.collider != null)
        {
            isFindWall = true;
        }
        else
        {
            isFindWall = false;
        }
        return isFindWall;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
