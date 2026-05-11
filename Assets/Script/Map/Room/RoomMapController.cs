using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomMapController : MonoBehaviour, IMapController<RoomController>
{
    [SerializeField] RoomController prefabObject;
    [SerializeField] List<RoomController> _roomControllers;
    [SerializeField] RoomController _current,_next;
    public int Columns { get; private set; }
    public int Rows { get; private set; }

    public void OnLoadMap()
    {
        _current = _next;
        _next = null;
        Debug.Log("CALL LOADMAP");
    }
    public void AddCell(Cell cell)
    {
        RoomController roomController = Instantiate(prefabObject, this.transform) as RoomController;
        roomController.AddCell(cell);
        _roomControllers.Add(roomController);
    }
    public RoomController GetValue(int index)
    {
        var roomController = _roomControllers[index];
        return roomController;
    }

    public void CheckValidateNextDoor(int index)
    {
        if (index < 0 || index >= _roomControllers.Count) return;
    }

    public void SetValue(int index, RoomController roomController)
    {
        _roomControllers[index] = roomController;
    }

    public Transform GetStartDoorPosition(RoomController current, RoomController next)
    {
        return transform;
    }

    public void Setting(int Columns,int Rows)
    {
        this.Columns = Columns;
        this.Rows = Rows;
        _current = this._roomControllers[0];
    }

    public RoomController GetNext(Vector2 direction)
    {
        Debug.Log("GetNext");
        direction.y = -direction.y;
        var positionNextRoom = _current.GetGridPosition() + direction;
        int index = (int)positionNextRoom.y * this.Columns + (int)positionNextRoom.x;
        _next = GetValue(index);
        direction.y = -direction.y;
        _next.GetStartDoorPosition(-direction);
        _current.UpdateStatusDoor(direction);

        return _next;
    }

    public RoomController GetStart()
    {
        var start = GetValue(0);
        return start;
    }
}
