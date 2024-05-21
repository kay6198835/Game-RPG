using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHolder : CoreCompoment
{
    [SerializeField] private Weapon weapon;
    public Weapon Weapon
    {
        get { return weapon; }
    }

    protected override void Awake()
    {
        base.Awake();
    }

    public void FindWeapon()
    {

    }

    public void Equid(Weapon weapon)
    {
        this.weapon = weapon;
        this.weapon.SetWeaponHolder(this);
    }
    public void UnEquid()
    {
        weapon.SetWeaponHolder(this);
        weapon = null;
    }
}
