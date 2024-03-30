using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHolder : CoreCompoment
{
    [SerializeField] private Weapon weapon;
    [SerializeField] private GameObject weaponGO;
    [SerializeField] private bool isCanPickWeapon;
    [SerializeField] private float range;
    [SerializeField] private Vector3 test;
    public Weapon Weapon
    {
        get { return weapon; }
    }
    public bool IsCanPickWeapon { get => isCanPickWeapon; }

    protected override void Awake()
    {
        base.Awake();
    }
    private void Start()
    {
         range = 1;
    }
    public bool FindWeapon()
    {
        Collider2D collider2D = Physics2D.OverlapCircle(transform.position,range,core.Player.Data.WeaponLayerMask);

        if (collider2D != null)
        {
            weaponGO = collider2D.gameObject;
            isCanPickWeapon = true;
        }
        else
        {
            isCanPickWeapon= false;
            weaponGO = null;
        }
        return isCanPickWeapon;
    }
    public void EquidWeapon()
    {
        if (weapon == weaponGO.GetComponent<Weapon>())
        {
            return;
        }
        weapon = weaponGO.GetComponent<Weapon>();
        weapon.transform.SetParent(transform);
        weapon.transform.position = transform.parent.position;
        weapon.SetWeaponHolder(this);
    }
    public void DropWeapon()
    {
        weapon = null;
        weapon.transform.SetParent(null);
        weapon.SetWeaponHolder(this);
    }
}
