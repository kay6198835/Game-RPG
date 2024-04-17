using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class EntityCoreComponent : MonoBehaviour
{
    protected EntityCore entityCore;

    public EntityCore EntityCore { get => entityCore; set => entityCore = value; }

    protected virtual void Awake()
    {
        entityCore = transform.parent.GetComponent<EntityCore>();
    }
}
