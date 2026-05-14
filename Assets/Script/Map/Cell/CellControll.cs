using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
[ExecuteInEditMode]
[RequireComponent(typeof(SpriteRenderer))]
public class CellController : MonoBehaviour
{
    private float CELL_SCALE = GameConstants.SettingStats.GAME_SCALE;
    private float PADDING_SCALE = GameConstants.SettingStats.LENGHT_CELL * 2;
    [SerializeField] private Cell _cellData;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private GameObject _top, _left, _right, _bottom;
    [SerializeField] public Transform StartDoorPosition { get; private set; }

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
    public void Setting()
    {
        if (_cellData == null)
        {
            return;
        }
        this.transform.localScale = Vector3.one * CELL_SCALE;
        this.transform.SetPositionAndRotation(MazeController.Instance.CellMapController.transform.position + new Vector3(_cellData.Column, -_cellData.Row) * CELL_SCALE * PADDING_SCALE, Quaternion.identity);
        _right.transform.SetLocalPositionAndRotation(Vector3.right, _right.transform.rotation);
        _top.transform.SetLocalPositionAndRotation(Vector3.up, _top.transform.rotation);
        _left.transform.SetLocalPositionAndRotation(Vector3.left, _left.transform.rotation);
        _bottom.transform.SetLocalPositionAndRotation(Vector3.down, _bottom.transform.rotation);
        if(_cellData.Top == STATUS_DOOR.OPEN) _top.SetActive(true);
        if(_cellData.Left == STATUS_DOOR.OPEN) _left.SetActive(true);
        if(_cellData.Right == STATUS_DOOR.OPEN) _right.SetActive(true);
        if(_cellData.Bottom == STATUS_DOOR.OPEN) _bottom.SetActive(true);



    }


    public void SetBeNextRoom(Vector2 direction)
    {
        _spriteRenderer.color = Color.red;
    }
    //public void SetStartDoorPosition(Vector2 direction)
    //{
    //    var nextDoor = new DoorController();
    //    if (direction == GameConstants.Direction.TOP)
    //    {
    //        nextDoor = _top;
    //    }
    //    else if (direction == GameConstants.Direction.RIGHT)
    //    {
    //        nextDoor = _right;
    //    }
    //    else if (direction == GameConstants.Direction.LEFT)
    //    {
    //        nextDoor = _left;
    //    }
    //    else if (direction == GameConstants.Direction.BOTTOM)
    //    {
    //        nextDoor = _bottom;
    //    }
    //    StartDoorPosition = nextDoor.transform;
    //    nextDoor.OpenDoor();
    //}



    public Vector2 GetGridPosition()
    {
        var direction = new Vector2(_cellData.Column, _cellData.Row);
        return direction;
    }

    //public void UpdateStatusDoor(Vector2 direction)
    //{
    //    GetDoor(direction).OpenDoor();
    //}

    //public void GetStartDoorPosition(Vector2 direction)
    //{
    //    var nextDoor = GetDoor(direction);
    //    nextDoor.OpenDoor();
    //    StartDoorPosition = nextDoor.gameObject.transform;
    //}
    public void AddCell(Cell cell)
    {
        this._cellData = cell;
        this.Setting();
    }
    //public DoorController GetDoor(Vector2 direction)
    //{
    //    var door = new DoorController();
    //    if (direction == GameConstants.Direction.TOP)
    //    {
    //        door = _top;
    //    }
    //    else if (direction == GameConstants.Direction.RIGHT)
    //    {
    //        door = _right;
    //    }
    //    else if (direction == GameConstants.Direction.LEFT)
    //    {
    //        door = _left;
    //    }
    //    else if (direction == GameConstants.Direction.BOTTOM)
    //    {
    //        door = _bottom;
    //    }
    //    return door;
    //}
}
