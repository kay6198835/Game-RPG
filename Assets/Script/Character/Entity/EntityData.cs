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
    [Header("Idle State")]
    [SerializeField] private float rangeCheckFieldOfView;
    [SerializeField] private float idleDurationTime;
    [Header("Move State")]
    [SerializeField] private float moveDurationTime;
    [SerializeField] private float movementVelocities = 10f;
    [SerializeField] private float rangeCheckAttack;
    [SerializeField, Range(1f, 50f)] private float rangeCheckChase = 10f;
    [Header("Attack State")]
    [SerializeField] private WeaponSO weaponSO;
    [Header("Death State")]
    [SerializeField, Range(0.1f, 5f)] private float deathDurationTime = 1f;

    //public float MaxHealth { get => maxHealth; }
    public LayerMask LayerMask { get => layerMask; }
    public AnimatorOverrideController Aima { get => aima;}
    public float RangeCheckFieldOfView { get => rangeCheckFieldOfView;}
    public float IdleDurationTime { get => idleDurationTime;}
    public float MovementVelocities { get => movementVelocities;}
    public float RangeCheckAttack { get => rangeCheckAttack; }
    public float RangeCheckChase { get => rangeCheckChase; }
    public float MoveDurationTime { get => moveDurationTime;}
    public float DeathDurationTime { get => deathDurationTime; }
    public WeaponSO WeaponSO { get => weaponSO;}
    public EntityStatsSO StatsSO { get => statsSO;}

    // Each spawned entity needs its own health pool: statsSO is an asset shared by every
    // instance of this enemy type, so damage would bleed across enemies and persist into
    // the asset between play sessions. Call on a runtime copy of this SO only.
    public void MakeRuntimeStats()
    {
        statsSO = Instantiate(statsSO);
        statsSO.ResetStats();
    }
    private void OnValidate()
    {

    }
}
