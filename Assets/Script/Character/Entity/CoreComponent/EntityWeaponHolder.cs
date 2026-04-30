using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityWeaponHolder : EntityCoreComponent
{
    [SerializeField] private EntityWeapon weapon;
    [SerializeField] private GameObject weaponGO;

    public EntityWeapon Weapon { get => weapon;}

    protected override void Awake()
    {
        base.Awake();
        weaponGO = Instantiate(entityCore.Entity.Data.WeaponSO.Weapon,this.transform.position,Quaternion.identity,this.transform);
        weapon = weaponGO.GetComponent<EntityWeapon>();
    }
}
