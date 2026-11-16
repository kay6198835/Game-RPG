using UnityEngine;

public class EntityNegativeReciver : NegativeReceiverBase<EntityCore>
{
    private EntityUIController entityUIController;
    private EntityMovement entityMovement;

    protected override void Start()
    {
        base.Start();
        Core.GetCoreComponent(out entityUIController);
        Core.GetCoreComponent(out entityMovement);
    }

    protected override float Mitigate(float amountDamage) => DamageCalculate(amountDamage);

    protected override void OnDamaged(float finalDamage, Vector2 attackPosition)
    {
        // NEED CHECK: removed — EntityUIController now redraws on IVitalComponent.CurrentStatsChanged.
        //entityUIController.UpdateUIHealth(UpdateUIHealth());
        entityMovement.SetPositionToCheck(attackPosition);
    }

    public float UpdateUIHealth()
    {
        return vital.GetCurrentStatValue(VitalStatType.HP) / stats.GetStatValue(StatType.HP);
    }

    // NEED CHECK: 2026-11-08 — percent mitigation per design/balance/combat-balance-2026-07-07.md
    // (damage × 100 / (100 + Defense)). Was flat `damage − Defense`, which made high-Defense enemies
    // (Tank, Boss) immune to normal hits and low-Defense ones die to a single hit.
    public float DamageCalculate(float amoutDamage)
    {
        float defense = Mathf.Max(0f, stats.GetStatValue(StatType.Defense));
        float finalDamage = amoutDamage * 100f / (100f + defense);
        return finalDamage < 0 ? 0 : finalDamage;
    }
}
