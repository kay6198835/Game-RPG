// The player takes damage unmitigated (no Defense) — an owner decision, kept as before.
// Override Mitigate() here to change it.
public class NegativeReciver : NegativeReceiverBase<Core>
{
    // NEED CHECK: new 2026-11-04 — lethal hit goes straight to PlayerDeathState (see NegativeReceiverBase.TakeDamage).
    protected override void OnLethalHit()
    {
        Player player = Core.Player;
        if (player == null || player.StateMachine.CurrentState == player.DeathState) return;
        player.StateMachine.ChangeState(player.DeathState);
    }
}
