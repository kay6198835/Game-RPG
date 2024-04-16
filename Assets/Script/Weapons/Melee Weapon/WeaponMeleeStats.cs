using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[CreateAssetMenu(menuName = "WeaponData/MeleeWeaponData")]
public class WeaponMeleeStats : WeaponStats
{
    [SerializeField] private string nameWeapon;
    [SerializeField] private ulong idWeapon = 0;
    [SerializeField] private Vector2 shieldEra;
    [SerializeField] private int blockDamage;
    #region Properties 
    public string NameWeapon { get => nameWeapon;}
    public ulong IdWeapon { get => idWeapon; }
    public Vector2 ShieldEra { get => shieldEra; }
    public int BlockDamage { get => blockDamage; }
    #endregion
    private void Awake()
    {
        type = WeaponType.MeleeWP;
        layerMask = LayerMask.GetMask("Enemy");
    }
}
