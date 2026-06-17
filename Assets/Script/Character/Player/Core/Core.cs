using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Core : MonoBehaviour
{
    [SerializeField] public Player Player { get; private set; }
    // [SerializeField] public PlayerMovement Movement { get; private set; }
    // [SerializeField] public WeaponHolder WeaponHolder { get; private set; }
    // [SerializeField] public AbilityHolder AbilityHolder { get; private set; }
    // [SerializeField] public Interactor Interactor { get; private set; }
    // [SerializeField] public PlayerInputHandler InputHandler { get; private set; }

    [SerializeField] private List<CoreComponent> coreComponents = new List<CoreComponent>();

    public void AddCoreComponent(CoreComponent coreComponent)
    {
        if (!coreComponents.Contains(coreComponent)) coreComponents.Add(coreComponent);
    }

    public void GetCoreComponent<T>(out T coreComponent) where T : CoreComponent
    {
        var comp = coreComponents.OfType<T>().FirstOrDefault();
        if (comp != null) coreComponent = comp;
        else coreComponent = null;
    }
    private void Awake()
    {
        Player = GetComponentInParent<Player>();
        // Movement = GetComponentInChildren<PlayerMovement>();
        // WeaponHolder = GetComponentInChildren<WeaponHolder>();
        // AbilityHolder = GetComponentInChildren<AbilityHolder>();
        // Interactor = GetComponentInChildren<Interactor>();
    }
}
