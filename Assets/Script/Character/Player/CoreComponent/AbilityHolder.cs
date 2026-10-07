// Registered in GameLifetimeScope by this concrete type; the [Inject] Construct lives on the base.
// Bindings are authored on PlayerData (CharacterData); the serialized list on this component is overwritten.
public class AbilityHolder : AbilityHolderBase<Core>
{
}

[System.Serializable]
public class AbilityBinding
{
    public AbilitySlot Slot;
    public AbilityDefinition Ability;
}
