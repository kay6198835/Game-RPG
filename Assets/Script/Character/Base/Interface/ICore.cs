public interface ICore
{
    public abstract void AddCoreComponent(ICoreComponent<ICore> coreComponent);
    public abstract void GetCoreComponent<T>(out T coreComponent) where T : ICoreComponent<ICore>;
}