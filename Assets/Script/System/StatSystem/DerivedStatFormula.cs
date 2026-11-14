using System;
using UnityEngine;

/// <summary>
/// Công thức cho MỘT chỉ số dẫn xuất, định nghĩa hoàn toàn bằng data:
///
///   value(target) = baseConstant + level × perLevel + Σ(sourceStat × coefficient)
///
/// Balance game = chỉnh số trong Inspector, không đụng vào code.
/// LƯU Ý: contributions chỉ nên tham chiếu PRIMARY stat để tránh phụ thuộc vòng
/// giữa các derived stat với nhau.
/// </summary>
// NEED CHECK: 2026-11-09 (stat split) — Evaluate() was removed; the formula is pure data now and is
// applied in one place, StatSheet.RecalculateAll().
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
}
