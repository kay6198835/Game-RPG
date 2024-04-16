using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Shooting))]
public class RangeWeapon : Weapon
{
    [SerializeField] private WeaponRangeStats statsMelee;

    public WeaponRangeStats StatsMelee { get => statsMelee;}

    public override void Attack()
    {

    }

    public override bool CheckCanAttack(NewPlayer player)
    {
        return canAttack;
    }

    public override void SetAbility()
    {

    }
}
