using System;
using UnityEngine;

/// <summary>
/// Kết quả một lần Evaluate: ba tầng của derived stat, tách riêng để UI dựng được breakdown.
/// Là struct để RecalculateDerived (chạy mỗi lần primary đổi) không cấp phát gì.
/// </summary>
public readonly struct DerivedTiers
{
    public readonly float BaseValue;
    public readonly float LevelUpValue;
    public readonly float EquipmentValue;

    public DerivedTiers(float baseValue, float levelUpValue, float equipmentValue)
    {
        BaseValue = baseValue;
        LevelUpValue = levelUpValue;
        EquipmentValue = equipmentValue;
    }
}

/// <summary>
/// Công thức cho MỘT chỉ số dẫn xuất, định nghĩa hoàn toàn bằng data:
///
///   BaseValue(target)      = baseConstant   + Σ(source.BaseValue    × coefficient)
///   LevelUpValue(target)   = level×perLevel + Σ(source.LevelUpValue × coefficient)
///   EquipmentValue(target) =                  Σ(source.BonusValue   × coefficient)
///
/// Balance game = chỉnh số trong Inspector, không đụng vào code.
/// LƯU Ý: contributions chỉ nên tham chiếu PRIMARY stat để tránh phụ thuộc vòng
/// giữa các derived stat với nhau.
/// </summary>
[Serializable]
public class DerivedStatFormula
{
    [Serializable]
    public struct StatContribution
    {
        public StatType sourceStat;
        public float coefficient;
    }

    public StatType targetStat;

    [Header("Thành phần cố định")]
    public float baseConstant;
    public float perLevel;

    [Header("Đóng góp từ primary stats")]
    public StatContribution[] contributions;

    public DerivedTiers Evaluate(Func<StatType, Stat> getStat, int level)
    {
        float baseValue = baseConstant;
        float levelUpValue = level * perLevel;
        float equipmentValue = 0f;

        if (contributions != null)
        {
            for (int i = 0; i < contributions.Length; i++)
            {
                Stat source = getStat(contributions[i].sourceStat);
                if (source == null) continue;

                float coefficient = contributions[i].coefficient;
                baseValue += source.BaseValue * coefficient;
                levelUpValue += source.LevelUpValue * coefficient;

                // Đọc BonusValue, KHÔNG phải EquipmentValue: bonus của một primary stat nằm
                // trong list modifier của nó, còn EquipmentValue của primary luôn bằng 0.
                equipmentValue += source.BonusValue * coefficient;
            }
        }

        return new DerivedTiers(baseValue, levelUpValue, equipmentValue);
    }
}
