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

    public float DamageCalculate(float amoutDamage)
    {
        amoutDamage -= stats.GetStatValue(StatType.Defense);
        return amoutDamage < 0 ? 0 : amoutDamage;
    }
}
