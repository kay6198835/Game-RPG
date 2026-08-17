using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField] private List<StatModifier> modifiers = new List<StatModifier>();

    public IReadOnlyList<StatModifier> Modifiers => modifiers;

    /// <summary>Gắn toàn bộ cụm vào một StatsSO, ghi nhận nguồn là source.</summary>
    public void ApplyTo(StatsSO stats, object source)
    {
        if (stats == null)
        {
            Debug.LogWarning($"[{nameof(StatModifierGroup)}] statsSO là null.");
            return;
        }
        stats.AddModifiersFromSource(source, modifiers);
    }

    /// <summary>Gỡ mọi modifier đến từ source (gồm cụm này và mọi cụm khác cùng nguồn).</summary>
    public void RemoveFrom(StatsSO stats, object source)
    {
        if (stats == null)
        {
            Debug.LogWarning($"[{nameof(StatModifierGroup)}] statsSO là null.");
            return;
        }
        stats.RemoveModifiersFromSource(source);
    }
}
