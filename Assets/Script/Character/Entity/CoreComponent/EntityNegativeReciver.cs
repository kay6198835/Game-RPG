using UnityEngine;
using UnityEngine.UIElements;

public class EntityNegativeReciver : EntityCoreComponent<EntityCore>, INegativeReceiver
{
    private EntityVitalStats entityVitalStats;
    private EntityStatsHandler entityStatsHandler;
    private EntityUIController entityUIController;
    private EntityInput entityInput;
    protected override void Start()
    {
        base.Start();
        Core.GetCoreComponent(out entityVitalStats);
        Core.GetCoreComponent(out entityStatsHandler);
        Core.GetCoreComponent(out entityUIController);
        Core.GetCoreComponent(out entityInput);
    }
    public void TakeDamage(float amoutDamage, Vector2 attackPosition)
    {
        float finalDamage = DamageCalculate(amoutDamage);
        if (finalDamage <= 0) return;
        entityVitalStats.ReceiveReduction(StatType.HP, finalDamage);
        entityInput.OnTakeDamage(attackPosition);
        entityUIController.UpdateUIHealth(UpdateUIHealth());
    }

    public float UpdateUIHealth()
    {
        return entityVitalStats.GetCurrentStatValue(StatType.HP) /
         entityStatsHandler.GetStatValue(StatType.HP);
    }

    public float DamageCalculate(float amoutDamage)
    {
        amoutDamage -= entityStatsHandler.GetStatValue(StatType.Defense);
        return amoutDamage < 0 ? 0 : amoutDamage;
    }
}