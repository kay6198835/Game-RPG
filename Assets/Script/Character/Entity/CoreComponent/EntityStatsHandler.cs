using UnityEngine;

public class EntityStatsHandler : StatHandlerBase<EntityCore>
{
    private BaseStatsSO runtimeClone;

    // Every enemy of one type shares one EntityData.StatsSO asset. A modifier applied to that asset
    // would hit every live enemy of the type and leak into the committed file in the Editor, so each
    // instance works on its own runtime clone.
    protected override BaseStatsSO ResolveProfile()
    {
        BaseStatsSO source = core.Entity.Data.StatsSO;
        runtimeClone = Instantiate(source);
        runtimeClone.name = source.name + " (Runtime)";
        statsSO = runtimeClone;
        return runtimeClone;
    }

    /// <summary>Drops buffs/debuffs left on this instance's clone, e.g. a timed debuff whose removal
    /// coroutine was stopped when the pool disabled the enemy.</summary>
    public void ResetRuntimeModifiers()
    {
        StatsSO.ClearRuntimeModifiers();
    }

    private void OnDestroy()
    {
        if (runtimeClone != null) Destroy(runtimeClone);
    }
}
