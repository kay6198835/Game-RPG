using UnityEngine;

public abstract class Weapon : InteractiveObjects
{
    [Header("Abtract Weapon")]
    [SerializeField] protected WeaponHolder holder;
    [SerializeField] protected AbilityHolder abilityHolder;
    [SerializeField] protected WeaponStats stats;
    [SerializeField] protected ActivateSkill currentAbilitySO;
    [SerializeField] protected Collider2D collider;
    protected float lastClickTime;
    protected float deplayTime;
    protected float durationNextAttack;
    protected bool canAttack;
    public AbilityHolder AbilityHolder { get => abilityHolder; }
    protected override void Awake()
    {
        base.Awake();
        collider = GetComponent<Collider2D>();
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
            collider.enabled = false;
            //holder = weaponHolder;
            abilityHolder = holder.Core.AbilityHolder;
            transform.SetParent(holder.transform);
            transform.position = transform.parent.position;
        }
        else
        {
            transform.position = transform.parent.position + Vector3.one * 1f;
            transform.SetParent(null);
            abilityHolder = null;
            //holder = null;
            collider.enabled = true;
        }
    }
    public override bool Interact(Interact interactor)
    {
        Equid((WeaponHolder)interactor);
        return true;
    }
    public void Equid(WeaponHolder weaponHolder)
    {
        holder = weaponHolder;
        holder.Equid_UnEquid(this);
        collider.enabled = false;
        abilityHolder = holder.Core.AbilityHolder;
        transform.SetParent(holder.transform);
        transform.position = transform.parent.position;
    }
    public void UnEquid()
    {
        transform.position = transform.parent.position + Vector3.one * 1f;
        transform.SetParent(null);
        abilityHolder = null;
        collider.enabled = true;
        holder.Equid_UnEquid(this);
        holder = null;
    }
}
