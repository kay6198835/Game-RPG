using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Một chỉ số đơn lẻ: StatType + các tầng giá trị + danh sách modifier runtime.
///
/// Hai tầng authored (serialize, sửa được ở Inspector):
///   BaseValue       — giá trị gốc của chỉ số
///   LevelUpValue    — cộng dồn từ lên cấp / phân bổ điểm
///
/// Một tầng ĐẦU VÀO do công thức ghi (không serialize, không author bằng tay):
///   EquipmentValue  — phần lan truyền từ primary stat qua DerivedStatFormula.
///                     Với primary stat luôn bằng 0: bonus của primary nằm trong modifiers.
///
/// Các giá trị DẪN XUẤT (tính ra, không lưu):
///   AdjustedValue = BaseValue + LevelUpValue                    (KHÔNG gồm EquipmentValue)
///   FinalValue    = (AdjustedValue + EquipmentValue + ΣFlat) × (1 + ΣPercentAdd) × Π(1 + PercentMult)
///   BonusValue    = FinalValue − AdjustedValue
///
/// HAI KÊNH ĐẦU VÀO TÁCH BIỆT, không kênh nào ghi đè kênh nào:
///   equipmentValue  chỉ StatsSO.RecalculateDerived() ghi; CalculateFinalValue() chỉ ĐỌC.
///   modifiers       chỉ AddModifier/RemoveModifier ghi; RecalculateDerived() không đụng tới.
/// Nhờ vậy modifier gắn thẳng vào một derived stat vẫn sống sót qua mỗi lần recalc công thức.
///
/// BonusValue BẮT BUỘC tính bằng hiệu, KHÔNG cộng dồn riêng trong vòng lặp modifier.
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
/// cachedValue và equipmentValue cố tình KHÔNG serialize: chúng là kết quả tính, nếu lưu
/// xuống .asset thì file tự thay đổi sau mỗi Play Mode.
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

    // KHÔNG serialize: đây là cache, không phải dữ liệu authored. Nếu serialize thì mọi lần
    // tính lại đều ghi vào .asset -> file tự đổi sau mỗi Play Mode. isDirty khởi tạo true để
    // lần đọc Value đầu tiên sau khi load luôn tính lại, thay vì tin vào cache đã lưu.
    [NonSerialized] private float cachedValue;
    [NonSerialized] private bool isDirty = true;

    [SerializeField] private List<StatModifier> modifiers = new List<StatModifier>();

    /// <summary>Bắn ra khi một tầng giá trị hoặc modifier thay đổi.</summary>
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

    /// <summary>
    /// Phần lan truyền từ primary stat qua công thức — ĐẦU VÀO của CalculateFinalValue,
    /// chỉ StatsSO.RecalculateDerived() được ghi. Với primary stat luôn là 0.
    /// Muốn biết tổng bonus hiển thị cho người chơi thì đọc BonusValue, không phải field này.
    /// </summary>
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

    // ------------------------- Modifier -------------------------

    public void AddModifier(StatModifier modifier)
    {
        if (modifier == null) return;
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

    private float CalculateFinalValue()
    {
        float value = AdjustedValue + equipmentValue;
        if (modifiers == null) return value;                // trả AdjustedValue, KHÔNG phải 0

        float percentAddSum = 0f;
        for (int i = 0; i < modifiers.Count; i++)
        {
            StatModifier mod = modifiers[i];
            switch (mod.Type)
            {
                case ModifierType.Flat:
                    value += mod.Value;
                    break;

                case ModifierType.PercentAdd:
                    percentAddSum += mod.Value;
                    // Cộng dồn hết các PercentAdd liên tiếp rồi mới nhân 1 lần
                    bool isLastPercentAdd = i + 1 >= modifiers.Count
                        || modifiers[i + 1].Type != ModifierType.PercentAdd;
                    if (isLastPercentAdd)
                    {
                        value *= 1f + percentAddSum;
                        percentAddSum = 0f;
                    }
                    break;

                case ModifierType.PercentMult:
                    value *= 1f + mod.Value;
                    break;
            }
        }
        return value;
    }
}
