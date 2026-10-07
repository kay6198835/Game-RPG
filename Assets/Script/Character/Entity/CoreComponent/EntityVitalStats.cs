public class EntityVitalStats : VitalStatsBase<EntityCore>
{
    private EntityStatsHandler entityStatsHandler;
    private IMovement movement;

    // Pooled enemies are re-enabled instead of re-instantiated, so every spawn refills to max.
    void OnEnable()
    {
        Reborn();
    }

    public override void Reborn()
    {
        if (entityStatsHandler == null) Core.GetCoreComponent(out entityStatsHandler);
        entityStatsHandler.ResetRuntimeModifiers();
        // Slows, locks and knockback left over from the previous life must not survive the pool.
        if (movement == null) Core.TryGetCapability(out movement);
        movement?.ClearImpacts();
        base.Reborn();
    }
}
