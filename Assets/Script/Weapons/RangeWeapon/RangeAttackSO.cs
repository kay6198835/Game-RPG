using UnityEngine;

[CreateAssetMenu(menuName = "WeaponData/AttackStage/Range")]
public class RangeAttackSO : AttackSO
{
    [Header("Projectile")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private BulletDataSO bulletData;

    [SerializeField, Range(1, 20)] private int projectileCount = 1;
    [SerializeField, Range(0f, 90f)] private float spreadAngle = 0f;
    [SerializeField, Range(0.02f, 5f)] private float recoveryTime = 0.3f;

    public GameObject BulletPrefab { get => bulletPrefab; }
    public BulletDataSO BulletData { get => bulletData; }
    public int ProjectileCount { get => projectileCount; }
    public float SpreadAngle { get => spreadAngle; }
    public float RecoveryTime { get => recoveryTime; }
}
