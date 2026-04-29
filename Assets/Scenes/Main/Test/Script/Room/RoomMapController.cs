using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomMapController : MonoBehaviour, IMapController
{
    [SerializeField] RoomController prefabObject;
    [SerializeField] List<RoomController> _roomControlls;
    [SerializeField] RoomController _current,_next;

    private void OnEnable()
    {

    }

    private void OnDisable()
    {
        
    }
    public void AddCell(Cell cell)
    {
        RoomController roomControll = Instantiate(prefabObject, this.transform) as RoomController;
        roomControll.AddCell(cell);
        _roomControlls.Add(roomControll);
    }
    public RoomController GetValue(int index)
    {
        var roomControll = _roomControlls[index];
        return roomControll;
    }

    public void CheckValidateNextDoor(int index)
    {
        if (index < 0 || index >= _roomControlls.Count) return;
    }

    public void SetValue(int index, RoomController roomController)
    {
        _roomControlls[index] = roomController;
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
        _next = _roomMapController.GetValue(index);
        _next.SetStartDoorPosition(direction);
        return _next;
    }

    public RoomController GetStartRoom()
    {
        var start = GetValue(0);
        return start;
    }
}
