using System;

public interface IEffectReceiver
{
    void AdjustMaxStats(StatModifierGroup modifierGroup, AbilityContext context, object source, Action callback = null);
    void AdjustCurrentStats(StatModifierGroup modifierGroup, AbilityContext context, Action callback = null);
    void AdjustCurrentForDuration(StatModifierGroup modifierGroup, float duration, AbilityContext context, Action callback = null);
    void AdjustCurrentPerTimeForDuration(StatModifierGroup modifierGroup, float perTime, float duration, AbilityContext context, Action callback = null);
}