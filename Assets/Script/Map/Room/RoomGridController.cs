using UnityEngine;

public class RoomGridController : BaseGrid<RoomCell>
{
    //private void OnEnable()
    //{
    //    EventManager.Resgister(EventID.ON_LOAD_MAP, LoadMap);
    //}

    //private void OnDisable()
    //{
    //    EventManager.UnResgister(EventID.ON_LOAD_MAP, LoadMap);
    //}
    public void OnLoadMap()
    {
        _current = _next;
        _next = null;
        LoadMap();
    }

    protected override void OnAfterGetNext(RoomCell current, RoomCell next, Vector2 direction)
    {
        next.GetStartDoorPosition(-direction);
        current.UpdateStatusDoor(direction);
    }

    public void LoadMap(object ojt = null)
    {
        int index = CaculateIndex(_current.GetGridPosition());
        LevelManager.Instance.LoadRoom(index, _current.transform.position);
        EventManager.Emit(EventID.ON_LOAD_MAP);
    }

    public override void Setting(int columns, int rows)
    {
        base.Setting(columns,rows);
        LevelManager.Instance.LoadRoom(0, _current.transform.position);
    }
}
