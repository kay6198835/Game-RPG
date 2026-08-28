using UnityEngine;
using VContainer;
using VContainer.Unity;

/// <summary>
/// Composition root for the stat layer. Owns the single StatsSO instance and hands it to
/// every consumer as IPlayerStatService, so nothing below this scope touches the asset
/// directly.
/// </summary>
public class GameLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterComponentInHierarchy<ObjectPoolManager>().As<IObjecPoolService>();
        builder.RegisterComponentInHierarchy<StatHandler>().As<IPlayerStatService>();

        builder.RegisterComponentInHierarchy<EnemySpawner>();
        builder.RegisterComponentInHierarchy<StatsUIController>();
        //builder.RegisterComponentInHierarchy<StatsScreenUIController>();
        //builder.RegisterComponentInHierarchy<StatPointAllocator>();
    }
}
