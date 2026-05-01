using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    [SerializeField] private int SCALE= 10;
    [SerializeField] private Cell _cellData;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private DoorController _top, _left, _right, _bottom;
    [SerializeField] private Transform StartDoorPosition {get;private set;}

    private void Awake()
    {
        _top.Setting(-GameConstants.Direction.TOP);
        _left.Setting(-GameConstants.Direction.LEFT);
        _right.Setting(-GameConstants.Direction.RIGHT);
        _bottom.Setting(-GameConstants.Direction.BOTTOM);
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
        this.transform.SetPositionAndRotation(new Vector3(_cellData.Column * this.transform.localScale.x, -_cellData.Row * this.transform.localScale.y)* SCALE, Quaternion.identity);

        if (_cellData.Top == (int)STATUS_DOOR.OPEN)
        {
            _top.OpenDoor();
        }
        if (_cellData.Right == (int)STATUS_DOOR.OPEN)
        {
            _right.OpenDoor();
        }
        if (_cellData.Left == (int)STATUS_DOOR.OPEN)
        {
            _left.OpenDoor();
        }
        if (_cellData.Bottom == (int)STATUS_DOOR.OPEN)
        {
            _bottom.OpenDoor();
        }
    }

    public void SetStartDoorPosition(Vector2 direction)
    {
        switch (direction)
        {
            case GameConstants.Direction.TOP:
            StartDoorPosition = _top.transform;
            break;
            case GameConstants.Direction.RIGHT:
            StartDoorPosition = _right.transform;
            break;
            case GameConstants.Direction.LEFT:
            StartDoorPosition = _left.transform;
            break;
            case GameConstants.Direction.BOTTOM:
            StartDoorPosition = _bottom.transform;
            break;
        }
    }



    public Vector2 GetGridPosition()
    {
        return new Vector2(_cellData.Column, _cellData.Row);
    }
}
