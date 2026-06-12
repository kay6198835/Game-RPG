using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Một chỉ số đơn lẻ: BaseValue + danh sách modifier.
/// Value chỉ tính lại khi có thay đổi (dirty flag), KHÔNG tính lại mỗi frame.
/// Công thức: FinalValue = (Base + ΣFlat) × (1 + ΣPercentAdd) × Π(1 + PercentMult)
/// </summary>
[Serializable]
public class Stat
{
    private float baseValue;
    private float cachedValue;
    private bool isDirty = true;
    private readonly List<StatModifier> modifiers = new List<StatModifier>();

    /// <summary>Bắn ra khi BaseValue hoặc modifier thay đổi.</summary>
    public event Action OnChanged;

    public Stat(float baseValue = 0f) => this.baseValue = baseValue;

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
        modifiers.Add(modifier);
        modifiers.Sort((a, b) => a.Order.CompareTo(b.Order));
        SetDirty();
    }

    public bool RemoveModifier(StatModifier modifier)
    {
        if (!modifiers.Remove(modifier)) return false;
        SetDirty();
        return true;
    }

    /// <summary>Gỡ mọi modifier đến từ một nguồn (tháo trang bị, hết buff...).</summary>
    public bool RemoveModifiersFromSource(object source)
    {
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
