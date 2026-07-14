using UnityEngine;

public class EntityCoreComponent : CoreComponentBase
{
    protected EntityCore entityCore;

    public EntityCore EntityCore { get => entityCore; set => entityCore = value; }

    protected virtual void Awake()
    {
        entityCore = transform.parent.GetComponent<EntityCore>();
        if (entityCore != null) entityCore.AddCoreComponent(this);
    }
}
