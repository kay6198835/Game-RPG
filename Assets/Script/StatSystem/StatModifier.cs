using System;
using UnityEngine;

/// <summary>
/// Thứ tự áp dụng: Flat → PercentAdd (cộng dồn rồi nhân 1 lần) → PercentMult (nhân riêng từng cái).
/// Giá trị enum cũng chính là Order.
/// </summary>
[System.Serializable]
public enum ModifierType
{
    Flat = 100,  // +20 Damage
    PercentAdd = 200,  // +10% (0.10f) — các PercentAdd cộng dồn với nhau trước khi nhân
    PercentMult = 300,  // x(1+15%) — nhân riêng, dùng cho buff đặc biệt
}

/// <summary>
/// Một bonus gắn vào Stat. Đóng hai vai:
/// - Authored: targetStat / type / value serialize được -> author trực tiếp trong Inspector
///   (StatModifierGroupSO: trang bị, buff, thẻ nâng cấp).
/// - Runtime: Source đóng dấu lúc apply, KHÔNG serialize, dùng để gỡ hàng loạt theo nguồn.
///
/// Asset là dữ liệu dùng chung, nên KHÔNG gắn thẳng instance authored vào Stat:
/// luôn đi qua WithSource() để nhân bản rồi mới đóng dấu nguồn.
/// </summary>
[System.Serializable]
public sealed class StatModifier
{
    [SerializeField] private StatType targetStat;
    [SerializeField] private ModifierType type;
    [SerializeField] private float value;

    [NonSerialized] private object source;

    /// <summary>Chỉ số mà modifier này gắn vào.</summary>
    public StatType TargetStat => targetStat;
    public ModifierType Type => type;
    public float Value => value;

    /// <summary>
    /// Khóa sắp xếp trong danh sách modifier của Stat. Luôn suy ra từ Type, không author được:
    /// Stat.CalculateFinalValue gom các PercentAdd LIÊN TIẾP dựa trên list đã sort theo Order,
    /// nên một Order tự đặt sẽ làm sai kết quả mà không báo lỗi.
    /// </summary>
    public int Order => (int)type;

    /// <summary>Nguồn tạo ra modifier (item, buff, passive...); StatsSO gỡ theo nguồn bằng ReferenceEquals.</summary>
    public object Source => source;

    public StatModifier() { }   // Unity deserialization

    public StatModifier(StatType targetStat, float value, ModifierType type, object source = null)
    {
        this.targetStat = targetStat;
        this.value = value;
        this.type = type;
        this.source = source;
    }

    /// <summary>Bản sao cùng dữ liệu nhưng đóng dấu nguồn mới (dùng khi apply một asset dùng chung).</summary>
    public StatModifier WithSource(object newSource) => new StatModifier(targetStat, value, type, newSource);
}
