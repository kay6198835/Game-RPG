// NEED CHECK: base class was MovementBase<EntityCore> (copy-paste in 786c031b), which made this a second IMovement
// instead of an IEffectReceiver. Now mirrors PlayerEffectReceiver. Lives on EnemyPrefab > Core > NegativeReciver
// (the hurtbox; moved from its own Core > EffectReceiver object on 2026-11-25)
// (2026-11-28, Q5: the sourceEffect field and SourceEffect.prefab were removed.)
//public class EntityEffectReceiver : MovementBase<EntityCore>
public class EntityEffectReceiver : EffectReceiverBase<EntityCore>
{

}
