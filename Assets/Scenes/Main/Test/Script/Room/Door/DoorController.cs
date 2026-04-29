using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
[RequireComponent(typeof(SpriteRenderer))]
public class DoorController : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    private bool isOpen = false;
    Vector2 _direction = new Vector2();
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")||!isOpen)
        {
            return;
        }
        EventManager.Emit(EventID.ON_PLAYER_ON_DOOR, _direction);
    }

    public void Setting(Vector2 direction)
    {
        _direction = direction;
    }

    public void OpenDoor()
    {
        isOpen = true;
        spriteRenderer.color = Color.red;
    }
}
