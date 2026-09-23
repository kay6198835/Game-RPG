using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Shared max-value façade over a character's <see cref="BaseStatsSO"/>. Subclasses only decide
/// where the profile comes from (<see cref="ResolveProfile"/>) — the template-method shape of BaseCell.Setting().
/// </summary>
public abstract class StatHandlerBase<TCore> : CoreComponentBase<TCore>, IStatService where TCore : CoreBase
{
    [SerializeField] protected BaseStatsSO statsSO;
    private BaseStatsSO _profile;

    // Lazy-resolved on first real use, not Awake: the owner reference ResolveProfile() reads is set inside
    // the hub's own Awake, and Unity does not order Awake across sibling GameObjects.
    protected BaseStatsSO StatsSO => _profile != null ? _profile : (_profile = ResolveProfile());

    protected abstract BaseStatsSO ResolveProfile();

    public int GetLevel() => StatsSO.Level;
    public Dictionary<StatType, StatsViewDTO> GetFullViewStats() => StatsSO.FullStatView();
    public StatsViewDTO GetViewStat(StatType statType) => StatsSO.GetViewStat(statType);
    public Stat GetStat(StatType statType) => StatsSO.GetStat(statType);
    public float GetStatValue(StatType statType) => StatsSO.GetStatValue(statType);
    public Dictionary<StatType, float> GetFullStat() => StatsSO.FullStatsValue();
    public void RemoveModifiersFromSource(object source) => StatsSO.RemoveModifiersFromSource(source);
    public void AddModifiersFromSource(object source, IReadOnlyList<StatModifier> modifiers)
        => StatsSO.AddModifiersFromSource(source, modifiers);
}
