using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "TileData", menuName = "Data/Tile")]
public class TileSO : ScriptableObject
{
    public TileBase tileBase;
    public string id;
}

