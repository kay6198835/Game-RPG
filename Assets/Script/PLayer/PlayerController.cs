using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]    WeaponsController weaponController;
    public LayerMask interactable;

    public PlayerMovement playerMovement;

    private GameObject currTele;

    private void Awake()
    {
        //playerMovement = GetComponent<PlayerMovement>();
    }

    public void HandleUpdate()
    {
        if (Input.GetKeyDown(KeyCode.E)) 
        {
            StartCoroutine(Interact());
        }
    }
    IEnumerator Interact()
    {
        var collider = Physics2D.OverlapCircle(transform.position, 1f, interactable);
        if(collider != null)
        {
            yield return collider.GetComponent<Interactable>()?.Interact();
        }
    }
    public PlayerMovement PlayerMovement => playerMovement;
}
