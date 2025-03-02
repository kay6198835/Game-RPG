using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class ObjectPooling : MonoBehaviour
{
    public ObjectPooling instance { get => this; }
    public ObjectPooling Instance { get => instance; }

    [SerializeField] private List<Transform> listPrefab;
    [SerializeField] private List<Transform> listSpawn;
    [SerializeField] private GameObject prefab;


    #region List Spawn
    [SerializeField] private List<Transform> slashSpawn;
    #endregion
    #region Holder
    [SerializeField] private Transform holderSlash;
    #endregion
    private void Spawn(Vector3 positon, Quaternion rotation)
    {

        bool isReSpawn=false;
        foreach (Transform t in listSpawn)
        {
            if (t.name == prefab.name && !t.gameObject.activeInHierarchy)
            {
                isReSpawn = true;
                t.rotation = rotation;
                t.position = positon;
                t.gameObject.SetActive(true);
                listSpawn.Remove(t);
                break;
            }
        }
        if (isReSpawn == false)
        {
            Instantiate(prefab,positon,rotation,holderSlash);
        }
    }
    public void DeSpawnSlash()
    {

    }
    public void SpawnSlash(Vector3 positon, Quaternion rotation)
    {
        SetSpawn(slashSpawn);
        Spawn(positon, rotation);
    }
    private void SetSpawn(List<Transform> list)
    {
        listPrefab = list;
    }
}
