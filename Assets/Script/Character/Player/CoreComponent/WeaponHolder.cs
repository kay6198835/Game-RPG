using UnityEngine;

public class WeaponHolder : Interact
{
    [SerializeField] private Weapon weapon;
    VitalStatsComponent vitalStatsComponent;

    public Weapon Weapon { get => weapon; }

    protected override void Awake()
    {
        base.Awake();
        interactableMask = LayerMask.GetMask("Weapon");
    }

    protected override void Start()
    {
        base.Start();
        Core.GetCoreComponent(out vitalStatsComponent);
    }

    public void Equid_UnEquid(Weapon weapon)
    {
        this.weapon = this.weapon == null ? weapon : null;
    }

    public override void Intertion()
    {
        if (weapon != null)
        {
            weapon.UnEquid(this);
            return;
        }
        base.Intertion();
    }

    /// <summary>Starts one attack stage on the equipped weapon. Safe to call repeatedly to chain.</summary>
    public void Attack()
    {
        if (weapon == null) return;
        weapon.OnAttackEnter(Core.Player);
    }

    public bool CanAttack() => weapon != null && weapon.CanAttack();

    public bool CanChain() => weapon != null && weapon.CanChain();

    public void MakeDamage()
    {
        if (weapon == null) return;
        weapon.OnActivate(CalculateCurrentDamage());
    }

    public void EndDamage()
    {
        if (weapon == null) return;
        weapon.OnDeactivate();
    }

    private float CalculateCurrentDamage()
    {
        float finalDamage = 0;
        finalDamage = vitalStatsComponent.GetCurrentStatValue(StatType.PhysicalDamage)
         + weapon.CurrentStage.attackDamage;
        if (Utility.RollChance(vitalStatsComponent.GetCurrentStatValue(StatType.CritChance)))
            finalDamage += vitalStatsComponent.GetCurrentStatValue(StatType.CritDamage);
        return finalDamage;
    }
}
