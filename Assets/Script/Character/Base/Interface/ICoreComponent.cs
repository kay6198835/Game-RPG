public interface ICoreComponent
{
}

public interface ICoreComponent<out T> : ICoreComponent where T : ICore
{
    // public abstract void Setup();
}