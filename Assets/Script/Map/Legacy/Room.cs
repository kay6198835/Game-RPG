using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public List<EntityData> entitiesData;
    public Transform spawnPosition;
    [SerializeField] public Entity enemyPrefab;

}
