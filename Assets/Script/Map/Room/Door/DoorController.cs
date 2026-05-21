using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(SpriteRenderer))]
public class DoorController : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    private STATUS_DOOR status = STATUS_DOOR.CLOSE;
    Vector2 _direction = new Vector2();

    public void Awake()
    {
        spriteRenderer.GetComponent<SpriteRenderer>();
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player") || status == STATUS_DOOR.CLOSE)
        {
            return;
        }
        EventManager.Emit(EventID.ON_PLAYER_ON_DOOR, _direction);
    }

    public void Setting(Vector2 direction, STATUS_DOOR status)
    {
        _direction = direction;
        switch (status)
        {
            case STATUS_DOOR.CLOSE:
                this.status = STATUS_DOOR.CLOSE;
                break;

            case STATUS_DOOR.OPEN:
                this.status = STATUS_DOOR.BE_OPEN;
                spriteRenderer.color = Color.red;
                break;

            case STATUS_DOOR.BE_OPEN:
                this.status = STATUS_DOOR.BE_OPEN;
                spriteRenderer.color = Color.white;
                break;

            default:
                break;
        }
    }

    public void OpenDoor()
    {
        if (status == STATUS_DOOR.OPEN) return;
        status = STATUS_DOOR.OPEN;
        spriteRenderer.color = Color.white;
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
    public void SetDirectionFrom(Vector3 targetPosition)
    {
        Vector2 toTarget = (Vector2)(targetPosition - transform.position);
        _direction = ToCardinalDirection(toTarget);
    }
    // Maps any Vector2 to the nearest cardinal direction (TOP / BOTTOM / LEFT / RIGHT).
    // Strategy: whichever axis has the larger magnitude is the dominant one;
    // the sign of that axis then picks between the two opposing directions.
    // Example: dir = (-3, 1) → |x|=3 > |y|=1 → horizontal dominant → x<0 → LEFT
    private static Vector2 ToCardinalDirection(Vector2 dir)
    {
        bool dominantAxisIsHorizontal = Mathf.Abs(dir.x) >= Mathf.Abs(dir.y);

        if (dominantAxisIsHorizontal)
            return dir.x >= 0 ? GameConstants.Direction.RIGHT : GameConstants.Direction.LEFT;

        return dir.y >= 0 ? GameConstants.Direction.TOP : GameConstants.Direction.BOTTOM;
    }
}
