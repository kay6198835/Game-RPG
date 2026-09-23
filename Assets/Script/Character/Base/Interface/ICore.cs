public interface ICore
{
    void AddCoreComponent(ICoreComponent<ICore> coreComponent);
    void GetCoreComponent<T>(out T coreComponent) where T : ICoreComponent<ICore>;
    /// <summary>
    /// Finds the first registered core component that implements <typeparamref name="T"/>, which may be
    /// any interface (IStatService, IVitalComponent…). Returns false and sets null when none does.
    /// Only valid after the hub's Setup() has run.
    /// </summary>
    bool TryGetCapability<T>(out T capability) where T : class;
    /// <summary>The character that owns this hub; null when the hub is not under an ICharacter.</summary>
    ICharacter Character { get; }
    void Setup();
}
