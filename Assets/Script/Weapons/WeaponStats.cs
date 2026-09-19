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

    // FormerlySerializedAs giữ dữ liệu đã author khi property đổi tên `modifiers` -> `StatModifiers`
    // (2026-08-21). Khóa cũ là dạng backing-field của auto-property, không phải "modifiers" trần.
    [field: SerializeField]
    [field: FormerlySerializedAs("<modifiers>k__BackingField")]
    public StatModifierGroup StatModifiers { get; protected set; }
}
