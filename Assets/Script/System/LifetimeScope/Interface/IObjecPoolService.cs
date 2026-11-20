using System;
using UnityEngine;
public interface IObjectPoolService
{
    GameObject Spawn(GameObject prefab, Vector2 position, Quaternion rotation, Transform parent = null);
    void Despawn(GameObject objectPool, Transform parent = null);
}