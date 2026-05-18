using UnityEngine;

public class RoomGridController : BaseGrid<RoomCell>
{
    public void OnLoadMap()
    {
        _current = _next;
        _next = null;
    }

    protected override void OnAfterGetNext(RoomCell current, RoomCell next, Vector2 direction)
    {
        next.GetStartDoorPosition(-direction);
        current.UpdateStatusDoor(direction);
    }
}
