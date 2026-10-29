using VContainer;
using VContainer.Unity;

// Composition root of the CombatTest scene. Keep the player half in sync with GameLifetimeScope:
// a service the player chain injects must be registered here too, or the test scene fails at injection.
// The five dungeon registrations (EnemySpawner, ItemSpawner, RoomGeneraterController, RoomGridController,
// LevelManager) are left out on purpose — RegisterComponentInHierarchy throws when they are absent.
public class CombatTestLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterComponentInHierarchy<ObjectPoolManager>().As<IObjectPoolService>();
        builder.RegisterComponentInHierarchy<PlayerManager>().AsSelf().As<IPlayerService>();
        builder.Register<IPlayerStatService>(r => r.Resolve<PlayerManager>().StatService, Lifetime.Singleton);
        builder.RegisterComponentInHierarchy<CombatTestEnemySpawner>();
    }
}
