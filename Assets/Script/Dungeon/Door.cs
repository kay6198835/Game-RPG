using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public enum DoorType
    {
        left, right, top, bottom
    }

    public DoorType doorType;

    public GameObject doorCollider;

    private GameObject player;

    private float widthOffSet = 6f;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag == "PLayer")
        {
            switch(doorType)
            {
                case DoorType.bottom: player.transform.position = new Vector2(transform.position.x, transform.position.y - widthOffSet); break;
                case DoorType.top: player.transform.position = new Vector2(transform.position.x, transform.position.y + widthOffSet); break;
                case DoorType.left: player.transform.position = new Vector2(transform.position.x - widthOffSet, transform.position.y); break;
                case DoorType.right: player.transform.position = new Vector2(transform.position.x + widthOffSet, transform.position.y); break;
            }
        }
    }
}
