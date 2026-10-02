using UnityEngine;

public class EntityCore : CoreBase
{
    [SerializeField] protected Entity entity;
    #region Properties
    public Entity Entity { get => entity; }
    public override CharacterData Data => (entity != null ? entity : GetComponentInParent<Entity>()).Data;
    #endregion

    protected override void Awake()
    {
        entity = GetComponentInParent<Entity>();
        base.Awake();
    }
}
