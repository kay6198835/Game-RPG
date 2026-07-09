using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    private Vector3[] spawnPosition;
    public void OnEnable()
    {
        EventManager.Resgister(EventID.ON_LOAD_MAP, OnDoneLoadRoomGrid);
    }
    public void OnDisable()
    {
        EventManager.UnResgister(EventID.ON_LOAD_MAP, OnDoneLoadRoomGrid);
    }
    public void Spawn()
    {

    }
    
}