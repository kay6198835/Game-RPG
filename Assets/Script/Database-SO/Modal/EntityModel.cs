using System.Collections.Generic;
using UnityEngine;
public class EntityModel : ScriptableObject
{
    [SerializeField] private int id;
    public int ID => id;

    public string nameEnity;

#if UNITY_EDITOR
    protected virtual void OnValidate()
    {
        if (id == 0)
        {
            id = System.Math.Abs(System.Guid.NewGuid().GetHashCode());
            UnityEditor.EditorUtility.SetDirty(this);
        }
    }
#endif
}