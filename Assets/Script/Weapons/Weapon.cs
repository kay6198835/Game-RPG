using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : CoreCompoment
{
    [SerializeField] protected WeaponDataSO statsSO;
    [SerializeField] protected float lastClickTime;
    [SerializeField] protected float deplayTime;
    [SerializeField] protected float durationNextAttack;
    [SerializeField] protected bool canAttack;


    public abstract void Attack();
    public abstract bool CheckCanAttack(NewPlayer player);
    protected void Equid(Collider2D collision, WeaponDataSO weaponData)
    {
        WeaponsController WPcontroller = collision.GetComponent<WeaponsController>();
        //Debug.Log("equip able");
        if (WPcontroller != null)
        {
            if (WPcontroller.slot < WPcontroller.maxSlot)
            {
                WPcontroller.equipped(weaponData);
                Destroy(gameObject);
            }
            else if (WPcontroller.slot >= WPcontroller.maxSlot)
            {
                WPcontroller.Drop();
                WPcontroller.equipped(weaponData);
                Destroy(gameObject);
            }
        }
    }
}
