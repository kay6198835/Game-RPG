using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeWeapon : Weapon
{
    [SerializeField] private RangeWeaponStats statsMelee;

    public RangeWeaponStats StatsMelee { get => statsMelee;}

    public override void Attack()
    {

    }

    public override bool CheckCanAttack()
    {
        return canAttack;
    }

    public override void SetAbility()
    {

    }
}
