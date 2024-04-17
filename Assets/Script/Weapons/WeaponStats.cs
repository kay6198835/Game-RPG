using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponStats : ScriptableObject
{
    [Header("WP data")]
    protected WeaponType type;
    [SerializeField] protected LayerMask layerMask;
    [SerializeField] protected List<AttackSO> attackState;
    [SerializeField] protected ActivateSkill ability;
    [SerializeField] protected ActivateSkill special;

    public WeaponType Type { get => type; }
    public LayerMask LayerMask { get => layerMask; }
    public ActivateSkill Ability { get => ability; }
    public ActivateSkill Special { get => special; }
    public List<AttackSO> AttackState { get => attackState; }
}
