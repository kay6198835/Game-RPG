using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomGridController : BaseGrid<RoomCell>
{

    [SerializeField] private FastMovement _fastMovement;
    [SerializeField] private DungeonRoomSO _dungeonRoomSO;
    [SerializeField] private List<TileSO> _listTiles;
    [SerializeField] private List<Tilemap> _genmap = new List<Tilemap>();
    [SerializeField] private SwapLevelData _swapLevelData = new SwapLevelData();
    [SerializeField] public LevelData CurentDoorLevelData { get; private set; } = new LevelData();
    [SerializeField] public LevelData Data { get; private set; } = new LevelData();
    [SerializeField] public List<DoorPoint> DoorPoints { get; private set; } = new List<DoorPoint>();
    public void OnEnable()
    {
        EventManager.Resgister(EventID.ON_LOAD_MAZE_DONE, OnDoneLoadRoomGrid);
        EventManager.Resgister(EventID.ON_CLEAR_ENEMY, DeleteDoorTileMap);
        EventManager.Resgister(EventID.ON_PLAYER_ON_DOOR, ClearRoom);
    }
    public void OnDisable()
    {
        EventManager.Resgister(EventID.ON_LOAD_MAZE_DONE, OnDoneLoadRoomGrid);
        EventManager.Resgister(EventID.ON_CLEAR_ENEMY, DeleteDoorTileMap);
        EventManager.Resgister(EventID.ON_PLAYER_ON_DOOR, ClearRoom);
    }
    public void OnLoadMap(Vector2 nextMap)
    {
        this._swapLevelData.Clear();
        int index = CaculateIndex(_current.GetGridPosition());
        _next = GetNext(nextMap);
        EventManager.Emit(EventID.ON_LOAD_MAP, index);
        this.LoadRoom(index, _next.transform.position);
        fastMovement.transform.SetPositionAndRotation(_next.StartDoorPosition, Quaternion.identity);
        _current = _next;
        _next = null;
    }

    protected override void OnAfterGetNext(RoomCell current, RoomCell next, Vector2 direction)
    {
        next.GetStartDoorPosition(-direction);
        current.UpdateStatusDoor(direction);
    }

    public void OnDoneLoadRoomGrid(object obj = null)
    {
        _dungeonRoomSO = LevelManager.Instance.GetDungeonRoomSO();
        _listTiles = LevelManager.Instance.GetTileSOs();
        _genmap = LevelManager.Instance.GetTilemaps();
        this.LoadRoom(0, _current.transform.position);
    }

    public void LoadRoom(int index, Vector3 positionLoadMap)
    {
        string filePath = "";
        index = index == null ? 0 : index;
        filePath = _dungeonRoomSO.room[index].filePath;
        string json = File.ReadAllText(Application.dataPath + filePath);
        Data = JsonUtility.FromJson<LevelData>(json);

        foreach (Tilemap gm in _genmap) gm.ClearAllTiles();

        bool hasLayerData = Data.layerIndices != null && Data.layerIndices.Count == Data.poses.Count;

        for (int i = 0; i < Data.poses.Count; i++)
        {
            Data.poses[i] += new Vector3Int((int)_current.transform.position.x, (int)_current.transform.position.y, 0);
            int layerIdx = hasLayerData ? Data.layerIndices[i] : 0;
            if (layerIdx < 0 || layerIdx >= _genmap.Count) layerIdx = 0;

            var tilemap = Data.tiles[i];
            //Refactor late
            if (tilemap == "Tile_Door")
            {
                // get direction
                Vector2 tilemapDirection = Utility.ToCardinalDirection
                    (Data.poses[i].ConvertTo<Vector3>());
                // check include
                bool isHaveTileDoor = this._current.ListDirectionDoors.Contains(tilemapDirection);
                // true swap tile
                if (!isHaveTileDoor)
                {
                    tilemap = GameConstants.TileName.ROOM;
                    _swapLevelData.directions.Add(tilemapDirection);
                    _swapLevelData.indexToLayer.Add(i, layerIdx);
                }
                else
                {
                    DoorPoints.Add(new DoorPoint
                    {
                        position = _current.transform.position + Data.poses[i],
                        direction = tilemapDirection
                    });
                    CurentDoorLevelData.tiles.Add(tilemap);
                    CurentDoorLevelData.poses.Add(Data.poses[i]);
                    CurentDoorLevelData.layerIndices.Add(Data.layerIndices[i]);

                }
                // false save tile data in lobal class
            }
            _genmap[layerIdx].SetTile(Data.poses[i], _listTiles.Find(t => t.name == tilemap).tile);
        }
        _current.SetDoorPoints(this.DoorPoints);
        SwapTileMap(GameConstants.TileName.DOOR);
    }
    private void SwapTileMap(string tileMapName)
    {
        Vector3Int convertVector3Int = new Vector3Int();
        var entries = new List<KeyValuePair<int, int>>(_swapLevelData.indexToLayer);

        for (int i = 0; i < _swapLevelData.directions.Count; i++)
        {

            convertVector3Int.Set((int)_swapLevelData.directions[i].x, (int)_swapLevelData.directions[i].y, 0);


            int tileIndex = entries[i].Key;
            int layerIndex = entries[i].Value;
            Vector3Int originalPos = Data.poses[tileIndex];

            Data.tiles[tileIndex] = tileMapName;
            Data.poses[tileIndex] += convertVector3Int;
            _genmap[layerIndex].SetTile(Data.poses[tileIndex], _listTiles.Find(t => t.name == tileMapName).tile);

            Data.tiles.Add(tileMapName);
            Data.layerIndices.Add(layerIndex);
            Data.poses.Add(originalPos);
        }
    }

    public void ClearRoom(object obj = null)
    {
        Debug.Log(_genmap.Count.ToString());
        for (int i = 0; i < Data.tiles.Count; i++)
        {
            var layerIndices = Data.layerIndices[i];
            var pos = Data.poses[i];
            _genmap[layerIndices].SetTile(pos, null);
        }
        this.OnLoadMap((Vector2)obj);
    }

    private void DeleteDoorTileMap(object obj = null)
    {
        for (int i = 0; i < CurentDoorLevelData.tiles.Count; i++)
        {
            _genmap[CurentDoorLevelData.layerIndices[i]].SetTile(CurentDoorLevelData.poses[i], null);
        }
        _current.OpenDoor();
    }
    [System.Serializable]
    private class SwapLevelData
    {
        public Dictionary<int, int> indexToLayer = new Dictionary<int, int>();
        public List<Vector2> directions = new List<Vector2>();

        public void Clear()
        {
            directions.Clear();
            indexToLayer.Clear();
        }
    }
}
