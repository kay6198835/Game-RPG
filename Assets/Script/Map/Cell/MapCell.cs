using UnityEngine;
using UnityEngine.UIElements;

[ExecuteInEditMode]
[RequireComponent(typeof(SpriteRenderer))]
public class MapCell : BaseCell
{
    private float CELL_SCALE = GameConstants.SettingStats.GAME_SCALE;
    private float PADDING_SCALE = GameConstants.SettingStats.LENGTH_CELL * 2;
    [SerializeField] private GameObject _top, _left, _right, _bottom;
    [SerializeField] public Transform StartDoorPosition { get; private set; }

    protected override void Setting()
    {
        if (_cellData == null) return;
        transform.localScale = Vector3.one * CELL_SCALE;
        transform.SetPositionAndRotation(
            MazeController.Instance.MapGrid.transform.position
                + new Vector3(_cellData.Column, -_cellData.Row) * CELL_SCALE * PADDING_SCALE,
            Quaternion.identity);
        _right.transform.SetLocalPositionAndRotation(Vector3.right, _right.transform.rotation);
        _top.transform.SetLocalPositionAndRotation(Vector3.up, _top.transform.rotation);
        _left.transform.SetLocalPositionAndRotation(Vector3.left, _left.transform.rotation);
        _bottom.transform.SetLocalPositionAndRotation(Vector3.down, _bottom.transform.rotation);
        if (_cellData.Doors[GameConstants.Direction.Name.TOP] == STATUS_DOOR.ENEBLE) _top.SetActive(true);
        if (_cellData.Doors[GameConstants.Direction.Name.LEFT] == STATUS_DOOR.ENEBLE) _left.SetActive(true);
        if (_cellData.Doors[GameConstants.Direction.Name.RIGHT] == STATUS_DOOR.ENEBLE) _right.SetActive(true);
        if (_cellData.Doors[GameConstants.Direction.Name.BOTTOM] == STATUS_DOOR.ENEBLE) _bottom.SetActive(true);
    }
}
