using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoreGameObject : MonoBehaviour
{
    public GameObject playerPrefab;
    private GameObject player;

    public GameObject Player { get => player;}
    private void Awake()
    {
        if(player == null)
        {
            player = Instantiate(playerPrefab, this.transform);
        }

        DontDestroyOnLoad(gameObject);
    }

    public void ResetGame()
    {
        Destroy(gameObject);
    }
}
