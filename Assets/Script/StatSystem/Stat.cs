using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Một chỉ số đơn lẻ: StatType + hai tầng giá trị authored + danh sách modifier runtime.
///
/// Hai tầng authored (serialize, sửa được ở Inspector):
///   BaseValue     — giá trị gốc của chỉ số
///   LevelUpValue  — cộng dồn từ lên cấp / phân bổ điểm
///
/// Ba giá trị dẫn xuất (KHÔNG serialize — tính ra, không author được):
///   AdjustedValue  = BaseValue + LevelUpValue
///   FinalValue     = (AdjustedValue + ΣFlat) × (1 + ΣPercentAdd) × Π(1 + PercentMult)
///   EquipmentValue = FinalValue − AdjustedValue   (đóng góp của trang bị và buff)
///
/// Trang bị tác động qua StatModifier (xem StatModifierGroup), không qua một field nhập tay.
///
/// EquipmentValue BẮT BUỘC tính bằng hiệu, không cộng dồn riêng trong vòng lặp modifier.
/// Đặt A = AdjustedValue, F = ΣFlat, p = 1 + phần trăm thì đóng góp E phải thoả
/// A + E = (A + F) × p, tức E = (A + F) × p − A — trong E có A. Vòng lặp chỉ nhìn thấy
/// modifiers nên không có hằng số nào ('0', '1', ...) thay được A: gieo bằng A rồi trừ ra
/// ở cuối là cách duy nhất đúng.
///
/// Value chỉ tính lại khi có thay đổi (dirty flag), KHÔNG tính lại mỗi frame.
///
/// LƯU Ý serialization: Unity ghi thẳng vào FIELD, không bao giờ đi qua property setter.
/// Sửa ở Inspector / Undo / prefab revert / deserialize đều KHÔNG gọi SetDirty() — vì vậy
/// StatsSO.OnValidate() phải gọi MarkDirty() cho từng Stat. Đừng bỏ hook đó.
///
/// CHỈ hai field authored được serialize. cachedValue, equipmentValue và modifiers cố tình
/// KHÔNG serialize: chúng là kết quả tính / trạng thái runtime, nếu lưu xuống .asset thì file
/// tự thay đổi sau mỗi Play Mode.
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

    // KHÔNG serialize: đều là kết quả tính, không phải dữ liệu authored. Nếu serialize thì mọi
    // lần tính lại đều ghi vào .asset -> file tự đổi sau mỗi Play Mode. isDirty khởi tạo true để
    // lần đọc Value đầu tiên sau khi load luôn tính lại, thay vì tin vào cache đã lưu.
    [NonSerialized] private float cachedValue;
    [NonSerialized] private float equipmentValue;
    [NonSerialized] private bool isDirty = true;

    // BẮT BUỘC [NonSerialized]: StatModifier nay đã Unity-serializable, mà Stat nằm trong
    // StatsSO.stats -> nếu để serialize, buff runtime sẽ ghi thẳng vào .asset và sống dai
    // qua các phiên Play Mode.
    [NonSerialized] private List<StatModifier> modifiers = new List<StatModifier>();

    /// <summary>Bắn ra khi một trong hai tầng authored hoặc modifier thay đổi.</summary>
    public event Action OnChanged;

    /// <summary>Buộc tính lại Value ở lần đọc kế tiếp (dùng sau khi sửa field trong Inspector).</summary>
    public void MarkDirty() => SetDirty();

    public Stat() { }   // Unity deserialization

    public Stat(StatType type, float baseValue = 0f, float levelUpValue = 0f)
    {
        Type = type;
        this.baseValue = baseValue;
        this.levelUpValue = levelUpValue;
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

    // ------------------------- Tầng dẫn xuất -------------------------

    /// <summary>Gốc + lên cấp, CHƯA tính modifier. Không serialize: đây là giá trị tính ra.</summary>
    public float AdjustedValue => baseValue + levelUpValue;

    /// <summary>Giá trị cuối cùng sau khi áp dụng toàn bộ modifier (trang bị, buff).</summary>
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

    /// <summary>
    /// Phần chênh do trang bị và buff tạo ra. CHỈ ĐỌC: đây là kết quả tính, không gán được —
    /// CalculateFinalValue() ghi đè nó ở mỗi lần recalc nên mọi giá trị gán từ ngoài đều bị nuốt.
    /// Đọc Value trước để chắc chắn equipmentValue đã ứng với lần recalc gần nhất.
    /// </summary>
    public float EquipmentValue
    {
        get
        {
            _ = Value;
            return equipmentValue;
        }
    }

    /// <summary>Bằng đúng EquipmentValue — giữ tên cũ cho UI hiển thị "+12" màu xanh.</summary>
    public float BonusValue => EquipmentValue;

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
    }

    // Gieo bằng AdjustedValue, KHÔNG phải 0 hay 1: modifier phần trăm nhân vào toàn bộ giá trị,
    // nên nếu bắt đầu từ hằng số thì "+20%" ra một con số cố định bất kể stat đang là 31 hay 5000.
    // Đóng góp của modifier bóc ra bằng phép trừ ở cuối — xem phần chứng minh ở doc của class.
    private float CalculateFinalValue()
    {
        float finalValue = AdjustedValue;
        if (modifiers == null)
        {
            equipmentValue = 0f;
            return finalValue;
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

        equipmentValue = finalValue - AdjustedValue;
        return finalValue;
    }
}
