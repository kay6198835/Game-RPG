using System;
using UnityEngine;
public class LightningController : SpawnSummonBase
{
    [SerializeField] private float radius = 2f;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private int maxTargets = 4;
    [SerializeField] private int randomIndex;
    private Collider2D[] _buffer;

    protected override void Awake()
    {
        base.Awake();
        _buffer = new Collider2D[maxTargets];
    }
    public override void Launch(float lifetime, AbilityContext context, Action<AbilityContext> currentContext)
    {
        base.Launch(lifetime, context, currentContext);
        randomIndex = UnityEngine.Random.Range(0, 10);
        animator.SetFloat("Index", randomIndex);
    }
    public override void Execute()
    {
        int n = Physics2D.OverlapCircleNonAlloc(transform.position, radius, _buffer, layerMask);
        for (int i = 0; i < n; i++)
        {
            if (!_buffer[i].TryGetComponent<INegativeReceiver>(out var r)) continue;
            _context.Services.NegativeReceiver = r;
            base.Execute();
        }

    }
}