using UnityEngine;

[CreateAssetMenu(menuName = "Game/Abilities/Effects/Charged Damage Area")]
public class ChargedDamageAreaEffect : AbilityEffectDefinition
{
    [Header("Charge")]
    public ChargeScaling Charge = new();

    [Header("Area")]
    public float Radius = 2f;
    public float RadiusAtFullCharge = 3.5f;
    public float ForwardOffset = 1f;
    [Range(0f, 360f)] public float Angle = 120f;
    public LayerMask TargetMask;
    public int MaxTargets = 12;

    [System.NonSerialized] private Collider2D[] _hits;

    public override bool Casting(AbilityContext context)
    {
        if (!base.Casting(context)) return false;
        return Charge.ShouldPayChargeTick(context == null ? 0f : context.HoldRatio);
    }

    public override void Apply(AbilityContext context)
    {
        if (context?.Caster == null) return;
        if (!Charge.CanRelease(context.HoldRatio)) return;

        if (_hits == null || _hits.Length != MaxTargets)
            _hits = new Collider2D[Mathf.Max(1, MaxTargets)];

        float holdRatio = Mathf.Clamp01(context.HoldRatio);
        float damage = Charge.EvaluateDamage(context);
        float radius = Mathf.Lerp(Radius, RadiusAtFullCharge, Charge.GetChargePower(holdRatio))
            * Charge.GetSizeMultiplier(holdRatio);

        Vector2 forward = new Vector2(context.Forward.x, context.Forward.y);
        if (forward.sqrMagnitude < 0.0001f) forward = Vector2.right;
        forward.Normalize();

        Vector2 center = (Vector2)context.Origin + forward * ForwardOffset;
        int count = Physics2D.OverlapCircleNonAlloc(center, radius, _hits, TargetMask);
        float halfAngle = Angle * 0.5f;

        for (int i = 0; i < count; i++)
        {
            var hit = _hits[i];
            if (hit == null) continue;

            Vector2 toTarget = (Vector2)hit.transform.position - (Vector2)context.Origin;
            if (Angle < 360f && toTarget.sqrMagnitude > 0.0001f
                && Vector2.Angle(forward, toTarget) > halfAngle)
                continue;

            if (hit.TryGetComponent(out INegativeReceiver receiver))
                receiver.TakeDamage(damage, context.Origin);
        }
    }
}
