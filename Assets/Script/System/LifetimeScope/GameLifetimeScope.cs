using UnityEngine;
using VContainer;
using VContainer.Unity;

public class GameLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterComponentInHierarchy<ObjectPoolManager>().As<IObjecPoolService>();

        // Player duoc spawn luc runtime boi PlayerManager (resolver.Instantiate inject ca hierarchy),
        // nen khong dang ky component nao cua player. Service cua player di qua PlayerManager.
        builder.RegisterComponentInHierarchy<PlayerManager>().AsSelf().As<IPlayerService>();
        builder.Register<IPlayerStatService>(r => r.Resolve<PlayerManager>().StatService, Lifetime.Singleton);

        builder.RegisterComponentInHierarchy<EnemySpawner>();
        builder.RegisterComponentInHierarchy<StatsUIController>();
        builder.RegisterComponentInHierarchy<ItemSpawner>();
        builder.RegisterComponentInHierarchy<RoomGeneraterController>();
        builder.RegisterComponentInHierarchy<RoomGridController>();
        builder.RegisterComponentInHierarchy<LevelManager>();
        // EntityInput song tren enemy prefab, spawn runtime qua ObjectPoolManager — khong nam
        // trong scene luc LifetimeScope.Configure() chay, RegisterComponentInHierarchy se throw.
        // ObjectPoolManager tu inject cho instance moi spawn (xem Pool.Spawn).
        //builder.RegisterComponentInHierarchy<StatsScreenUIController>();
        //builder.RegisterComponentInHierarchy<StatPointAllocator>();
    }
}
