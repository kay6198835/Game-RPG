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
    public override void Execute()
    {
        // set INegative
        //
        base.Execute();
    }
}