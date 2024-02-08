using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTeleport : MonoBehaviour
{
    private GameObject currTele;

    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if(currTele != null)
            {
                transform.position = currTele.GetComponent<Teleport>().GetSpawnPoint().position;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Reload")) 
        {
            currTele = collision.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Reload"))
        {
            currTele = null;
        }
    }
}
