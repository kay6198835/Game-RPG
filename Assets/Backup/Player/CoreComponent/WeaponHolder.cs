using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHolder : Interact
{
    [SerializeField] private Weapon weapon;

    public Weapon Weapon
    {
        get { return weapon; }
    }

    protected override void Awake()
    {
        base.Awake();
        interactableMask = LayerMask.GetMask("Weapon"); 
    }
    public void Equid_UnEquid(Weapon weapon)
    {
        if(this.weapon == null)
        {
            this.weapon = weapon;
        }else
        {
            this.weapon = null;
        }
    }
    public override void Intertion()
    {
        if(weapon != null)
        {
            weapon.UnEquid();

        }
        if(weapon == null)
        {
            base.Intertion();
        }
    }
}
