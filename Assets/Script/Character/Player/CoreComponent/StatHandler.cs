public class StatHandler : StatHandlerBase<Core>, IPlayerStatService
{
    // A profile assigned in the Inspector wins; otherwise the one on PlayerData.
    protected override BaseStatsSO ResolveProfile() => statsSO != null ? statsSO : base.ResolveProfile();

    public int GetLevelUpStatsBonus()
    {
        return StatsSO.GetStatUnusedBonus();
    }

    public void AddPrimaryPoint(StatType statType, int amount)
    {
        // amount must be forwarded: Decrease/Revert pass a negative value.
        StatsSO.AddPrimaryPoint(statType, amount);
    }
}
