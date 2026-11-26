using System.Collections.Generic;

/// <summary>
/// An effect that decides <b>who is hit</b> — spawn, area, wave, self (GDD AB-E15). Only a Delivery
/// goes in <see cref="AbilityDefinition.Effects"/>. What happens to the character that was hit is
/// its <see cref="SubEffects"/>: Payload effects only, never another Delivery. A Delivery that needs
/// to chain into another (projectile → explosion) is written as its own Delivery.
/// </summary>
public abstract class DeliveryEffectBase : AbilityEffectDefinition
{
    // NEED CHECK: 2026-11-26 — moved here from AbilityEffectDefinition with the same name, so existing
    // spawn effect assets keep their list (GDD AB-E16).
    public List<AbilityEffectDefinition> SubEffects = new();

    /// <summary>
    /// Runs every SubEffect against <see cref="AbilityContext.Target"/>, in list order. Does nothing
    /// when there is no target (AB-S1). SubEffects skip TryCast and are never charged (AB-E6).
    /// </summary>
    protected void ApplyOnHitEffects(AbilityContext currentContext)
    {
        if (currentContext?.Target == null || SubEffects == null) return;
        for (int i = 0; i < SubEffects.Count; i++)
        {
            if (SubEffects[i] == null) continue;
            SubEffects[i].Apply(currentContext);
        }
    }
}
