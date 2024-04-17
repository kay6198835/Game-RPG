using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCharacter : MonoBehaviour
{
    [SerializeField] List<GameObject> character;
    [SerializeField] float distance;

    //Character character1;
    // Start is called before the first frame update
    void Start()
    {
        
        for(int i = 0; i < character.Count; i++)
        {
            Instantiate(character[i].gameObject,Vector3.one*i*distance,Quaternion.identity);
        }
    }
}
