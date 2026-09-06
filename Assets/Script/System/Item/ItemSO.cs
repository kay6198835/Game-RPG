using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(menuName = "Item SO/Item")]
public class ItemSO : ScriptableObject
{
    public enum StyleItem
    {
        Equippable,
        Consumable
    }
    [field: SerializeField] public Sprite ItemSprite { get; private set; }
    [field: SerializeField] public string ItemName { get; private set; }
    [field: SerializeField] public StyleItem Style { get; private set; } = StyleItem.Consumable;
    [field: SerializeField] public List<ItemEffectDefinition> effects { get; private set; } = new List<ItemEffectDefinition>();
    [field: SerializeField, TextArea(2, 5)] public string Description;
    public IReadOnlyList<ItemEffectDefinition> Effects => effects;
    [SerializeField] private string id;
    public string Id => id;

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
