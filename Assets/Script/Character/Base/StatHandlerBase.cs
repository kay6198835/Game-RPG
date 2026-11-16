using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Shared max-value façade of a character. Owns one <see cref="StatSheet"/> built from
/// <c>Core.Data.Stats</c>; every <see cref="IStatService"/> call goes to that sheet, so the authored
/// asset is never written and each character has its own values.
/// </summary>
// NEED CHECK: 2026-11-09 (stat split) — was an Instantiate()d BaseStatsSO clone, destroyed on reload.
public abstract class StatHandlerBase<TCore> : CoreComponentBase<TCore>, IStatService<TCore> where TCore : CoreBase
{
    private StatSheet _sheet;

    // Lazy, not Awake: Core.Data is set inside the hub's own Awake, and Unity does not order Awake across
    // sibling GameObjects. Not cached while there is no profile, so a later read can still build it.
    protected StatSheet Sheet
    {
        get
        {
            if (_sheet != null) return _sheet;
            BaseStatsSO profile = Core.Data != null ? Core.Data.Stats : null;
            if (profile == null) return null;
            _sheet = new StatSheet(profile);
            _sheet.Changed += RelayStatChanged;
            RefreshDebugView();
            return _sheet;
        }
    }

    /// <summary>Re-raised from the current sheet, so subscribers survive a sheet rebuild.</summary>
    public event Action<StatType> StatChanged;

#if UNITY_EDITOR
    [Serializable]
    private struct StatDebugRow
    {
        public string name;
        public float baseValue;
        public float levelUp;
        public float adjusted;
        public float primaryBonus;
        public float modifierBonus;
        public float final;
    }

    [Header("Debug (read only — refreshed on every recalculation)")]
    [SerializeField] private List<StatDebugRow> debugView = new List<StatDebugRow>();
#endif

    private void RelayStatChanged(StatType statType)
    {
        RefreshDebugView();
        StatChanged?.Invoke(statType);
    }

    private void RefreshDebugView()
    {
#if UNITY_EDITOR
        debugView.Clear();
        if (_sheet == null) return;
        foreach (Stat stat in _sheet.All)
        {
            debugView.Add(new StatDebugRow
            {
                name = stat.Type.ToString(),
                baseValue = stat.BaseValue,
                levelUp = stat.LevelUpValue,
                adjusted = stat.AdjustedValue,
                primaryBonus = stat.PrimaryBonus,
                modifierBonus = stat.ModifierBonus,
                final = stat.FinalValue,
            });
        }
#endif
    }

    private void ReleaseSheet()
    {
        if (_sheet == null) return;
        _sheet.Changed -= RelayStatChanged;
        _sheet = null;
        RefreshDebugView();
    }

    public void ResignStatsChange(Action<StatType> action) => StatChanged += action;
    public void UnResignStatsChange(Action<StatType> action) => StatChanged -= action;

    public int GetLevel() => Sheet != null ? Sheet.GetLevel() : 1;
    public Dictionary<StatType, StatsViewDTO> GetFullViewStats() => Sheet?.GetFullViewStats();
    public StatsViewDTO GetViewStat(StatType statType) => Sheet?.GetViewStat(statType);
    public Stat GetStat(StatType statType) => Sheet?.GetStat(statType);
    public float GetStatValue(StatType statType) => Sheet != null ? Sheet.GetStatValue(statType) : 0f;
    public Dictionary<StatType, float> GetFullStat() => Sheet?.GetFullStat();
    public void RemoveModifiersFromSource(object source) => Sheet?.RemoveModifiersFromSource(source);
    public void AddModifiersFromSource(object source, IReadOnlyList<StatModifier> modifiers)
        => Sheet?.AddModifiersFromSource(source, modifiers);

    /// <summary>Drops buffs/debuffs left on this character, e.g. a timed debuff whose removal
    /// coroutine was stopped when the pool disabled the character.</summary>
    public void ResetRuntimeModifiers() => Sheet?.ClearModifiers();

    // Throws the sheet away; the next read builds a fresh one from the (possibly new) CharacterData.
    public override void ReloadComponent()
    {
        base.ReloadComponent();
        ReleaseSheet();
    }

    protected virtual void OnDestroy()
    {
        ReleaseSheet();
    }
}
