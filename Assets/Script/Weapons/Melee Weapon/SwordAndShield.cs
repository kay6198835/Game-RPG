using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//[RequireComponent(typeof(BoxCollider2D))]


public class SwordAndShield : WeaponMelee
{
    private void Awake()
    {
        LoadWeaponMelee();
    }
    private void Start()
    {
        SettingWeaponMelee();
    }
    protected override void LoadWeaponMelee()
    {
        base.LoadWeaponMelee();
    }
    public override void SettingWeaponMelee()
    {
        base.SettingWeaponMelee();
    }
    
}
