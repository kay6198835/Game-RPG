using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Authored stat profile of a character — DATA ONLY. Holds the level, the base value of each primary
/// stat, and the formula of each derived stat. It never computes or stores a runtime value: a character
/// builds a <see cref="StatSheet"/> from it, and only the sheet calculates.
/// </summary>
// NEED CHECK: 2026-11-09 (stat split) — was the whole stat system (lookup, modifiers, dirty flags,
// derived recalculation, view DTOs, events, point allocation). All of that moved to StatSheet.
// `primaryStats` was `stats` (List<Stat>, every StatType) and `derivedStats` was `statFormulas`. All 15
// profile assets were migrated in YAML (old derived rows, levelUp/final tiers, statUnusedBonus and isDevMode
// dropped; primary base values and formulas copied verbatim), so no FormerlySerializedAs is kept.
public class BaseStatsSO : ScriptableObject
{
    [SerializeField, Min(1)] private int level = 1;

    [Header("Primary stats (edit base value)")]
    [SerializeField]
    private List<PrimaryStatEntry> primaryStats = new List<PrimaryStatEntry>();

    [Header("Derived stats (value = formula of primary stats)")]
    [SerializeField]
    private DerivedStatFormula[] derivedStats = Array.Empty<DerivedStatFormula>();

    /// <summary>Profile level, never below 1 (old assets can store 0).</summary>
    public int Level => Mathf.Max(1, level);

    public IReadOnlyList<PrimaryStatEntry> PrimaryStats => primaryStats;
    public IReadOnlyList<DerivedStatFormula> DerivedStats => derivedStats ?? Array.Empty<DerivedStatFormula>();

    /// <summary>Authored base value of a primary stat; 0 when the row is missing.</summary>
    public float GetPrimaryBase(StatType type)
    {
        for (int i = 0; i < primaryStats.Count; i++)
        {
            PrimaryStatEntry entry = primaryStats[i];
            if (entry != null && entry.Type == type) return entry.BaseValue;
        }
        return 0f;
    }

    /// <summary>Formula of a derived stat; null when the asset has none (the stat then reads 0).</summary>
    public DerivedStatFormula GetFormula(StatType type)
    {
        if (derivedStats == null) return null;
        for (int i = 0; i < derivedStats.Length; i++)
        {
            DerivedStatFormula formula = derivedStats[i];
            if (formula != null && formula.targetStat == type) return formula;
        }
        return null;
    }

    // Data cleanup only, no calculation: keep exactly one row per primary stat, in enum order.
    private void OnValidate()
    {
        var kept = new List<PrimaryStatEntry>();
        foreach (StatType type in Enum.GetValues(typeof(StatType)))
        {
            if (!type.IsPrimary()) continue;
            PrimaryStatEntry found = null;
            for (int i = 0; i < primaryStats.Count; i++)
            {
                if (primaryStats[i] != null && primaryStats[i].Type == type) { found = primaryStats[i]; break; }
            }
            kept.Add(found ?? new PrimaryStatEntry(type));
        }
        if (!SameRows(kept)) primaryStats = kept;
    }

    private bool SameRows(List<PrimaryStatEntry> kept)
    {
        if (kept.Count != primaryStats.Count) return false;
        for (int i = 0; i < kept.Count; i++)
        {
            if (!ReferenceEquals(kept[i], primaryStats[i])) return false;
        }
        return true;
    }
}
