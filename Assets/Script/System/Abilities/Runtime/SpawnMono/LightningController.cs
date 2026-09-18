using System;
using UnityEngine;
public class LightningController : SpawnSummonBase
{
    [SerializeField] private int randomIndex;
    protected override void OnEnable()
    {
        base.OnEnable();
        //randomIndex = Random.Range(0, 10);
        randomIndex = 0;
        animator.SetInteger("Index", randomIndex);
    }
    public override void Launch(float lifetime, AbilityContext context, Action<AbilityContext> currentContext)
    {
        base.Launch(lifetime, context, currentContext);
    }
    public override void Execute()
    {
        // get colider in a range
        // get INegative
        // foreach INegative _callback.Invoke(_context);

        base.Execute();
    }
}