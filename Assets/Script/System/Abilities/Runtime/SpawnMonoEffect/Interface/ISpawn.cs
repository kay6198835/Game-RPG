using System;
using System.Collections;
using UnityEngine;

public interface ISpawnEffect
{
    void Launch(float lifetime,
                AbilityContext context, Action<AbilityContext> execute);
    void DespawnOneSelf();
}

public interface ISpawn
{
    void Launch(float lifetime);
    void DespawnOneSelf();
}