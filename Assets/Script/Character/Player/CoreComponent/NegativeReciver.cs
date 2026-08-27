using UnityEngine;

public class NegativeReciver : CoreComponent<Core>, INegativeReceiver
{
    public int currentHealth;
    public void TakeDamage(int amoutDamage, Vector2 attackPosition)
    {
        if (currentHealth <= 0) return;
        currentHealth -= amoutDamage;
        Core.GetCoreComponent(out PlayerInputHandler input);
        input.OnTakeDamage(attackPosition);
        Core.GetCoreComponent(out VitalStatsComponent vitalStatsComponent);
        vitalStatsComponent.ReceiveReduction(StatType.HP, amount);
    }
}
