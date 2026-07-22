using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityCore : CoreBase, INegativeReceiver
{
    [SerializeField] private Entity entity;
    #region Properties
    public Entity Entity { get => entity; }
    #endregion

    private void Awake()
    {
        entity = GetComponentInParent<Entity>();
    }
}
