using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCharacter : MonoBehaviour
{
    [SerializeField] List<GameObject> character;

    Character character1;
    // Start is called before the first frame update
    void Start()
    {
        Instantiate(character[0].gameObject);
    }
}
