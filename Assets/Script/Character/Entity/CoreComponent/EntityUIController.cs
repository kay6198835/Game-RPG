using UnityEngine;
using UnityEngine.UI;
public class EntityUIController : EntityCoreComponent<EntityCore>
{
    [SerializeField] private Slider slider;
    // NEED CHECK: was concrete EntityVitalStats / EntityStatsHandler fields, resolved but never read.
    private IVitalComponent<EntityCore> vital;
    private IStatService<EntityCore> stats;

    // NEED CHECK: new 2026-11-04 — the bar redraws itself on current-HP changes (damage, heal, reload) and on
    // max-stat changes, instead of only when EntityNegativeReciver pushed a value after a hit.
    protected override void Start()
    {
        base.Start();
        Core.TryGetCapability(out vital);
        Core.TryGetCapability(out stats);
        if (vital != null) vital.CurrentStatsChanged += RefreshHealth;
        stats?.ResignStatsChange(OnMaxStatChanged);
        RefreshHealth();
    }

    void OnDestroy()
    {
        if (vital != null) vital.CurrentStatsChanged -= RefreshHealth;
        stats?.UnResignStatsChange(OnMaxStatChanged);
    }

    void OnMaxStatChanged(StatType statType)
    {
        if (statType == StatType.HP) RefreshHealth();
    }

    /// <summary>Sets the bar to current HP / max HP; leaves it unchanged when either side is missing.</summary>
    public void RefreshHealth()
    {
        if (vital == null || stats == null || slider == null) return;
        float max = stats.GetStatValue(StatType.HP);
        if (max <= 0f) return;
        slider.value = vital.GetCurrentStatValue(VitalStatType.HP) / max;
    }

    public void UpdateUIHealth(float value)
    {
        slider.value = value;
    }
    void OnEnable()
    {
        slider.value = 1;
    }
}
