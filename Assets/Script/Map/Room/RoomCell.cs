using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[ExecuteInEditMode]
public class RoomCell : BaseCell
{
    private float SCALE = GameConstants.SettingStats.GAME_SCALE;
    private float PADDING_DOOR_TELE_SCALE = GameConstants.SettingStats.PADDING_DOOR_TELE_SCALE;

    [SerializeField] private DoorController doorPrefab;
    [SerializeField] private List<DoorController> _listDoors = new List<DoorController>();
    [SerializeField] public Vector2 StartDoorPosition { get; private set; }
    [SerializeField] public List<Vector2> ListDirectionDoors { get; private set; }

    // parameter levelData is used to set the room as cleared and open doors,
    //  also store the data of the room for reloading when player come back to this room
    [SerializeField] public bool IsCleared { get; private set; } = false;
    [SerializeField] public LevelData Data { get; private set; } = new LevelData();
    [SerializeField] public List<DoorPoint> DoorPoints { get; private set; } = new List<DoorPoint>();
    [SerializeField] public List<int> IndexLevelDataDoor { get; private set; } = new List<int>();
    [SerializeField] public int EnemyCount { get; private set; } = 0;
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

    public void OnEnable()
    {
        EventManager.Resgister(EventID.ON_DONE_SPAWN_ENEMY, OnDoneSpawnEnemy);
        EventManager.Resgister(EventID.ON_ENEMY_DEATH, OnEnemyDeath);
        EventManager.Resgister(EventID.ON_SPAWN_EXTRA_ENEMY, OnSpawnExtraEnemy);
    }
    public void OnDisable()
    {
        EventManager.UnResgister(EventID.ON_DONE_SPAWN_ENEMY, OnDoneSpawnEnemy);
        EventManager.UnResgister(EventID.ON_ENEMY_DEATH, OnEnemyDeath);
        EventManager.UnResgister(EventID.ON_SPAWN_EXTRA_ENEMY, OnSpawnExtraEnemy);
    }

    public void OnDoneSpawnEnemy(object obj = null)
    {
        EnemyCount = (int)obj;
    }

    public void OnSpawnExtraEnemy(object obj = null)
    {
        EnemyCount++;
    }

    public void OnEnemyDeath(object obj = null)
    {
        EnemyCount--;
        if (EnemyCount == 0)
        {
            EventManager.UnResgister(EventID.ON_CLEAR_ENEMY);
        }
    }

    public void UpdateStatusDoor(Vector2 direction)
    {
        var door = GetDoor(direction);
        if (door != null) door.OpenDoor();
    }

    public void GetStartDoorPosition(Vector2 direction)
    {
        var nextDoor = GetDoor(direction);
        if (nextDoor == null) return;

        nextDoor.OpenDoor();
        StartDoorPosition = (Vector2)nextDoor.transform.position - direction * PADDING_DOOR_TELE_SCALE;
    }

    public DoorController GetDoor(Vector2 direction)
    {
        foreach (var door in _listDoors)
        {
            if (door.GetDirection() == direction) return door;
        }

        Debug.LogWarning($"[{nameof(RoomCell)}] Không tìm thấy door theo hướng {direction} trong room {name}.");
        return null;
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
                sum += p.position;
            }

            result.Add(new DoorPoint
            {
                // Tilemap cells are anchored at their bottom-left corner, so their visual centre sits
                // +0.5 on Y in world space (Vector3.up * 0.5). An additional 0.5 offset along the door's
                // direction pushes the door collider flush against the room wall edge, matching the
                // tilemap boundary exactly.
                position = (sum / kvp.Value.Count) + (kvp.Key + Vector2.one) * 0.5f,
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
    public Vector2 position;
    public Vector2 direction;
}