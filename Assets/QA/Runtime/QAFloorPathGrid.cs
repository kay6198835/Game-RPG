#if UNITY_EDITOR
using UnityEngine;

namespace QA.Automation
{
    // EntityMovement.Start reads EnemyManager.Instance.Grid and caches it. A QA scene has no tilemap, so an
    // open floor is built from code: every tile inside the arena is walkable, which lets the enemy really
    // wander and chase (QA-ENT-10 / QA-ENT-15). Installed in Start, before the suite spawns its first enemy.
    public class QAFloorPathGrid : MonoBehaviour
    {
        public const int HalfWidth = 30;
        public const int HalfHeight = 20;

        private void Start()
        {
            if (EnemyManager.Instance == null || EnemyManager.Instance.Grid != null) return;
            LevelData floor = new();
            for (int x = -HalfWidth; x <= HalfWidth; x++)
            for (int y = -HalfHeight; y <= HalfHeight; y++)
            {
                floor.tiles.Add(GameConstants.TileName.FLOOR);
                floor.poses.Add(new Vector3Int(x, y, 0));
                floor.layerIndices.Add(0);
            }
            PathfindingGrid grid = new();
            grid.BuildGrid(floor, Vector2.zero);
            EnemyManager.Instance.SetPathfindingGrid(grid);
        }
    }
}
#endif
