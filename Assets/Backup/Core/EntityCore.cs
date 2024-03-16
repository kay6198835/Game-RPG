using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityCore : MonoBehaviour
{
    [SerializeField] private Entity entity;
    [SerializeField] private EntityMovement entityMovement;
    [SerializeField] private EntityFindTarget findTarget;
    public EntityMovement EntityMovement { get => entityMovement;}
    public EntityFindTarget FindTarget { get => findTarget; }
    public Entity Entity { get => entity;}

    private void Awake()
    {
        entity = GetComponentInParent<Entity>();
        entityMovement = GetComponentInChildren<EntityMovement>();
        findTarget = GetComponentInChildren<EntityFindTarget>();
    }
}
