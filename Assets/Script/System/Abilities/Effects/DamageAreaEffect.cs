using UnityEngine;

[CreateAssetMenu(menuName = "Game/Abilities/Effects/Damage Area Effect")]
public class DamageAreaEffect : AbilityEffectDefinition
{
    [Header("Damage")]
    public float BaseDamage = 20f;

    [Header("Area")]
    public float Radius = 2f;
    public float ForwardOffset = 1f;
    [Range(0f, 360f)] public float Angle = 120f;
    public LayerMask TargetMask;
    public int MaxTargets = 12;

    [System.NonSerialized] private Collider2D[] _hits;

    public override void Apply(AbilityContext context)
    {
        if (context?.Caster == null) return;

        if (_hits == null || _hits.Length != Mathf.Max(1, MaxTargets))
            _hits = new Collider2D[Mathf.Max(1, MaxTargets)];

        var power = ResolvePower(context, BaseDamage);

        Vector2 forward = new Vector2(context.Forward.x, context.Forward.y);
        if (forward.sqrMagnitude < 0.01f)
            forward = Vector2.right;
        forward.Normalize();

        Vector2 origin = context.Origin;
        Vector2 center = origin + forward * ForwardOffset;
        float radius = Radius * power.SizeMultiplier;

        int count = Physics2D.OverlapCircleNonAlloc(center, radius, _hits, TargetMask);
        float halfAngle = Angle * 0.5f;

        for (int i = 0; i < count; i++)
        {
            var hit = _hits[i];
            if (hit == null) continue;

            Vector2 toTarget = (Vector2)hit.transform.position - origin;
            if (Angle < 360f && toTarget.sqrMagnitude > 0.0001f
                && Vector2.Angle(forward, toTarget) > halfAngle)
                continue;

            if (hit.TryGetComponent(out INegativeReceiver receiver))
                receiver.TakeDamage(power.Damage, origin);
        }
    }
}
