using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Shooting))]
public class RangeWeapon : Weapon
{
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
