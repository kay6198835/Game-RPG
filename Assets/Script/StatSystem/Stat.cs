using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Một chỉ số đơn lẻ: StatType + BaseValue + danh sách modifier.
/// type + baseValue được serialize để chỉnh trong Inspector; modifiers là runtime-only.
/// Value chỉ tính lại khi có thay đổi (dirty flag), KHÔNG tính lại mỗi frame.
/// Công thức: FinalValue = (Base + ΣFlat) × (1 + ΣPercentAdd) × Π(1 + PercentMult)
/// </summary>
[Serializable]
public class Stat
{
    [SerializeField] private StatType type;
    [SerializeField] private float baseValue;

    private float cachedValue;
    private bool isDirty = true;
    private List<StatModifier> modifiers;   // runtime-only: StatModifier không Unity-serializable

    /// <summary>Loại chỉ số mà Stat này đại diện (STR, MaxHP, ...).</summary>
    public StatType Type => type;

    /// <summary>Bắn ra khi BaseValue hoặc modifier thay đổi.</summary>
    public event Action OnChanged;

    public Stat() { }   // Unity deserialization

    public Stat(StatType type, float baseValue = 0f)
    {
        this.type = type;
        this.baseValue = baseValue;
    }

    private List<StatModifier> Modifiers => modifiers ??= new List<StatModifier>();

    public float BaseValue
    {
        get => baseValue;
        set
        {
            if (Mathf.Approximately(baseValue, value)) return;
            baseValue = value;
            SetDirty();
        }
    }

    /// <summary>Giá trị cuối cùng sau khi áp dụng toàn bộ modifier.</summary>
    public float Value
    {
        get
        {
            if (isDirty)
            {
                cachedValue = CalculateFinalValue();
                isDirty = false;
            }
            return cachedValue;
        }
    }

    public void AddModifier(StatModifier modifier)
    {
        Modifiers.Add(modifier);
        Modifiers.Sort((a, b) => a.Order.CompareTo(b.Order));
        SetDirty();
    }

    public bool RemoveModifier(StatModifier modifier)
    {
        if (modifiers == null || !modifiers.Remove(modifier)) return false;
        SetDirty();
        return true;
    }

    /// <summary>Gỡ mọi modifier đến từ một nguồn (tháo trang bị, hết buff...).</summary>
    public bool RemoveModifiersFromSource(object source)
    {
        if (modifiers == null) return false;
        bool removed = false;
        for (int i = modifiers.Count - 1; i >= 0; i--)
        {
            if (ReferenceEquals(modifiers[i].Source, source))
            {
                modifiers.RemoveAt(i);
                removed = true;
            }
        }
        if (removed) SetDirty();
        return removed;
    }

    private void SetDirty()
    {
        isDirty = true;
        OnChanged?.Invoke();
    }

    private float CalculateFinalValue()
    {
        float finalValue = baseValue;
        if (modifiers == null) return finalValue;

        float percentAddSum = 0f;
        for (int i = 0; i < modifiers.Count; i++)
        {
            StatModifier mod = modifiers[i];
            switch (mod.Type)
            {
                case ModifierType.Flat:
                    finalValue += mod.Value;
                    break;

                case ModifierType.PercentAdd:
                    percentAddSum += mod.Value;
                    // Cộng dồn hết các PercentAdd liên tiếp rồi mới nhân 1 lần
                    bool isLastPercentAdd = i + 1 >= modifiers.Count
                        || modifiers[i + 1].Type != ModifierType.PercentAdd;
                    if (isLastPercentAdd)
                    {
                        finalValue *= 1f + percentAddSum;
                        percentAddSum = 0f;
                    }
                    break;

                case ModifierType.PercentMult:
                    finalValue *= 1f + mod.Value;
                    break;
            }
        }
        return finalValue;
    }
}
