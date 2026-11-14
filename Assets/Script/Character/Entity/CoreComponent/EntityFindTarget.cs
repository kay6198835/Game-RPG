using UnityEngine;

public class EntityFindTarget : EntityCoreComponent<EntityCore>
{
    [Header("Target")]
    [SerializeField] private Transform target;

    // NEED CHECK: was a serialized field, 0 on EnemyPrefab and assigned nowhere, so IsInFOV() never saw a
    // target (inbox NOTE-20261104-6). Detection range is EntityData.rangeCheckFieldOfView, per role
    // (owner decision 2026-11-04; briefly MaxRange the same day). Must stay >= MaxRangeAttack.
    //[SerializeField] private float range;
    private float range => Core.Entity.Data.RangeView(statService);
    private float fieldOfView => Core.Entity.Data.AngleView(statService);
    // Inner margin of the attack band used to stop moving; Idle only leaves when the band itself is left.
    [SerializeField, Range(0f, 2f)] private float rangeHysteresis = 0.25f;
    [SerializeField, Range(1f, 2f)] private float loseTargetRangeMultiplier = 1.5f;

    [SerializeField] private LayerMask player;
    [SerializeField] private LayerMask obstracles;

    [SerializeField] private Collider2D collider;
    [SerializeField] private float distanceToPlayer = float.MaxValue;

    // NEED CHECK: removed — one value on the shared EnemyPrefab (0.5 / 1) for every role. Now per-role stats.
    //[Header("Attack Range")]
    //[Range(0, 20)]
    //[SerializeField] private float minRange;
    //[Range(0, 20)]
    //[SerializeField] private float maxRange;

    [SerializeField] private EntityInput entityInput;
    private IStatService statService;

    /// <summary>StatType.MinRangeAttack: the entity backs off when the target is closer than this.</summary>
    public float MinRange => statService != null ? statService.GetStatValue(StatType.MinRangeAttack) : 0f;
    /// <summary>StatType.MaxRangeAttack: the entity attacks when the target is at most this far.</summary>
    public float MaxRange => statService != null ? statService.GetStatValue(StatType.MaxRangeAttack) : 0f;

    /// <summary>Distance the entity tries to keep from a locked target: the middle of the attack band.</summary>
    public float PreferredDistance => (MinRange + MaxRange) * 0.5f;

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
        // NEED CHECK: new — range stats (MinRange / MaxRange).
        Core.TryGetCapability(out statService);
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

    /// <summary>Refreshes and returns the distance to the real player position (lock or not).</summary>
    public float DistanceToPlayer()
    {
        if (entityInput == null || !entityInput.HasPlayer)
            return distanceToPlayer = float.MaxValue;

        return distanceToPlayer = Vector2.Distance(
            Core.Entity.transform.Position2D(),
            entityInput.PlayerPosition);
    }

    #endregion

    #region Attack Range

    /// <summary>Locked target closer than MinRange.</summary>
    public bool IsNearPlayer()
    {
        return HasTarget && distanceToPlayer < MinRange;
    }

    /// <summary>Locked target farther than MaxRange.</summary>
    public bool OutOfRange()
    {
        return HasTarget && distanceToPlayer > MaxRange;
    }

    /// <summary>Locked target inside [MinRange, MaxRange].</summary>
    public bool IsInRangeAttack()
    {
        return HasTarget
            && distanceToPlayer >= MinRange
            && distanceToPlayer <= MaxRange;
    }

    /// <summary>Locked target inside the attack band shrunk by the hysteresis margin: where moving stops.</summary>
    public bool IsInComfortRange()
    {
        if (!HasTarget) return false;
        float margin = Mathf.Min(rangeHysteresis, (MaxRange - MinRange) * 0.25f);
        return distanceToPlayer >= MinRange + margin
            && distanceToPlayer <= MaxRange - margin;
    }

    #endregion

    #region FOV

    /// <summary>
    /// Checks whether a position is inside the entity's detection range and view cone. No side effects.
    /// </summary>
    private bool IsInFOV(Vector2 targetPosition)
    {
        Vector2 origin = transform.position;
        Vector2 direction = targetPosition - origin;

        if (direction.sqrMagnitude > range * range)
            return false;

        if (direction.sqrMagnitude <= Mathf.Epsilon)
            return true;

        Vector2 forward = entityInput.DirectionLookVector;
        // No facing yet (spawned, or standing on its own look point): treat as all-round vision.
        if (forward.sqrMagnitude <= Mathf.Epsilon)
            return true;

        float minDot = Mathf.Cos(fieldOfView * 0.5f * Mathf.Deg2Rad);
        return Vector2.Dot(forward.normalized, direction.normalized) >= minDot;
    }

    /// <summary>
    /// Whether the player can be seen now: range, view cone and no obstacle in between.
    /// </summary>
    public bool CanDetectTarget()
    {
        if (entityInput == null || !entityInput.HasPlayer)
            return false;

        Vector2 targetPosition = entityInput.PlayerPosition;

        if (!IsInFOV(targetPosition))
            return false;

        return !FindWall(targetPosition - (Vector2)transform.position);
    }

    #endregion

    #region Find Target

    /// <summary>Locks the player when seen; releases the lock when the player is far beyond detection range.</summary>
    public void FindTargetMethod()
    {
        if (entityInput == null) return;

        if (HasTarget)
        {
            if (!entityInput.HasPlayer || distanceToPlayer > range * loseTargetRangeMultiplier)
                entityInput.SetLockTarget(false);
            return;
        }

        if (CanDetectTarget())
            entityInput.SetLockTarget(true);
    }

    #endregion

    #region Obstacle

    private bool FindWall(Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            direction.normalized,
            direction.magnitude,
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

        Vector2 forward = entityInput != null && entityInput.DirectionLookVector.sqrMagnitude > Mathf.Epsilon
            ? entityInput.DirectionLookVector.normalized
            : (Vector2)transform.right;

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
        float rangeCapNoti = range;
        if (DistanceToPlayer() < rangeCapNoti)
        {
            entityInput.SetLockTarget(true);
        }
    }

    private void CaptureNotifications(object obj = null)
    {
        if (entityInput == null || HasTarget) return;
        CheckRangeCapNoti();
    }
    #endregion
}
