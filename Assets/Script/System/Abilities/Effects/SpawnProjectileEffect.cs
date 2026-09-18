using System;
using System.Collections;
using UnityEngine;
[CreateAssetMenu(menuName = "Game/Abilities/Effects/Spawn Projectile Effect")]
public class SpawnProjectileEffect : SpawnEffectBase
{
    [SerializeField] private float damage = 30;
    public override void Apply(AbilityContext context)
    {
        base.Apply(context);
    }

    protected override float Angle()
    {
        if (dir.sqrMagnitude < 0.01f)
            dir = Vector2.right;
        dir.Normalize();
        return Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
    }

    protected override void Execute(AbilityContext currentContext)
    {
        var negativeReceiver = currentContext.Services.NegativeReceiver;
        if (negativeReceiver != null)
        {
            negativeReceiver.TakeDamage(damage, currentContext.Origin);
        }
    }

    protected override Vector2 SpawnPos()
    {
        return (Vector2)_context.Origin + dir * SpawnOffset;
    }
}