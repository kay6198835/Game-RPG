using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[CreateAssetMenu(menuName = "WeaponData/MeleeWeaponData")]
public class WeaponMeleeStats : WeaponDataSO
{
    [SerializeField] private string nameWeapon;
    [SerializeField] private ulong idWeapon = 0;
    [SerializeField] private List<AttackSO> attackState;
    [SerializeField] private Vector2 shieldEra;
    [SerializeField] private int blockDamage;
    #region
    public string NameWeapon { get => nameWeapon;}
    public ulong IdWeapon { get => idWeapon; }
    public List<AttackSO> AttackState { get => attackState; }
    public Vector2 ShieldEra { get => shieldEra; }
    public int BlockDamage { get => blockDamage; }
    #endregion
    private void Awake()
    {
        type = WeaponType.MeleeWP;
        enemyLayers = LayerMask.GetMask("Enemy");
    }
}
