using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHolder : Interact
{
    public Weapon Weapon;

    protected override void Awake()
    {
        base.Awake();
        interactableMask = LayerMask.GetMask("Weapon");
    }
    public void Equid_UnEquid(Weapon weapon)
    {
        if (this.Weapon == null)
        {
            this.Weapon = weapon;
        }
        else
        {
            // this.Weapon = null;
        }
    }
    public override void Intertion()
    {
        if (Weapon != null)
        {
            Weapon.UnEquid(this);

        }
        if (Weapon == null)
        {
            base.Intertion();
        }
    }

    public void Attack()
    {
        if (Weapon == null) return;
        Weapon.SetAnimation(Core.Player);
    }


}
