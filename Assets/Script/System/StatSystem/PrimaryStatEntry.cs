using System;
using UnityEngine;

/// <summary>
/// One authored primary stat (STR, DEX, INT, VIT, LUK) on a <see cref="BaseStatsSO"/>: its type and base value.
/// The only number a designer types per stat; everything else is computed by <see cref="StatSheet"/>.
/// </summary>
// NEED CHECK: new 2026-11-09 (stat split). Serialized keys match the old Stat class
// (`<Type>k__BackingField`, `baseValue`) so existing assets load into this type unchanged.
[Serializable]
public class PrimaryStatEntry
{
    [field: SerializeField] public StatType Type { get; private set; }
    [SerializeField] private float baseValue;

    public float BaseValue => baseValue;

    public PrimaryStatEntry() { }   // Unity deserialization

    public PrimaryStatEntry(StatType type, float baseValue = 0f)
    {
        Type = type;
        this.baseValue = baseValue;
    }
}
