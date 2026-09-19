using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
public class ItemController : InteractiveObjects
{
    [SerializeField] private SpriteRenderer ItemSprite;
    [SerializeField] private string NameItem;
    [SerializeField] private IReadOnlyList<ItemEffectDefinition> effects;

    protected override void Awake()
    {
        base.Awake();
        ItemSprite = GetComponent<SpriteRenderer>();
    }

    public void SetDataItem(ItemSO data)
    {
        ItemSprite.sprite = data.ItemSprite;
        name = data.ItemName;
        effects = data.Effects;
    }

    public override bool Interact(Interact interactor)
    {
        if (interactor == null)
        {
            Debug.LogWarning("Interact: interactor is null");
            return false;
        }
        // Do something apply effect 
        foreach (var effect in effects)
        {
            effect.Apply((ResourceReceiver)interactor);
        }
        EventManager.Emit(EventID.ON_COLLECT_ITEM, gameObject);
        return true;
    }


}