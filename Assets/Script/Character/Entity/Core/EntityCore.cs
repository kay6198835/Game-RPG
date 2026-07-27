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

    protected override void Awake()
    {
        entity = GetComponentInParent<Entity>();
        base.Awake();
    }
}
