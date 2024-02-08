using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Shooting))]
public class RangeWeapon : Weapon
{
    [SerializeField] private RangeWeaponDataSO stats ;
    [SerializeField] private Shooting rangeAttack ;

    private void Start()
    {
        statsSO = stats;
        attack = rangeAttack;
    }


    protected void OnTriggerEnter2D(Collider2D collision)
    {
        base.Equid(collision, stats);
    }

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    WeaponsController WPcontroller = collision.GetComponent<WeaponsController>();
    //    //Debug.Log("equip able");
    //    if(WPcontroller != null)
    //    {
    //        if(WPcontroller.slot < WPcontroller.maxSlot)
    //        {
    //            WPcontroller.equipped(weaponData);
    //            Destroy(gameObject);
    //        }
    //        else if (WPcontroller.slot >= WPcontroller.maxSlot)
    //        {
    //            WPcontroller.Drop();
    //            WPcontroller.equipped(weaponData);
    //            Destroy(gameObject);
    //        }
    //    }
    //}
}
