/// <summary>
/// The PLAYER's stat service, as registered in GameLifetimeScope. Adds the progression-only
/// members an enemy does not have. Character-level code should depend on <see cref="IStatService"/>.
/// </summary>
public interface IPlayerStatService : IStatService
{
    void AddPrimaryPoint(StatType statType, int amount);
    int GetLevelUpStatsBonus();
}
