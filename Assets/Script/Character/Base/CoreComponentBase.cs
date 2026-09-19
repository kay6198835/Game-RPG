using UnityEngine;

public abstract class CoreComponentBase<T> : MonoBehaviour, ICoreComponent<T> where T : CoreBase
{
    [SerializeField] protected T core;
    public T Core { get => core; set => core = value; }

    protected virtual void Awake()
    {
        Setup();
    }

    protected virtual void Start()
    {

    }

    public virtual void Setup()
    {
        core = transform.GetComponentInParent<T>();
    }

}
