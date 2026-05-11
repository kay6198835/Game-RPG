using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMapController : MonoBehaviour
{
    [SerializeField] RoomController current, next;
    [SerializeField] FastMovement fastMovement;
    
    private void Start()
    {

    }
    public void Move(object ojt = null)
 {
        next = MazeController.Instance.RoomMapController.GetNext((Vector2)ojt);
        MazeController.Instance.RoomMapController.OnLoadMap();
        fastMovement.gameObject.transform.SetLocalPositionAndRotation(next.StartDoorPosition.transform.position, Quaternion.identity);
    }
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
    public void LoadRoom()
    {
        Debug.Log("CALL LOAD");
        current = next;
        next = null;
        EventManager.Emit(EventID.ON_LOAD_MAP);
    }

    public void OnLoadMap(object ojt = null)
    {
        current = MazeController.Instance.RoomMapController.GetStart();
    }
}
