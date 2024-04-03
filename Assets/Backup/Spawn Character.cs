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
        Instantiate(character[0].gameObject);
        Instantiate(character[1].gameObject, (Vector2)character[0].transform.position+Vector2.right* distance, Quaternion.identity);
    }
}
