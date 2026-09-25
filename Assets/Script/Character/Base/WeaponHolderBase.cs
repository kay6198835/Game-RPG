using UnityEngine;

/// <summary>
/// Shared weapon-using layer of Player and Entity: holds the equipped <see cref="Weapon"/> and drives
/// its attack lifecycle (CanAttack → Attack → MakeDamage → EndDamage → CanChain). Damage is computed
/// from the owner's current stats, so the same weapon works on either side. No weapon, no attack:
/// every attack entry point returns false / does nothing while <see cref="Weapon"/> is null.
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
        if (weapon == null) EquipDefaultWeapon();
    }

    // Same for Player and Entity: CharacterData.DefaultWeapon is equipped once, and only when its prefab
    // carries a Weapon. A pooled character keeps it across re-spawns, so this never duplicates it.
    private void EquipDefaultWeapon()
    {
        WeaponSO weaponSO = Core.Data != null ? Core.Data.DefaultWeapon : null;
        GameObject prefab = weaponSO != null ? weaponSO.Weapon : null;
        if (prefab == null || !prefab.TryGetComponent(out Weapon _)) return;

        GameObject instance = Instantiate(prefab, transform.position, Quaternion.identity);
        instance.GetComponent<Weapon>().Equid(this);
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
