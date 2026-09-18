using UnityEngine;
[RequireComponent(typeof(Animator))]
public class SpawnSummonBase : SpawnMono
{
    protected Animator animator;
    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
    }

    protected virtual void OnEnable()
    {
        animator.SetBool("Animating", true);
    }

    //Call by Animation Event
    public virtual void Execute()
    {
        _callback.Invoke(_context);
        animator.SetBool("Animating", false);
    }
}