using UnityEngine;

[CreateAssetMenu(menuName = "WeaponData/AttackStage/Melee")]
public class AttackSO : ScriptableObject
{
    [SerializeField] public string nameState;

    [Header("Stats")]
    [SerializeField, Range(0.1f, 20f)] public float attackRange = 1f;
    [SerializeField, Range(0, 999)] public int attackDamege = 0;
    [SerializeField, Range(0.1f, 10f)] public float attackRate = 1f;

    [Header("Atributte")]
    [SerializeField] public AnimatorOverrideController directionAttackAnimatorOV;
    [SerializeField] public ActivateSkill ability;
}
