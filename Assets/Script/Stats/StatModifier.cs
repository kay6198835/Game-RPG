public class StatModifier
{
    public StatModifierType Type;
    public float Value;
    public object Source;

    public StatModifier(StatModifierType type, float value, object source)
    {
        Type = type;
        Value = value;
        Source = source;
    }
}
