using System;
using System.Collections;
using UnityEngine;
[CreateAssetMenu(menuName = "Game/Abilities/Effects/Spawn Summon Effect")]
public class SpawnSummonEffect : SpawnEffectBase
{
    protected override float Angle(AbilityContext context)
    {
        return 0;
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
        ApplyOnHitEffects(currentContext);
    }

    protected override Vector2 SpawnPos(AbilityContext context)
    {
        return context.TargetPoint;
    }
}