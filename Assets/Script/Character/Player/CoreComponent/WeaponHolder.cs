using UnityEngine;

public class WeaponHolder : Interact
{
    [SerializeField] private Weapon weapon;

    public Weapon Weapon { get => weapon; }

    protected override void Awake()
    {
        base.Awake();
        interactableMask = LayerMask.GetMask("Weapon");
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
        weapon.OnActivate();
    }

    public void EndDamage()
    {
        if (weapon == null) return;
        weapon.OnDeactivate();
    }
}
