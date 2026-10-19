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
    [SerializeField] private BaseStatsSO stats;
    [SerializeField] private List<AbilityBinding> abilityBindings = new List<AbilityBinding>();
    public BaseStatsSO Stats => stats;
    public List<AbilityBinding> AbilityBindings => abilityBindings;
    [SerializeField] public AnimatorOverrideController animationController;
    [field: SerializeField] public Weapon Weapon { get; set; }
}
