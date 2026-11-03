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
    [SerializeField, FormerlySerializedAs("statsSO")] protected BaseStatsSO stats;
    [SerializeField] protected List<AbilityBinding> abilityBindings = new List<AbilityBinding>();
    // The authored asset. Never write modifiers to it; use CreateRuntimeStats() for a per-character copy.
    public BaseStatsSO Stats => stats;
    public List<AbilityBinding> AbilityBindings => abilityBindings;
    [SerializeField] public RuntimeAnimatorController animationController;
    [field: SerializeField] public Weapon Weapon { get; set; }

    // The caller owns the clone and must Destroy it: this SO is shared by every character built from it,
    // so a clone cached here would be shared too and outlive Play Mode.
    public BaseStatsSO CreateRuntimeStats()
    {
        if (stats == null) return null;
        var clone = Instantiate(stats);
        clone.name = stats.name + " (Runtime)";
        return clone;
    }
}
