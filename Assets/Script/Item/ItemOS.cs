using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "Item SO/Item")]
public class ItemOS : ScriptableObject
{
    public enum StyleItem
    {
        Equippable,
        Consumable
    }
    [SerializeField] protected Sprite itemSprite;
    [SerializeField] protected string nameItem;
    [SerializeField] protected int dropChance;
    [SerializeField] protected int value;
    [SerializeField] protected StyleItem style;
    public Sprite ItemSprite { get => itemSprite;}
    public string NameItem { get => nameItem;}
    public int DropChance { get => dropChance; }
    public int Value { get => value; }
    public StyleItem Style { get => style;}
}
