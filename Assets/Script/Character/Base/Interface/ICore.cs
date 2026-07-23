public interface ICore
{
    void AddCoreComponent(ICoreComponent<ICore> coreComponent);
    void GetCoreComponent<T>(out T coreComponent) where T : ICoreComponent<ICore>;
    void Setup();
}
