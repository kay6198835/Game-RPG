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
    [SerializeField] private float rangeView;
    [SerializeField] private float angleView;
    [SerializeField] private float idleDurationTime;
    [SerializeField] private float moveDurationTime;

    [Header("Multipliers")]

    [SerializeField] private float rangeViewMultiplier = 1.2f;
    [SerializeField] private float angleMultiplier = 1.5f;
    [SerializeField] private float idleMultiplier = 0.1f;
    [SerializeField] private float moveMultiplier = 0.5f;


    public LayerMask LayerMask { get => layerMask; }
    public float RangeView { get => rangeView; }
    public float AngleView { get => angleView; }
    public float IdleDurationTime { get => idleDurationTime; }
    public float MoveDurationTime { get => moveDurationTime; }
    private void OnValidate()
    {
        rangeView = Stats.GetStatValue(StatType.MaxRangeAttack) * rangeViewMultiplier;
        angleView = Stats.GetStatValue(StatType.INT) * angleMultiplier > 360f ? 360f : Stats.GetStatValue(StatType.INT) * angleMultiplier;
        idleDurationTime = Stats.GetStatValue(StatType.STR) * idleMultiplier;
        moveDurationTime = Stats.GetStatValue(StatType.STR) * moveMultiplier;
    }
}
