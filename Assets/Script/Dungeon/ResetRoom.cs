using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetRoom : MonoBehaviour
{
    GameObject spawnPoint;

    GameObject player;

    private void Awake()
    {
        spawnPoint = GameObject.FindGameObjectWithTag("Spawn");
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player.transform.position = spawnPoint.transform.position;
            LoadNewRooms();
        }
    }

    void LoadNewRooms()
    {
        RoomController.instance.ResetRoom();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Destroy(gameObject);
    }
}
