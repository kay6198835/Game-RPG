using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint;

    public Transform GetSpawnPoint()
    {
        return spawnPoint;
    }
}
