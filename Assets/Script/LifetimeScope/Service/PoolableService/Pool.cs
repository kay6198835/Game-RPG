using System.Collections.Generic;
using UnityEngine;
public class Pool : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private Queue<PoolMember> inactiveObjects = new Queue<PoolMember>();
    public void Spawn(Vector2 position, Transform parent = null)
    {
        Spawn(position, Quaternion.identity, parent);
    }

    public GameObject Spawn(Vector2 position, Quaternion rotation, Transform parent = null)
    {
        if (inactiveObjects.Count == 0)
        {
            GameObject obj = Instantiate(prefab, position, rotation, parent == null ? transform : parent);
            if (!obj.TryGetComponent<PoolMember>(out PoolMember member))
            {
                member = obj.AddComponent<PoolMember>();
            }
            member = obj.GetComponent<PoolMember>();
            member.Initialize(this);
            return obj;
        }
        return Reload(position, rotation, parent);
    }

    private void Reload(Vector2 position, Transform parent = null)
    {
        Reload(position, Quaternion.identity, parent);
    }

    private GameObject Reload(Vector2 position, Quaternion rotation, Transform parent = null)
    {
        var reload = inactiveObjects.Dequeue();
        reload.gameObject.SetActive(true);
        reload.gameObject.transform.SetPositionAndRotation(position, rotation);
        reload.transform.parent = parent == null ? this.transform : parent;
        reload.SwitchIsInPool(false);
        return reload.gameObject;
    }

    public void Release(GameObject gameObject, Transform parent = null)
    {
        gameObject.SetActive(false);
        if (!gameObject.TryGetComponent<PoolMember>(out PoolMember member))
        {
            member = gameObject.AddComponent<PoolMember>();
        }

        if (member.isInPool) return;
        gameObject.transform.parent = parent == null ? this.transform : parent;
        inactiveObjects.Enqueue(member);
        member.SwitchIsInPool(true);
    }

    public void Register(GameObject gameObject)
    {
        this.prefab = gameObject;
    }
}