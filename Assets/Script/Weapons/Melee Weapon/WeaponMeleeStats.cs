using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "WeaponData/MeleeWeaponData")]
public class WeaponMeleeStats : WeaponDataSO
{
    [SerializeField] public string nameWeapon;
    [SerializeField] public ulong idWeapon = 0;
    [SerializeField] public List<AttackSO> attackState;

    [SerializeField] public Vector2 shieldEra;
    [SerializeField] public int blockDamage;

    [SerializeField] public LayerMask enemyLayers;
    [SerializeField] public AbilitySO auxiliaryAbility;
    [SerializeField] public AbilitySO specialAbility;

    private void Awake()
    {
        enemyLayers = LayerMask.GetMask("Enemy");
    }
}
