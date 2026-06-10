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

    [SerializeField] private DoorController doorPrefab;
    [SerializeField] private List<DoorController> _listDoors = new List<DoorController>();
    [SerializeField] public Vector3 StartDoorPosition { get; private set; }
    [SerializeField] public List<Vector2> ListDirectionDoors { get; private set; }

    // parameter levelData is used to set the room as cleared and open doors,
    //  also store the data of the room for reloading when player come back to this room
    [SerializeField] public bool IsCleared { get; private set; } = false;
    [SerializeField] public LevelData Data { get; private set; } = new LevelData();
    [SerializeField] public List<DoorPoint> DoorPoints { get; private set; } = new List<DoorPoint>();
    [SerializeField] public List<int> IndexLevelDataDoor { get; private set; } = new List<int>();
    protected override void Setting()
    {
        ListDirectionDoors = new List<Vector2>();
        if (_cellData == null) return;
        transform.localScale = Vector3.one * SCALE;
        transform.SetPositionAndRotation(
            new Vector3(_cellData.Column, -_cellData.Row) * SCALE * GameConstants.SettingStats.LENGTH_ROOM,
            Quaternion.identity);
        foreach (var (name, status) in _cellData.Doors)
        {
            if (status != STATUS_DOOR.DISABLE)
            {
                var door = Instantiate(doorPrefab, transform);
                door.SetDirection(name);
                door.SetStatus(status);
                _listDoors.Add(door);
                ListDirectionDoors.Add(GameConstants.Direction.NameToDirection[name]);
            }
        }
    }

    public void UpdateStatusDoor(Vector2 direction) => GetDoor(direction).OpenDoor();

    public void GetStartDoorPosition(Vector2 direction)
    {
        var nextDoor = GetDoor(direction);
        nextDoor.OpenDoor();
        StartDoorPosition = nextDoor.transform.position - (Vector3)direction * PADDING_DOOR_TELE_SCALE;
    }

    public DoorController GetDoor(Vector2 direction)
    {
        DoorController nextDoor = new DoorController();
        foreach (var door in _listDoors)
        {
            if (door.GetDirection() == direction) nextDoor = door;
        }

        return nextDoor;
    }

    public void SetDoorPoints(List<DoorPoint> points)
    {
        var groups = new Dictionary<Vector2, List<DoorPoint>>();

        foreach (var p in points)
        {
            if (!groups.ContainsKey(p.direction))
                groups[p.direction] = new List<DoorPoint>();
            groups[p.direction].Add(p);
        }

        var result = new List<DoorPoint>();
        foreach (var kvp in groups)
        {
            var sum = Vector2.zero;
            foreach (var p in kvp.Value)
            {
                sum += new Vector2(p.position.x, p.position.y);
            }

            result.Add(new DoorPoint
            {
                // Tilemap cells are anchored at their bottom-left corner, so their visual centre sits
                // +0.5 on Y in world space (Vector3.up * 0.5). An additional 0.5 offset along the door's
                // direction pushes the door collider flush against the room wall edge, matching the
                // tilemap boundary exactly.
                position = (new Vector3(sum.x, sum.y, 0) / kvp.Value.Count) + ((Vector3)kvp.Key + Vector3.one) * 0.5f,
                direction = kvp.Key
            });
        }
        foreach (var dp in result)
        {
            var door = GetDoor(dp.direction);
            if (door != null)
                door.transform.position = dp.position;
        }
    }

    public void OpenDoors()
    {
        foreach (var door in _listDoors)
        {
            door.SetStatus(STATUS_DOOR.OPEN);
        }
    }

    public void CloseDoor()
    {
        foreach (var door in _listDoors)
        {
            door.SetStatus(STATUS_DOOR.CLOSE);
        }
    }

    public void ClearRoom(LevelData levelData, List<int> indexLevelDataDoor, List<DoorPoint> points)
    {
        this.DoorPoints.Clear();
        this.Data.Clear();
        this.IndexLevelDataDoor.Clear();

        this.DoorPoints.AddRange(points);
        this.Data.CopyData(levelData);
        this.IndexLevelDataDoor.AddRange(indexLevelDataDoor);
        this.IsCleared = true;
        CloseDoor();
    }

}

[System.Serializable]
public struct DoorPoint
{
    public Vector3 position;
    public Vector2 direction;
}