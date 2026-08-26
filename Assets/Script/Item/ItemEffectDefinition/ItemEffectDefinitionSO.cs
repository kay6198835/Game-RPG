public abstract class ItemEffectDefinition : ScriptableObject
{
    public abstract void Apply(ResourceReceiver player);
}