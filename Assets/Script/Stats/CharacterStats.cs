using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    [Header("Base Stats")]
    [SerializeField] private float baseAttack = 10f;
    [SerializeField] private float baseMoveSpeed = 5f;
    [SerializeField] private float baseMaxMana = 100f;

    public RuntimeStat Attack { get; private set; }
    public RuntimeStat MoveSpeed { get; private set; }
    public RuntimeStat MaxMana { get; private set; }

    public float CurrentMana { get; private set; }

    private void Awake()
    {
        Attack = new RuntimeStat(baseAttack);
        MoveSpeed = new RuntimeStat(baseMoveSpeed);
        MaxMana = new RuntimeStat(baseMaxMana);

        CurrentMana = MaxMana.Value;
    }

    public float GetStatValue(StatType type)
    {
        return type switch
        {
            StatType.Attack => Attack.Value,
            StatType.MoveSpeed => MoveSpeed.Value,
            StatType.MaxMana => MaxMana.Value,
            _ => 0f
        };
    }

    public void SpendMana(float amount)
    {
        CurrentMana -= amount;
        if (CurrentMana < 0f) CurrentMana = 0f;
    }

    public void RecoverMana(float amount)
    {
        CurrentMana += amount;
        float maxMana = MaxMana.Value;
        if (CurrentMana > maxMana) CurrentMana = maxMana;
    }
}
