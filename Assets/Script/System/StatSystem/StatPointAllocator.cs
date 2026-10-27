using UnityEngine;
using VContainer;

/// <summary>
/// Owns every write into _playerStatService that the stats UI asks for. Lives in the gameplay layer,
/// not the UI layer, so the screen stays a pure view (see .claude/rules/ui-code.md). The rules themselves
/// live in <see cref="StatPointSession"/>; this component only adapts them to EventManager.
///
/// Commands in  : ON_OPEN_STATS_PLAYER_UI, ON_INCREASE_STATS_BY_UI, ON_DECREASE_STATS_BY_UI,
///                ON_UPDATE_STATS_BY_UI, ON_REVERT_STATS_BY_UI, ON_RESTORE_STATS_BY_UI
/// Notifications out: ON_CHANGE_STATS_BY_UI_RUN_TIME (int remaining points),
///                    ON_RESET_STATS_UI_SESSION
/// </summary>
public class StatPointAllocator : MonoBehaviour
{
    private StatPointSession _session;
    [Inject]
    public void Construct(IPlayerStatService playerStatService)
    {
        _session = new StatPointSession(playerStatService);
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
        if (_session == null) return;
        _session.Begin();
        Broadcast();
    }

    private void OnIncrease(object obj)
    {
        if (_session == null || obj == null) return;
        if (_session.TryIncrease((StatType)obj)) Broadcast();
    }

    private void OnDecrease(object obj)
    {
        if (_session == null || obj == null) return;
        if (_session.TryDecrease((StatType)obj)) Broadcast();
    }

    private void OnRevert(object _ = null)
    {
        if (_session == null) return;
        _session.Revert();
        ResetSession();
    }

    private void OnRestore(object _ = null)
    {
        if (_session == null) return;
        _session.Restore();
        ResetSession();
    }

    private void OnAccept(object _ = null)
    {
        if (_session == null) return;
        _session.Accept();
        ResetSession();
    }

    // ---------- notifications ----------

    private void Broadcast() => EventManager.Emit(EventID.ON_CHANGE_STATS_BY_UI_RUN_TIME, _session.PointsRemaining);

    private void ResetSession()
    {
        EventManager.Emit(EventID.ON_RESET_STATS_UI_SESSION, _session.PointsRemaining);
        Broadcast();
    }
}
