using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomMapController : MonoBehaviour, IMapController
{
    [SerializeField] int Columns,Rows;
    [SerializeField] RoomController prefabObject;
    [SerializeField] List<RoomController> _roomControllers;
    [SerializeField] RoomController _current,_next;

    private void OnEnable()
    {

    }

    private void OnDisable()
    {
        
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
    public void Clear()
    {
        throw new System.NotImplementedException();
    }

    public void Setting()
    {
        throw new System.NotImplementedException();
    }

    public void Setup()
    {
        throw new System.NotImplementedException();
    }

    public RoomController GetNextRoom(Vector2 direction)
    {
        var positionNextRoom = _current.GetGridPosition() + direction;
        int index = (int)positionNextRoom.y * -this.Columns + (int)positionNextRoom.x;
        _next = GetValue(index);
        _next.SetStartDoorPosition(direction);
        return _next;
    }

    public RoomController GetStartRoom()
    {
        var start = GetValue(0);
        return start;
    }
}
