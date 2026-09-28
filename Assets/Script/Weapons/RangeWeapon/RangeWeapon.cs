using UnityEngine;
using VContainer;

public class RangeWeapon : Weapon, IProjectilePayload
{
    [Header("Range")]
    [SerializeField] private Vector2 firePoint;
    [SerializeField] private float spawnOffset = 0.5f;
    private IObjecPoolService _objecPoolService;

    private float nextFireTime;

    private RangeWeaponStats StatsRange => stats as RangeWeaponStats;
    private RangeAttackSO CurrentRangeStage => currentStage as RangeAttackSO;

    [Inject]
    public void Construct(IObjecPoolService objecPoolService)
    {
        _objecPoolService = objecPoolService;
    }

    public override bool CanAttack() =>
        base.CanAttack() && StatsRange != null
        && _objecPoolService != null;

    public override bool CanChain()
    {
        if (!CanAttack()) return false;
        return StatsRange.AutoFire || CurrentStageIndex != 0;
    }

    public override void Equid(IWeaponHolder weaponHolder)
    {
        base.Equid(weaponHolder);
    }

    public override void OnAttackEnter(IWeaponHolder user)
    {
        base.OnAttackEnter(user);
        firePoint = (Vector2)holder.OwnerTransform.position + aim.AimDirection.normalized * spawnOffset;
    }

    public override void OnActivate(float finalDamage)
    {
        var stage = CurrentRangeStage;
        if (stage == null || stage.ProjectilePrefab == null) return;

        ProjectileConfig config = stage.ProjectileConfig;

        Vector2 forward = aim.AimDirection.normalized;
        float baseAngle = Mathf.Atan2(forward.y, forward.x) * Mathf.Rad2Deg;
        float step = stage.ProjectileCount > 1
            ? stage.SpreadAngle / (stage.ProjectileCount - 1)
            : 0f;
        float startAngle = baseAngle - stage.SpreadAngle * 0.5f;

        for (int i = 0; i < stage.ProjectileCount; i++)
        {
            float angle = stage.ProjectileCount > 1 ? startAngle + step * i : baseAngle;
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            GameObject go = _objecPoolService.Spawn(stage.ProjectilePrefab, firePoint, rotation);

            if (go != null && go.TryGetComponent(out ProjectileBody body))
                body.Launch(rotation * Vector3.right, config, this);
        }

        nextFireTime = Time.time + stage.RecoveryTime;
    }

    // Reads only power and target — never weapon state — so a shot in flight keeps the damage
    // it was fired with even after a stage change or an unequip.
    public void OnHit(Collider2D target, Vector2 hitPos)
    {
        if (target.TryGetComponent(out INegativeReceiver receiver))
            receiver.TakeDamage(currentStage.attackDamage, hitPos);
    }

    private void OnDrawGizmosSelected()
    {
        if (firePoint != null && currentStage != null)
        {
            Gizmos.DrawLine(firePoint,
                firePoint + firePoint * currentStage.attackRange);
        }
    }
}