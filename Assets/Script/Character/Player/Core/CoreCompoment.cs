using UnityEngine;

public class CoreComponent : CoreComponentBase
{
    [SerializeField] protected Core core;

    public Core Core { get => core; }

    protected virtual void Awake()
    {
        core = transform.parent.GetComponent<Core>();
        if (core != null) core.AddCoreComponent(this);
    }
}
