using System;
using UnityEngine;
public interface IObjecPoolService
{
    GameObject Spawn(Vector2 position, Quaternion rotation, GameObject prefab, Transform parent = null);
    void Release(GameObject poolObject, GameObject objectPrefab, Transform parent = null);
}