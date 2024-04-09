using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    [Header("Abtract Weapon")]
    [SerializeField] protected WeaponHolder holder;
    [SerializeField] protected AbilityHolder abilityHolder;
    [SerializeField] protected WeaponDataSO stats;
    [SerializeField] protected AbilitySO currentAbilitySO;
    protected float lastClickTime;
    protected float deplayTime;
    protected float durationNextAttack;
    protected bool canAttack;
    public AbilityHolder AbilityHolder { get => abilityHolder; }

    protected virtual void Awake()
    {
        holder = GetComponentInParent<WeaponHolder>();
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
    public virtual void SetAbility()
    {
        abilityHolder.SetAblityWeapon(currentAbilitySO);
    }
    public virtual void SetWeaponHolder(WeaponHolder weaponHolder)
    {
        if (holder == null)
        {
            holder = weaponHolder;
            abilityHolder = weaponHolder.Core.AbilityHolder;
        }
        else
        {
            holder = null;
            abilityHolder = null;
        }
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
