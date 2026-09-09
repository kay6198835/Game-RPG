public interface IResourceReceiver
{
    void ReceverModifierGroup(StatModifierGroup statModifierGroup);
    void ReceverRecovery(StatType statType, float amount);
    void BuffDebuffForDuration(StatModifierGroup statModifierGroup, float duration);
}