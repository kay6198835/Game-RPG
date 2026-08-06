using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "WeaponData/RangeWeaponData")]

public class RangeWeaponStats : WeaponStats
{
    [Header("RW Stats")]
    public string weaponName;

    public float firerate;
    public float timeBtwShots;
    public float StartTimeBtwShots;
    void OnValidate()
    {
        this.Type = WeaponType.RangeWP;
    }
}
