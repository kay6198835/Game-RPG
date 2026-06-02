using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(BoxCollider2D))]
public class DoorController : MonoBehaviour
{
    [SerializeField] public STATUS_DOOR Status { get; private set; } = STATUS_DOOR.DISABLE;
    [SerializeField] Vector2 _direction = new Vector2();
    [SerializeField] BoxCollider2D _collider;
    [SerializeField] string Name;

    void Awake()
    {
        _collider = GetComponent<BoxCollider2D>();
        _collider.size /= GameConstants.SettingStats.GAME_SCALE;;

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (!collision.CompareTag("Player") || Status == STATUS_DOOR.DISABLE)
        {
            return;
        }
        EventManager.Emit(EventID.ON_PLAYER_ON_DOOR, _direction);
    }

    public void OpenDoor()
    {
        if (Status == STATUS_DOOR.DISABLE) Status = STATUS_DOOR.ENEBLE;
    }

    public void CheckCanBeOpened()
    {
        if (Status == STATUS_DOOR.DISABLE)
        {
            return;
        }
    }

    // Computes direction from this door's position to targetPosition,
    // then snaps _direction to the nearest cardinal (Top/Right/Left/Bottom).
    public void SetDirection(string name)
    {
        this.Name = name;
        _direction = GameConstants.Direction.NameToDirection[name];
    }

    public void SetStatus(STATUS_DOOR status)
    {
        this.Status = status;
        SwitchColiderByStatus();
    }

    public Vector2 GetDirection()
    {
        return _direction;
    }

    private void SwitchColiderByStatus()
    {
        _collider.enabled = Status == STATUS_DOOR.OPEN;
    }

}
