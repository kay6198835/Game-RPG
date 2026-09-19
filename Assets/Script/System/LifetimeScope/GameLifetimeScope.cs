using UnityEngine;
using VContainer;
using VContainer.Unity;

public class GameLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterComponentInHierarchy<ObjectPoolManager>().As<IObjecPoolService>();

        // Player phai duoc dat san trong scene (Hierarchy) — FindComponentProvider chi tim,
        // khong tu spawn. Neu Player khong nam trong scene luc Awake(), 3 dong duoi day throw.
        builder.RegisterComponentInHierarchy<Player>();
        builder.RegisterComponentInHierarchy<PlayerManager>().As<IPlayerService>();
        builder.RegisterComponentInHierarchy<StatHandler>().As<IPlayerStatService>();

        builder.RegisterComponentInHierarchy<EnemySpawner>();
        builder.RegisterComponentInHierarchy<StatsUIController>();
        builder.RegisterComponentInHierarchy<ItemSpawner>();
        builder.RegisterComponentInHierarchy<RoomGeneraterController>();
        builder.RegisterComponentInHierarchy<AbilityHolder>();
        // EntityInput song tren enemy prefab, spawn runtime qua ObjectPoolManager — khong nam
        // trong scene luc LifetimeScope.Configure() chay, RegisterComponentInHierarchy se throw.
        // ObjectPoolManager tu inject cho instance moi spawn (xem Pool.Spawn).
        //builder.RegisterComponentInHierarchy<StatsScreenUIController>();
        //builder.RegisterComponentInHierarchy<StatPointAllocator>();
    }
}
