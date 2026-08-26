using System.Diagnostics;
using UnityEngine;

//Multiple inheritance interface
public class VitalStatsComponent : CoreComponent<Core>
{
    Dictionary<StatType, float> currentStats = new();
    IPlayerStatService playerStatService;
    [Inject]
    public void Construct(IPlayerStatService playerStatService, ObjectPoolManager objectPoolManager)
    {
        this.playerStatService = playerStatService;
    }
    protected override void Awake()
    {
        base.Awake();
    }

    public void ApplyBuffDebuff(StatModifierGroup statModifierGroup)
    {
        statModifierGroup.Apply(playerStatService.AddModifiersFromSource);
    }

    public void ReceverRecovery(StatType statType, float amount)
    {
        Debug.Log("ReceverRecovery: " + statType);
    }

    public void ReceverCurrency(float amount)
    {

    }
}