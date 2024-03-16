using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
[CreateAssetMenu(fileName = "newEntityData", menuName = "Data/Entity Data/Base Data")]
public class EntityData : ScriptableObject
{
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private AnimatorController aima;
    [Header("Idle State")]
    [SerializeField] private float rangeCheckFieldOfView;
    [SerializeField] private float idleDurationTime;
    [Header("Move State")]
    [SerializeField] private float moveDurationTime;
    [SerializeField] private float movementVelocities = 10f;
    [SerializeField] private float rangeCheckAttack;
    [Header("Attack State")]
    [SerializeField] private List<AttackSO> attackSOs;

    public LayerMask LayerMask { get => layerMask; }
    public AnimatorController Aima { get => aima;}
    public float RangeCheckFieldOfView { get => rangeCheckFieldOfView;}
    public float IdleDurationTime { get => idleDurationTime;}
    public float MovementVelocities { get => movementVelocities;}
    public float RangeCheckAttack { get => rangeCheckAttack; }
    public float MoveDurationTime { get => moveDurationTime;}

    private void Awake()
    {
        layerMask = LayerMask.GetMask("Player");
    }
}
