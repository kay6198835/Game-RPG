using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// Runtime room data for the in-game map flow: the tile catalogue and the tilemaps rooms are drawn on.
// Lives under Room/Grid; RoomGeneraterController owns it by reference (no DI) and hands it the room
// library. Room authoring (Save / Load / Edit / Import) is LevelEditorTool, kept out of this flow.
public class LevelManager : MonoBehaviour
{
    [SerializeField] private List<Tilemap> genmap = new List<Tilemap>();
    [SerializeField] private List<TileSO> _listTiles;
    private DungeonRoomSO dungeonRoomSO;

    public void SetDungeonRoomSO(DungeonRoomSO roomLibrary) => dungeonRoomSO = roomLibrary;

    /// <summary>
    /// Wrapper: seal tất cả Tile_Door có hướng không nằm trong <paramref name="directions"/>.
    /// Logic xử lý thực tế nằm ở <see cref="Utility.SealUnusedDoors"/>.
    /// </summary>
    public void SealUnusedDoors(Vector2[] directions)
    {
        TileBase roomTile = _listTiles.Find(t => t.name == GameConstants.TileName.ROOM)?.tile;
        TileBase doorTile = _listTiles.Find(t => t.name == GameConstants.TileName.DOOR)?.tile;
        if (roomTile == null || doorTile == null) return;

        Utility.SealUnusedDoors(directions, genmap, roomTile, doorTile);
    }

    /// <summary>
    /// Wrapper: trả về vị trí world của cụm Tile_Door nằm trên cạnh wall theo <paramref name="direction"/>.
    /// Logic xử lý thực tế nằm ở <see cref="Utility.GetDoorWorldPosition"/>.
    /// </summary>
    public Vector2 GetDoorWorldPosition(Vector2 direction)
    {
        TileBase roomTile = _listTiles.Find(t => t.name == GameConstants.TileName.ROOM)?.tile;
        TileBase doorTile = _listTiles.Find(t => t.name == GameConstants.TileName.DOOR)?.tile;
        if (roomTile == null || doorTile == null) return Vector2.zero;

        return Utility.GetDoorWorldPosition(direction, genmap, roomTile, doorTile);
    }

    public List<TileSO> GetTileSOs() => _listTiles;
    public List<Tilemap> GetTilemaps() => genmap;
    public DungeonRoomSO GetDungeonRoomSO() => dungeonRoomSO;
}

[System.Serializable]
public class LevelData
{
    //Do not change parameter name or spell
    public List<string> tiles = new List<string>();
    public List<Vector3Int> poses = new List<Vector3Int>();
    public List<int> layerIndices = new List<int>();
    public LevelData() { }
    public LevelData(LevelData other)
    {
        this.tiles = new List<string>(other.tiles);
        this.poses = new List<Vector3Int>(other.poses);
        this.layerIndices = new List<int>(other.layerIndices);
    }

    public void CopyData(LevelData other)
    {
        this.tiles = new List<string>(other.tiles);
        this.poses = new List<Vector3Int>(other.poses);
        this.layerIndices = new List<int>(other.layerIndices);
    }

    public void Clear()
    {
        tiles.Clear();
        poses.Clear();
        layerIndices.Clear();
    }
}