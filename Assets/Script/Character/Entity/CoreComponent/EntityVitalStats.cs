public class EntityVitalStats : VitalStatsBase<EntityCore>
{
    // Pooled enemies are re-enabled instead of re-instantiated, so every spawn refills to max.
    void OnEnable()
    {
        Reborn();
    }
}
