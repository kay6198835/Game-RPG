public class EntityStatsHandler : StatHandlerBase<EntityCore>
{
    protected override BaseStatsSO ResolveProfile()
    {
        statsSO = core.Entity.Data.StatsSO;
        return statsSO;
    }
}
