using System.Collections.Generic;
using UnityEngine;

public abstract class CoreBase : MonoBehaviour, ICore
{
    [SerializeField] private List<ICoreComponentBase> coreComponents = new List<ICoreComponentBase>();
    private readonly Dictionary<System.Type, ICoreComponentBase> _cache = new Dictionary<System.Type, ICoreComponentBase>();

    public void AddCoreComponent(ICoreComponentBase coreComponent)
    {
        if (!coreComponents.Contains(coreComponent)) coreComponents.Add(coreComponent);
    }

    public void GetCoreComponent<T>(out T coreComponent) where T : ICoreComponentBase
    {
        var type = typeof(T);
        if (_cache.TryGetValue(type, out var cached))
        {
            coreComponent = (T)cached;
            return;
        }
        coreComponent = null;
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
}
