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
    // The authored, data-only stat profile. A character calculates from it through its own StatSheet.
    public BaseStatsSO Stats => stats;
    public List<AbilityBinding> AbilityBindings => abilityBindings;
    // Shared by enemies too (was EntityData.aima). No committed asset stores the old "aima" key since the
    // Knight enemy assets were deleted (2026-11-16); FormerlySerializedAs("aima") can go at the next cleanup.
    [SerializeField, FormerlySerializedAs("aima")] public RuntimeAnimatorController animationController;
    [field: SerializeField] public Weapon Weapon { get; set; }

    // NEED CHECK: 2026-11-09 (stat split) — CreateRuntimeStats() (Instantiate a BaseStatsSO clone) removed;
    // StatHandlerBase builds a StatSheet from Stats instead.
}
