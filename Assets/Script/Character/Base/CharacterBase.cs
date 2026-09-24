using UnityEngine;

/// <summary>
/// Shared base of Player and Entity (the BaseGrid&lt;T&gt; of the character layer). Resolves the
/// character contract from its own core hub; nothing here comes from the DI container.
/// </summary>
public abstract class CharacterBase<TCore> : BaseEntity, ICharacter<TCore> where TCore : CoreBase
{
    [SerializeField] protected TCore core;

    private IStatService _stats;
    private IVitalComponent _vital;
    private INegativeReceiver _damageReceiver;

    public TCore Core => core;
    ICore ICharacter.Core => core;
    public Transform Transform => transform;

    // Lazy for the same reason as StatHandlerBase: the hub fills its component list in its own Awake.
    public IStatService Stats => _stats ??= Resolve<IStatService>();
    public IVitalComponent Vital => _vital ??= Resolve<IVitalComponent>();
    public INegativeReceiver DamageReceiver => _damageReceiver ??= Resolve<INegativeReceiver>();

    private T Resolve<T>() where T : class
    {
        if (core != null && core.TryGetCapability(out T capability)) return capability;
        Debug.LogError($"[{name}] no {typeof(T).Name} registered under its core hub.", this);
        return null;
    }
}
