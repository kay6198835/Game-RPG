using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Data every character is built from, shared by PlayerData and EntityData the way the component bases
/// are shared: the stat profile, the Abilities v2 bindings and the weapon equipped at spawn.
/// Side-specific tuning (enemy AI ranges, timers) stays on the subclass.
/// </summary>
public abstract class CharacterData : ScriptableObject
{
    // FormerlySerializedAs keeps both sides' authored data: PlayerData used `stats` and an auto-property
    // backing field, EntityData used `statsSO`, `abilityBindings` and `weaponSO`.
    [Header("Character")]
    [SerializeField, FormerlySerializedAs("statsSO")] private BaseStatsSO stats;
    [SerializeField, FormerlySerializedAs("<AbilityBindings>k__BackingField")]
    private List<AbilityBinding> abilityBindings = new List<AbilityBinding>();
    [SerializeField, FormerlySerializedAs("weaponSO")] private WeaponSO defaultWeapon;

    public BaseStatsSO Stats => stats;
    public List<AbilityBinding> AbilityBindings => abilityBindings;
    /// <summary>Weapon item equipped when the character spawns empty-handed. Null = starts unarmed.</summary>
    public WeaponSO DefaultWeapon => defaultWeapon;
}
