using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class WeaponStats : ScriptableObject
{
    [field: SerializeField] public LayerMask LayerMask { get; protected set; }

    [field: SerializeField]
    [field: FormerlySerializedAs("<AttackState>k__BackingField")]
    public List<AttackSO> AttackStages { get; protected set; } = new List<AttackSO>();

    [field: SerializeField] public ActivateSkill AbilityWeapon { get; protected set; }
    [field: SerializeField] public ActivateSkill SkillWeapon { get; protected set; }

    public abstract WeaponType Type { get; }

    public int StageCount => AttackStages == null ? 0 : AttackStages.Count;

    public AttackSO GetStage(int index) => AttackStages[index];
    [field: SerializeField] public StatModifierGroup modifiers { get; protected set; }
}
