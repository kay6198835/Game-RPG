using UnityEngine;

// NEED CHECK: new 2026-11-28 (B6, GDD OQ-4) — status registry on the effect receiver. Test branch.
//   Key       = (AbilityInstance that applied it, effect asset).
//   Same key  → refresh the time only: modifiers are not applied again, the tick phase is kept.
//   Other key → its own status; statuses coexist and add up (limits / balance: pending).
//   No AbilityInstance (items, legacy calls) → Source = new object(): never matches, so an independent
//   status every call. The fallback key is pending (B6).
// Instant effects (AdjustCurrentStats) and permanent ones (AdjustMaxStats) never enter the registry.

/// <summary>What an effect asks the receiver to hold for a while. A struct: one is built per hit.</summary>
public struct StatusRequest
{
    /// <summary>The effect asset: second half of the key.</summary>
    public ScriptableObject Effect;
    public StatusType StatusType;
    /// <summary>Held on the StatSheet for the whole duration (buff / debuff). Null = none.</summary>
    public StatModifierGroup MaxModifiers;
    /// <summary>Applied to the current vitals every TickInterval (DoT / HoT). Null = no tick.</summary>
    public StatModifierGroup TickModifiers;
    /// <summary>Seconds. &lt;= 0 is ignored (a permanent modifier goes through AdjustMaxStats).</summary>
    public float Duration;
    /// <summary>Seconds between ticks; &lt;= 0 = no tick. The first tick lands when the status is created.</summary>
    public float TickInterval;
    public AbilityContext Context;
}

/// <summary>One live status on a receiver. It is also the modifier source on the StatSheet.</summary>
public class ActiveStatus
{
    public object Source;                    // AbilityInstance, or new object() when there is none
    public ScriptableObject Effect;          // effect asset
    public StatusType Type;
    public StatModifierGroup MaxModifiers;
    public StatModifierGroup TickModifiers;
    public float Duration;
    public float TickInterval;
    public AbilityContext Context;           // passed on to the tick only; never read back (shared by later hits)

    public float EndTime;
    public float NextTick;

    public bool HasTick => TickModifiers != null && TickInterval > 0f;
}
