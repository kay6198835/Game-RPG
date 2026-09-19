public class ElementReactionDefinition
{
    public ElementType elementA;
    public ElementType elementB;

    public StatusEffectDefinition result;
}

public struct ElementReaction
{
    public ElementType Source;
    public ElementType Target;
    public StatusType Result;
}

public enum ElementType
{
    None,
    Fire,
    Water,
    Ice,
    Lightning,
    Wind,
    Earth,
    Poison
}

public enum StatusType
{
    None,
    Burn,
    Freeze,
    Shock,
    Poisoned,
    Stunned
}
public class StatusEffectDefinition
{
    public StatusType type;
    public float duration;
    public float intensity;
}