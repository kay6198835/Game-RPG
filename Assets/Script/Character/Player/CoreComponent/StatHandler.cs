using UnityEngine;

public class StatHandler : StatHandlerBase<Core>, IPlayerStatService
{
    WalletComponent wallet;
    [SerializeField] private int requiredExp;
    [SerializeField] private int maxLevel = GameConstants.SettingStats.LEVEL_MAX;

    protected override void Start()
    {
        base.Start();
        core.GetCoreComponent(out wallet);
    }
    public int GetLevelUpStatsBonus()
    {
        return Sheet != null ? Sheet.GetLevelUpStatsBonus() : 0;
    }

    public void AddPrimaryPoint(StatType statType, int amount)
    {
        // amount must be forwarded: Decrease/Revert pass a negative value.
        Sheet?.AddPrimaryPoint(statType, amount);
    }

    public bool TryLevelup()
    {
        GetRequiredExpForNextLevel();
        if (!wallet.TryPayCoin(requiredExp))
            return false;
        return true;
    }

    private int GetRequiredExpForNextLevel()
    {
        int currentLevel = GetLevel();
        if (currentLevel < 1 || currentLevel >= maxLevel)
            return 0;

        requiredExp = Mathf.CeilToInt(35f * Mathf.Pow(currentLevel, 1.8f));

        return requiredExp;
    }
}
