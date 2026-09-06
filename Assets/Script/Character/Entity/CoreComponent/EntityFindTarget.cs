using UnityEngine;

public class EntityFindTarget : EntityCoreComponent<EntityCore>
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Detection")]
    [SerializeField] private float range;
    [SerializeField, Range(0f, 360f)] private float fieldOfView = 90f;

    [SerializeField] private LayerMask player;
    [SerializeField] private LayerMask obstracles;

    [SerializeField] private Collider2D collider;
    [SerializeField] private float distanceToPlayer;

    [Header("Attack Range")]
    [Range(0, 20)]
    [SerializeField] private float minRange;

    [Range(0, 20)]
    [SerializeField] private float maxRange;

    [SerializeField] private EntityInput entityInput;

    public Transform Target => target;

    public float FieldOfView => fieldOfView;
    public float Range => range;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();

        Core.GetCoreComponent(out entityInput);
    }

    protected void OnEnable()
    {
        EventManager.Resgister(EventID.ON_ENEMY_DEATH, CaptureNotifications);
    }

    protected void OnDisable()
    {
        EventManager.UnResgister(EventID.ON_ENEMY_DEATH, CaptureNotifications);
    }

    #region Target

    public bool HasTarget =>
        entityInput != null &&
        entityInput.IsLockTarget == true;

    public float DistanceToPlayer()
    {
        return distanceToPlayer = Vector2.Distance(
            Core.Entity.transform.Position2D(),
            entityInput.TargetPosition()); ;
    }

    #endregion

    #region Attack Range

    public bool IsNearPlayer()
    {
        var isNearPlayer = distanceToPlayer < minRange;
        return isNearPlayer;
    }

    public bool OutOfRange()
    {
        var isOutOfRange = distanceToPlayer > maxRange;
        return isOutOfRange;
    }

    public bool IsInRangeAttack()
    {
        var isInRangeAttack = distanceToPlayer >= minRange
                             && distanceToPlayer <= maxRange;
        return isInRangeAttack;
    }


    #endregion

    #region FOV

    /// <summary>
    /// Checks whether a position is inside the entity's FOV.
    /// </summary>
    private bool IsInFOV(Vector2 targetPosition)
    {
        Vector2 origin = transform.position;
        Vector2 direction = targetPosition - origin;

        // Target is outside detection range.
        if (direction.sqrMagnitude > range * range)
            return false;

        // Target is at the same position.
        if (direction.sqrMagnitude <= Mathf.Epsilon)
            return true;

        direction.Normalize();

        Vector2 forward = entityInput.DirectionLookVector;

        float minDot =
            Mathf.Cos(fieldOfView * 0.5f * Mathf.Deg2Rad);
        if (Vector2.Dot(forward, direction) >= minDot)
        {
            entityInput.SetLockTarget(true);
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Checks whether the current target is detectable.
    /// Includes range, FOV and obstacle check.
    /// </summary>
    public bool CanDetectTarget()
    {
        if (!HasTarget)
            return false;

        Vector2 targetPosition =
            entityInput.TargetPosition();

        if (!IsInFOV(targetPosition))
            return false;

        return !FindWall(
            targetPosition - (Vector2)transform.position,
            range);
    }

    #endregion

    #region Find Target

    public void FindTargetMethod()
    {
        if (HasTarget) return;

        var range = Core.Entity.Data.RangeCheckFieldOfView;
        if (distanceToPlayer >= range) return;
        if (IsInFOV(entityInput.TargetPosition())) return;
        if (CanDetectTarget()) 
        {
            entityInput.SetLockTarget(true);
        }
    }

    #endregion

    #region Obstacle

    private bool FindWall(Vector2 direction, float speed)
    {
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            direction.normalized,
            speed * 0.5f,
            obstracles);

        return hit.collider != null;
    }

    #endregion

    #region Debug

    private void OnDrawGizmos()
    {
        // Detection range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(
            transform.position,
            range);

        // FOV
        DrawFOVGizmo();
    }

    private void DrawFOVGizmo()
    {
        Vector3 origin = transform.position;

        Vector2 forward = transform.right;

        Vector2 leftDirection =
            Quaternion.Euler(
                0f,
                0f,
                fieldOfView * 0.5f) * forward;

        Vector2 rightDirection =
            Quaternion.Euler(
                0f,
                0f,
                -fieldOfView * 0.5f) * forward;

        Gizmos.color = Color.cyan;

        Gizmos.DrawLine(
            origin,
            origin + (Vector3)leftDirection * range);

        Gizmos.DrawLine(
            origin,
            origin + (Vector3)rightDirection * range);
    }

    #endregion

    #region Catch Enemy Death
    private void CheckRangeCapNoti()
    {
        float rangeCapNoti = Core.Entity.Data.RangeCheckFieldOfView;
        if (distanceToPlayer < rangeCapNoti)
        {
            entityInput.SetLockTarget(true);
        }
    }

    private void CaptureNotifications(object obj = null)
    {
        if (HasTarget) return;
        CheckRangeCapNoti();
    }
    #endregion
}