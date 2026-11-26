
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using VContainer;


public class RoomGeneraterController : MonoBehaviour
{
    // Full room library (Maze_Storage): every authored room. Read-only at runtime.
    [SerializeField] private DungeonRoomSO _fullDungeonRoomSO;
    // Rooms of the current run, one per maze cell (index = y * Columns + x). Runtime only, rebuilt in Setting().
    private List<RoomFile> _mazeRooms = new List<RoomFile>();
    // Child of Room (Room/Grid/Level Manager): tile catalogue + tilemaps. Not injected.
    [SerializeField] private LevelManager _levelManager;
    // Cached from _levelManager in Setting().
    private List<TileSO> _listTiles;
    private List<Tilemap> _genmap = new List<Tilemap>();
    // State of the room being shown.
    private SwapLevelData _swapLevelData = new SwapLevelData();
    private List<int> IndexLevelDataDoor = new List<int>();
    private LevelData Data = new LevelData();
    private List<DoorPoint> DoorPoints = new List<DoorPoint>();
    [SerializeField] List<Vector2Int> spawnPositions = new List<Vector2Int>();
    [SerializeField] PathfindingGrid pathfindingGrid;
    // Child object of Room: pathfinding grid + enemy spawning for the room being loaded.
    [SerializeField] private EnemyManager _enemyManager;
    // Every run has at least one room from each group (e.g. a shop, a support / special room).
    [SerializeField] private List<RequiredRoomGroup> _requiredRoomGroups = new List<RequiredRoomGroup>
    {
        new RequiredRoomGroup { label = "Shop", roomTypes = new List<RoomType> { RoomType.ShopRoom } },
        new RequiredRoomGroup { label = "Support / special", roomTypes = new List<RoomType> { RoomType.TreasureRoom, RoomType.RestRoom, RoomType.PuzzleRoom, RoomType.SecretRoom } },
    };
    IPlayerService _playerService;
    [Inject]
    public void Construct(IPlayerService playerService)
    {
        _playerService = playerService;
    }
    public void Setting(int startIndex, int endIndex, int listCount)
    {
        if (_enemyManager == null)
            _enemyManager = GetComponentInChildren<EnemyManager>(true);
        if (_enemyManager == null)
            Debug.LogError($"[{nameof(RoomGeneraterController)}] No EnemyManager child under {name}; rooms cannot spawn enemies.");

        if (_levelManager == null)
            _levelManager = GetComponentInChildren<LevelManager>(true);
        if (_levelManager == null || _fullDungeonRoomSO == null)
        {
            Debug.LogError($"[{nameof(RoomGeneraterController)}] Needs a LevelManager child and _fullDungeonRoomSO on {name}; no room can be built.");
            return;
        }

        _listTiles = _levelManager.GetTileSOs();
        _genmap = _levelManager.GetTilemaps();

        if (!BuildMazeRooms(listCount, startIndex, endIndex)) return;
        pathfindingGrid = new PathfindingGrid();
        if (_enemyManager != null) _enemyManager.SetPathfindingGrid(pathfindingGrid);
    }

    // Start / end rooms are chosen by RoomType (StartRoom / BossRoom), never by their position in the
    // library, so adding, deleting or re-sorting rooms cannot move them. Every other cell gets a distinct
    // random room of any other type, with at least one room from each _requiredRoomGroups entry.
    private bool BuildMazeRooms(int cellCount, int startIndex, int endIndex)
    {
        List<RoomFile> library = _fullDungeonRoomSO.room;
        List<RoomFile> startRooms = library.FindAll(r => r.roomType == RoomType.StartRoom);
        List<RoomFile> bossRooms = library.FindAll(r => r.roomType == RoomType.BossRoom);
        List<RoomFile> middleRooms = library.FindAll(r => r.roomType != RoomType.StartRoom && r.roomType != RoomType.BossRoom);
        int middleCount = cellCount - 2;

        if (startRooms.Count == 0 || bossRooms.Count == 0 || middleRooms.Count < middleCount)
        {
            Debug.LogError($"[{nameof(RoomGeneraterController)}] {_fullDungeonRoomSO.name} needs at least one StartRoom, one BossRoom " +
                           $"and {middleCount} other rooms for a {cellCount}-cell maze (has {startRooms.Count} / {bossRooms.Count} / {middleRooms.Count}).");
            return false;
        }

        Shuffle(middleRooms);
        List<RoomFile> picked = PickRequiredRooms(middleRooms, middleCount);
        foreach (RoomFile room in middleRooms)
        {
            if (picked.Count >= middleCount) break;
            if (!picked.Contains(room)) picked.Add(room);
        }
        Shuffle(picked);

        _mazeRooms.Clear();
        int next = 0;
        for (int cell = 0; cell < cellCount; cell++)
        {
            if (cell == startIndex) _mazeRooms.Add(startRooms[Random.Range(0, startRooms.Count)]);
            else if (cell == endIndex) _mazeRooms.Add(bossRooms[Random.Range(0, bossRooms.Count)]);
            else _mazeRooms.Add(picked[next++]);
        }
        return true;
    }

