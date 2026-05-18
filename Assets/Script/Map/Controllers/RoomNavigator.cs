using System.Collections.Generic;
using UnityEngine;

public class RoomNavigator : MonoBehaviour
{
    [SerializeField] RoomCell current, next;
    [SerializeField] FastMovement fastMovement;

    private void OnEnable()
    {
        EventManager.Resgister(EventID.ON_PLAYER_ON_DOOR, Move);
        EventManager.Resgister(EventID.ON_LOAD_MAZE_DONE, OnLoadMap);
    }

    private void OnDisable()
    {
        EventManager.UnResgister(EventID.ON_PLAYER_ON_DOOR, Move);
        EventManager.UnResgister(EventID.ON_LOAD_MAZE_DONE, OnLoadMap);
    }

    public void Move(object ojt = null)
    {
        next = MazeController.Instance.RoomGrid.GetNext((Vector2)ojt);
        MazeController.Instance.RoomGrid.OnLoadMap();
        fastMovement.transform.SetPositionAndRotation(next.StartDoorPosition, Quaternion.identity);
    }

    public void OnLoadMap(object ojt = null)
    {
        current = MazeController.Instance.RoomGrid.GetStart();
    }
}
