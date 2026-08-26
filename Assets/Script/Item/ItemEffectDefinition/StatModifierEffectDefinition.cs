public class StatModifierEffectDefinition()
{
    [SerializeField] StatModifierGroup statModifierGroup;
    public override void Apply(ResourceReceiver resourceReceiver)
    {
        resourceReceiver.ReceverModifierGroup();
    }
}