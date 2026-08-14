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
[CreateAssetMenu(fileName = "StatModifierGroup", menuName = "Game/Stat Modifier Group")]
public class StatModifierGroupSO : ScriptableObject
{
    [SerializeField] private List<StatModifier> modifiers = new List<StatModifier>();

    public IReadOnlyList<StatModifier> Modifiers => modifiers;

    /// <summary>Gắn toàn bộ cụm vào một StatsSO, ghi nhận nguồn là source.</summary>
    public void ApplyTo(StatsSO stats, object source)
    {
        if (stats == null)
        {
            Debug.LogWarning($"[{nameof(StatModifierGroupSO)}] statsSO là null.", this);
            return;
        }
        stats.AddModifiersFromSource(source, modifiers);
    }

    /// <summary>Gỡ mọi modifier đến từ source (gồm cụm này và mọi cụm khác cùng nguồn).</summary>
    public void RemoveFrom(StatsSO stats, object source)
    {
        if (stats == null)
        {
            Debug.LogWarning($"[{nameof(StatModifierGroupSO)}] statsSO là null.", this);
            return;
        }
        stats.RemoveModifiersFromSource(source);
    }
}
