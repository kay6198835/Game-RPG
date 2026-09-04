using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
public class ItemController : InteractiveObjects
{
    [SerializeField] public Sprite ItemSprite { get; protected set; }
    [SerializeField] public string NameItem { get; protected set; }
    [SerializeField] public int Id { get; protected set; }
    [SerializeField] public ItemOS Data { get; protected set; }

    protected override void Awake()
    {
        base.Awake();
        ItemSprite = GetComponent<Sprite>();
    }

    public void SetDataItem()
    {
        //warning need fix
        //Data = data;
    }

    public override bool Interact(Interact interactor)
    {
        // Do something apply effect 
        foreach (var item in Data.Effects)
        {
            item.Apply((ResourceReceiver)interactor);
        }
        return true;
    }


}