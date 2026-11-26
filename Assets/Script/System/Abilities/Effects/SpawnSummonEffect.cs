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
        // PENDING (owner fixes later), 2026-11-26: the Casting phase is the power build-up phase, not the spawn.
        // Calling Apply here spawned the summon a second time (TryCast in Casting + Apply in Do) → every hit and
        // SubEffect ran twice (Rain Arrows, Consecrate). Apply is the only spawn until the Casting flow is redesigned.
        // base.Apply(context);
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