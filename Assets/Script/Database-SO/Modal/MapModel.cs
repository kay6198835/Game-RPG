using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapModel", menuName = "Game/Map Model")]
public class MapModel : EntityModel
{
    public List<RoomModel> fullRoomList = new List<RoomModel>();

    private List<RoomModel> _pool = new List<RoomModel>();

    public RoomModel GetRandomRoom()
    {
        if (fullRoomList == null || fullRoomList.Count == 0)
        {
            Debug.LogWarning("GetRandomRoom: fullRoomList is empty");
            return null;
        }

        if (_pool.Count == 0)
        {
            foreach (var r in fullRoomList)
                if (r != null) _pool.Add(r);
            if (_pool.Count == 0) return null;
        }

        int picked = Random.Range(0, _pool.Count);
        RoomModel room = _pool[picked];
        _pool.RemoveAt(picked);
        return room;
    }
}