using UnityEngine;

[CreateAssetMenu(menuName = "WeaponData/MeleeWeaponData")]
public class MeleeWeaponStats : WeaponStats
{
    [Header("Melee")]
    [SerializeField] private string nameWeapon;
    [SerializeField] private ulong idWeapon = 0;
    [SerializeField] private Vector2 shieldEra;
    [SerializeField, Range(0, 500)] private int blockDamage;

    public override WeaponType Type => WeaponType.MeleeWP;

    #region Properties
    public string NameWeapon { get => nameWeapon; }
    public ulong IdWeapon { get => idWeapon; }
    public Vector2 ShieldEra { get => shieldEra; }
    public int BlockDamage { get => blockDamage; }
    #endregion
}
