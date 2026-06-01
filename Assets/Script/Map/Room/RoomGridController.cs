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
    public LevelData CurrentDoorLevelData { get; private set; } = new LevelData();
    public LevelData Data { get; private set; } = new LevelData();
    public List<DoorPoint> DoorPoints { get; private set; } = new List<DoorPoint>();

    public void OnEnable()
    {
        EventManager.Resgister(EventID.ON_LOAD_MAZE_DONE, OnDoneLoadRoomGrid);
        EventManager.Resgister(EventID.ON_CLEAR_ENEMY, DeleteDoorTileMap);
        EventManager.Resgister(EventID.ON_PLAYER_ON_DOOR, ClearRoom);
    }

    public void OnDisable()
    {
        EventManager.UnResgister(EventID.ON_LOAD_MAZE_DONE, OnDoneLoadRoomGrid);
        EventManager.UnResgister(EventID.ON_CLEAR_ENEMY, DeleteDoorTileMap);
        EventManager.UnResgister(EventID.ON_PLAYER_ON_DOOR, ClearRoom);
    }

    public void OnLoadMap(Vector2 nextMap)
    {
        _swapLevelData.Clear();
        _next = GetNext(nextMap);
        int index = CaculateIndex(_next.GetGridPosition());
        Vector3 teleportPos = _next.StartDoorPosition;
        EventManager.Emit(EventID.ON_LOAD_MAP, index);
        _current = _next;
        _next = null;
        LoadRoom(index, _current.transform.position);
        _fastMovement.transform.SetPositionAndRotation(teleportPos, Quaternion.identity);
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
        LoadRoom(0, _current.transform.position);
    }

    public void LoadRoom(int index, Vector3 positionLoadMap)
    {
        DoorPoints.Clear();
        CurrentDoorLevelData = new LevelData();

        string filePath = _dungeonRoomSO.room[index].filePath;
        string json = File.ReadAllText(Application.dataPath + filePath);
        Data = JsonUtility.FromJson<LevelData>(json);

        foreach (Tilemap gm in _genmap) gm.ClearAllTiles();

        bool hasLayerData = Data.layerIndices != null && Data.layerIndices.Count == Data.poses.Count;

        for (int i = 0; i < Data.poses.Count; i++)
        {
            Data.poses[i] += new Vector3Int((int)_current.transform.position.x, (int)_current.transform.position.y, 0);
            int layerIdx = hasLayerData ? Data.layerIndices[i] : 0;
            if (layerIdx < 0 || layerIdx >= _genmap.Count) layerIdx = 0;

            var tileName = Data.tiles[i];
            if (tileName == GameConstants.TileName.DOOR)
            {
                Vector2 tilemapDirection = Utility.ToCardinalDirection(Data.poses[i].ConvertTo<Vector3>());
                bool isHaveTileDoor = _current.ListDirectionDoors.Contains(tilemapDirection);
                if (!isHaveTileDoor)
                {
                    tileName = GameConstants.TileName.ROOM;
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
                    CurrentDoorLevelData.tiles.Add(tileName);
                    CurrentDoorLevelData.poses.Add(Data.poses[i]);
                    CurrentDoorLevelData.layerIndices.Add(Data.layerIndices[i]);
                }
            }

            var tileSO = _listTiles.Find(t => t.name == tileName);
            if (tileSO == null) continue;
            _genmap[layerIdx].SetTile(Data.poses[i], tileSO.tile);
        }
        _current.SetDoorPoints(DoorPoints);
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
            var tileSO = _listTiles.Find(t => t.name == tileMapName);
            if (tileSO != null)
                _genmap[layerIndex].SetTile(Data.poses[tileIndex], tileSO.tile);

            Data.tiles.Add(tileMapName);
            Data.layerIndices.Add(layerIndex);
            Data.poses.Add(originalPos);
        }
    }

    public void ClearRoom(object obj = null)
    {
        for (int i = 0; i < Data.tiles.Count; i++)
        {
            _genmap[Data.layerIndices[i]].SetTile(Data.poses[i], null);
        }
        OnLoadMap((Vector2)obj);
    }

    private void DeleteDoorTileMap(object obj = null)
    {
        for (int i = 0; i < CurrentDoorLevelData.tiles.Count; i++)
        {
            _genmap[CurrentDoorLevelData.layerIndices[i]].SetTile(CurrentDoorLevelData.poses[i], null);
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
