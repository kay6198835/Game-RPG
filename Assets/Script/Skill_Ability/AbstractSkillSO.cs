using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public abstract class AbstractSkillSO : ScriptableObject
{
    [SerializeField] protected string skillName;
    [SerializeField] protected bool overWriteDescription;
    [TextArea(1, 4)][SerializeField] private string skillDescription;
    #region Properties
    public string SkillName { get => skillName; }
    public string SkillDescription { get => skillDescription; }
    public bool OverWriteDescription { get => overWriteDescription; }
    #endregion
    protected virtual void OnValidate()
    {
        skillName = name;
        GenerateDescription();
    }
    protected abstract void GenerateDescription();
}
