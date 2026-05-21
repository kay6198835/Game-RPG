using System.Collections.Generic;
using UnityEngine;

public class RoomNavigator : MonoBehaviour
{
    [SerializeField] RoomCell current, next;
    [SerializeField] FastMovement fastMovement;
    [SerializeField] List<RoomCell> rooms;

    private void OnEnable()
    {
        EventManager.Resgister(EventID.ON_PLAYER_ON_DOOR, SetNextMap);
        EventManager.Resgister(EventID.ON_LOAD_MAZE_DONE, OnLoadMap);
        EventManager.Resgister(EventID.ON_LOAD_MAP, SetPlayerPosition);
    }

    private void OnDisable()
    {
        EventManager.UnResgister(EventID.ON_PLAYER_ON_DOOR, SetNextMap);
        EventManager.UnResgister(EventID.ON_LOAD_MAZE_DONE, OnLoadMap);
        EventManager.UnResgister(EventID.ON_LOAD_MAP, SetPlayerPosition);
    }

    public void SetNextMap(object ojt = null)
    {
        next = MazeController.Instance.RoomGrid.GetNext((Vector2)ojt);
        MazeController.Instance.RoomGrid.OnLoadMap();
    }

    public void SetPlayerPosition(object ojt = null)
    {
        fastMovement.transform.SetPositionAndRotation(next.StartDoorPosition, Quaternion.identity);
    }

    public void OnLoadMap(object ojt = null)
    {
        current = MazeController.Instance.RoomGrid.GetStart();
    }
}
