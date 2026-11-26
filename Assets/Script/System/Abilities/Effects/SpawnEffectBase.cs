using UnityEngine;

// Delivery (GDD AB-E15): spawns the object that reports hits; SubEffects / ApplyOnHitEffects live on DeliveryEffectBase.
public abstract class SpawnEffectBase : DeliveryEffectBase
{
    public SpawnEffectMono Prefab;
    public float SpawnOffset = 0.8f;
    public float Lifetime = 8f;
    protected AbilityContext _context;
    public override void Apply(AbilityContext context)
    {
        if (Prefab == null || context?.Caster == null)
        {
            Debug.LogWarning($"[{GetType().Name}] {name}: Prefab is not assigned or the caster is missing.");
            return;
        }
        _context = context;

        // dir = new Vector2(_context.Forward.x, _context.Forward.y);
        // if (dir.sqrMagnitude < 0.01f)
        //     dir = Vector2.right;
        // dir.Normalize();
        float angle = Angle();
        Vector2 spawnPos = SpawnPos();
        var obj = _context.Services.Pool.Spawn(Prefab.gameObject, spawnPos, Quaternion.Euler(0f, 0f, angle));
        var orb = obj.GetComponent<SpawnEffectMono>();
        if (orb != null)
            orb.Launch(Lifetime, _context, Execute);
        else
            Debug.LogWarning($"[{GetType().Name}] {name}: Prefab has no SpawnMono component.");
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
