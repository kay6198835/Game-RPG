using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSpawnEnemy : MonoBehaviour
{
    public GameObject enemy;
    public int number;
    private void Reset()
    {
        for (int i = 0; i < number; i++)
        {
            
        }
    }
    private void Update()
    {
        
    }
    private void Start()
    {
        Instantiate(enemy);
    }
}
