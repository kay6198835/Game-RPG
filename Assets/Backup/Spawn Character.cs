using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCharacterTest : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] GameObject weapon;
    [SerializeField] GameObject enemy;

    [SerializeField] bool isEnemy;
    [SerializeField] bool isWeapon;
    [SerializeField] bool isPlayer;

    [SerializeField] float distance;


    //Character character1;
    // Start is called before the first frame update
    void Start()
    {
        if (isEnemy)
        {
            Instantiate(enemy,transform.position+Vector3.one*distance,Quaternion.identity);
        }
        if (isWeapon)
        {
            Instantiate(weapon, transform.position + Vector3.one*2 * distance, Quaternion.identity);
        }
        if (isPlayer)
        {
            Instantiate(player, transform.position + Vector3.one*3 * distance, Quaternion.identity);
        }
    }
}
