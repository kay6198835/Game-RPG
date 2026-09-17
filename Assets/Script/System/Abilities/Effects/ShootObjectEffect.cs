using UnityEngine;

[CreateAssetMenu(menuName = "Game/Abilities/Effects/Shoot Object Effect")]
public class ShootObjectEffect : AbilityEffectDefinition
{
    [Header("Projectile")]
    public SpawnMono Prefab;
    public float Speed = 10f;
    public float SpawnOffset = 0.8f;
    public float Lifetime = 8f;
    public float BaseDamage = 10f;

    public override void Apply(AbilityContext context)
    {
        if (Prefab == null || context?.Caster == null)
        {
            Debug.LogWarning($"[{name}] Prefab chưa được assign.");
            return;
        }
        if (context.Services?.Pool == null) return;

        var power = ResolvePower(context, BaseDamage);

        Vector2 dir = new Vector2(context.Forward.x, context.Forward.y);
        if (dir.sqrMagnitude < 0.01f)
            dir = Vector2.right;
        dir.Normalize();

        Vector2 spawnPos = (Vector2)context.Origin + dir * SpawnOffset;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        var obj = context.Services.Pool.Spawn(Prefab.gameObject, spawnPos, Quaternion.Euler(0f, 0f, angle));
        if (obj == null) return;

        if (!Mathf.Approximately(power.SizeMultiplier, 1f))
            obj.transform.localScale = Vector3.one * power.SizeMultiplier;

        var orb = obj.GetComponent<SpawnMono>();
        if (orb == null)
        {
            Debug.LogWarning($"[{name}] Prefab thiếu component SpawnMono.");
            return;
        }

        // Captured, not read back off this asset: two projectiles in flight must not share one damage value.
        float damage = power.Damage;
        orb.Launch(dir, Speed * power.SpeedMultiplier, Lifetime, damage, context.Services.Pool,
            (receiver, attackPosition) => receiver.TakeDamage(damage, attackPosition));
    }
}
