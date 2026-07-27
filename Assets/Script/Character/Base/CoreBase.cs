using System.Collections.Generic;
using UnityEngine;

public abstract class CoreBase : MonoBehaviour, ICore
{
    [SerializeField] public List<ICoreComponent<ICore>> coreComponents = new List<ICoreComponent<ICore>>();
    protected readonly Dictionary<System.Type, ICoreComponent<ICore>> _cache = new Dictionary<System.Type, ICoreComponent<ICore>>();

    protected virtual void Awake()
    {
        Setup();
    }
    public virtual void AddCoreComponent(ICoreComponent<ICore> coreComponent)
    {
        if (!coreComponents.Contains(coreComponent))
        {
            coreComponents.Add(coreComponent);
        }
    }
    public virtual void GetCoreComponent<T>(out T coreComponent) where T : ICoreComponent<ICore>
    {
        var type = typeof(T);
        if (_cache.TryGetValue(type, out var cached))
        {
            coreComponent = (T)cached;
            return;
        }
        coreComponent = (T)cached;
        foreach (var comp in coreComponents)
        {
            if (comp is T match)
            {
                coreComponent = match;
                _cache[type] = match;
                return;
            }
        }
    }
    public virtual void Setup()
    {
        var allCoreComponents = GetComponentsInChildren<ICoreComponent>(true);
        foreach (var comp in allCoreComponents)
        {
            AddCoreComponent((ICoreComponent<ICore>)comp);
        }
    }
}
