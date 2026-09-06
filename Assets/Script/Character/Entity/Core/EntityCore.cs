using UnityEngine;

public class EntityCore : CoreBase
{
    [SerializeField] protected Entity entity;
    #region Properties
    public Entity Entity { get => entity; }
    #endregion

    protected override void Awake()
    {
        entity = GetComponentInParent<Entity>();
        base.Awake();
    }
}
