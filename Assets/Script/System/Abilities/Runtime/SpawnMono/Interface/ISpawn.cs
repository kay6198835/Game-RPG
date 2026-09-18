using System;
using System.Collections;
using UnityEngine;

public interface ISpawn
{
    void Launch(Vector2 target, float lifetime,
                AbilityContext context, Action<AbilityContext> execute);
}