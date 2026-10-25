// File name kept as VitalComponent.cs: PlayerTest.prefab references this script by GUID.
public class VitalStatsComponent : VitalStatsBase<Core>
{
    // Nothing else reloads the player on spawn (enemies do it from OnEnable), so the HUD would never get ON_PLAYER_READY.
    protected override void Start()
    {
        base.Start();
        ReloadComponent();
    }

    public override void ReloadComponent()
    {
        base.ReloadComponent();
        // Values are seeded only now, so UI that binds on this event can read them straight away.
        EventManager.Emit(EventID.ON_PLAYER_READY, GetComponentInParent<ICharacter>());
    }
}
