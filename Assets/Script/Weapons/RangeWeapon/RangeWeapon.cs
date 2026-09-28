using UnityEngine;
using VContainer;

public class RangeWeapon : Weapon
{
    [Header("Range")]
    [SerializeField] private Transform firePoint;
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
        base.CanAttack() && StatsRange != null && firePoint != null
        && _objecPoolService != null && Time.time >= nextFireTime;

    public override bool CanChain()
    {
        if (!CanAttack()) return false;
        return StatsRange.AutoFire || CurrentStageIndex != 0;
    }
    public override void Equid(IWeaponHolder weaponHolder)
    {
        base.Equid(weaponHolder);
        firePoint = weaponHolder.OwnerTransform;
    }

    public override void OnAttackEnter(IWeaponHolder user)
    {
        base.OnAttackEnter(user);
        firePoint.right = aim.AimDirection.normalized;
    }

    public override void OnActivate(float finalDamage)
    {
        var stage = CurrentRangeStage;
        if (stage == null || stage.ProjectilePrefab == null) return;

        Vector2 forward = firePoint.right;
        float baseAngle = Mathf.Atan2(forward.y, forward.x) * Mathf.Rad2Deg;
        float step = stage.ProjectileCount > 1
            ? stage.SpreadAngle / (stage.ProjectileCount - 1)
            : 0f;
        float startAngle = baseAngle - stage.SpreadAngle * 0.5f;

        for (int i = 0; i < stage.ProjectileCount; i++)
        {
            float angle = stage.ProjectileCount > 1 ? startAngle + step * i : baseAngle;
            _objecPoolService.Spawn(stage.ProjectilePrefab, firePoint.position, Quaternion.AngleAxis(angle, Vector3.forward));
        }

        nextFireTime = Time.time + stage.RecoveryTime;
    }

    private void OnDrawGizmosSelected()
    {
        if (firePoint != null && currentStage != null)
        {
            Gizmos.DrawLine(firePoint.position,
                firePoint.position + firePoint.right * currentStage.attackRange);
        }
    }
}
