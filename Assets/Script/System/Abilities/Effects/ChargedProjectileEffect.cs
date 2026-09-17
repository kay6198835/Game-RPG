using UnityEngine;

[CreateAssetMenu(menuName = "Game/Abilities/Effects/Charged Projectile")]
public class ChargedProjectileEffect : AbilityEffectDefinition
{
    [Header("Charge")]
    public ChargeScaling Charge = new();

    [Header("Projectile")]
    public GameObject Prefab;
    public float SpawnOffset = 0.8f;
    public float Lifetime = 8f;

    [Header("Charge Shaping")]
    public float Speed = 10f;
    public float SpeedAtFullCharge = 16f;
    public float Scale = 1f;
    public float ScaleAtFullCharge = 1.8f;

    public override bool Casting(AbilityContext context)
    {
        if (!base.Casting(context)) return false;
        return Charge.ShouldPayChargeTick(context == null ? 0f : context.HoldRatio);
    }

    public override void Apply(AbilityContext context)
    {
        if (context?.Caster == null) return;
        if (Prefab == null)
        {
            Debug.LogWarning($"[{name}] Prefab is not assigned.");
            return;
        }
        if (context.Services?.Pool == null) return;
        if (!Charge.CanRelease(context.HoldRatio)) return;

        float holdRatio = Mathf.Clamp01(context.HoldRatio);
        float power = Charge.GetChargePower(holdRatio);
        float sizeMultiplier = Charge.GetSizeMultiplier(holdRatio);

        float damage = Charge.EvaluateDamage(context);
        float speed = Mathf.Lerp(Speed, SpeedAtFullCharge, power);
        float scale = Mathf.Lerp(Scale, ScaleAtFullCharge, power) * sizeMultiplier;

        Vector2 dir = new Vector2(context.Forward.x, context.Forward.y);
        if (dir.sqrMagnitude < 0.0001f) dir = Vector2.right;
        dir.Normalize();

        Vector2 spawnPos = (Vector2)context.Origin + dir * SpawnOffset;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        var obj = context.Services.Pool.Spawn(Prefab, spawnPos, Quaternion.Euler(0f, 0f, angle));
        if (obj == null) return;

        obj.transform.localScale = Vector3.one * scale;

        if (!obj.TryGetComponent<SpawnMono>(out var projectile))
            projectile = obj.AddComponent<SpawnMono>();

        projectile.Launch(dir, speed, Lifetime, damage, context.Services.Pool,
            (receiver, attackPosition) => receiver.TakeDamage(damage, attackPosition));
    }
}
