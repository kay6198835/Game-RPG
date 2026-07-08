using System.Collections;
using System.Collections.Generic;
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
    private readonly Dictionary<System.Type, CoreComponent> _cache = new Dictionary<System.Type, CoreComponent>();

    public void AddCoreComponent(CoreComponent coreComponent)
    {
        if (!coreComponents.Contains(coreComponent)) coreComponents.Add(coreComponent);
    }

    public void GetCoreComponent<T>(out T coreComponent) where T : CoreComponent
    {
        var type = typeof(T);
        if (_cache.TryGetValue(type, out var cached))
        {
            coreComponent = (T)cached;
            return;
        }
        coreComponent = null;
        foreach (var comp in coreComponents)
        {
            if (comp is T match)
            {
                coreComponent = match;
                _cache[type] = match;
                return;
            }
        }
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
