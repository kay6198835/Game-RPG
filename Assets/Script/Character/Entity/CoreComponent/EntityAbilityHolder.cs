using System.Collections.Generic;

/// <summary>
/// Enemy Abilities v2 owner — same AbilityDefinitions and runtime as the player. Optional: add it to an
/// enemy prefab and author bindings on EntityData. Not registered in VContainer; Pool.Spawn() injects it.
/// </summary>
public class EntityAbilityHolder : AbilityHolderBase<EntityCore>
{
    protected override List<AbilityBinding> ResolveBindings() => core.Entity.Data.AbilityBindings;
}
