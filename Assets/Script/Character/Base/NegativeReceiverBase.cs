using UnityEngine;

/// <summary>
/// Shared damage pipeline of a character's hurtbox. The flow is fixed here — mitigate, reduce HP,
/// notify input, then a subclass hook — so both sides apply their rules in the same order.
/// </summary>
public abstract class NegativeReceiverBase<TCore> : CoreComponentBase<TCore>, INegativeReceiver where TCore : CoreBase
{
    protected IVitalComponent<TCore> vital;
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
        // NEED CHECK: new 2026-11-04 — a dead character ignores further hits, and a hit whose damage is
        // >= current HP sends the character to its death state from whatever state it is in. Before, death was
        // only noticed by the basic state while the 0.1 s take-damage flag was up, so a lethal hit landing during
        // the take-damage animation left the character alive at 0 HP (inbox NOTE-20261101-2).
        float currentHP = vital.GetCurrentStatValue(VitalStatType.HP);
        if (currentHP <= 0) return;
        float finalDamage = Mitigate(amountDamage);
        if (finalDamage <= 0) return;
        bool lethal = finalDamage >= currentHP;
        vital.Reduction(VitalStatType.HP, finalDamage);
        input?.OnTakeDamage(attackPosition);
        OnDamaged(finalDamage, attackPosition);
        if (lethal) OnLethalHit();
    }

    /// <summary>Called once, after HP was reduced to zero by a hit: switch the character to its death state.</summary>
    protected abstract void OnLethalHit();

    /// <summary>Damage reduction before HP is touched. Default: none.</summary>
    protected virtual float Mitigate(float amountDamage) => amountDamage;

    /// <summary>Side effects after HP was reduced (UI, knockback…).</summary>
    protected virtual void OnDamaged(float finalDamage, Vector2 attackPosition) { }
}
