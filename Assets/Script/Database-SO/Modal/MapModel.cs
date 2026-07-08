using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapModel", menuName = "Game/Map Model")]
public class MapModel : EntityModel
{
    public string mapName;
    public List<int> idRooms;
    public int totalWeight;
}