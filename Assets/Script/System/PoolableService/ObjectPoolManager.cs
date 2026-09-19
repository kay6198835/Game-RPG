using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class ObjectPoolManager : MonoBehaviour, IObjecPoolService
{
    public Dictionary<GameObject, Pool> pools { get; set; } = new Dictionary<GameObject, Pool>();
    private IObjectResolver resolver;

    [Inject]
    public void Construct(IObjectResolver resolver)
    {
        this.resolver = resolver;
    }

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
        Spawn(request.Prefab, request.Position, request.Rotation, request.Parent);
    }

    public GameObject Spawn(GameObject prefab, Vector2 position, Quaternion rotation, Transform parent = null)
    {
        var pool = Get(prefab, parent);
        if (pool == null) return null;
        return pool.Spawn(position, rotation, parent);
    }
    public void Release(GameObject poolObject, Transform parent = null)
    {
        if (!poolObject.TryGetComponent(out PoolMember member))
            return;

        Pool pool = member.GetPool();

        if (pool == null)
            return;

        pool.Release(poolObject, parent);
    }

    private void Register(GameObject prefab, Transform parent = null)
    {
        if (pools.TryGetValue(prefab, out Pool pool)) return;
        GameObject poolObj = parent == null ? new GameObject($"{prefab.name} Pool") : parent.gameObject;
        poolObj.transform.parent = parent == null ? this.transform : parent.parent;
        pool = poolObj.AddComponent<Pool>();
        pool.SetResolver(resolver);
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
        Parent = null;
    }
    ObjectPoolRequest(Vector2 position, Quaternion rotation, GameObject prefab, Transform parent)
    {
        Position = position;
        Rotation = rotation;
        Prefab = prefab;
        Parent = parent;
    }
}