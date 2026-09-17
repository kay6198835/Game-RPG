using UnityEngine;

public class SpawnEffectBase : AbilityEffectDefinition
{
    [Header("Projectile")]
    public SpawnMono Prefab;
    public float SpawnOffset = 0.8f;
    public float Lifetime = 8f;
    protected AbilityContext context;
    protected Vector2 dir;
    public override void Apply(AbilityContext context)
    {
        if (Prefab == null || context?.Caster == null)
        {
            Debug.LogWarning("[ShootSpiritOrbEffect] OrbPrefab chưa được assign.");
            return;
        }
        this.context = context;

        dir = new Vector2(context.Forward.x, context.Forward.y);
        if (dir.sqrMagnitude < 0.01f)
            dir = Vector2.right;
        dir.Normalize();
        Vector2 spawnPos = SpawnPos();
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        var obj = context.Services.Pool.Spawn(Prefab.gameObject, spawnPos, Quaternion.Euler(0f, 0f, angle));
        var orb = obj.GetComponent<SpawnMono>();
        if (orb != null)
            orb.Launch(dir, Lifetime, context);
        else
            Debug.LogWarning("[ShootSpiritOrbEffect] OrbPrefab thiếu component SpiritOrbProjectile.");
        ReloadEffect();
    }

    protected virtual Vector2 SpawnPos()
    {
        return (Vector2)context.Origin + dir * SpawnOffset;
    }

    public virtual void Execute(AbilityContext context)
    {

    }

    public override bool Casting(AbilityContext context)
    {
        if (!base.Casting(context)) return false;
        return true;
    }
}
