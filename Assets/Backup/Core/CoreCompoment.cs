using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoreCompoment : MonoBehaviour
{
    protected Core core;
    protected virtual void Awake()
    {
        core = transform.parent.GetComponent<Core>();
    }
}
