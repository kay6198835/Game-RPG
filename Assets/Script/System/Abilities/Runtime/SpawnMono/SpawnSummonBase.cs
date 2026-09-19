using UnityEngine;
[RequireComponent(typeof(Animator))]
public class SpawnSummonBase : SpawnMono
{
    protected Animator animator;
    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
    }

    //Call by Animation Event
    public virtual void Execute()
    {
        _callback.Invoke(_context);
    }
}