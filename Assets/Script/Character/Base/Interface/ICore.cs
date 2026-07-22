public interface ICore
{
    public void AddCoreComponent(CoreComponentBase coreComponent);
    public void GetCoreComponent<T>(out T coreComponent) where T : CoreComponentBase;

}