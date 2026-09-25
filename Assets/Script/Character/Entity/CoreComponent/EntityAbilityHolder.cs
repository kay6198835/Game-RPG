/// <summary>
/// Enemy Abilities v2 owner — same AbilityDefinitions and runtime as the player, bindings from EntityData
/// (CharacterData). Optional: add it to an enemy prefab. Not registered in VContainer; Pool.Spawn() injects it.
/// </summary>
public class EntityAbilityHolder : AbilityHolderBase<EntityCore>
{
}
