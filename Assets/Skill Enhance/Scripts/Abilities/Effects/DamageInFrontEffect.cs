using UnityEngine;

[CreateAssetMenu(menuName = "Game/Abilities/Effects/Damage In Front")]
public class DamageInFrontEffect : AbilityEffectDefinition
{
    [Header("Damage")]
    public float BaseDamage = 20f;
    public float BonusDamageAtMaxHold = 40f;

    [Header("Area")]
    public float Radius = 2f;
    [Range(0f, 180f)] public float Angle = 90f;
    public LayerMask TargetMask;

    public override void Apply(AbilityContext context)
    {
        if (context?.Caster == null) return;

        float attack = context.Caster.Stats.GetStatValue(StatType.Attack);
        float scaledBonus = BonusDamageAtMaxHold * context.HoldRatio;
        float finalDamage = BaseDamage + scaledBonus + attack;

        Collider[] hits = Physics.OverlapSphere(context.Origin, Radius, TargetMask);

        for (int i = 0; i < hits.Length; i++)
        {
            var hit = hits[i];
            Vector3 dir = (hit.transform.position - context.Origin).normalized;
            float angle = Vector3.Angle(context.Forward, dir);

            if (angle > Angle * 0.5f)
                continue;

            var damageable = hit.GetComponent<Damageable>();
            if (damageable != null)
            {
                damageable.ReceiveDamage(finalDamage);
            }
            else
            {
                var health = hit.GetComponent<Health>();
                if (health != null)
                    health.TakeDamage(finalDamage);
            }
        }

        Debug.Log(
            $"[{context.AbilityDefinition.DisplayName}] DamageInFrontEffect => " +
            $"HoldRatio={context.HoldRatio:F2}, Damage={finalDamage:F2}");
    }
}
