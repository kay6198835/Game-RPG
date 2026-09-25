using System.Collections.Generic;

// Registered in GameLifetimeScope by this concrete type; the [Inject] Construct lives on the base.
public class AbilityHolder : AbilityHolderBase<Core>
{
    // Bindings are authored on PlayerData; the serialized list on this component is overwritten.
    protected override List<AbilityBinding> ResolveBindings() => core.Player.Data.AbilityBindings;
}

[System.Serializable]
public class AbilityBinding
{
    public AbilitySlot Slot;
    public AbilityDefinition Ability;
}
