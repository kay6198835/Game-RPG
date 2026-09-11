using System;
using UnityEngine;
public interface IObjecPoolService
{
    GameObject Spawn(GameObject prefab, Vector2 position, Quaternion rotation, Transform parent = null);
    void Release(GameObject objectPool, Transform parent = null);
}