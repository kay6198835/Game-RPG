/// <summary>
/// An effect that acts on <b>one character</b>: damage, stat buff / debuff, heal, drain (GDD AB-E15).
/// It lives in a Delivery's SubEffects and has no SubEffects of its own (AB-E16). Its
/// <see cref="AbilityEffectDefinition.Recipient"/> picks the character: the one that was hit
/// (Target) or the caster (e.g. a life steal heal next to a DamageEffect, AB-E18).
/// </summary>
public abstract class PayloadEffectBase : AbilityEffectDefinition
{
}
