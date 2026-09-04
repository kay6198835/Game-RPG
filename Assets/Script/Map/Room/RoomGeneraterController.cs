
using Unity.VisualScripting;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;
using VContainer;


public class RoomGeneraterController : MonoBehaviour
{
    [SerializeField] private DungeonRoomSO _dungeonRoomSO;
    // [SerializeField] private DungeonRoomSO _fullDungeonRoomSO;
    [SerializeField] private List<TileSO> _listTiles;
    [SerializeField] private List<Tilemap> _genmap = new List<Tilemap>();
    [SerializeField] private SwapLevelData _swapLevelData = new SwapLevelData();
    [SerializeField] public List<int> IndexLevelDataDoor { get; private set; } = new List<int>();
    [SerializeField] public LevelData Data { get; private set; } = new LevelData();
    [SerializeField] public List<DoorPoint> DoorPoints { get; private set; } = new List<DoorPoint>();
    [SerializeField] private int _startIndex;
    [SerializeField] private int _endIndex;
    [SerializeField] List<int> randomMazeRoomsIndex = new List<int>();
    [SerializeField] List<Vector2Int> spawnPositions = new List<Vector2Int>();
    [SerializeField] PathfindingGrid pathfindingGrid;
    IPlayerService _playerService;
    [Inject]
    public void Construct(IPlayerService playerService)
    {
        _playerService = playerService;
    }
    public void OnDisable()
    {
        _dungeonRoomSO.room.Clear();
    }
    public void Setting(int startIndex, int endIndex, int listCount)
    {
        _startIndex = startIndex;
        _endIndex = endIndex;

        // need refactor to get _fullDungeonRoomSO, _listTiles, _genmap from other class, not from LevelManager
        DungeonRoomSO _fullDungeonRoomSO = LevelManager.Instance.GetDungeonRoomSO();
        _listTiles = LevelManager.Instance.GetTileSOs();
        _genmap = LevelManager.Instance.GetTilemaps();

        // get start, get radoom, get end
        var totalCount = _fullDungeonRoomSO.room.Count;
        var pickCount = listCount;
        randomMazeRoomsIndex = Utility.PickUniqueIndex(totalCount, pickCount);
        for (int i = 0; i < randomMazeRoomsIndex.Count; i++)
        {
            _dungeonRoomSO.room.Add(_fullDungeonRoomSO.room[randomMazeRoomsIndex[i]]);
        }
        _dungeonRoomSO.room[_startIndex] = _fullDungeonRoomSO.room[0];
        _dungeonRoomSO.room[_endIndex] = _fullDungeonRoomSO.room[_fullDungeonRoomSO.room.Count - 1];
        pathfindingGrid = new PathfindingGrid();
        EnemyManager.Instance.SetPathfindingGrid(pathfindingGrid);
    }

    public void OnDoneLoadRoomGrid(RoomCell _current)
    {
        this.LoadRoom(_startIndex, _current);
        _playerService.SetPlayerPosition(_current.StartDoorPosition + (Vector2)_current.transform.position);
    }

