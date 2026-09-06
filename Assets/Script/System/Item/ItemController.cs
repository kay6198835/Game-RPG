using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
public class ItemController : InteractiveObjects
{
    [SerializeField] private Sprite ItemSprite;
    [SerializeField] private string NameItem;
    [SerializeField] private IReadOnlyList<ItemEffectDefinition> effects;

    protected override void Awake()
    {
        base.Awake();
        ItemSprite = GetComponent<Sprite>();
    }

    public void SetDataItem(ItemSO data)
    {
        ItemSprite = data.ItemSprite;
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
        foreach (var item in effects)
        {
            item.Apply((ResourceReceiver)interactor);
        }
        return true;
    }


}