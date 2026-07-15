using System.Collections.Generic;
using UnityEngine;

public abstract class CoreBase : MonoBehaviour
{
    [SerializeField] private List<CoreComponentBase> coreComponents = new List<CoreComponentBase>();
    private readonly Dictionary<System.Type, CoreComponentBase> _cache = new Dictionary<System.Type, CoreComponentBase>();

    public void AddCoreComponent(CoreComponentBase coreComponent)
    {
        if (!coreComponents.Contains(coreComponent)) coreComponents.Add(coreComponent);
    }

    public void GetCoreComponent<T>(out T coreComponent) where T : CoreComponentBase
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
