using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.WSA;

public abstract class EntityWeaponComponent : EntityCoreComponent
{
    protected override void Awake()
    {
        base.Awake();
    }
    [SerializeField] protected WeaponDataSO stats;
    [SerializeField] protected AbilitySO currentAbilitySO;
    [SerializeField] protected float lastClickTime;
    [SerializeField] protected float deplayTime;
    [SerializeField] protected float durationNextAttack;
    [SerializeField] protected bool canAttack;
    public AbilitySO CurrentAbilitySO { get => currentAbilitySO; }
    protected WeaponDataSO Stats { get => stats;}
    protected float LastClickTime { get => lastClickTime;}
    protected float DeplayTime { get => deplayTime; }
    protected float DurationNextAttack { get => durationNextAttack;}
    protected bool CanAttack { get => canAttack;}

    public abstract void Attack();
    public virtual bool CheckCanAttack(Entity entity, float lastClickTime)
    {
        this.lastClickTime = lastClickTime;

        if (this.lastClickTime + deplayTime > Time.time)
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
    //protected void Equid(Collider2D collision, WeaponDataSO weaponData)
    //{
    //    WeaponsController WPcontroller = collision.GetComponent<WeaponsController>();
    //    if (WPcontroller != null)
    //    {
    //        if (WPcontroller.slot < WPcontroller.maxSlot)
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
