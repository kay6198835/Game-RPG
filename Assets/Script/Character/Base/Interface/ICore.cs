public interface ICore
{
    protected abstract void AddCoreComponent(ICoreComponent<ICore> coreComponent);
    public abstract void GetCoreComponent<T>(out T coreComponent) where T : ICoreComponent<ICore>;
    protected abstract void Setup();
}