using System.Collections.Generic;
using UnityEngine;

public static class GameConstants
{
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
    public static class Input
    {
        //public static readonly {{Type}} {{Name}} = Button_Name;
        public static readonly string VERTICAL = "Vertical";
        public static readonly string HORIZONTAL = "Horizontal";
        public static class Name
        {

        }
    }
    public static class RouteAsset
    {
        //public static readonly string {{Name}} = {{Assets\Script\Skill_Ability}};
    }
    public static class SettingStats
    {
        public static float LENGTH_ROOM = 10;
        public static float LENGTH_CELL = 1;
        public static float GAME_SCALE = 1;
        public static float PADDING_DOOR_TELE_SCALE = GAME_SCALE * LENGTH_ROOM / 10;
    }
    public static readonly Dictionary<RoomType, string> RoomTypeNames = new Dictionary<RoomType, string>
    {
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
}