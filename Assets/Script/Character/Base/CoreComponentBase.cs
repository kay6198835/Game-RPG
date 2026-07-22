using UnityEngine;

public abstract class CoreComponentBase<T> : MonoBehaviour, ICoreComponent<T>
{
    [SerializeField] public T core;

    protected virtual void Awake()
    {
        Setup();
    }

    protected virtual void Setup<T>()
    {
        core = transform.parent.GetComponent<T>();
        if (core != null) core.AddCoreComponent(this);
    }

}
