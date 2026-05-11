using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    [SerializeField] private int SCALE = 10;
    [SerializeField] private Cell _cellData;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private DoorController _top, _left, _right, _bottom;
    [SerializeField] public Transform StartDoorPosition { get; private set; }

    private void Awake()
    {

    }
    private void OnEnable()
    {

    }

    private void OnDisable()
    {

    }
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
        this.transform.SetPositionAndRotation(new Vector3(_cellData.Column * this.transform.localScale.x, -_cellData.Row * this.transform.localScale.y) * SCALE, Quaternion.identity);
        _top.Setting(GameConstants.Direction.TOP, _cellData.Top);
        _left.Setting(GameConstants.Direction.LEFT, _cellData.Left);
        _right.Setting(GameConstants.Direction.RIGHT, _cellData.Right);
        _bottom.Setting(GameConstants.Direction.BOTTOM, _cellData.Bottom);
    }

    public void SetBeNextRoom(Vector2 direction)
    {
        _spriteRenderer.color = Color.red;
    }
    public void SetStartDoorPosition(Vector2 direction)
    {
        var nextDoor = new DoorController();
        if (direction == GameConstants.Direction.TOP)
        {
            nextDoor = _top;
        }
        else if (direction == GameConstants.Direction.RIGHT)
        {
            nextDoor = _right;
        }
        else if (direction == GameConstants.Direction.LEFT)
        {
            nextDoor = _left;
        }
        else if (direction == GameConstants.Direction.BOTTOM)
        {
            nextDoor = _bottom;
        }
        StartDoorPosition = nextDoor.transform;
        nextDoor.OpenDoor();
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
        StartDoorPosition = nextDoor.gameObject.transform;
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
