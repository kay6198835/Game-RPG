using UnityEngine;

/// <summary>
/// Shared damage pipeline of a character's hurtbox. The flow is fixed here — mitigate, reduce HP,
/// notify input, then a subclass hook — so both sides apply their rules in the same order.
/// </summary>
public abstract class DamageReceiverBase<TCore> : CoreComponentBase<TCore>, INegativeReceiver where TCore : CoreBase
{
    protected IVitalComponent vital;
    protected IStatService stats;
    protected ICharacterInput input;

    protected override void Start()
    {
        base.Start();
        Core.TryGetCapability(out vital);
        Core.TryGetCapability(out stats);
        Core.TryGetCapability(out input);
    }

    public void TakeDamage(float amountDamage, Vector2 attackPosition)
    {
        float finalDamage = Mitigate(amountDamage);
        if (finalDamage <= 0) return;
        vital.Reduction(StatType.HP, finalDamage);
        input?.OnTakeDamage(attackPosition);
        OnDamaged(finalDamage, attackPosition);
    }

    /// <summary>Damage reduction before HP is touched. Default: none.</summary>
    protected virtual float Mitigate(float amountDamage) => amountDamage;

    /// <summary>Side effects after HP was reduced (UI, knockback…).</summary>
    protected virtual void OnDamaged(float finalDamage, Vector2 attackPosition) { }
}
