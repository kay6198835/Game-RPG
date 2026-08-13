using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    public Dictionary<GameObject, Pool> pools { get; set; } = new Dictionary<GameObject, Pool>();
    public Pool Get(GameObject prefab)
    {
        if (!pools.TryGetValue(prefab, out Pool pool))
        {
            Register(prefab);
        }
        return pools[prefab];
    }

    public void Spawn(Vector2 position, GameObject prefab)
    {
        var pool = Get(prefab);
        if (pool == null) return;
        pool.Spawn(position);
    }

    public GameObject Spawn(Vector2 position, Quaternion rotation, GameObject prefab)
    {
        var pool = Get(prefab);
        if (pool == null) return null;
        return pool.Spawn(position, rotation);
    }

    public void Register(GameObject prefab)
    {
        if (pools.TryGetValue(prefab, out Pool pool)) return;
        GameObject poolObj = new GameObject($"{prefab.name} Pool");
        poolObj.transform.parent = this.transform;
        pool = poolObj.AddComponent<Pool>();
        pool.Register(prefab);
        pools.Add(prefab, pool);
    }
}