using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCharacter : MonoBehaviour
{
    [SerializeField] List<GameObject> character;
    [SerializeField] List<Transform> SpawnPosition;

    [SerializeField] List<EntityData> entitiesData;
    [SerializeField] Entity enemyPrefab;


    [SerializeField] float distance;

    //Character character1;
    // Start is called before the first frame update
    void Start()
    {
        
        for(int i = 0; i < character.Count; i++)
        {
            Instantiate(character[i].gameObject,this.transform.position+Vector3.up*i*distance,Quaternion.identity);
        }

        for (int i = 0; i < entitiesData.Count; i++)
        {
            enemyPrefab.SetDataEntity(entitiesData[i]);
            Instantiate(enemyPrefab, this.transform.position + Vector3.up * i * distance, Quaternion.identity);
        }
    }
}
