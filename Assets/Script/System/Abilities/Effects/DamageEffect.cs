using UnityEngine;

/// <summary>
/// The only effect that deals ability hit damage (GDD AB-S7, AB-S11). It goes through
/// IEffectReceiver.ApplyDamage, so the hit is mitigated and the recipient enters its TakeDamage state.
/// HP loss without a hit reaction (DOT tick, cost-like drain) uses RecoveryReductionStatsEffect /
/// RecoveryReductionPerTimeForDuration instead.
/// Usually a SubEffect of a spawn effect with Recipient = Target: the spawned object sets ctx.Target.
/// </summary>
[CreateAssetMenu(menuName = "Game/Abilities/Effects/Damage Effect")]
public class DamageEffect : AbilityEffectDefinition
{
    [SerializeField] private float baseDamage = 30f;
    [Tooltip("Caster stat added to the damage. Ignored when Coefficient is 0.")]
    [SerializeField] private StatType scalingStat = StatType.PhysicalDamage;
    [Tooltip("damage = baseDamage + coefficient × caster's scalingStat. 0 = flat damage.")]
    [SerializeField] private float coefficient;

    public override void Apply(AbilityContext context)
    {
        if (context == null) return;
        if (!context.TryGetRecipientComponent(Recipient, out IEffectReceiver receiver)) return;
        receiver.ApplyDamage(CalculateDamage(context), context.Origin, context);
    }

    private float CalculateDamage(AbilityContext context)
    {
        if (coefficient == 0f || context.Caster == null) return baseDamage;
        // GetCurrentStatValue(StatType) returns the stat's final value (IStatService), not a vital.
        return baseDamage + coefficient * context.Caster.GetCurrentStatValue(scalingStat);
    }
}
