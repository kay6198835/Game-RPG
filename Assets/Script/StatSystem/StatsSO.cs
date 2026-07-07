using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StatsProfile", menuName = "Game/Stats Profile")]
public class StatsSO : ScriptableObject
{
    [SerializeField, Min(1)] private int level = 1;
    [SerializeField] private List<Stat> stats = new List<Stat>();          // nguồn dữ liệu: sửa trực tiếp ở Inspector
    [SerializeField] DerivedStatFormula[] statFormulas;

    private readonly Dictionary<StatType, Stat> lookup = new Dictionary<StatType, Stat>(); // index runtime, KHÔNG serialize -> Get O(1)
    private bool initialized;

    /// <summary>Bắn ra mỗi khi một StatType đổi giá trị (UI subscribe để cập nhật).</summary>
    public event Action<StatType> OnStatChanged;

    public int Level
    {
        get => level;
        set
        {
            int clamped = Mathf.Max(1, value);
            if (clamped == level) return;
            level = clamped;
            RecalculateDerived();
        }
    }

    private void OnValidate()
    {
        for (int i = 0; i < stats.Count; i++)
            stats[i]?.MarkDirty();
        RecalculateDerived();
    }

    public void Test()
    {
        foreach (var item in stats)
        {
            Debug.Log($"[StatsSO] Level changed, {item.Type} = {lookup[item.Type]}");
        }
    }

    public void Reset()
    {
        stats.Clear();
        lookup.Clear();
        foreach (StatType t in Enum.GetValues(typeof(StatType)))
        {
            Stat s = new(t, 0f);
            stats.Add(s);
            Debug.Log($"[StatsSO] Reset {t} = {s.Value}");
            lookup[t] = s;
        }
        initialized = true;
        RecalculateDerived();
        //Test();
    }

    private void OnEnable() => initialized = false;   // rebuild index khi SO nạp lại (vào/ra Play Mode)

    // ------------------------- API chính -------------------------

    /// <summary>Đọc giá trị cuối cùng của một chỉ số. O(1).</summary>
    public Stat Get(StatType type)
    {
        EnsureInitialized();
        return lookup[type] == null ? null : lookup[type];
    }

    /// <summary>Gắn một modifier (buff/trang bị) vào chỉ số.</summary>
    public void AddModifier(StatType type, StatModifier modifier)
    {
        EnsureInitialized();
        GetOrCreate(type).AddModifier(modifier);
        AfterChanged(type);
    }

    /// <summary>Gỡ mọi modifier đến từ một nguồn (tháo trang bị, hết buff).</summary>
    public void RemoveModifiersFromSource(object source)
    {
        EnsureInitialized();
        bool primaryChanged = false;
        for (int i = 0; i < stats.Count; i++)
        {
            Stat stat = stats[i];
            if (!stat.RemoveModifiersFromSource(source)) continue;

            OnStatChanged?.Invoke(stat.Type);
            if (stat.Type.IsPrimary()) primaryChanged = true;
        }
        if (primaryChanged) RecalculateDerived();
    }

    /// <summary>Cộng điểm gốc cho một primary stat (lên cấp / phân bổ điểm).</summary>
    public void AddPrimaryPoint(StatType type, float amount = 1f)
    {
        if (!type.IsPrimary())
        {
            Debug.LogWarning($"[StatsSO] {type} không phải primary stat.");
            return;
        }
        EnsureInitialized();
        GetOrCreate(type).BaseValue += amount;
        AfterChanged(type);
    }

    // ------------------------- Nội bộ -------------------------

    private void EnsureInitialized()
    {
        if (initialized) return;
        initialized = true;

        lookup.Clear();
        // Bỏ null / trùng key ngay trong list authored -> giữ bất biến list ↔ dict 1:1.
        for (int i = stats.Count - 1; i >= 0; i--)
        {
            Stat s = stats[i];
            if (s == null || lookup.ContainsKey(s.Type))
            {
                stats.RemoveAt(i);
                continue;
            }
            lookup[s.Type] = s;
        }

        // Bù các StatType còn thiếu trong enum, không thiếu chỉ số nào.
        foreach (StatType t in Enum.GetValues(typeof(StatType)))
            if (!lookup.ContainsKey(t))
            {
                Stat s = new Stat(t, 0f);
                stats.Add(s);
                lookup[t] = s;
            }

        RecalculateDerived();
    }

    private void AfterChanged(StatType type)
    {
        OnStatChanged?.Invoke(type);
        if (type.IsPrimary()) RecalculateDerived();
    }

    private float GetFun(StatType type)
    {
        return Get(type).Value;
    }

    private void RecalculateDerived()
    {
        if (statFormulas == null) return;
        foreach (var formula in statFormulas)
        {
            if (formula == null) continue;

            Stat target = GetOrCreate(formula.targetStat);
            float newBase = formula.Evaluate(GetFun, level);
            if (Mathf.Approximately(target.BaseValue, newBase)) continue;

            target.BaseValue = newBase;
            lookup[target.Type] = target;
            Debug.Log($"[StatsSO] RecalculateDerived: {target.Type} = {newBase}/ {lookup[target.Type].Value}");
            OnStatChanged?.Invoke(target.Type);
        }
    }

    private Stat GetOrCreate(StatType type)
    {
        Stat stat = Get(type);
        if (stat == null)
        {
            stat = new Stat(type, 0f);
            stats.Add(stat);
            lookup[type] = stat;
        }
        return stat;
    }
}
