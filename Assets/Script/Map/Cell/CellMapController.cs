using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellMapController : MonoBehaviour, IMapController
{
    [SerializeField] CellControll prefabObject;
    [SerializeField] List<CellControll> _cellControll;
    public void AddCell(Cell cell)
    {
        CellControll cellControll = Instantiate(prefabObject, this.transform) as CellControll;
        cellControll.AddCell(cell);
        _cellControll.Add(cellControll);
    }

    public CellControll GetValue(int index)
    {
        return _cellControll[index];
    }

    public void SetValue(int index, CellControll cellControll)
    {
        _cellControll[index] = cellControll;
    }

    public void Clear()
    {
        throw new System.NotImplementedException();
    }

    public void Setting()
    {
        throw new System.NotImplementedException();
    }

    public void Setup()
    {
        throw new System.NotImplementedException();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
