using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Shared max-value façade over a character's <see cref="BaseStatsSO"/>. Every instance works on its own
/// runtime clone, so modifiers never reach the authored asset or another character sharing it.
/// </summary>
public abstract class StatHandlerBase<TCore> : CoreComponentBase<TCore>, IStatService<TCore> where TCore : CoreBase
{
    private BaseStatsSO _runtime;
    // Lazy-created on first real use, not Awake: Core.Data is set inside the hub's own Awake, and Unity
    // does not order Awake across sibling GameObjects.
    protected BaseStatsSO StatsSO => _runtime != null ? _runtime : (_runtime = CreateRuntimeProfile());
        /// <summary>Re-raised from the current clone, so subscribers survive a clone swap.</summary>
    public event Action<StatType> StatChanged;

    protected virtual BaseStatsSO CreateRuntimeProfile()
    {
        BaseStatsSO clone = Core.Data?.CreateRuntimeStats();
        if (clone == null) return null;
        clone.OnStatChanged += RelayStatChanged;
        return clone;
    }

    private void RelayStatChanged(StatType statType) => StatChanged?.Invoke(statType);

    private void ReleaseRuntime()
    {
        if (_runtime == null) return;
        _runtime.OnStatChanged -= RelayStatChanged;
        Destroy(_runtime);
        _runtime = null;
    }

    public void ResignStatsChange(Action<StatType> action) => StatChanged += action;
    public void UnResignStatsChange(Action<StatType> action) => StatChanged -= action;

    public int GetLevel() => StatsSO.Level;
    public Dictionary<StatType, StatsViewDTO> GetFullViewStats() => StatsSO.FullStatView();
    public StatsViewDTO GetViewStat(StatType statType) => StatsSO.GetViewStat(statType);
    public Stat GetStat(StatType statType) => StatsSO.GetStat(statType);
    public float GetStatValue(StatType statType) => StatsSO.GetStatValue(statType);
    public Dictionary<StatType, float> GetFullStat() => StatsSO.FullStatsValue();
    public void RemoveModifiersFromSource(object source) => StatsSO.RemoveModifiersFromSource(source);
    public void AddModifiersFromSource(object source, IReadOnlyList<StatModifier> modifiers)
        => StatsSO.AddModifiersFromSource(source, modifiers);

    /// <summary>Drops buffs/debuffs left on this instance's clone, e.g. a timed debuff whose removal
    /// coroutine was stopped when the pool disabled the character.</summary>
    public void ResetRuntimeModifiers() => StatsSO?.ClearRuntimeModifiers();

    // Throws the clone away; the next read builds a fresh one from the (possibly new) CharacterData.
    public override void ReloadComponent()
    {
        base.ReloadComponent();
        ReleaseRuntime();
    }

    protected virtual void OnDestroy()
    {
        ReleaseRuntime();
    }
}
