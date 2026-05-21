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
        _direction = SnapToCardinal(toTarget);
    }

    private static readonly Vector2[] Cardinals =
    {
        GameConstants.Direction.TOP,
        GameConstants.Direction.RIGHT,
        GameConstants.Direction.LEFT,
        GameConstants.Direction.BOTTOM,
    };

    private static Vector2 SnapToCardinal(Vector2 dir)
    {
        Vector2 normalized = dir.normalized;
        Vector2 best = Cardinals[0];
        float bestDot = float.MinValue;

        foreach (Vector2 cardinal in Cardinals)
        {
            float dot = Vector2.Dot(normalized, cardinal);
            if (dot > bestDot)
            {
                bestDot = dot;
                best = cardinal;
            }
        }

        return best;
    }


}
