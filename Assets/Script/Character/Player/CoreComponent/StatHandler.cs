using System;
using UnityEngine;

public class StatHandler : StatHandlerBase<Core>, IPlayerStatService
{
    WalletComponent wallet;
    private Action<StatType> actionResign;
    [SerializeField] private int requiredExp;
    [SerializeField] private int maxLevel = GameConstants.SettingStats.LEVEL_MAX;

    protected override void Start()
    {
        base.Start();
        core.GetCoreComponent(out wallet);
    }
    void OnDisable()
    {
        ClearResignChangeStat();
    }
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

    public void ResignChangeStat(Action<StatType> action)
    {
        actionResign += action;
        statsSO.OnStatChanged += action;
    }

    private void ClearResignChangeStat()
    {
        statsSO.OnStatChanged -= actionResign;
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
