using UnityEngine;

/// <summary>
/// Delivery that "hits" the caster itself (GDD AB-E17): sets <see cref="AbilityContext.Target"/> to the
/// caster's hurtbox collider and runs the SubEffects, so self buffs / heals use Recipient = Target like
/// any other Payload. Used by Blessing and Avatar of Light.
/// </summary>
[CreateAssetMenu(menuName = "Game/Abilities/Effects/Self Apply Effect")]
public class SelfApplyEffect : DeliveryEffectBase
{
    public override void Apply(AbilityContext context)
    {
        if (context?.Caster == null) return;
        Collider2D hurtbox = FindCasterHurtbox(context);
        if (hurtbox == null)
        {
            Debug.LogWarning($"[SelfApplyEffect] {name}: the caster has no hurtbox (INegativeReceiver + Collider2D), " +
                             "SubEffects skipped.", this);
            return;
        }

        // The context is shared by every effect of this cast: restore Target afterwards.
        Collider2D previous = context.Target;
        context.Target = hurtbox;
        ApplyOnHitEffects(context);
        context.Target = previous;
    }

    // NEED CHECK: 2026-11-26 — the hurtbox is the GameObject that carries INegativeReceiver (and, since
    // 2026-11-25, IEffectReceiver); NegativeReceiverBase reads its BoxCollider2D from the same GameObject.
    private static Collider2D FindCasterHurtbox(AbilityContext context)
    {
        ICharacter character = context.Caster.Transform.GetComponentInParent<ICharacter>();
        if (character == null || !character.Core.TryGetCapability(out INegativeReceiver receiver)) return null;
        return (receiver as Component)?.GetComponent<Collider2D>();
    }
}
