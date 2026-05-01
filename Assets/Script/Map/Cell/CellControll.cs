using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(SpriteRenderer))]
public class CellControll : MonoBehaviour
{
    [SerializeField] private Cell _cellData;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private GameObject _top,_left,_right,_bottom;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
    public void Setting()
    {
        if (_cellData == null)
        {
            return;
        }
        this.transform.SetPositionAndRotation(new Vector3(_cellData.Column, -_cellData.Row) * 2, Quaternion.identity);
        if (_cellData.Visited)
        {
            _spriteRenderer.color = Color.red;
        }

        if (_cellData.Top == (int)STATUS_DOOR.OPEN)
        {
            _top.SetActive(true);   
        }
        if (_cellData.Right == (int)STATUS_DOOR.OPEN)
        {
            _right.SetActive(true);
        }
        if (_cellData.Left == (int)STATUS_DOOR.OPEN)
        {
            _left.SetActive(true);
        }
        if (_cellData.Bottom == (int)STATUS_DOOR.OPEN)
        {
            _bottom.SetActive(true);
        }
    }

    public void AddCell(Cell cell)
    {
        this._cellData = cell;
        this.Setting();
    }
    public Vector2 GetGridPosition()
    {
        return new Vector2(_cellData.Column, _cellData.Row);
    }
}
