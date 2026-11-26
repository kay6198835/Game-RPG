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
///   (StatModifierGroup: trang bị, buff, thẻ nâng cấp — class thuần nhúng trong WeaponStats,
///    KHÔNG phải ScriptableObject; xem ADR-0001).
/// - Runtime: Source đóng dấu lúc apply, KHÔNG serialize, dùng để gỡ hàng loạt theo nguồn.
///
/// Asset là dữ liệu dùng chung, nên KHÔNG gắn thẳng instance authored vào Stat:
/// luôn đi qua WithSource() để nhân bản rồi mới đóng dấu nguồn.
/// </summary>
[System.Serializable]
public sealed class StatModifier
{
    [SerializeField] private StatType targetStatType;
    [SerializeField] private ModifierType type;
    // NEED CHECK: 2026-11-26 (A1) — value is a magnitude only (MinValue..MaxValue). The direction is impactType:
    // every reader of Value checks ImpactType itself (Reduction = subtract, Recovery = add).
    // Design note (GDD AB-E12): a Reduction PercentAdd / PercentMult above 1 is not blocked — PercentMult 2 gives
    // x(1-2) = x(-1); a PercentAdd sum above 1 drives the stat below 0. Keeping it in 0.1..1 is the designer's job.
    public const float MinValue = 0.1f;
    public const float MaxValue = 999f;
    [SerializeField, Range(MinValue, MaxValue)] private float value;
    // Serialized again (it was not, so every asset loaded as Recovery and a Reduction healed instead).
    // Assets saved before this have no key and load as Recovery (enum 0), which matches all of them today.
    [SerializeField] private StatImpactType impactType;
    [NonSerialized] private object source;

    /// <summary>Chỉ số mà modifier này gắn vào.</summary>
    public StatType TargetStatType => targetStatType;
    public ModifierType Type => type;
    public StatImpactType ImpactType => impactType;
    /// <summary>Magnitude, never negative. Use <see cref="ImpactType"/> for the sign.</summary>
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

    /// <param name="value">Magnitude: the sign is dropped and the result is clamped to <see cref="MinValue"/>..<see cref="MaxValue"/>.</param>
    /// <param name="impactType">Direction: Reduction subtracts, Recovery adds.</param>
    public StatModifier(StatType targetStat, float value, ModifierType type, StatImpactType impactType, object source = null)
    {
        this.targetStatType = targetStat;
        this.value = Mathf.Clamp(Math.Abs(value), MinValue, MaxValue);
        this.type = type;
        this.impactType = impactType;
        this.source = source;
    }
    public void SetType(ModifierType updateType) => type = updateType;
    /// <summary>Bản sao cùng dữ liệu nhưng đóng dấu nguồn mới (dùng khi apply một asset dùng chung).</summary>
    public StatModifier WithSource(object newSource) => new StatModifier(targetStatType, value, type, impactType, newSource);
}