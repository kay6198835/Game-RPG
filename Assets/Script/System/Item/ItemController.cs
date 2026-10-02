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
        ICharacter target = interactor.GetComponentInParent<ICharacter>();
        if (target == null)
        {
            Debug.LogWarning("Interact: interactor has no owning character");
            return false;
        }
        foreach (var effect in effects)
        {
            effect.Apply(target);
        }
        EventManager.Emit(EventID.ON_COLLECT_ITEM, gameObject);
        return true;
    }


}