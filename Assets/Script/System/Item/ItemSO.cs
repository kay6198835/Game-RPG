using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item SO/Item")]
public class ItemSO : ScriptableObject
{
    [field: SerializeField] public Sprite ItemSprite { get; private set; }
    [field: SerializeField] public string ItemName { get; private set; }
    [field: SerializeField] public List<ItemEffectDefinitionSO> effects { get; private set; } = new List<ItemEffectDefinitionSO>();
    [field: SerializeField, TextArea(2, 5)] public string Description;
    public IReadOnlyList<ItemEffectDefinitionSO> Effects => effects;
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
