using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemToSpawn
{
    public GameObject item;
    public float spawnRate;
    [HideInInspector] public float minSpawnProb, maxSpawnProb;
}

public class Chest : MonoBehaviour, Interactable
{
    public ItemToSpawn[] itemToSpawn;

    private void Start()
    {
        for (int i = 0; i < itemToSpawn.Length; i++)
        {
            if (i == 0)
            {
                itemToSpawn[i].minSpawnProb = 0;
                itemToSpawn[i].maxSpawnProb = itemToSpawn[i].spawnRate - 1;
            }
            else
            {
                itemToSpawn[i].minSpawnProb = itemToSpawn[i - 1].maxSpawnProb;
                itemToSpawn[i].maxSpawnProb = itemToSpawn[i].minSpawnProb + itemToSpawn[i].spawnRate - 1;
            }
        }
    }

    public IEnumerator Interact()
    {
        Spawn();
        Destroy(gameObject);
        yield return null;
    }

    void Spawn()
    {
        float randomNum = Random.Range(0, 100);

        for (int i = 0; i < itemToSpawn.Length; i++)
        {
            if (randomNum >= itemToSpawn[i].minSpawnProb && randomNum <= itemToSpawn[i].maxSpawnProb)
            {
                Instantiate(itemToSpawn[i].item, transform.position, Quaternion.identity);
                break;
            }
        }
    }

}
