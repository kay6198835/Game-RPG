using System;
using System.Collections.Generic;
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
    [FormerlySerializedAs("modifiers")]
    private List<StatModifier> authoredModifiers = new List<StatModifier>();

    public IReadOnlyList<StatModifier> Modifiers => authoredModifiers;

    /// <summary>Gắn toàn bộ cụm vào một StatsSO, ghi nhận nguồn là source.</summary>
    public void ApplyTo(BaseStatsSO stats, object source)
    {
        if (stats == null)
        {
            Debug.LogWarning($"[{nameof(StatModifierGroup)}] statsSO là null.");
            return;
        }
        stats.AddModifiersFromSource(source, authoredModifiers);
    }
    public void Apply(Action<object, IReadOnlyList<StatModifier>> action, object source)
    {
        action(source, Modifiers);
    }
    public void Remmove(Action<object> action, object source)
    {
        action(source);
    }

    /// <summary>Gỡ mọi modifier đến từ source (gồm cụm này và mọi cụm khác cùng nguồn).</summary>
    public void RemoveFrom(BaseStatsSO stats, object source)
    {
        if (stats == null)
        {
            Debug.LogWarning($"[{nameof(StatModifierGroup)}] statsSO là null.");
            return;
        }
        stats.RemoveModifiersFromSource(source);
    }
}
