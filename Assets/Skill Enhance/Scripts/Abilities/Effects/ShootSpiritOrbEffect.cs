using UnityEngine;

[CreateAssetMenu(menuName = "Game/Abilities/Effects/Shoot Spirit Orb")]
public class ShootSpiritOrbEffect : AbilityEffectDefinition
{
    [Header("Projectile")]
    public GameObject OrbPrefab;
    public float Speed = 10f;
    public float SpawnOffset = 0.8f;
    public float OrbLifetime = 8f;

    [Header("DoT")]
    public float DamagePerTick = 25f;
    public float Duration = 5f;

    [Header("Summon on Death")]
    public GameObject SummonPrefab;

    public override void Apply(AbilityContext context)
    {
        if (OrbPrefab == null || context?.Caster == null)
        {
            Debug.LogWarning("[ShootSpiritOrbEffect] OrbPrefab chưa được assign.");
            return;
        }

        Vector2 dir = new Vector2(context.Forward.x, context.Forward.y);
        if (dir.sqrMagnitude < 0.01f)
            dir = Vector2.right;
        dir.Normalize();

        Vector2 spawnPos = (Vector2)context.Origin + dir * SpawnOffset;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        var obj = Object.Instantiate(OrbPrefab, spawnPos, Quaternion.Euler(0f, 0f, angle));
        var orb = obj.GetComponent<SpiritOrbProjectile>();
        if (orb != null)
            orb.Launch(dir, Speed, OrbLifetime, DamagePerTick, Duration, SummonPrefab);
        else
            Debug.LogWarning("[ShootSpiritOrbEffect] OrbPrefab thiếu component SpiritOrbProjectile.");
    }
}
