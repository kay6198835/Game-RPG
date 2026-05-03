using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMapController
{
    void Setup();
    void Clear();
    void AddCell(Cell cell);
    void Setting(int Columns, int Rows);
}
