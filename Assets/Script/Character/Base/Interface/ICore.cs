public interface ICore
{
    public void AddCoreComponent(ICoreComponent<ICore> coreComponent);
    public void GetCoreComponent<T>(out T coreComponent) where T : ICoreComponent<ICore>;

}