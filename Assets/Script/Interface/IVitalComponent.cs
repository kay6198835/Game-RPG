public interface IVitalComponent
{
    float GetCurrentStatValue(StatType statType);
    void ApplyBuffDebuff(StatModifierGroup statModifierGroup);
    void ReceiverRecovery(StatType statType, float amount);
    void ReceiveReduction(StatType statType, float amount);
    void BuffDebuffForDuration(StatModifierGroup statModifierGroup, float duration);
}