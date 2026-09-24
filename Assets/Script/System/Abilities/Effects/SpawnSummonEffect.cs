using System;
using System.Collections;
using UnityEngine;
[CreateAssetMenu(menuName = "Game/Abilities/Effects/Spawn Summon Effect")]
public class SpawnSummonEffect : SpawnEffectBase
{
    [SerializeField] protected SpawnSummonBase summonPrefab;
    [SerializeField] protected float baseDamage = 30;

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

    protected virtual void SummonExecute(AbilityContext currentContext)
    {
        var negativeReceiver = currentContext.Target?.DamageReceiver;
        if (negativeReceiver != null)
        {
            var finalDamage = baseDamage + currentContext.CasterCharacter.Vital.GetCurrentStatValue(StatType.PhysicalDamage);
            negativeReceiver.TakeDamage(finalDamage, currentContext.Origin);
        }
        ApplyOnHitEffects(currentContext);
    }

    protected override Vector2 SpawnPos()
    {
        return _context.TargetPoint;
    }
}