using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "WeaponData/MeleeWeaponData")]
public class MeleeWeaponStats : WeaponStats
{
    [SerializeField] private string nameWeapon;
    [SerializeField] private ulong idWeapon = 0;
    [SerializeField] private Vector2 shieldEra;
    [SerializeField] private int blockDamage;
    [field: SerializeField] public List<AttackSO> AttackState { get; protected set; }
    void OnValidate()
    {
        this.Type = WeaponType.MeleeWP;
    }
    #region Properties 
    public string NameWeapon { get => nameWeapon; }
    public ulong IdWeapon { get => idWeapon; }
    public Vector2 ShieldEra { get => shieldEra; }
    public int BlockDamage { get => blockDamage; }
    #endregion
    private void Awake()
    {
        Type = WeaponType.MeleeWP;
    }
}
