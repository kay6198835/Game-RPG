using UnityEngine;

[CreateAssetMenu(fileName = "newEntityData", menuName = "Data/Entity Data/Base Data")]
public class EntityData : CharacterData
{
    // Stats, ability bindings and the weapon (was `weaponSO`) live on CharacterData, shared with PlayerData.
    [Header("Entity Component")]
    [SerializeField] private LayerMask layerMask;
    // NEED CHECK: removed — the enemy controller now lives on CharacterData.animationController (player style).
    //[SerializeField] private AnimatorOverrideController aima;

    // NEED CHECK: 2026-11-09 (stat split) — rangeView / angleView / idleDurationTime / moveDurationTime were
    // cached here by OnValidate from the authored asset (and read 0 when the asset was not recalculated yet).
    // They are now computed from the enemy's own stats, so buffs and debuffs reach them.
    [Header("Multipliers (value = stat × multiplier)")]
    [SerializeField] private float rangeViewMultiplier = 1.2f;   // × MaxRangeAttack
    [SerializeField] private float angleMultiplier = 1.5f;       // × INT, capped at 360
    [SerializeField] private float idleMultiplier = 0.1f;        // × STR
    [SerializeField] private float moveMultiplier = 0.5f;        // × STR

    public LayerMask LayerMask => layerMask;

    /// <summary>Detection range: MaxRangeAttack × multiplier, from the given character's stats.</summary>
    public float RangeView(IStatService stats) => Read(stats, StatType.MaxRangeAttack) * rangeViewMultiplier;

    /// <summary>View cone in degrees: INT × multiplier, capped at 360.</summary>
    public float AngleView(IStatService stats) => Mathf.Min(360f, Read(stats, StatType.INT) * angleMultiplier);

    /// <summary>Idle time in seconds: STR × multiplier.</summary>
    public float IdleDurationTime(IStatService stats) => Read(stats, StatType.STR) * idleMultiplier;

    /// <summary>Wander time in seconds: STR × multiplier.</summary>
    public float MoveDurationTime(IStatService stats) => Read(stats, StatType.STR) * moveMultiplier;

    private static float Read(IStatService stats, StatType type) => stats != null ? stats.GetStatValue(type) : 0f;
}