    // One random room per required group, taken from the (already shuffled) middle pool. A group with no
    // matching room in the library is reported and skipped; groups beyond the free cells are dropped.
    private List<RoomFile> PickRequiredRooms(List<RoomFile> middleRooms, int middleCount)
    {
        List<RoomFile> picked = new List<RoomFile>();
        foreach (RequiredRoomGroup group in _requiredRoomGroups)
        {
            if (group == null || group.roomTypes == null || group.roomTypes.Count == 0) continue;
            if (picked.Exists(r => group.roomTypes.Contains(r.roomType))) continue;

            RoomFile room = middleRooms.Find(r => group.roomTypes.Contains(r.roomType) && !picked.Contains(r));
            if (room == null)
            {
                Debug.LogWarning($"[{nameof(RoomGeneraterController)}] Required group '{group.label}' has no room in {_fullDungeonRoomSO.name}; skipped.");
                continue;
            }
            if (picked.Count >= middleCount)
            {
                Debug.LogWarning($"[{nameof(RoomGeneraterController)}] Maze has only {middleCount} free cells; required group '{group.label}' dropped.");
                continue;
            }
            picked.Add(room);
        }
        return picked;
    }

    private static void Shuffle(List<RoomFile> rooms)
    {
        for (int i = rooms.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (rooms[i], rooms[j]) = (rooms[j], rooms[i]);
        }
    }

    public void LoadRoom(int index, RoomCell nextRoomCell)
    {
        if (!nextRoomCell.IsCleared)
        {
            // Read from the TextAsset reference, not from Application.dataPath: Assets/ is not shipped in a build.
            TextAsset roomJson = _mazeRooms[index].roomJson;
            if (roomJson == null)
            {
                Debug.LogError($"[{nameof(RoomGeneraterController)}] Room {_mazeRooms[index].roomName} has no roomJson TextAsset " +
                               $"in {_fullDungeonRoomSO.name}; run LevelEditorTool > Import to relink it.");
                return;
            }
            Data = JsonUtility.FromJson<LevelData>(roomJson.text);
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
            worldPose = Data.poses[i] + Vector3Int.RoundToInt(nextRoomCell.transform.position);
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
            pathfindingGrid.BuildGrid(Data, nextRoomCell.transform.position);
            RoomType roomType = _mazeRooms[index].roomType;
            if (AutoOpenRoomTypes.Contains(roomType))
                EventManager.Emit(EventID.ON_OPEN_DOOR);
            else if (_enemyManager == null)
                Debug.LogError($"[{nameof(RoomGeneraterController)}] No EnemyManager; room {_mazeRooms[index].roomName} spawns nothing.");
            else if (roomType == RoomType.BossRoom)
                _enemyManager.SpawnBoss(spawnPositions);
            else
                _enemyManager.SpawnRoom(spawnPositions);
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
        if (_current.IsCleared == true) return;
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
    [System.Serializable]
    private class RequiredRoomGroup
    {
        public string label;
        public List<RoomType> roomTypes = new List<RoomType>();
    }
    private static readonly HashSet<RoomType> AutoOpenRoomTypes = new HashSet<RoomType>
    {
        RoomType.ShopRoom,
        RoomType.RestRoom,
        RoomType.TreasureRoom,
    };
}