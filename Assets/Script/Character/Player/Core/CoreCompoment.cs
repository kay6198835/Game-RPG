using UnityEngine;

public class CoreComponent : CoreComponentBase
{
    [SerializeField] protected Core core;

    public Core Core { get => core; }

    protected virtual void Awake()
    {
        core = transform.parent.GetComponent<Core>();
        core.AddCoreComponent(this);
    }
    protected virtual void Start()
    {

    }
}
