public interface IEffectReceiver
{
    void AdjustMaxToCurrentStats(StatModifierGroup modifierGroup, bool isIgnoreMax = true, object obj = null);
    void AdjustMaxStats(StatModifierGroup modifierGroup, object obj = null);
    void AdjustCurrentForDuration(StatModifierGroup modifierGroup, float duration, object obj = null);
    void AdjustCurrentPerTimeForDuration(StatModifierGroup modifierGroup, float perTime, float duration, object obj = null);
}