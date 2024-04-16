using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponStats : ScriptableObject
{
    [Header("WP data")]
    protected WeaponType type;
    [SerializeField] protected LayerMask layerMask;
    [SerializeField] protected List<AttackSO> attackState;
    [SerializeField] protected AbilitySO ability;
    [SerializeField] protected AbilitySO special;

    public WeaponType Type { get => type; }
    public LayerMask LayerMask { get => layerMask; }
    public AbilitySO Ability { get => ability; }
    public AbilitySO Special { get => special; }
    public List<AttackSO> AttackState { get => attackState; }
}
