using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Một "cục" modifier authored sẵn: trang bị, buff, thẻ nâng cấp per-run.
/// Gắn/gỡ cả cụm bằng một lời gọi thay vì AddModifier từng cái.
///
/// QUAN TRỌNG — source phải là reference ổn định của thứ ĐANG SỞ HỮU buff
/// (instance item đang trang bị, MonoBehaviour, instance ability), KHÔNG phải chính asset này:
/// StatsSO gỡ modifier bằng ReferenceEquals, nên nếu lấy asset làm source thì hai bản sao
/// cùng asset sẽ không tháo độc lập được.
/// </summary>
[System.Serializable]
public class StatModifierGroup
{
    // Dữ liệu do designer author — BẮT BUỘC serialize. Đây là điểm khác biệt then chốt với
    // Stat.modifiers (danh sách runtime, không được serialize) — xem comment ở Stat.cs.
    // Đổi tên `modifiers` -> `authoredModifiers` (2026-08-21) để hai thứ không còn trùng tên;
    // FormerlySerializedAs giữ nguyên dữ liệu đã author trong SnS_Stat.asset.
    [SerializeField]
    private List<StatModifier> statsModifiers = new List<StatModifier>();

    public IReadOnlyList<StatModifier> GetStatsModifiers => statsModifiers;
    [SerializeField]
    private List<VitalModifier> vitalModifiers = new List<VitalModifier>();

    public IReadOnlyList<VitalModifier> GetVitalModifiers => vitalModifiers;

    // [SerializeField] private StatModifierGroupType groupType = StatModifierGroupType.None;

    // public void FilterByGroupType()
    // {
    //     if (groupType == StatModifierGroupType.Primary)
    //     {
    //         PrimaryFilter();
    //     }
    //     else if (groupType == StatModifierGroupType.Derived)
    //     {
    //         DerivedFilter();
    //     }
    // }
    // public IReadOnlyList<StatModifier> PrimaryFilter()
    // {
    //     modifiers = modifiers.Where(x => x.TargetStatType.IsPrimary() == true).ToList();
    //     return modifiers;
    // }
    // public IReadOnlyList<StatModifier> DerivedFilter()
    // {
    //     modifiers = modifiers.Where(x => x.TargetStatType.IsPrimary() == false).ToList();
    //     return modifiers;
    // }
}

// public enum StatModifierGroupType
// {
//     None,
//     Primary,
//     Derived
// }