    public void LoadRoom(int index, RoomCell nextRoomCell)
    {
        string filePath = "";
        filePath = _dungeonRoomSO.room[index].filePath;
        string json = File.ReadAllText(Application.dataPath + filePath);
        if (!nextRoomCell.IsCleared)
        {
            Data = JsonUtility.FromJson<LevelData>(json);
        }
        else
        {
            Data.CopyData(nextRoomCell.Data);
            DoorPoints.AddRange(nextRoomCell.DoorPoints);
            IndexLevelDataDoor.AddRange(nextRoomCell.IndexLevelDataDoor);
        }


        foreach (Tilemap gm in _genmap) gm.ClearAllTiles();

        bool hasLayerData = Data.layerIndices != null && Data.layerIndices.Count == Data.poses.Count;
        Vector3Int origanalTileMapPosition = new Vector3Int();
        Vector3Int worldPose = new Vector3Int();
        for (int i = 0; i < Data.poses.Count; i++)
        {
            origanalTileMapPosition = Data.poses[i];
            worldPose = nextRoomCell.IsCleared ? Data.poses[i] + Vector3Int.RoundToInt(nextRoomCell.transform.position)
            : Data.poses[i] + Vector3Int.RoundToInt(nextRoomCell.transform.position);
            int layerIdx = hasLayerData ? Data.layerIndices[i] : 0;
            if (layerIdx < 0 || layerIdx >= _genmap.Count) layerIdx = 0;

            var tilemap = Data.tiles[i];
            if (tilemap == null) continue;
            //Refactor late
            if (tilemap == GameConstants.TileName.DOOR && !nextRoomCell.IsCleared)
            {
                // get direction
                Vector2 tilemapDirection = Utility.ToCardinalDirection
                    (origanalTileMapPosition);

                // check include
                bool isIncludeDirection = nextRoomCell.ListDirectionDoors.Contains(tilemapDirection);
                // true swap tile
                if (!isIncludeDirection)
                {
                    tilemap = GameConstants.TileName.ROOM;
                    _swapLevelData.directions.Add(tilemapDirection);
                    _swapLevelData.indexToLayer.Add(i, layerIdx);
                    //_swapLevelData._swapLevelDataIndex.Add(i);
                }
                // false save tile data in lobal class
                else
                {
                    DoorPoints.Add(new DoorPoint
                    {
                        position = new Vector2(origanalTileMapPosition.x, origanalTileMapPosition.y),
                        direction = tilemapDirection
                    });
                    IndexLevelDataDoor.Add(i);
                }
            }
            if (tilemap == GameConstants.TileName.SPAWN && !nextRoomCell.IsCleared)
            {
                spawnPositions.Add((Vector2Int)worldPose);
            }
            _genmap[layerIdx].SetTile(worldPose, _listTiles.Find(t => t.name == tilemap).tile);
        }
        nextRoomCell.SetDoorPoints(this.DoorPoints);
        if (!nextRoomCell.IsCleared)
        {
            SwapTileMap(GameConstants.TileName.ROOM, nextRoomCell);
            EventManager.Emit(EventID.ON_GET_SPAWN_POSITIONS, spawnPositions);
            pathfindingGrid.BuildGrid(Data, nextRoomCell.transform.position);
        }
        else
        {
            // on colider door
            nextRoomCell.OpenDoors();
        }
    }

    private void SwapTileMap(string tileMapName, RoomCell roomCell)
    {
        var entries = new List<KeyValuePair<int, int>>(_swapLevelData.indexToLayer);

        for (int i = 0; i < _swapLevelData.directions.Count; i++)
        {
            int tileIndex = entries[i].Key;
            int layerIndex = entries[i].Value;
            Vector3Int originalPos = Data.poses[tileIndex];

            Data.tiles[tileIndex] = tileMapName;
            Data.poses[tileIndex] += Vector3Int.RoundToInt(_swapLevelData.directions[i]);
            _genmap[layerIndex].SetTile(Data.poses[tileIndex] + Vector3Int.RoundToInt(roomCell.transform.position), _listTiles.Find(t => t.name == tileMapName).tile);

            Data.tiles.Add(tileMapName);
            Data.layerIndices.Add(layerIndex);
            Data.poses.Add(originalPos);
        }
    }

    public void ClearRoom(RoomCell _current)
    {
        _current.ClearRoom(Data, IndexLevelDataDoor, DoorPoints);
        for (int i = 0; i < Data.tiles.Count; i++)
        {
            var layerIndices = Data.layerIndices[i];
            var pos = Data.poses[i] + Vector3Int.RoundToInt(_current.transform.position);
            _genmap[layerIndices].SetTile(pos, null);
        }
        this._swapLevelData.Clear();
        this.IndexLevelDataDoor.Clear();
        this.DoorPoints.Clear();
        this.Data.Clear();
        this.spawnPositions.Clear();
    }

    public void DeleteDoorTileMap(RoomCell _current)
    {
        for (int i = 0; i < IndexLevelDataDoor.Count; i++)
        {
            _genmap[Data.layerIndices[IndexLevelDataDoor[i]]].SetTile(Data.poses[IndexLevelDataDoor[i]] + Vector3Int.RoundToInt(_current.transform.position), null);
            Data.layerIndices[IndexLevelDataDoor[i]] = 0;
            Data.poses[IndexLevelDataDoor[i]] = Vector3Int.zero;
            Data.tiles[IndexLevelDataDoor[i]] = null;
        }
        _current.OpenDoors();
    }

    public void SetNextRoom(Vector2 startDoorPosition)
    {
        _playerService.SetPlayerPosition(startDoorPosition);
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