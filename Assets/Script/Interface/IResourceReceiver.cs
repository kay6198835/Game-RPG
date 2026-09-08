public interface IResourceReceiver
{
    void ReceverModifierGroup(StatModifierGroup statModifierGroup);
    void ReceverRecovery(StatType statType, float amount);
    void ReceverModifierGroup(StatModifierGroup statModifierGroup);
    void BuffDebuffForDuration(StatModifierGroup statModifierGroup, float duration);
}