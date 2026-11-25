using System;
using UnityEngine;

public interface IEffectReceiver
{
    /// <summary>
    /// Hit damage from an ability (DamageEffect). Runs the full combat pipeline of the character's hurtbox:
    /// mitigation (Defense), already-dead check, HP reduction, then the hit reaction (TakeDamage state,
    /// knockback). For HP loss without a hit reaction use AdjustCurrentStats instead (GDD AB-S11).
    /// </summary>
    void ApplyDamage(float amount, Vector2 attackPosition, AbilityContext context = null);
    void AdjustMaxStats(StatModifierGroup modifierGroup, AbilityContext context = null, Action callback = null);
    void AdjustCurrentStats(StatModifierGroup modifierGroup, AbilityContext context = null, Action callback = null);
    void AdjustMaxForDuration(StatModifierGroup modifierGroup, float duration, AbilityContext context = null, Action callback = null);
    void AdjustMaxPerTimeForDuration(StatModifierGroup modifierGroup, float perTime, float duration, AbilityContext context = null, Action callback = null);
}
