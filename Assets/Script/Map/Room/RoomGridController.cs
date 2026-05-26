using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomGridController : BaseGrid<RoomCell>
{
    [SerializeField] private DungeonRoomSO _dungeonRoomSO;
    [SerializeField] private List<TileSO> _listTiles;
    [SerializeField] private List<Tilemap> _genmap = new List<Tilemap>();
    public void OnEnable()
    {
        EventManager.Resgister(EventID.ON_LOAD_MAZE_DONE, OnDoneLoadRoomGrid);
    }
    public void OnDisable()
    {
        EventManager.Resgister(EventID.ON_LOAD_MAZE_DONE, OnDoneLoadRoomGrid);
    }
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
        //LevelManager.Instance.LoadRoom(0, _current.transform.position);
        _dungeonRoomSO = LevelManager.Instance.GetDungeonRoomSO();
        _listTiles = LevelManager.Instance.GetTileSOs();
        _genmap = LevelManager.Instance.GetTilemaps();

    }

    public void OnDoneLoadRoomGrid(object obj = null)
    {
        this.LoadRoom(0, _current.transform.position);
    }

    public void LoadRoom(int index, Vector3 positionLoadMap)
    {
        string filePath = "";
        index = index == null ? 0 : index;
        if (index == 0)
        {
            filePath = _dungeonRoomSO.room[index].filePath;
        }
        else
        {
            //filePath = listRooms[index].filePath;
        }
        string json = File.ReadAllText(Application.dataPath + filePath);
        LevelData data = JsonUtility.FromJson<LevelData>(json);

        foreach (Tilemap gm in _genmap) gm.ClearAllTiles();

        bool hasLayerData = data.layerIndices != null && data.layerIndices.Count == data.poses.Count;

        for (int i = 0; i < data.poses.Count; i++)
        {
            int layerIdx = hasLayerData ? data.layerIndices[i] : 0;
            if (layerIdx < 0 || layerIdx >= _genmap.Count) layerIdx = 0;

            var tilemap = data.tiles[i];
            //Refactor late
            if (tilemap == "Tile_Door")
            {
                // get direction
                Vector2 tilemapDirection = Utility.ToCardinalDirection
                    (data.poses[i].ConvertTo<Vector3>());
                // check include
                bool isHaveTileDoor = this._current.ListDirectionDoors.Contains(tilemapDirection);
                // true swap tile
                tilemap = isHaveTileDoor ? "Tile_Door" : "Tile_Room";
                // false save tile data in lobal class

            }


            _genmap[layerIdx].SetTile(data.poses[i], _listTiles.Find(t => t.name == tilemap).tile);
        }

        //this.SetPosition(positionLoadMap);
    }
}
