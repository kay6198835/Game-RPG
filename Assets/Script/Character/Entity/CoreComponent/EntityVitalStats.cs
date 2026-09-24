public class EntityVitalStats : VitalStatsBase<EntityCore>
{
    private EntityStatsHandler entityStatsHandler;

    // Pooled enemies are re-enabled instead of re-instantiated, so every spawn refills to max.
    void OnEnable()
    {
        Reborn();
    }

    public override void Reborn()
    {
        if (entityStatsHandler == null) Core.GetCoreComponent(out entityStatsHandler);
        entityStatsHandler.ResetRuntimeModifiers();
        base.Reborn();
    }
}
