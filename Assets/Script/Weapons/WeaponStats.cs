using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponStats : ScriptableObject
{
    [field: SerializeField] public WeaponType Type { get; protected set; }
    [field: SerializeField] public LayerMask LayerMask { get; protected set; }
    // [field: SerializeField] public List<AttackSO> AttackState { get; protected set; }
    [field: SerializeField] public ActivateSkill AbilityWeapon { get; protected set; }
    [field: SerializeField] public ActivateSkill SkillWeapon { get; protected set; }
}
