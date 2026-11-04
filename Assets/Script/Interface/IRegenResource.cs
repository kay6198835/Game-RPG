public interface IRegenResource
{
    void Regenning();
}

public interface IRegenResource<TCore> : IRegenResource where TCore : ICore
{
}