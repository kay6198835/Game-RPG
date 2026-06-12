using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Hub chỉ số của một nhân vật — player lẫn enemy đều dùng chung component này.
///
/// Luồng dữ liệu:
///   Primary BaseValue (+ modifier từ đồ/buff)
///     → DerivedStatFormula tính ra BaseValue của derived
///       → Derived (+ modifier trực tiếp lên derived, vd "+10% AttackSpeed")
///         → Value cuối cùng mà Combat/UI đọc qua Get().
///
/// Combat system CHỈ gọi Get(StatType), không bao giờ tự tính công thức.
/// </summary>
public class CharacterStats : MonoBehaviour
{
    [Serializable]
    private struct PrimaryStatEntry
    {
        public StatType type;
        public float baseValue;
    }

    [SerializeField, Min(1)] private int level = 1;

    [SerializeField]
    private PrimaryStatEntry[] primaryStats =
    {
            new PrimaryStatEntry { type = StatType.STR, baseValue = 5 },
            new PrimaryStatEntry { type = StatType.DEX, baseValue = 5 },
            new PrimaryStatEntry { type = StatType.INT, baseValue = 5 },
            new PrimaryStatEntry { type = StatType.VIT, baseValue = 5 },
            new PrimaryStatEntry { type = StatType.LUK, baseValue = 1 },
        };

    [Tooltip("Mỗi derived stat = 1 asset công thức. Kéo thả toàn bộ vào đây.")]
    [SerializeField] private DerivedStatFormula[] formulas;

    private readonly Dictionary<StatType, Stat> stats = new Dictionary<StatType, Stat>();
    private bool initialized;

    /// <summary>UI / Combat subscribe vào đây để biết stat nào vừa đổi.</summary>
    public event Action<StatType> OnStatChanged;

    public int Level
    {
        get => level;
        set
        {
            level = Mathf.Max(1, value);
            RecalculateDerived();
        }
    }

    private void Awake() => Initialize();

    private void Initialize()
    {
        if (initialized) return;
        initialized = true;

        foreach (var entry in primaryStats)
            stats[entry.type] = new Stat(entry.baseValue);

        if (formulas != null)
        {
            foreach (var formula in formulas)
                if (formula != null && !stats.ContainsKey(formula.targetStat))
                    stats[formula.targetStat] = new Stat(0f);
        }

        RecalculateDerived();
    }

    // ------------------------- API chính -------------------------

    public float Get(StatType type)
    {
        Initialize();
        return stats.TryGetValue(type, out Stat stat) ? stat.Value : 0f;
    }

    public void AddModifier(StatType type, StatModifier modifier)
    {
        Initialize();
        GetOrCreate(type).AddModifier(modifier);
        AfterChanged(type);
    }

    /// <summary>Gỡ toàn bộ modifier từ một nguồn trên MỌI stat (tháo đồ, hết buff).</summary>
    public void RemoveModifiersFromSource(object source)
    {
        Initialize();
        bool primaryChanged = false;

        foreach (var pair in stats)
        {
            if (pair.Value.RemoveModifiersFromSource(source))
            {
                if (pair.Key.IsPrimary()) primaryChanged = true;
                OnStatChanged?.Invoke(pair.Key);
            }
        }

        if (primaryChanged) RecalculateDerived();
    }

    /// <summary>Cộng điểm primary (khi lên level, dùng item tăng vĩnh viễn...).</summary>
    public void AddPrimaryPoint(StatType type, float amount = 1f)
    {
        if (!type.IsPrimary())
        {
            Debug.LogWarning($"[CharacterStats] {type} không phải primary stat, không thể cộng điểm trực tiếp.");
            return;
        }
        Initialize();
        GetOrCreate(type).BaseValue += amount;
        AfterChanged(type);
    }

    // ------------------------- Nội bộ -------------------------

    private void AfterChanged(StatType type)
    {
        // Primary đổi → mọi derived phụ thuộc phải tính lại BaseValue
        if (type.IsPrimary()) RecalculateDerived();
        OnStatChanged?.Invoke(type);
    }

    private void RecalculateDerived()
    {
        if (formulas == null) return;

        foreach (var formula in formulas)
        {
            if (formula == null) continue;

            Stat target = GetOrCreate(formula.targetStat);
            float newBase = formula.Evaluate(Get, level);

            if (!Mathf.Approximately(target.BaseValue, newBase))
            {
                target.BaseValue = newBase;
                OnStatChanged?.Invoke(formula.targetStat);
            }
        }
    }

    private Stat GetOrCreate(StatType type)
    {
        if (!stats.TryGetValue(type, out Stat stat))
        {
            stat = new Stat(0f);
            stats[type] = stat;
        }
        return stat;
    }

#if UNITY_EDITOR
    [ContextMenu("Log All Stats")]
    private void LogAllStats()
    {
        Initialize();
        foreach (var pair in stats)
            Debug.Log($"{pair.Key}: {pair.Value.Value:0.##} (base {pair.Value.BaseValue:0.##})");
    }
#endif
}

