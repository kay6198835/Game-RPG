using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
[RequireComponent(typeof(RoomGeneraterController))]
public class RoomGridController : BaseGrid<RoomCell>
{
    [SerializeField] public RoomGeneraterController roomGeneraterController;
    [SerializeField] private int _startIndex;
    [SerializeField] private int _endIndex;
    public override void Setting(int columns, int rows)
    {
        base.Setting(columns, rows);
        roomGeneraterController = GetComponent<RoomGeneraterController>();
        Vector2 position = new Vector2();
        position.y = MazeController.Instance.GetCellStart().Row;
        position.x = MazeController.Instance.GetCellStart().Column;
        _startIndex = CaculateIndex(position);

        position.y = MazeController.Instance.GetCellEnd().Row;
        position.x = MazeController.Instance.GetCellEnd().Column;
        _endIndex = CaculateIndex(position);
        roomGeneraterController.Setting(_startIndex, _endIndex,_list.Count);
    }
    public void OnEnable()
    {
        EventManager.Resgister(EventID.ON_LOAD_MAZE_DONE, OnDoneLoadRoomGrid);
        EventManager.Resgister(EventID.ON_CLEAR_ENEMY, DeleteDoorTileMap);
        EventManager.Resgister(EventID.ON_PLAYER_ON_DOOR, ClearRoom);
    }
    public void OnDisable()
    {
        EventManager.UnResgister(EventID.ON_LOAD_MAZE_DONE, OnDoneLoadRoomGrid);
        EventManager.UnResgister(EventID.ON_CLEAR_ENEMY, DeleteDoorTileMap);
        EventManager.UnResgister(EventID.ON_PLAYER_ON_DOOR, ClearRoom);
    }
    public void OnLoadMap(Vector2 directionToNextMap)
    {
        _next = GetNext(directionToNextMap);
        int index = CaculateIndex(_next.GetGridPosition());
        this.roomGeneraterController.LoadRoom(index, _next);
        _next.GetStartDoorPosition(-directionToNextMap);
        _current.UpdateStatusDoor(directionToNextMap);
        roomGeneraterController._fastMovement.transform.SetPositionAndRotation(_next.StartDoorPosition, Quaternion.identity);
        _current = _next;
        _next = null;
        EventManager.Emit(EventID.ON_LOAD_MAP, index);
    }

    public void OnDoneLoadRoomGrid(object obj = null)
    {
        _current = GetValue(_startIndex);
        this.roomGeneraterController.LoadRoom(_startIndex, _current);
        //this.roomGeneraterController._fastMovement.transform.SetPositionAndRotation(this._current.StartDoorPosition, Quaternion.identity);
    }
    public void ClearRoom(object obj = null)
    {
        roomGeneraterController.ClearRoom(ref _current);
        this.OnLoadMap((Vector2)obj);
    }

    private void DeleteDoorTileMap(object obj = null)
    {
        roomGeneraterController.DeleteDoorTileMap(_current);
    }
}
