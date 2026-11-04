// NEED CHECK: base class was MovementBase<EntityCore> (copy-paste in 786c031b), which made this a second IMovement
// instead of an IEffectReceiver. Now mirrors PlayerEffectReceiver. Added to EnemyPrefab > Core > EffectReceiver
// with sourceEffect = SourceEffect.prefab (same as PlayerTest.prefab) — confirm in the Inspector after recompile.
//public class EntityEffectReceiver : MovementBase<EntityCore>
public class EntityEffectReceiver : EffectReceiverBase<EntityCore>
{

}
