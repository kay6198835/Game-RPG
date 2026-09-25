using UnityEngine;

public abstract class SpawnEffectBase : AbilityEffectDefinition
{
    public SpawnMono Prefab;
    public float SpawnOffset = 0.8f;
    public float Lifetime = 8f;
    protected AbilityContext _context;
    protected Vector2 dir;
    public override void Apply(AbilityContext context)
    {
        if (Prefab == null || context?.Caster == null)
        {
            Debug.LogWarning("[ShootSpiritOrbEffect] OrbPrefab chưa được assign.");
            return;
        }
        _context = context;

        dir = new Vector2(_context.Forward.x, _context.Forward.y);
        if (dir.sqrMagnitude < 0.01f)
            dir = Vector2.right;
        dir.Normalize();
        float angle = Angle();
        Vector2 spawnPos = SpawnPos();
        var obj = _context.Services.Pool.Spawn(Prefab.gameObject, spawnPos, Quaternion.Euler(0f, 0f, angle));
        var orb = obj.GetComponent<SpawnMono>();
        if (orb != null)
            orb.Launch(Lifetime, _context, Execute);
        else
            Debug.LogWarning("[ShootSpiritOrbEffect] OrbPrefab thiếu component SpiritOrbProjectile.");
    }

    /// <summary>
    /// Runs SubEffects as on-hit effects against the character just hit. A SubEffect whose recipient
    /// is Target therefore acts on whoever the spawned object hit — player or enemy alike.
    /// </summary>
    protected void ApplyOnHitEffects(AbilityContext currentContext)
    {
        if (currentContext.Target == null || SubEffects == null) return;
        for (int i = 0; i < SubEffects.Count; i++)
        {
            if (SubEffects[i] == null) continue;
            SubEffects[i].Apply(currentContext);
        }
    }

    protected abstract Vector2 SpawnPos();
    protected abstract float Angle();
    protected abstract void Execute(AbilityContext currentContext);
    public override bool TryCast(AbilityContext context)
    {
        if (!base.TryCast(context)) return false;
        return true;
    }
}
