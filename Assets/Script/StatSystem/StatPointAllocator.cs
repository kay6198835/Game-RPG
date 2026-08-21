using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Owns every write into StatsSO that the stats UI asks for. Lives in the gameplay layer,
/// not the UI layer, so the screen stays a pure view (see .claude/rules/ui-code.md).
///
/// Commands in  : ON_OPEN_STATS_PLAYER_UI, ON_INCREASE_STATS_BY_UI, ON_DECREASE_STATS_BY_UI,
///                ON_UPDATE_STATS_BY_UI, ON_REVERT_STATS_BY_UI, ON_RESTORE_STATS_BY_UI
/// Notifications out: ON_CHANGE_STATS_BY_UI_RUN_TIME (int remaining points),
///                    ON_RESET_STATS_UI_SESSION
/// </summary>
public class StatPointAllocator : MonoBehaviour
{
    [SerializeField] private StatsSO statsSO;

    /// <summary>Points spent this session, per primary stat — the undo record for Revert.</summary>
    private readonly Dictionary<StatType, int> _sessionGains = new();

    private int _pointsAtOpen;
    private int _pointsRemaining;

    private void Awake()
    {
        ResolveMissingAssets();
    }

    private void OnEnable()
    {
        EventManager.Resgister(EventID.ON_OPEN_STATS_PLAYER_UI, OnOpen);
        EventManager.Resgister(EventID.ON_INCREASE_STATS_BY_UI, OnIncrease);
        EventManager.Resgister(EventID.ON_DECREASE_STATS_BY_UI, OnDecrease);
        EventManager.Resgister(EventID.ON_UPDATE_STATS_BY_UI, OnAccept);
        EventManager.Resgister(EventID.ON_REVERT_STATS_BY_UI, OnRevert);
        EventManager.Resgister(EventID.ON_RESTORE_STATS_BY_UI, OnRestore);
    }

    private void OnDisable()
    {
        EventManager.UnResgister(EventID.ON_OPEN_STATS_PLAYER_UI, OnOpen);
        EventManager.UnResgister(EventID.ON_INCREASE_STATS_BY_UI, OnIncrease);
        EventManager.UnResgister(EventID.ON_DECREASE_STATS_BY_UI, OnDecrease);
        EventManager.UnResgister(EventID.ON_UPDATE_STATS_BY_UI, OnAccept);
        EventManager.UnResgister(EventID.ON_REVERT_STATS_BY_UI, OnRevert);
        EventManager.UnResgister(EventID.ON_RESTORE_STATS_BY_UI, OnRestore);
    }

    // ---------- commands ----------

    private void OnOpen(object _ = null)
    {
        if (statsSO == null) return;
        _sessionGains.Clear();
        _pointsAtOpen = statsSO.StatUnusedBonus;
        _pointsRemaining = _pointsAtOpen;
        Broadcast();
    }

    private void OnIncrease(object obj)
    {
        if (statsSO == null || obj == null) return;
        StatType type = (StatType)obj;
        if (!type.IsPrimary() || _pointsRemaining <= 0) return;

        statsSO.AddPrimaryPoint(type, 1f);
        _sessionGains.TryGetValue(type, out int spent);
        _sessionGains[type] = spent + 1;
        _pointsRemaining--;
        Broadcast();
    }

    private void OnDecrease(object obj)
    {
        if (statsSO == null || obj == null) return;
        StatType type = (StatType)obj;
        // Only points spent in THIS session can be taken back — otherwise the player
        // would strip levels they already committed. Full respec is Restore.
        if (!_sessionGains.TryGetValue(type, out int spent) || spent <= 0) return;

        statsSO.AddPrimaryPoint(type, -1f);
        _sessionGains[type] = spent - 1;
        _pointsRemaining++;
        Broadcast();
    }

    private void OnRevert(object _ = null)
    {
        if (statsSO == null) return;
        foreach (var (type, spent) in _sessionGains)
        {
            if (spent > 0) statsSO.AddPrimaryPoint(type, -spent);
        }
        _sessionGains.Clear();
        _pointsRemaining = _pointsAtOpen;
        ResetSession();
    }

    /// <summary>Full respec: hands every level-up point ever spent back to the pool.</summary>
    private void OnRestore(object _ = null)
    {
        if (statsSO == null) return;
        foreach (StatType type in Enum.GetValues(typeof(StatType)))
        {
            if (!type.IsPrimary()) continue;
            Stat stat = statsSO.Get(type);
            if (stat == null || stat.LevelUpValue == 0f) continue;
            statsSO.AddPrimaryPoint(type, -stat.LevelUpValue);
        }
        _sessionGains.Clear();
        statsSO.CalculateStatUnusedBonus();
        _pointsAtOpen = statsSO.StatUnusedBonus;
        _pointsRemaining = _pointsAtOpen;
        ResetSession();
    }

    private void OnAccept(object _ = null)
    {
        if (statsSO == null) return;
        _sessionGains.Clear();
        statsSO.CalculateStatUnusedBonus();
        _pointsAtOpen = statsSO.StatUnusedBonus;
        _pointsRemaining = _pointsAtOpen;
        ResetSession();
    }

    // ---------- notifications ----------

    private void Broadcast() => EventManager.Emit(EventID.ON_CHANGE_STATS_BY_UI_RUN_TIME, _pointsRemaining);

    private void ResetSession()
    {
        EventManager.Emit(EventID.ON_RESET_STATS_UI_SESSION, _pointsRemaining);
        Broadcast();
    }

    /// <summary>
    /// Editor-only safety net so the sample scene plays with an empty Inspector slot.
    /// Assign the slot explicitly for a real build.
    /// </summary>
    private void ResolveMissingAssets()
    {
#if UNITY_EDITOR
        if (statsSO == null)
            statsSO = UnityEditor.AssetDatabase.LoadAssetAtPath<StatsSO>("Assets/SO/Stat/PlayerStats.asset");
#endif
    }
}
