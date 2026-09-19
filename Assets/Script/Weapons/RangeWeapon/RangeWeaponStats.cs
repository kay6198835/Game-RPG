using UnityEngine;

[CreateAssetMenu(menuName = "WeaponData/RangeWeaponData")]
public class RangeWeaponStats : WeaponStats
{
    [Header("Range")]
    [SerializeField] private string nameWeapon;

    // Holding the trigger replays the stage list instead of stopping at the last stage.
    [SerializeField] private bool autoFire = true;

    public override WeaponType Type => WeaponType.RangeWP;

    public string NameWeapon { get => nameWeapon; }
    public bool AutoFire { get => autoFire; }
}
