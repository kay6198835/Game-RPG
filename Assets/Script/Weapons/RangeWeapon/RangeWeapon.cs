using UnityEngine;

public class RangeWeapon : Weapon
{
    [Header("Range")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private ObjectPoolManager poolManager;

    private float nextFireTime;

    private RangeWeaponStats StatsRange => stats as RangeWeaponStats;
    private RangeAttackSO CurrentRangeStage => currentStage as RangeAttackSO;

    public override bool CanAttack() =>
        base.CanAttack() && StatsRange != null && firePoint != null
        && poolManager != null && Time.time >= nextFireTime;

    public override bool CanChain()
    {
        if (!CanAttack()) return false;
        return StatsRange.AutoFire || CurrentStageIndex < stats.StageCount;
    }

    public override void OnAttackEnter(Player player)
    {
        base.OnAttackEnter(player);
        firePoint.right = aim.AimDirection.normalized;
    }

    public override void OnActivate()
    {
        var stage = CurrentRangeStage;
        if (stage == null || stage.BulletPrefab == null) return;

        Vector2 forward = firePoint.right;
        float baseAngle = Mathf.Atan2(forward.y, forward.x) * Mathf.Rad2Deg;
        float step = stage.ProjectileCount > 1
            ? stage.SpreadAngle / (stage.ProjectileCount - 1)
            : 0f;
        float startAngle = baseAngle - stage.SpreadAngle * 0.5f;

        for (int i = 0; i < stage.ProjectileCount; i++)
        {
            float angle = stage.ProjectileCount > 1 ? startAngle + step * i : baseAngle;
            poolManager.Spawn(firePoint.position, Quaternion.AngleAxis(angle, Vector3.forward), stage.BulletPrefab);
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
