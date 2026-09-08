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