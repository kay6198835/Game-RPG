using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour, IObjecPoolService
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

    public void Spawn(ObjectPoolRequest request)
    {
        Spawn(request.position, request.rotation, request.parent, request.parent);
    }

    public GameObject Spawn(Vector2 position, Quaternion rotation, GameObject prefab, Transform parent = null)
    {
        var pool = Get(prefab, parent);
        if (pool == null) return null;
        return pool.Spawn(position, rotation, parent);
    }
    public void Release(GameObject poolObject, Transform parent = null)
    {
        if (!poolObject.TryGetComponent<PoolMember>(out PoolMember member))
            return;

        Pool pool = member.GetPool();

        if (pool == null)
            return;

        pool.Release(poolObject, parent);
    }

    private void Register(GameObject prefab, Transform parent = null)
    {
        if (pools.TryGetValue(prefab, out Pool pool)) return;
        GameObject poolObj = new GameObject($"{prefab.name} Pool");
        poolObj.transform.parent = this.transform;
        pool = poolObj.AddComponent<Pool>();
        pool.Register(prefab);
        pools.Add(prefab, pool);
    }
}

public class ObjectPoolRequest
{
    public Vector2 Position;
    public Quaternion Rotation;
    public GameObject Prefab;
    public Transform Parent = null;
    ObjectPoolRequest(Vector2 position, GameObject prefab)
    {
        Position = position;
        Rotation = Quaternion.identity;
        Prefab = prefab;
        parent = null;
    }
    ObjectPoolRequest(Vector2 position, Quaternion rotation, GameObject prefab, Transform parent)
    {
        Position = position;
        Rotation = rotation;
        Prefab = prefab;
        Parent = parent;
    }
}