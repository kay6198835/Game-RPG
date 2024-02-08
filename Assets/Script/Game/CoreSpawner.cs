using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoreSpawner : MonoBehaviour
{
    [SerializeField] GameObject coreGamePrefab;

    private void Awake()
    {
        var existingObject = FindObjectsOfType<CoreGameObject>();

        if(existingObject.Length == 0) 
        {
            Instantiate(coreGamePrefab, new Vector3(0 ,0 ,0), Quaternion.identity);
        }
    }

    private void Start()
    {
        Time.timeScale = 1.0f;
    }
}
