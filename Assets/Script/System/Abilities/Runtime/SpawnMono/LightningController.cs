using System;
using UnityEngine;
public class LightningController : SpawnSummonBase
{
    [SerializeField] private int randomIndex;
    public override void Launch(float lifetime, AbilityContext context, Action<AbilityContext> currentContext)
    {
        base.Launch(lifetime, context, currentContext);
        randomIndex = UnityEngine.Random.Range(0, 10);
        animator.SetFloat("Index", randomIndex);
    }
    public override void Execute()
    {
        // get colider in a range
        // get INegative
        // foreach INegative _callback.Invoke(_context);

        base.Execute();
    }
}