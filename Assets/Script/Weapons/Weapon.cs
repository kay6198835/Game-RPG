using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] protected WeaponDataSO statsSO;
    [SerializeField] protected WeaponAttack attack;

    public virtual void Attack()
    {
        attack.Attack(this);
    }
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
