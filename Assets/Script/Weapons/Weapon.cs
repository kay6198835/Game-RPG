using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : CoreCompoment
{
    [Header("Abtract Weapon")]
    [SerializeField] protected WeaponDataSO stats;
    [SerializeField] protected AbilitySO currentAbilitySO;
    protected float lastClickTime;
    protected float deplayTime;
    protected float durationNextAttack;
    protected bool canAttack;
    public AbilitySO CurrentAbilitySO { get => currentAbilitySO; }

    protected override void Awake()
    {
        base.Awake();
    }
    public abstract void Attack();
    public virtual bool CheckCanAttack(NewPlayer player)
    {
        lastClickTime = player.AttackState.StartAttackTime;

        if (lastClickTime + deplayTime > Time.time)
        {
            canAttack = false;
        }
        else
        {
            canAttack = true;
        }
        return canAttack;
    }
    public abstract AbilitySO SetAbility();
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
