using UnityEngine;

public abstract class CoreComponentBase<T> : MonoBehaviour, ICoreComponent<T> where T : CoreBase
{
    [SerializeField] public T Core {get; private set; }

    protected virtual void Awake()
    {
        
    }

    protected virtual void Start()
    {
        
    }

    // public virtual void Setup()
    // {
    //     Core = transform.parent.GetComponent<T>();
    //     if (Core != null) Core.AddCoreComponent(this);
    // }

}
