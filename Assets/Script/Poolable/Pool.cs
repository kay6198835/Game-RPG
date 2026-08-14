using System.Collections.Generic;
using UnityEngine;
public class Pool : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private Queue<PoolMember> inactiveObjects = new Queue<PoolMember>();
    public void Spawn(Vector2 position)
    {
        Spawn(position, Quaternion.identity);
    }

    public GameObject Spawn(Vector2 position, Quaternion rotation)
    {
        if (inactiveObjects.Count == 0)
        {
            GameObject obj = Instantiate(prefab, position, rotation, transform);
            if (!obj.TryGetComponent<PoolMember>(out PoolMember member))
            {
                member = obj.AddComponent<PoolMember>();
            }
            member = obj.GetComponent<PoolMember>();
            member.Initialize(this);
            return obj;
        }
        return Reload(position, rotation);
    }

    public void Reload(Vector2 position)
    {
        Reload(position, Quaternion.identity);
    }

    public GameObject Reload(Vector2 position, Quaternion rotation)
    {
        var reload = inactiveObjects.Dequeue();
        reload.gameObject.SetActive(true);
        reload.gameObject.transform.SetPositionAndRotation(position, rotation);
        reload.SwitchIsInPool(false);
        return reload.gameObject;
    }

    public void Release(GameObject gameObject)
    {
        gameObject.SetActive(false);
        if (!gameObject.TryGetComponent<PoolMember>(out PoolMember member))
        {
            member = gameObject.AddComponent<PoolMember>();
        }

        if (member.isInPool) return;
        inactiveObjects.Enqueue(member);
        member.SwitchIsInPool(true);
    }

    public void Register(GameObject gameObject)
    {
        this.prefab = gameObject;
    }
}