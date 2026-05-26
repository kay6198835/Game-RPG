using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteInEditMode]
public class RoomCell : BaseCell
{
    private float SCALE = GameConstants.SettingStats.GAME_SCALE;
    private float PADDING_DOOR_TELE_SCALE = GameConstants.SettingStats.PADDING_DOOR_TELE_SCALE;
    //[SerializeField] private DoorController _top, _left, _right, _bottom;
    [SerializeField] private List<DoorController> listDoors;
    [SerializeField] public Vector3 StartDoorPosition { get; private set; }
    [SerializeField] public List<Vector2> ListDirectionDoors { get; private set; }

    protected override void Setting()
    {
        ListDirectionDoors = new List<Vector2>();
        if (_cellData == null) return;
        transform.localScale = Vector3.one * SCALE;
        transform.SetPositionAndRotation(
            new Vector3(_cellData.Column, -_cellData.Row) * SCALE * GameConstants.SettingStats.LENGTH_ROOM,
            Quaternion.identity);
        foreach (var (name, status) in _cellData.Doors)
            if (status != STATUS_DOOR.CLOSE)
                ListDirectionDoors.Add(GameConstants.Direction.NameToDirection[name]);
    }

    public void UpdateStatusDoor(Vector2 direction) => GetDoor(direction).OpenDoor();

    public void GetStartDoorPosition(Vector2 direction)
    {
        var nextDoor = GetDoor(direction);
        nextDoor.OpenDoor();
        StartDoorPosition = nextDoor.transform.position - direction.ConvertTo<Vector3>() * PADDING_DOOR_TELE_SCALE * SCALE*1.1f;
    }

    public DoorController GetDoor(Vector2 direction)
    {
        DoorController nextDoor = new DoorController();
        foreach (var door in listDoors)
        {
            if (door.GetDirection() == direction) nextDoor = door;
        }

        return nextDoor;
    }
}
