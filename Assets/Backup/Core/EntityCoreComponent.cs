using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class EntityCoreComponent : MonoBehaviour
{
    protected EntityCore entityCore;
    protected virtual void Awake()
    {
        entityCore = transform.parent.GetComponent<EntityCore>();
    }
}
