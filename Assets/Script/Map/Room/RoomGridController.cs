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
    private int enemyCount = 0;
    public override void Setting(int columns, int rows)
    {
        base.Setting(columns, rows);
        roomGeneraterController = GetComponent<RoomGeneraterController>();
        Vector2 position = new Vector2(MazeController.Instance.GetCellStart().Row, MazeController.Instance.GetCellStart().Column);
        _startIndex = CaculateIndex(position);

        position.y = MazeController.Instance.GetCellEnd().Row;
        position.x = MazeController.Instance.GetCellEnd().Column;
        _endIndex = CaculateIndex(position);
        roomGeneraterController.Setting(_startIndex, _endIndex, _list.Count);
    }
    public void OnEnable()
    {
        EventManager.Resgister(EventID.ON_LOAD_MAZE_DONE, OnDoneLoadRoomGrid);
        EventManager.Resgister(EventID.ON_CLEAR_ENEMY, DeleteDoorTileMap);
        EventManager.Resgister(EventID.ON_PLAYER_ON_DOOR, ClearRoom);
        EventManager.Resgister(EventID.ON_ENEMY_DEATH, OnEnemyDeath);
        EventManager.Resgister(EventID.ON_DONE_SPAWN_ENEMY, OnDoneSpawnEnemy);
        EventManager.Resgister(EventID.ON_SPAWN_EXTRA_ENEMY, OnSpawnExtraEnemy);
    }
    public void OnDisable()
    {
        EventManager.UnResgister(EventID.ON_LOAD_MAZE_DONE, OnDoneLoadRoomGrid);
        EventManager.UnResgister(EventID.ON_CLEAR_ENEMY, DeleteDoorTileMap);
        EventManager.UnResgister(EventID.ON_PLAYER_ON_DOOR, ClearRoom);
        EventManager.UnResgister(EventID.ON_ENEMY_DEATH, OnEnemyDeath);
        EventManager.UnResgister(EventID.ON_DONE_SPAWN_ENEMY, OnDoneSpawnEnemy);
        EventManager.UnResgister(EventID.ON_SPAWN_EXTRA_ENEMY, OnSpawnExtraEnemy);
    }

    public void OnEnemyDeath(object obj = null)
    {
        _current.OnEnemyDeath();
    }

    public void OnDoneSpawnEnemy(object obj = null)
    {
        _current.OnDoneSpawnEnemy((int)obj);
    }

    public void OnSpawnExtraEnemy(object obj = null)
    {
        _current.OnSpawnExtraEnemy();
    }

    // This method is called when the player interacts with a door to transition to the next room.
    public void OnLoadMap(Vector2 directionToNextMap)
    {
        _next = GetNext(directionToNextMap);
        int index = CaculateIndex(_next.GetGridPosition());
        //Check why can't get right GetStartDoorPosition when go to room the first time

        
        _current = _next;
        _current.UpdateStatusDoor(directionToNextMap);
        this.roomGeneraterController.LoadRoom(index, _current);
        _current.GetStartDoorPosition(-directionToNextMap);
        roomGeneraterController.SetNextRoom(_next.StartDoorPosition);
        _next = null;
        EventManager.Emit(EventID.ON_LOAD_MAP, index);
    }

    // This method is called when the maze generation is done, and it loads the initial room based on the start index.
    public void OnDoneLoadRoomGrid(object obj = null)
    {
        _current = GetValue(_startIndex);
        this.roomGeneraterController.LoadRoom(_startIndex, _current);
        //this.roomGeneraterController._fastMovement.transform.SetPositionAndRotation(this._current.StartDoorPosition, Quaternion.identity);
    }
    public void ClearRoom(object obj = null)
    {
        roomGeneraterController.ClearRoom(_current);
        this.OnLoadMap((Vector2)obj);
    }
    public void OnPlayerOnDoor(object obj = null)
    {
        OnLoadMap((Vector2)obj);
    }

    private void DeleteDoorTileMap(object obj = null)
    {
        roomGeneraterController.DeleteDoorTileMap(_current);
    }
}
