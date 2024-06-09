using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
[CreateAssetMenu(fileName = "newEntityData", menuName = "Data/Entity Data/Base Data")]
public class EntityData : ScriptableObject
{
    [Header("Player Stats")]
    [SerializeField] private EntityStatsSO statsSO;
    [Header("Player Component")]
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private AnimatorOverrideController aima;
    [Header("Player Stats")]
    [SerializeField] private float maxHealth;
    [Header("Idle State")]
    [SerializeField] private float rangeCheckFieldOfView;
    [SerializeField] private float idleDurationTime;
    [Header("Move State")]
    [SerializeField] private float moveDurationTime;
    [SerializeField] private float movementVelocities = 10f;
    [SerializeField] private float rangeCheckAttack;
    [Header("Attack State")]
    [SerializeField] private WeaponSO weaponSO;

    public float MaxHealth { get => maxHealth; }
    public LayerMask LayerMask { get => layerMask; }
    public AnimatorOverrideController Aima { get => aima;}
    public float RangeCheckFieldOfView { get => rangeCheckFieldOfView;}
    public float IdleDurationTime { get => idleDurationTime;}
    public float MovementVelocities { get => movementVelocities;}
    public float RangeCheckAttack { get => rangeCheckAttack; }
    public float MoveDurationTime { get => moveDurationTime;}
    public WeaponSO WeaponSO { get => weaponSO;}
    public EntityStatsSO StatsSO { get => statsSO;}
    private void OnValidate()
    {
        
    }
}
