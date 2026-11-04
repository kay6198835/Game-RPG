using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "newEntityData", menuName = "Data/Entity Data/Base Data")]
public class EntityData : CharacterData
{
    // Stats, ability bindings and the weapon (was `weaponSO`) live on CharacterData, shared with PlayerData.
    [Header("Entity Component")]
    [SerializeField] private LayerMask layerMask;
    // NEED CHECK: removed — the enemy controller now lives on CharacterData.animationController (player style).
    //[SerializeField] private AnimatorOverrideController aima;
    [Header("Idle State")]
    [SerializeField] private float rangeCheckFieldOfView;
    [SerializeField] private float idleDurationTime;
    [Header("Move State")]
    [SerializeField] private float moveDurationTime;
    // NEED CHECK: removed — had no reader. Move speed is StatType.MoveSpeed (MovementBase.MoveSpeed),
    // attack range is StatType.MaxRangeAttack / MinRangeAttack (EntityFindTarget).
    //[SerializeField] private float movementVelocities = 10f;
    //[SerializeField] private float rangeCheckAttack;

    //public float MaxHealth { get => maxHealth; }
    public LayerMask LayerMask { get => layerMask; }
    // NEED CHECK: removed with `aima`; callers read animationController instead.
    //public AnimatorOverrideController Aima { get => aima;}
    public float RangeCheckFieldOfView { get => rangeCheckFieldOfView;}
    public float IdleDurationTime { get => idleDurationTime;}
    // NEED CHECK: removed with the fields above.
    //public float MovementVelocities { get => movementVelocities;}
    //public float RangeCheckAttack { get => rangeCheckAttack; }
    public float MoveDurationTime { get => moveDurationTime;}
    private void OnValidate()
    {
        
    }
}
