// File name kept as VitalComponent.cs: PlayerTest.prefab references this script by GUID.
public class VitalStatsComponent : VitalStatsBase<Core>
{
    public override void Reborn()
    {
        base.Reborn();
        // Values are seeded only now, so UI that binds on this event can read them straight away.
        EventManager.Emit(EventID.ON_PLAYER_READY, GetComponentInParent<ICharacter>());
    }
}
