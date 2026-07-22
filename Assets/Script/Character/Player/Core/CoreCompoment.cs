using UnityEngine;

public class CoreComponent : CoreComponentBase<T> where T : Core
{

    protected virtual void Awake()
    {
        core = transform.parent.GetComponent<T>();
        if (core != null) core.AddCoreComponent(this);
    }
}
