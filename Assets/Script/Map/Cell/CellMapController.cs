using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellMapController : MonoBehaviour, IMapController
{
    [SerializeField] CellController prefabObject;
    [SerializeField] List<CellController> _cellController;
    public void AddCell(Cell cell)
    {
        CellController cellController = Instantiate(prefabObject, this.transform) as CellController;
        cellController.AddCell(cell);
        _cellController.Add(cellController);
    }

    public CellController GetValue(int index)
    {
        return _cellController[index];
    }

    public void SetValue(int index, CellController cellController)
    {
        _cellController[index] = cellController;
    }

    public void Clear()
    {
        throw new System.NotImplementedException();
    }

    public void Setting(int Columns, int Rows)
    {
        
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
