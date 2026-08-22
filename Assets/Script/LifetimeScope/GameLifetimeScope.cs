// GameLifetimeScope.cs — đặt ở root của LoadRandomMap.unity
public sealed class GameLifetimeScope : LifetimeScope
{
    [SerializeField] private DungeonRoomSO _dungeonRooms;
    [SerializeField] private StatsSO _statsTemplate;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.Register<IPlayerStatService, PlayerStatService>(Lifetime.Singleton);

    }
}