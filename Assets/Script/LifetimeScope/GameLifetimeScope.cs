using UnityEngine;
using VContainer;
using VContainer.Unity;

public class GameLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterComponentInHierarchy<ObjectPoolManager>().As<IObjecPoolService>();
        builder.RegisterComponentInHierarchy<StatHandler>().As<IPlayerStatService>();

        builder.RegisterComponentInHierarchy<EnemySpawner>();
        builder.RegisterComponentInHierarchy<StatsUIController>();
        builder.RegisterComponentInHierarchy<ItemSpawner>();
        //builder.RegisterComponentInHierarchy<StatsScreenUIController>();
        //builder.RegisterComponentInHierarchy<StatPointAllocator>();
    }
}
