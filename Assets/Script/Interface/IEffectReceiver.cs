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
    /// <summary>
    /// Holds a status for a while: modifiers on max stats and / or a tick on current vitals (B6, GDD OQ-4).
    /// Key = (AbilityInstance, effect asset): the same key refreshes the time only; another key is its own
    /// status and adds up. See ActiveStatus.cs.
    /// </summary>
    void ApplyStatus(StatusRequest request);
    /// <summary>Legacy: an independent status with no key. Prefer ApplyStatus.</summary>
    void AdjustMaxForDuration(StatModifierGroup modifierGroup, float duration, AbilityContext context = null, Action callback = null);
    /// <summary>Legacy: an independent ticking status with no key; duration in seconds. Prefer ApplyStatus.</summary>
    void AdjustMaxPerTimeForDuration(StatModifierGroup modifierGroup, float perTime, float duration, AbilityContext context = null, Action callback = null);
}
