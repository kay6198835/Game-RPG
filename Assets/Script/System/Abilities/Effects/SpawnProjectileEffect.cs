using System;
using System.Collections;
using UnityEngine;
[CreateAssetMenu(menuName = "Game/Abilities/Effects/Spawn Projectile Effect")]
public class SpawnProjectileEffect : SpawnEffectBase
{
    protected Vector2 dir;
    public override void Apply(AbilityContext context)
    {
        base.Apply(context);
    }

    protected override float Angle(AbilityContext context)
    {
        dir = new Vector2(context.Forward.x, context.Forward.y);
        if (dir.sqrMagnitude < 0.01f)
            dir = Vector2.right;
        dir.Normalize();
        return Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
    }

    // NEED CHECK: 2026-11-25 — no damage here any more. Damage is a DamageEffect in SubEffects (GDD AB-S11).
    protected override void Execute(AbilityContext currentContext)
    {
        ApplyOnHitEffects(currentContext);
    }

    protected override Vector2 SpawnPos(AbilityContext context)
    {
        return (Vector2)context.Origin + dir * SpawnOffset;
    }
}