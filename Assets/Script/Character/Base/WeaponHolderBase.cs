using UnityEngine;

/// <summary>
/// Shared weapon-using layer of Player and Entity: holds the equipped <see cref="Weapon"/> and drives
/// its attack lifecycle (CanAttack → Attack → MakeDamage → EndDamage → CanChain). Damage is computed
/// from the owner's current stats, so the same weapon works on either side.
/// </summary>
public abstract class WeaponHolderBase<TCore> : CoreComponentBase<TCore>, IWeaponHolder where TCore : CoreBase
{
    [SerializeField] protected Weapon weapon;

    protected IVitalComponent vital;
    protected ICharacterInput input;
    private Animator animator;
    private ICharacter owner;

    public Weapon Weapon => weapon;
    public Transform Transform => transform;
    public Transform OwnerTransform => Owner != null ? Owner.Transform : transform;
    public Animator Animator => animator != null ? animator : (animator = GetComponentInParent<Animator>());
    public IAimProvider Aim => input;
    private ICharacter Owner => owner ??= GetComponentInParent<ICharacter>();

    protected override void Start()
    {
        base.Start();
        Core.TryGetCapability(out vital);
        Core.TryGetCapability(out input);
    }

    public void Equid_UnEquid(Weapon weapon)
    {
        this.weapon = this.weapon == null ? weapon : null;
    }

    /// <summary>Starts one attack stage on the equipped weapon. Safe to call repeatedly to chain.</summary>
    public void Attack()
    {
        if (weapon == null) return;
        weapon.OnAttackEnter(this);
    }

    public bool CanAttack() => weapon != null && weapon.CanAttack();

    public bool CanChain() => weapon != null && weapon.CanChain();

    public void MakeDamage()
    {
        if (weapon == null) return;
        weapon.OnActivate(CalculateCurrentDamage());
    }

    public void EndDamage()
    {
        if (weapon == null) return;
        weapon.OnDeactivate();
    }

    protected virtual float CalculateCurrentDamage()
    {
        float finalDamage = vital.GetCurrentStatValue(StatType.PhysicalDamage)
            + weapon.CurrentStage.attackDamage;
        if (Utility.RollChance(vital.GetCurrentStatValue(StatType.CritChance)))
            finalDamage += vital.GetCurrentStatValue(StatType.CritDamage);
        return finalDamage;
    }
}
