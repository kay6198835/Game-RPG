using System;
using UnityEngine;
[Serializable]
public class AbilityContext
{
    public IAbilityOwner Caster;
    public Vector3 Origin;
    public Vector3 Forward;
    public Vector3 TargetPoint;

    public float HoldTime;
    public float HoldRatio;

    [NonSerialized] public AbilityInstance AbilityInstance;
    public AbilityDefinition AbilityDefinition;

    [NonSerialized] public IAbilityServices Services;
}

public interface IAbilityServices
{
    IObjecPoolService Pool { get; }
    IPlayerStatService Stats { get; }
    IResourceReceiver ResourceReceiver { get; }
    IVitalComponent Vital { get; }
    INegativeReceiver NegativeReceiver { get; }
}

public sealed class AbilityServices : IAbilityServices
{
    public IObjecPoolService Pool { get; }
    public IPlayerStatService Stats { get; }
    public IResourceReceiver ResourceReceiver { get; }
    public IVitalComponent Vital { get; }
    public INegativeReceiver NegativeReceiver { get; }

    public AbilityServices(
        IObjecPoolService pool,
        IPlayerStatService stats,
        IResourceReceiver resourceReceiver,
        IVitalComponent vital,
        INegativeReceiver negativeReceiver)
    {
        Pool = pool;
        Stats = stats;
        ResourceReceiver = resourceReceiver;
        Vital = vital;
        NegativeReceiver = negativeReceiver;
    }
}
