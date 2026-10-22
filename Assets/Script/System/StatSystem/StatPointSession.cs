using System;
using System.Collections.Generic;

/// <summary>
/// One stat-point allocation session over the player's IPlayerStatService: points spent between Begin() and
/// Accept() can be taken back one at a time or all at once with Revert(). Plain class so both the event-driven
/// StatPointAllocator and the UIFlow provider (no VContainer in GameplayUI) can own one.
/// </summary>
public class StatPointSession
{
    private readonly IPlayerStatService _playerStatService;
    /// <summary>Points spent this session, per primary stat — the undo record for Revert.</summary>
    private readonly Dictionary<StatType, int> _sessionGains = new();

    public int PointsAtOpen { get; private set; }
    public int PointsRemaining { get; private set; }
    public bool HasChanges => PointsRemaining != PointsAtOpen;

    public StatPointSession(IPlayerStatService playerStatService)
    {
        _playerStatService = playerStatService ?? throw new ArgumentNullException(nameof(playerStatService));
    }

    public int SpentThisSession(StatType type) => _sessionGains.TryGetValue(type, out int spent) ? spent : 0;

    /// <summary>True when any level-up point was ever committed, i.e. Restore() would hand something back.</summary>
    public bool CanRestore
    {
        get
        {
            foreach (StatType type in Enum.GetValues(typeof(StatType)))
            {
                if (!type.IsPrimary()) continue;
                Stat stat = _playerStatService.GetStat(type);
                if (stat != null && stat.LevelUpValue != 0f) return true;
            }
            return false;
        }
    }

    public void Begin()
    {
        _sessionGains.Clear();
        PointsAtOpen = _playerStatService.GetLevelUpStatsBonus();
        PointsRemaining = PointsAtOpen;
    }

    public bool TryIncrease(StatType type)
    {
        if (!type.IsPrimary() || PointsRemaining <= 0) return false;

        _playerStatService.AddPrimaryPoint(type, 1);
        _sessionGains[type] = SpentThisSession(type) + 1;
        PointsRemaining--;
        return true;
    }

    public bool TryDecrease(StatType type)
    {
        // Only points spent in THIS session can be taken back — otherwise the player
        // would strip levels they already committed. Full respec is Restore.
        int spent = SpentThisSession(type);
        if (spent <= 0) return false;

        _playerStatService.AddPrimaryPoint(type, -1);
        _sessionGains[type] = spent - 1;
        PointsRemaining++;
        return true;
    }

    public void Revert()
    {
        foreach (var (type, spent) in _sessionGains)
        {
            if (spent > 0) _playerStatService.AddPrimaryPoint(type, -spent);
        }
        _sessionGains.Clear();
        PointsRemaining = PointsAtOpen;
    }

    /// <summary>Full respec: hands every level-up point ever spent back to the pool.</summary>
    public void Restore()
    {
        foreach (StatType type in Enum.GetValues(typeof(StatType)))
        {
            if (!type.IsPrimary()) continue;
            Stat stat = _playerStatService.GetStat(type);
            if (stat == null || stat.LevelUpValue == 0f) continue;
            _playerStatService.AddPrimaryPoint(type, -(int)stat.LevelUpValue);
        }
        Begin();
    }

    public void Accept() => Begin();
}
