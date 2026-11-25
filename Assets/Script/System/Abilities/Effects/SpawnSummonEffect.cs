using System;
using System.Collections;
using UnityEngine;
[CreateAssetMenu(menuName = "Game/Abilities/Effects/Spawn Summon Effect")]
public class SpawnSummonEffect : SpawnEffectBase
{
    [SerializeField] protected SpawnSummonBase summonPrefab;

    protected override float Angle()
    {
        return 0;
    }
    public override void Apply(AbilityContext context)
    {
        var obj = context.Services.Pool.Spawn(summonPrefab.gameObject, context.TargetPoint, Quaternion.identity);
        var controller = obj.GetComponent<SpawnSummonBase>();
        controller.Launch(Lifetime, context, SummonExecute);
    }
    public override bool TryCast(AbilityContext context)
    {
        if (!base.TryCast(context)) return false;
        base.Apply(context);
        return true;
    }

    //Callback
    protected override void Execute(AbilityContext currentContext)
    {
        //Do nothing
    }

    // NEED CHECK: 2026-11-25 — no damage here any more. Damage is a DamageEffect in SubEffects (GDD AB-S11);
    // the summon only reports who it hit (ctx.Target) and runs the on-hit SubEffects.
    protected virtual void SummonExecute(AbilityContext currentContext)
    {
        ApplyOnHitEffects(currentContext);
    }

    protected override Vector2 SpawnPos()
    {
        return _context.TargetPoint;
    }
}