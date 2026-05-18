using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(SpriteRenderer))]
public abstract class BaseCell : MonoBehaviour, IGridItem
{
    [SerializeField] protected Cell _cellData;
    [SerializeField] protected SpriteRenderer _spriteRenderer;

    protected virtual void Awake()
    {
        if (_spriteRenderer == null)
            _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void AddCell(Cell cell)
    {
        _cellData = cell;
        Setting();
    }

    public Vector2 GetGridPosition() => new Vector2(_cellData.Column, _cellData.Row);

    public void SetBeNextRoom(Vector2 direction)
    {
        _spriteRenderer.color = Color.red;
    }

    protected abstract void Setting();
}
