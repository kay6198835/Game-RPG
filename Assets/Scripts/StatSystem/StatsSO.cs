using System;
using System.Collections.Generic;   // [1]
using UnityEngine;

[CreateAssetMenu(fileName = "StatsProfile", menuName = "Game/Stats Profile")]
public class StatsSO : ScriptableObject
{
    [Serializable]
    public struct StatEntry { public StatType type; public float baseValue; }   // [7] author primary

    [SerializeField, Min(1)] private int level = 1;
    [SerializeField] private StatEntry[] primaryBase;            // STR/DEX/INT/VIT/LUK
    [SerializeField] private DerivedStatFormula[] statFormulas;  // [3] tên thống nhất

    private readonly Dictionary<StatType, Stat> stats = new();   // [2][6] khởi tạo sẵn
    private bool initialized;

    public event Action<StatType> OnStatChanged;

    public int Level
    {
        get => level;
        set { level = Mathf.Max(1, value); RecalculateDerived(); }
    }
    
    public void Start()
    {
        Initialize();
    }
    private void Initialize()
    {
        if (initialized) return;
        initialized = true;

        // full enum -> không thiếu key (tùy chọn, đúng ý "full biến theo enum")
        foreach (var t in StatTypes.Primary) stats[t] = new Stat(0f);
        foreach (var t in StatTypes.Derived) stats[t] = new Stat(0f);

        // [7] nạp base author cho primary
        if (primaryBase != null)
            foreach (var e in primaryBase)
                GetOrCreate(e.type).BaseValue = e.baseValue;

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

    public void RemoveModifiersFromSource(object source)
    {
        Initialize();
        bool primaryChanged = false;
        foreach (var pair in stats)
            if (pair.Value.RemoveModifiersFromSource(source))
            {
                if (pair.Key.IsPrimary()) primaryChanged = true;
            }
        if (primaryChanged) RecalculateDerived();
    }

    public void AddPrimaryPoint(StatType type, float amount = 1f)
    {
        if (!type.IsPrimary())
        {
            Debug.LogWarning($"[StatsSO] {type} không phải primary stat.");   // [11]
            return;
        }
        Initialize();
        GetOrCreate(type).BaseValue += amount;
        AfterChanged(type);
    }

    // ------------------------- Nội bộ -------------------------

    private void AfterChanged(StatType type)
    {
        if (type.IsPrimary()) RecalculateDerived();
    }

    private void RecalculateDerived()
    {
        if (statFormulas == null) return;
        foreach (var formula in statFormulas)
        {
            if (formula == null) continue;
            Stat target = GetOrCreate(formula.targetStat);
            float newBase = formula.Evaluate(Get, level);    // [4][8] gán kết quả
            if (!Mathf.Approximately(target.BaseValue, newBase))
            {
                target.BaseValue = newBase;
            }
            stats[formula.targetStat] = target;
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
}