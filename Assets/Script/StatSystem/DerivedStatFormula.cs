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

    public float Evaluate(Func<StatType, Stat> getStat, int level)
    {
        float value = baseConstant + level * perLevel;

        if (contributions != null)
        {
            for (int i = 0; i < contributions.Length; i++)
            {
                Stat source = getStat(contributions[i].sourceStat);
                Debug.Log(contributions[i].sourceStat);
                Debug.Log($"[DerivedStatFormula] Evaluate: {source.Value} * {contributions[i].coefficient} = {source.Value * contributions[i].coefficient}");
                value += source.Value * contributions[i].coefficient;
            }
        }
        return value;
    }
}