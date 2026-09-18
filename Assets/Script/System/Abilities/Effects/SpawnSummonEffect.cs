using System;
using System.Collections;
using UnityEngine;
[CreateAssetMenu(menuName = "Game/Abilities/Effects/Spawn Summon Effect")]
public class SpawnSummonEffect : SpawnEffectBase
{
    // thay summonPrefab thành 1 script gì đó để có thể control
    [SerializeField] protected GameObject summonPrefab;

    protected override float Angle()
    {
        return 0;
    }

    protected override void Execute(AbilityContext currentContext)
    {
        _context.Services.Pool.Spawn(summonPrefab,_context.TargetPoint,Quaternion.identity);
    }

    protected override Vector2 SpawnPos()
    {
        return _context.TargetPoint;
    }
}