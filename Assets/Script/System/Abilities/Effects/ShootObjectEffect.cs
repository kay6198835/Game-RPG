using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Abilities/Effects/Shoot Object Effect")]
public class ShootObjectEffect : AbilityEffectDefinition
{
    [Header("Projectile")]
    public GameObject Prefab;
    public float Speed = 10f;
    public float SpawnOffset = 0.8f;
    public float Lifetime = 8f;
    public float BaseDamage = 10f;
    public float FinalDamage => BaseDamage + SubDamage;
    [Header("Sub stats")]
    public float Duration = 5f;
    public float DamagePerTick = 25f;
    public float CurrentTickTime = 0f;
    public int TickCountMax = 5;
    public int CurrentTickCount = 0;
    public float SubDamage = 0;
    private AbilityContext _context;
    public void OnEnable()
    {
        ReloadEffect();
    }
    public override void Apply(AbilityContext context)
    {
        if (Prefab == null || context?.Caster == null)
        {
            Debug.LogWarning("[ShootSpiritOrbEffect] OrbPrefab chưa được assign.");
            return;
        }
        _context = context;

        Vector2 dir = new Vector2(context.Forward.x, context.Forward.y);
        if (dir.sqrMagnitude < 0.01f)
            dir = Vector2.right;
        dir.Normalize();

        Vector2 spawnPos = (Vector2)context.Origin + dir * SpawnOffset;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        var obj = context.Services.Pool.Spawn(Prefab, spawnPos, Quaternion.Euler(0f, 0f, angle));
        var orb = GetSriptEffect(obj);
        if (orb != null)
            orb.Launch(dir, Speed, Lifetime, FinalDamage, context.Services.Pool, TakeDamage);
        else
            Debug.LogWarning("[ShootSpiritOrbEffect] OrbPrefab thiếu component SpiritOrbProjectile.");
        ReloadEffect();
    }

    public void TakeDamage(INegativeReceiver receiver, Vector2 attackposition)
    {
        receiver.TakeDamage(FinalDamage, attackposition);
    }

    public override bool Casting(AbilityContext context)
    {
        if (!base.Casting(context)) return false;
        foreach (var condition in SubConditions)
        {
            if (condition.IsMet(context)) continue;
            return false;
        }


        CurrentTickTime += Time.deltaTime;
        if (CurrentTickTime < Duration)
        {
            return false;
        }
        CurrentTickCount++;
        CurrentTickTime = 0f;
        if (CurrentTickCount > TickCountMax)
        {
            CurrentTickCount = TickCountMax;
        }
        SubDamage = CurrentTickCount * DamagePerTick;
        return true;
    }
    private void ReloadEffect()
    {
        SubDamage = 0;
        CurrentTickCount = 0;
        CurrentTickTime = 0;
    }

    private SpawnMono GetSriptEffect(GameObject obj)
    {
        if (!obj.TryGetComponent<SpawnMono>(out var effectScript))
            effectScript = obj.AddComponent<SpawnMono>();
        return effectScript;
    }
}
