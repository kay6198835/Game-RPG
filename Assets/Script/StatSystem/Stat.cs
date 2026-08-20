using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Một chỉ số đơn lẻ: StatType + ba tầng giá trị authored + danh sách modifier runtime.
///
/// Ba tầng authored (serialize, sửa được ở Inspector):
///   BaseValue       — giá trị gốc của chỉ số
///   LevelUpValue    — cộng dồn từ lên cấp / phân bổ điểm
///   EquipmentValue  — cộng dồn từ trang bị đang mặc
///
/// Hai tầng dẫn xuất (KHÔNG serialize — tính ra, không author được):
///   AdjustedValue = BaseValue + LevelUpValue
///   FinalValue    = (AdjustedValue + EquipmentValue + ΣFlat) × (1 + ΣPercentAdd) × Π(1 + PercentMult)
///
/// Value chỉ tính lại khi có thay đổi (dirty flag), KHÔNG tính lại mỗi frame.
///
/// LƯU Ý serialization: Unity ghi thẳng vào FIELD, không bao giờ đi qua property setter.
/// Sửa ở Inspector / Undo / prefab revert / deserialize đều KHÔNG gọi SetDirty() — vì vậy
/// StatsSO.OnValidate() phải gọi MarkDirty() cho từng Stat. Đừng bỏ hook đó.
///
/// CHỈ ba field authored được serialize. cachedValue và modifiers cố tình KHÔNG serialize:
/// chúng là trạng thái runtime, nếu lưu xuống .asset thì file tự thay đổi sau mỗi Play Mode.
///
/// Tầng này chỉ thao tác TỪNG modifier một. Mọi thao tác hàng loạt (gắn/gỡ theo nguồn)
/// nằm ở StatsSO — nơi đã giữ sẵn việc gom event và RecalculateDerived.
/// </summary>
[Serializable]
public class Stat
{
    // Serialize dưới key `<Type>k__BackingField`, KHÔNG phải `type`. Mọi .asset phải dùng
    // đúng key đó, nếu không Unity đọc không ra và cả list rơi hết về STR (0).
    [field: SerializeField] public StatType Type { get; private set; }

    [SerializeField] private float baseValue;
    [SerializeField] private float levelUpValue;
    private float equipmentValue;
    private float adjustedValue;
    private float finalValue;

    // KHÔNG serialize: đây là cache, không phải dữ liệu authored. Nếu serialize thì mọi lần
    // tính lại đều ghi vào .asset -> file tự đổi sau mỗi Play Mode. isDirty khởi tạo true để
    // lần đọc Value đầu tiên sau khi load luôn tính lại, thay vì tin vào cache đã lưu.
    [NonSerialized] private float cachedValue;
    [NonSerialized] private bool isDirty = true;

    // BẮT BUỘC [NonSerialized]: StatModifier nay đã Unity-serializable, mà Stat nằm trong
    // StatsSO.stats -> nếu để serialize, buff runtime sẽ ghi thẳng vào .asset và sống dai
    // qua các phiên Play Mode.
    [SerializeField] private List<StatModifier> modifiers = new List<StatModifier>();

    /// <summary>Bắn ra khi một trong ba tầng authored hoặc modifier thay đổi.</summary>
    public event Action OnChanged;

    /// <summary>Buộc tính lại Value ở lần đọc kế tiếp (dùng sau khi sửa field trong Inspector).</summary>
    public void MarkDirty() => SetDirty();

    public Stat() { }   // Unity deserialization

    public Stat(StatType type, float baseValue = 0f, float levelUpValue = 0f, float equipmentValue = 0f)
    {
        Type = type;
        this.baseValue = baseValue;
        this.levelUpValue = levelUpValue;
        this.equipmentValue = equipmentValue;
    }

    /// <summary>Modifier đang gắn (chỉ đọc). StatsSO duyệt list này để gỡ theo nguồn.</summary>
    // Đọc thôi thì đừng tạo list rỗng cho stat chưa từng có modifier nào.
    public IReadOnlyList<StatModifier> Modifiers => (IReadOnlyList<StatModifier>)modifiers ?? Array.Empty<StatModifier>();

    // ------------------------- Tầng authored -------------------------

    public float BaseValue
    {
        get => baseValue;
        set => SetField(ref baseValue, value);
    }

    public float LevelUpValue
    {
        get => levelUpValue;
        set => SetField(ref levelUpValue, value);
    }

    public float EquipmentValue
    {
        get => equipmentValue;
        set => SetField(ref equipmentValue, value);
    }

    // ------------------------- Tầng dẫn xuất -------------------------

    /// <summary>Gốc + lên cấp, CHƯA tính trang bị và modifier. Không serialize: đây là giá trị tính ra.</summary>
    public float AdjustedValue => baseValue + levelUpValue;

    /// <summary>Giá trị cuối cùng sau khi áp dụng trang bị và toàn bộ modifier.</summary>
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

    /// <summary>Alias của Value — giữ đúng tên trong bảng công thức stat.</summary>
    public float FinalValue => Value;

    /// <summary>Phần chênh do trang bị và modifier tạo ra (UI hiển thị "+12" màu xanh).</summary>
    public float BonusValue => Value - AdjustedValue;

    public void Recaulate()
    {
        adjustedValue = AdjustedValue;
        CalculateFinalValue();
    }

    // ------------------------- Modifier -------------------------

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

    public void ClearModifiers()
    {
        if (modifiers == null || modifiers.Count == 0) return;
        modifiers.Clear();
        SetDirty();
    }

    // ------------------------- Nội bộ -------------------------

    private void SetField(ref float field, float value)
    {
        if (Mathf.Approximately(field, value)) return;
        field = value;
        SetDirty();
    }

    private void SetDirty()
    {
        isDirty = true;
        OnChanged?.Invoke();
        Recaulate();
    }

    private float CalculateFinalValue()
    {
        float finalValue = AdjustedValue;

        if (modifiers == null)
        {
            this.equipmentValue = 0f;
            this.finalValue = finalValue;
            return finalValue;                     // trả AdjustedValue, KHÔNG phải 0
        }

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
        if (Type.IsPrimary())
        {
            this.finalValue = finalValue;
            this.equipmentValue = finalValue - AdjustedValue;
        }
        else
        {
            this.equipmentValue += finalValue - AdjustedValue;
        }
        if (Type == StatType.MaxHP)
        {
            Debug.Log(Type + " " + finalValue);
        }
        return this.finalValue;
    }
}
