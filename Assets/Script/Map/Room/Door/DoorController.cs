using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class DoorController : MonoBehaviour
{
    private STATUS_DOOR status = STATUS_DOOR.CLOSE;
    Vector2 _direction = new Vector2();
    [SerializeField] string nameDoor = "dasdasdasdasd";
    string Name;
    public void Awake()
    {
        Debug.Log("Player on door " + nameDoor);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (!collision.CompareTag("Player") || status == STATUS_DOOR.CLOSE)
        {
            return;
        }
        EventManager.Emit(EventID.ON_PLAYER_ON_DOOR, _direction);
    }

    //public void Setting(Vector3 targetPosition, STATUS_DOOR status)
    //{
    //    this.SetDirection(targetPosition);
    //    this.SetStatus(status);
    //}

    public void OpenDoor()
    {
        if (status == STATUS_DOOR.OPEN) return;
        status = STATUS_DOOR.OPEN;
    }

    public void CheckCanBeOpened()
    {
        if (status == STATUS_DOOR.CLOSE)
        {
            return;
        }
    }

    // Computes direction from this door's position to targetPosition,
    // then snaps _direction to the nearest cardinal (Top/Right/Left/Bottom).
    public void SetDirection(Vector3 targetPosition)
    {
        Vector2 toTarget = (Vector2)(targetPosition - transform.position);
        _direction = Utility.ToCardinalDirection(toTarget);
    }

    public void SetStatus(STATUS_DOOR status)
    {
        this.status = status;
    }

    public STATUS_DOOR GetStatus()
    {
        return status;
    }

    public Vector2 GetDirection()
    {
        return _direction;
    }
}
