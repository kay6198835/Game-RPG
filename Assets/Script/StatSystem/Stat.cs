using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Một chỉ số đơn lẻ: StatType + BaseValue + danh sách modifier.
/// type + baseValue được serialize để chỉnh trong Inspector; modifiers là runtime-only.
/// Value chỉ tính lại khi có thay đổi (dirty flag), KHÔNG tính lại mỗi frame.
/// Công thức: FinalValue = (Base + ΣFlat) × (1 + ΣPercentAdd) × Π(1 + PercentMult)
///
/// Tầng này chỉ thao tác TỪNG modifier một. Mọi thao tác hàng loạt (gắn/gỡ theo nguồn)
/// nằm ở StatsSO — nơi đã giữ sẵn việc gom event và RecalculateDerived.
/// </summary>
[Serializable]
public class Stat
{
    [field: SerializeField] public StatType Type { get; private set; }
    [SerializeField] private float baseValue;
    [SerializeField] private float levelUpValue;
    [SerializeField] private float equipmentValue;

    public float BaseValue { get => baseValue; set => SetField(ref baseValue, value); }
    public float LevelUpValue { get => levelUpValue; set => SetField(ref levelUpValue, value); }
    public float AdjustedValue => baseValue + levelUpValue;   // derived — no setter, no serialize
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
    }                     // lazy, dirty-flag path you already have

    private void SetField(ref float field, float value)
    {
        if (Mathf.Approximately(field, value)) return;
        field = value;
        SetDirty();
    }

    private bool isDirty = false;
    [field: SerializeField] public List<StatModifier> modifiers { get; private set; } = new List<StatModifier>();

    /// <summary>Loại chỉ số mà Stat này đại diện (STR, MaxHP, ...).</summary>
    public StatType Type => type;

    /// <summary>Bắn ra khi BaseValue hoặc modifier thay đổi.</summary>
    public event Action OnChanged;
    /// <summary>Buộc tính lại Value ở lần đọc kế tiếp (dùng sau khi sửa baseValue trong Inspector).</summary>
    public void MarkDirty() => SetDirty();

    public Stat() { }   // Unity deserialization

    public Stat(StatType type, float baseValue = 0f)
    {
        this.type = type;
        this.baseValue = baseValue;
    }

    // private List<StatModifier> ModifierList => modifiers ??= new List<StatModifier>();

    /// <summary>Modifier đang gắn (chỉ đọc). StatsSO duyệt list này để gỡ theo nguồn.</summary>
    // Không dùng ModifierList: đọc thôi thì đừng tạo list rỗng cho stat chưa từng có modifier nào.
    public IReadOnlyList<StatModifier> Modifiers => (IReadOnlyList<StatModifier>)modifiers ?? Array.Empty<StatModifier>();



    public void AddModifier(StatModifier modifier)
    {
        modifiers.Add(modifier);
        modifiers.Sort((a, b) => a.Order.CompareTo(b.Order));
        SetDirty();
    }

    public bool RemoveModifier(StatModifier modifier)
    {
        if (modifiers == null || !modifiers.Remove(modifier)) return false;
        SetDirty();
        return true;
    }

    /// <summary>Gỡ modifier ở vị trí index — dùng khi caller đã duyệt sẵn list, khỏi tìm lại tuyến tính.</summary>
    public void RemoveModifierAt(int index)
    {
        if (modifiers == null || index < 0 || index >= modifiers.Count) return;
        modifiers.RemoveAt(index);
        SetDirty();
    }

    private void SetDirty()
    {
        isDirty = true;
        OnChanged?.Invoke();
    }

    private float CalculateFinalValue()
    {
        float finalValue = AdjustedValue;
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
    public void ClearModifiers()
    {
        if (modifiers == null || modifiers.Count == 0) return;
        modifiers.Clear();
        SetDirty();
    }
}
