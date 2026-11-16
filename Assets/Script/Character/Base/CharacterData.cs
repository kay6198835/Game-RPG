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
    // NEED CHECK: now shared by enemies too (was EntityData.aima). FormerlySerializedAs("aima") carries old
    // EntityData assets over on load; keep it until every enemy asset has been re-saved. Confirm Knight.asset
    // still shows its controller in the Inspector after recompile.
    [SerializeField, FormerlySerializedAs("aima")] public RuntimeAnimatorController animationController;
    [field: SerializeField] public Weapon Weapon { get; set; }

    // NEED CHECK: 2026-11-09 (stat split) — CreateRuntimeStats() (Instantiate a BaseStatsSO clone) removed;
    // StatHandlerBase builds a StatSheet from Stats instead.
}
