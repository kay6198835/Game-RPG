using UnityEngine;

/// <summary>
/// Shared base of Player and Entity (the BaseGrid&lt;T&gt; of the character layer). Owns the typed core
/// hub reference and marks the root as an <see cref="ICharacter"/>. It does not hand out components.
/// </summary>
public abstract class CharacterBase<TCore> : BaseEntity, ICharacter<TCore> where TCore : CoreBase
{
    [SerializeField] protected TCore core;

    public TCore Core => core;
    ICore ICharacter.Core => core;
    public Transform Transform => transform;
    public abstract void SetCharacterData(CharacterData data);

    /// <summary>Applies the current CharacterData to every core component: stats, weapon, abilities, vitals, effects.</summary>
    // Order matters: drop the old stat clone first so the weapon bonus lands on the new clone, then re-seed
    // vitals from those max values. Not core.ReloadCore(): it would also zero the wallet.
    protected void ApplyCharacterData()
    {
        if (core.TryGetCapability(out StatHandlerBase<TCore> stats)) stats.ReloadComponent();
        if (core.TryGetCapability(out WeaponHolderBase<TCore> weapons)) weapons.EquipDefaultWeapon();
        if (core.TryGetCapability(out AbilityHolderBase<TCore> abilities)) abilities.SetAbility();
        if (core.TryGetCapability(out VitalStatsBase<TCore> vitals)) vitals.ReloadComponent();
        if (core.TryGetCapability(out EffectReceiverBase<TCore> effects)) effects.ReloadComponent();
    }

    public virtual void Reborn()
    {
        core.ReloadCore();
    }
}