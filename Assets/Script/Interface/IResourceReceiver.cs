public interface IResourceReceiver
{
    void ReceverModifierGroup(StatModifierGroup statModifierGroup);
    void Recovery(StatType statType, float amount);
    void BuffDebuffForDuration(StatModifierGroup statModifierGroup, float duration);
}