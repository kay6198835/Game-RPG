using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    public string sceneToLoad;

    [SerializeField] Transform spawnPoint;

    PlayerController player;

    public bool isReset = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(SwitchScene());
        }    
    }

    IEnumerator SwitchScene()
    {
        DontDestroyOnLoad(gameObject);

        yield return SceneManager.LoadSceneAsync(sceneToLoad);

        var desPortal = FindObjectsOfType<Portal>().First(x => x != this);
        player.PlayerMovement.SetPosition(desPortal.SpawnPoint.position);

        Destroy(gameObject);
    }

    public Transform SpawnPoint => spawnPoint;

}
