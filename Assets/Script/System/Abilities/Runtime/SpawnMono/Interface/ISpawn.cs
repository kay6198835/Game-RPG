using System;
using System.Collections;
using UnityEngine;

public interface ISpawn
{
    void Launch(float lifetime,
                AbilityContext context, Action<AbilityContext> execute);
}