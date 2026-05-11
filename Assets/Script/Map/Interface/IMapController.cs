using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMapController
{
    public int Columns { get; }
    public int Rows { get;}
    void AddCell(Cell cell);
    void Setting(int Columns, int Rows);


}


public interface IMapController<T>: IMapController
{
    T GetValue(int index);
    void SetValue(int index, T controller);
    T GetNext(Vector2 direction);
    T GetStart();
}
