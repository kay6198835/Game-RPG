using System;

public interface IEffectReceiver
{
    void AdjustMaxStats(StatModifierGroup modifierGroup, AbilityContext context, object source, Action callback = null);
    void AdjustCurrentStats(StatModifierGroup modifierGroup, AbilityContext context, Action callback = null);
    void AdjustMaxForDuration(StatModifierGroup modifierGroup, float duration, AbilityContext context, Action callback = null);
    void AdjustMaxPerTimeForDuration(StatModifierGroup modifierGroup, float perTime, float duration, AbilityContext context, Action callback = null);
}