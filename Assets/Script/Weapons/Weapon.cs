using UnityEngine;

public abstract class Weapon : InteractiveObjects
{
    [Header("Abtract Weapon")]
    [SerializeField] protected WeaponStats stats;
    [SerializeField] protected ActivateSkill currentAbilitySO;
    [SerializeField] protected Collider2D collider;
    protected float lastClickTime;
    protected float deplayTime;
    protected bool canAttack;
    protected AbilityHolder abilityHolder;
    protected WeaponHolder weaponHolder;
    protected override void Awake()
    {
        base.Awake();
        collider = GetComponent<Collider2D>();
    }
    public abstract void Attack();
    public virtual bool CheckCanAttack(Player player)
    {
        if (lastClickTime + deplayTime > Time.time)
        {
            canAttack = false;
        }
        else
        {
            canAttack = true;
            lastClickTime = Time.time;
        }
        return canAttack;
    }
    public virtual void SetAnimation(Player player)
    { }
    public virtual void ResetCombo()
    { }
    public virtual void SetAbility()
    {
        abilityHolder.SetAblityWeapon(currentAbilitySO);
    }
    public virtual void SetWeaponHolder(WeaponHolder weaponHolder)
    {
        // if (holder == null)
        // {
        //     collider.enabled = false;
        //     //holder = weaponHolder;
        //     //abilityHolder = holder.Core.AbilityHolder;
        //     transform.SetParent(holder.transform);
        //     transform.position = transform.parent.position;
        // }
        // else
        // {
        //     transform.position = transform.parent.position + Vector3.one * 1f;
        //     transform.SetParent(null);
        //     abilityHolder = null;
        //     //holder = null;
        //     collider.enabled = true;
        // }
    }
    public override bool Interact(Interact interactor)
    {
        Equid((WeaponHolder)interactor);
        return true;
    }
    public virtual void Equid(WeaponHolder weaponHolder)
    {
        collider.enabled = false;
        weaponHolder.Core.GetCoreComponent(out this.weaponHolder);
        weaponHolder.Core.GetCoreComponent(out this.abilityHolder);
        weaponHolder.Equid_UnEquid(this);
        transform.SetParent(weaponHolder.transform);
        transform.position = transform.parent.position;
    }
    public virtual void UnEquid()
    {
        transform.position = transform.parent.position + Vector3.one * 1f;
        transform.SetParent(null);
        collider.enabled = true;
        weaponHolder.Equid_UnEquid(this);
        abilityHolder = null;
        weaponHolder = null;
    }
}
