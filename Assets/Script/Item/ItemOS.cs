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
    [SerializeField] public Image ItemSprite { get; protected set; }
    [SerializeField] public string NameItem { get; protected set; }
    [SerializeField] public int DropChance { get; protected set; }
    [SerializeField] public int Value { get; protected set; }
    [SerializeField] public StyleItem Style { get; protected set; }
    [SerializeField] private List<ItemEffectDefinition> effects;
    [SerializeField, TextArea(2, 5)] public string Description;
    public IReadOnlyList<ItemEffectDefinition> Effects => effects;
    private string id;
    public string Id => id;

    public string itemName;

#if UNITY_EDITOR
    private void OnValidate()
    {
        // Chỉ gán 1 lần, không ghi đè nếu đã có
        if (string.IsNullOrEmpty(id))
        {
            id = System.Guid.NewGuid().ToString();
            UnityEditor.EditorUtility.SetDirty(this);
        }
    }
#endif
}
