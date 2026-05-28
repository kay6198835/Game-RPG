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
    [SerializeField] private SwapLevelData swapLevelData = new SwapLevelData();
    //[SerializeField] public List<DoorPoint> DoorPoints { get; private set; } = new List<DoorPoint>();
    [SerializeField] private List<DoorPoint> doorPoints = new List<DoorPoint>();
    public List<DoorPoint> DoorPoints => doorPoints;
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
        base.Setting(columns, rows);
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
                if (!isHaveTileDoor)
                {
                    tilemap = "Tile_Room";
                    swapLevelData.levelData.tiles.Add(data.tiles[i]);
                    swapLevelData.levelData.poses.Add(data.poses[i]);
                    swapLevelData.levelData.layerIndices.Add(data.layerIndices[i]);
                    swapLevelData.directions.Add(tilemapDirection);
                }
                else
                {
                    DoorPoints.Add(new DoorPoint
                    {
                        position = _current.transform.position + data.poses[i],
                        direction = tilemapDirection
                    });
                }
                // false save tile data in lobal class
            }
            _genmap[layerIdx].SetTile(data.poses[i], _listTiles.Find(t => t.name == tilemap).tile);
        }
        _current.SetDoorPoints(this.DoorPoints);
        SwapTileMap("Tile_Room", swapLevelData);
    }
    private void SwapTileMap(string tileMapName, SwapLevelData levelData)
    {
        Vector3Int convertVector3Int = new Vector3Int();

        for (int i = 0; i < levelData.directions.Count; i++)
        {
            convertVector3Int.Set((int)levelData.directions[i].x, (int)levelData.directions[i].y, 0);
            levelData.levelData.tiles[i] = tileMapName;
            levelData.levelData.poses[i] += convertVector3Int;
            _genmap[levelData.levelData.layerIndices[i]].SetTile(levelData.levelData.poses[i], _listTiles.Find(t => t.name == tileMapName).tile);
        }
    }

    private class SwapLevelData
    {
        public LevelData levelData = new LevelData();
        public List<Vector2> directions = new List<Vector2>();
    }
}
