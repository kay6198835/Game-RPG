using System.Collections.Generic;
using UnityEngine;

public static class GameConstants
{
    #region Animation
    public static class AnimationName
    {
        public static readonly string IDLE = "Idle";
        public static readonly string MOVE = "Move";
        public static readonly string ATTACK = "Attack";
        public static readonly string EQUIP_UNEQUIP = "EquipUnequip";
        public static readonly string INTERACTOR = "Interactor";
        public static readonly string ABILITY = "Ability";
        public static readonly string TAKE_DAMAGE = "TakeDamage";

        public static class Parameter
        {
             public static readonly string DIRECTION = "Direction";
        }
    }
    #endregion
    
    #region Direction
    public static class Direction
    {
        //Main Direction
        public static readonly Vector2 TOP = Vector2.up;
        public static readonly Vector2 RIGHT = Vector2.right;
        public static readonly Vector2 LEFT = Vector2.left;
        public static readonly Vector2 BOTTOM = Vector2.down;
        //Sub Direction
        public static readonly Vector2 TOP_RIGHT = (Vector2.up + Vector2.right).normalized;
        public static readonly Vector2 RIGHT_DOWN = (Vector2.right + Vector2.down).normalized;
        public static readonly Vector2 DOWN_LEFT = (Vector2.down + Vector2.left).normalized;
        public static readonly Vector2 LEFT_TOP = (Vector2.left + Vector2.up).normalized;
        public static class Name
        {
            public static readonly string TOP = "TOP";
            public static readonly string RIGHT = "RIGHT";
            public static readonly string LEFT = "LEFT";
            public static readonly string BOTTOM = "BOTTOM";
            //Sub Direction
            public static readonly string TOP_RIGHT = "TOP_RIGHT";
            public static readonly string RIGHT_DOWN = "RIGHT_DOWN";
            public static readonly string DOWN_LEFT = "DOWN_LEFT";
            public static readonly string LEFT_TOP = "LEFT_TOP";
        }
        public static readonly Dictionary<string, Vector2> NameToDirection = new Dictionary<string, Vector2>
        {
            { Name.TOP, TOP },
            { Name.RIGHT, RIGHT },
            { Name.LEFT, LEFT },
            { Name.BOTTOM, BOTTOM },
            { Name.TOP_RIGHT, TOP_RIGHT },
            { Name.RIGHT_DOWN, RIGHT_DOWN },
            { Name.DOWN_LEFT, DOWN_LEFT },
            { Name.LEFT_TOP, LEFT_TOP },
        };
        public static readonly Dictionary<Vector2, string> DirectionToName = new Dictionary<Vector2, string>
        {
            { TOP, Name.TOP },
            { RIGHT, Name.RIGHT },
            { LEFT, Name.LEFT },
            { BOTTOM, Name.BOTTOM },
            { TOP_RIGHT, Name.TOP_RIGHT },
            { RIGHT_DOWN, Name.RIGHT_DOWN },
            { DOWN_LEFT, Name.DOWN_LEFT },
            { LEFT_TOP, Name.LEFT_TOP },
        };
    }
    #endregion

    #region Input
    public static class Input
    {
        //public static readonly {{Type}} {{Name}} = Button_Name;
        public static readonly string VERTICAL = "Vertical";
        public static readonly string HORIZONTAL = "Horizontal";
        public static class Name
        {

        }
    }
    #endregion

    #region Route Asset
    public static class RouteAsset
    {
        //public static readonly string {{Name}} = (Resources){{Data\Skill_Ability}};
        public static class GO_PREFABS
        {
            // Tile Map
            public static string TILE_MAP_DOOR = "Tile_Spawn";
            public static string TILE_MAP_ROOM = "Tile_Spawn";
            public static string TILE_MAP_FLOOR = "Tile_Spawn";
        }
        public static class SO_DATAS
        {
            public static string DUNGEON_ROOM = "Tile_Spawn";
            // Tile Map
            public static string TILE_MAP_DOOR = "Tile_Spawn";
            public static string TILE_MAP_ROOM = "Tile_Spawn";
            public static string TILE_MAP_FLOOR = "Tile_Spawn";

        }
    }
    #endregion

    #region Setup Stats
    public static class SettingStats
    {
        public static float LENGTH_ROOM = 10;
        public static float LENGTH_CELL = 1;
        public static float GAME_SCALE = 3;
        public static float PADDING_DOOR_TELE_SCALE = 2f * LENGTH_ROOM / 10;
    }
    #endregion

    #region Dics Type-Name
    public static readonly Dictionary<RoomType, string> RoomTypeNames = new Dictionary<RoomType, string>{
        { RoomType.NormalRoom,   "NormalRoom"   },
        { RoomType.StartRoom,    "StartRoom"    },
        { RoomType.BossRoom,     "BossRoom"     },
        { RoomType.CombatRoom,   "CombatRoom"   },
        { RoomType.TreasureRoom, "TreasureRoom" },
        { RoomType.ShopRoom,     "ShopRoom"     },
        { RoomType.RestRoom,     "RestRoom"     },
        { RoomType.PuzzleRoom,   "PuzzleRoom"   },
        { RoomType.SecretRoom,   "SecretRoom"   },
        { RoomType.ExitRoom,     "ExitRoom"     },
    };
    #endregion

    #region Tile name
    public static class TileName
    {
        public static string ROOM = "Tile_Room";
        public static string DOOR = "Tile_Door";
        public static string FLOOR = "Tile_Floor";
        public static string SPAWN = "Tile_Spawn";
    }
    #endregion
}