using UnityEngine;

public class ResourceReceiver : Interact
{
    VitalStatsComponent vitalStatsComponent;
    protected override void Awake()
    {
        base.Awake();
    }

    public void ReceverModifierGroup(StatModifierGroup statModifierGroup)
    {
        Core.GetCoreComponent<VitalStatsComponent>(vitalStatsComponent);
        vitalStatsComponent.ApplyBuffDebuff(StatModifierGroup);
    }

    public void ReceverRecovery(StatType statType, float amount)
    {

    }
}