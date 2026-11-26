using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleSummon : SpawnSummonBase
{
    [SerializeField] public SpawnSummonBase spawnPrefab;
    public override void Execute()
    {
        SpawnSummonBase spawn = _context.Services.Pool.
        Spawn(spawnPrefab.gameObject, transform.position, Quaternion.identity)
        .GetComponent<SpawnSummonBase>();
        spawn.Launch(0, _context, _callback);
    }
}
