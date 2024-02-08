using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "Item SO/Item")]
public class ItemOS : ScriptableObject
{
    public Sprite itemSprite;
    public string nameItem;
    public int dropChance;
    public int value;
}
