using UnityEngine;

public class ResourceReceiver : Interact, INegativeReceiver, IResourceReceiver
{
    VitalStatsComponent vitalStatsComponent;
    protected override void Awake()
    {
        base.Awake();
    }

    public void ReceverModifierGroup(StatModifierGroup statModifierGroup)
    {
        Core.GetCoreComponent<VitalStatsComponent>(out vitalStatsComponent);
        vitalStatsComponent.ApplyBuffDebuff(statModifierGroup);
    }

    public void ReceverRecovery(StatType statType, float amount)
    {
        vitalStatsComponent.ReceiveReduction(statType, amount);
    }
    public void ReceiveReduction(StatType statType, float amount)
    {
        vitalStatsComponent.ReceiverRecovery(statType, amount);
    }
    public void BuffDebuffForDuration(StatType statType, float duration)
    {
        vitalStatsComponent.BuffDebuffForDuration(statType, duration);
    }
    public void TakeDamage(float amoutDamage, Vector2 attackPosition)
    {
        Core.GetCoreComponent(out VitalStatsComponent vitalStatsComponent);
        vitalStatsComponent.ReceiveReduction(StatType.HP, amoutDamage);
        Core.GetCoreComponent(out PlayerInputHandler input);
        input.OnTakeDamage(attackPosition);
    }
}