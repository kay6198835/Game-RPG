using UnityEngine;

public abstract class Weapon : InteractiveObjects
{
    [Header("Abtract Weapon")]
    [SerializeField] protected WeaponHolder holder;
    [SerializeField] protected AbilityHolder abilityHolder;
    [SerializeField] protected WeaponStats stats;
    [SerializeField] protected ActivateSkill currentAbilitySO;
    protected float lastClickTime;
    protected float deplayTime;
    protected float durationNextAttack;
    protected bool canAttack;
    public AbilityHolder AbilityHolder { get => abilityHolder; }
    protected override void Awake()
    {
        base.Awake();
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
            transform.SetParent(holder.transform);
            transform.position = transform.parent.position;
        }
        else
        {
            transform.position = transform.parent.position + Vector3.one * 1f;
            transform.SetParent(null);
            holder = null;
            abilityHolder = null;

        }
    }



    public override bool Interact(Interactor interactor)
    {
        if(interactor.Core.WeaponHolder.Weapon == null)
        {
            interactor.Core.WeaponHolder.Equid(this);
            Debug.Log("Equid");
        }
        else
        {
            interactor.Core.WeaponHolder.UnEquid();
            Debug.Log("UnEquid");
        }
        return true;
    }
}
