using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TalentManagger : MonoBehaviour
{
    private int strength,dexterity,intelligence,charima;
    [SerializeField] private int skillPoint;
    #region Properties
    public int Strength =>  strength;
    public int Dexterity => dexterity;
    public int Intelligence => intelligence;
    public int Charisma => charima;
    public int SkillPoint => skillPoint;
    #endregion
    public UnityAction OnSkillPointChanged;
    private void Awake()
    {
        strength = 10;
        dexterity = 10;
        intelligence = 10;
        charima = 10;
        skillPoint = 10;

    }
    public void GainSkillPoint()
    {
        skillPoint ++;
        OnSkillPointChanged?.Invoke();
    }
}
