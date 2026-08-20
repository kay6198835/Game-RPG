using System;
using UnityEngine;

/// <summary>
/// Công thức cho MỘT chỉ số dẫn xuất, định nghĩa hoàn toàn bằng data:
///
///   BaseValue(target) = baseConstant + level × perLevel + Σ(sourceStat × coefficient)
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

    public Stat Evaluate(Func<StatType, Stat> getStat, int level)
    {
        float value = baseConstant + level * perLevel;
        float BaseValue = value;
        float LevelUpValue = value;
        float EquipmentValue = value;
        float sourceBaseValue = 0;
        float sourceLevelUpValue = 0;
        float sourceEquipmentValue = 0;
        if (contributions != null)
        {
            for (int i = 0; i < contributions.Length; i++)
            {
                BaseValue += sourceBaseValue * contributions[i].coefficient;
                sourceBaseValue = getStat(contributions[i].sourceStat).BaseValue;

                LevelUpValue += sourceLevelUpValue * contributions[i].coefficient;
                sourceLevelUpValue = getStat(contributions[i].sourceStat).LevelUpValue;
                
                EquipmentValue += sourceEquipmentValue * contributions[i].coefficient;
                sourceEquipmentValue = getStat(contributions[i].sourceStat).EquipmentValue;
            }
        }

        return new Stat(targetStat, BaseValue, LevelUpValue, EquipmentValue);
    }
}