using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityCore : CoreBase, INegativeReceiver
{
    [SerializeField] private Entity entity;
    #region Properties
    public Entity Entity { get => entity; }

    public void TakeDamage(int amountDamage, Vector2 attackPosition)
    {
        throw new System.NotImplementedException();
    }
    #endregion

    private void Awake()
    {
        entity = GetComponentInParent<Entity>();
    }
}
