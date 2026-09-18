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

    //Callback
    protected override void Execute(AbilityContext currentContext)
    {
        var obj = _context.Services.Pool.Spawn(summonPrefab.gameObject, _context.TargetPoint, Quaternion.identity);
        var controller = obj.GetComponent<SpawnSummonBase>();
        controller.Launch(Lifetime, _context, SummonExecute);
    }

    protected virtual void SummonExecute(AbilityContext currentContext)
    {
        var negativeReceiver = currentContext.Services.NegativeReceiver;
        if (negativeReceiver != null)
        {
            var finalDamage = baseDamage + currentContext.Services.Vital.GetCurrentStatValue(StatType.PhysicalDamage);
            negativeReceiver.TakeDamage(finalDamage, currentContext.Origin);
        }
    }

    protected override Vector2 SpawnPos()
    {
        return _context.TargetPoint;
    }
}