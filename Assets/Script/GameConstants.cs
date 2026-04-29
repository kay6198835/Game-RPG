using UnityEngine;

public static class GameConstants
{
	public static class Direction
	{
		public static readonly Vector2 TOP = Vector2.up;
        public static readonly Vector2 RIGHT = Vector2.right;
        public static readonly Vector2 LEFT = Vector2.left;
        public static readonly Vector2 BOTTOM = Vector2.down;
		//Sub Direction
        public static readonly Vector2 TOP_RIGHT = Vector2.up;
        public static readonly Vector2 RIGHT_DOWN = Vector2.up;
        public static readonly Vector2 DOWN_LEFT = Vector2.up;
        public static readonly Vector2 LEFT_TOP = Vector2.up;
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
	}
	public static class Input{
		public static readonly string VERTICAL = "Vertical";
		public static readonly string HORIZONTAL = "Horizontal";
        public static class Name
		{

        }
	}

	public static class Event
	{

	}
}