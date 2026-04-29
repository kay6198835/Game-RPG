using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMapController : MonoBehaviour
{
    [SerializeField] RoomController current, next;
    private void Start()
    {
        //Avatar.transform.position = current.transform.position;
        current = MazeController.Instance.GetStartRoom();
    }
    public void Move(object ojt = null)
    {
        next = MazeController.Instance.GetNextRoom(current, (Vector2)ojt);
    }
    private void OnEnable()
    {
        EventManager.Resgister(EventID.ON_PLAYER_ON_DOOR, Move);
    }

    private void OnDisable()
    {
        EventManager.UnResgister(EventID.ON_PLAYER_ON_DOOR, Move);
    }
    public void LoadRoom()
    {
        current = next;
        next = null;
        EventManager.Emit(EventID.ON_LOAD_MAP,cuonefsakdjfhnasdklfhjasdrrent);
    }
}
