using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    public Dictionary<GameObject, Pool> pools { get; set; } = new Dictionary<GameObject, Pool>();
    public Pool Get(GameObject prefab, Transform parent = null)
    {
        if (!pools.TryGetValue(prefab, out Pool pool))
        {
            Register(prefab, parent);
        }
        return pools[prefab];
    }

    // public void Spawn(Vector2 position, Quaternion rotation = , GameObject prefab, Transform parent = null)
    // {
    //     var pool = Get(prefab);
    //     if (pool == null) return;
    //     pool.Spawn(position, rotation, parent);
    // }

    public GameObject Spawn(Vector2 position, Quaternion rotation, GameObject prefab, Transform parent = null)
    {
        var pool = Get(prefab, parent);
        if (pool == null) return null;
        return pool.Spawn(position, rotation, parent);
    }
    public void Release(GameObject prefab)
    {
        var pool = Get(prefab, parent);
        if (pool == null) return null;
        return pool.Release(prefab);
    }

    private void Register(GameObject prefab, Transform parent = null)
    {
        if (pools.TryGetValue(prefab, out Pool pool)) return;
        GameObject poolObj = parent == null ? new GameObject($"{prefab.name} Pool") : parent.gameObject;
        poolObj.transform.parent = parent == null ? this.transform : parent.parent;
        pool = poolObj.AddComponent<Pool>();
        pool.Register(prefab);
        pools.Add(prefab, pool);
    }
}