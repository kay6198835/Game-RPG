using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;
[ExecuteInEditMode]

public class RoomController : MonoBehaviour
{
    [SerializeField] private float SCALE;
    [Range(0, 100)]
    [SerializeField] private float PADDING_SCALE = GameConstants.SettingStats.PLAYER_SCALE * GameConstants.SettingStats.LENGHT_ROOM / 10;
    [SerializeField] private Cell _cellData;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private DoorController _top, _left, _right, _bottom;
    [SerializeField] public Vector3 StartDoorPosition { get; private set; }
    public void AddCell(Cell cell)
    {
        this._cellData = cell;
        this.Setting();
    }
    public void Setting()
    {
        if (_cellData == null)
        {
            return;
        }
        this.transform.localScale = Vector3.one * SCALE;
        this.transform.SetPositionAndRotation(new Vector3(_cellData.Column, -_cellData.Row) * SCALE * GameConstants.SettingStats.LENGHT_ROOM, Quaternion.identity);
        _top.Setting(GameConstants.Direction.TOP, _cellData.Top);
        _left.Setting(GameConstants.Direction.LEFT, _cellData.Left);
        _right.Setting(GameConstants.Direction.RIGHT, _cellData.Right);
        _bottom.Setting(GameConstants.Direction.BOTTOM, _cellData.Bottom);
    }

    public void SetBeNextRoom(Vector2 direction)
    {
        _spriteRenderer.color = Color.red;
    }



    public Vector2 GetGridPosition()
    {
        var direction = new Vector2(_cellData.Column, _cellData.Row);
        return direction;
    }

    public void UpdateStatusDoor(Vector2 direction)
    {
        GetDoor(direction).OpenDoor();
    }

    public void GetStartDoorPosition(Vector2 direction)
    {
        var nextDoor = GetDoor(direction);
        nextDoor.OpenDoor();
        StartDoorPosition = nextDoor.gameObject.transform.position - direction.ConvertTo<Vector3>() * PADDING_SCALE * SCALE;
    }

    public DoorController GetDoor(Vector2 direction)
    {
        var door = new DoorController();
        if (direction == GameConstants.Direction.TOP)
        {
            door = _top;
        }
        else if (direction == GameConstants.Direction.RIGHT)
        {
            door = _right;
        }
        else if (direction == GameConstants.Direction.LEFT)
        {
            door = _left;
        }
        else if (direction == GameConstants.Direction.BOTTOM)
        {
            door = _bottom;
        }
        return door;
    }
}
