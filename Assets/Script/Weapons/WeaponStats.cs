using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponStats : ScriptableObject
{
    [Header("WP data")]
    protected WeaponType type;
    [SerializeField] protected LayerMask layerMask;
    [SerializeField] protected List<AttackSO> attackState;
    [SerializeField] protected ActivateSkill abilityWeapon;
    [SerializeField] protected ActivateSkill skillWeapon;

    public WeaponType Type { get => type; }
    public LayerMask LayerMask { get => layerMask; }
    public ActivateSkill AbilityWeapon { get => abilityWeapon; }
    public ActivateSkill SkillWeapon { get => skillWeapon; }
    public List<AttackSO> AttackState { get => attackState; }
}
