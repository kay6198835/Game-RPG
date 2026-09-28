using UnityEngine;

[CreateAssetMenu(menuName = "WeaponData/AttackStage/Range")]
public class RangeAttackSO : AttackSO
{
    [field: SerializeField] public GameObject ProjectilePrefab { get; private set; }
    [field: SerializeField, Range(1, 20)] public int ProjectileCount { get; private set; } = 1;
    [field: SerializeField, Range(0f, 90f)] public float SpreadAngle { get; private set; } = 0f;
    [field: SerializeField, Range(0.02f, 5f)] public float RecoveryTime { get; private set; } = 0.3f;
    [field: SerializeField] public ProjectileConfig ProjectileConfig { get; private set; }
}
