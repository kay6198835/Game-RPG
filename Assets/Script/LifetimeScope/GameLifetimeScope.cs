using UnityEngine;
using VContainer;
using VContainer.Unity;

/// <summary>
/// Composition root for the stat layer. Owns the single StatsSO instance and hands it to
/// every consumer as IPlayerStatService, so nothing below this scope touches the asset
/// directly.
/// </summary>
public class GameLifetimeScope : LifetimeScope
{
    [Header("Data")]
    [SerializeField] private StatsSO _playerStats;

    [Header("Injection targets")]
    // Explicit list instead of RegisterComponentInHierarchy: this scope is dropped into
    // several scenes that hold different subsets of the stat UI, and a hierarchy lookup
    // throws on whichever component the current scene omits.
    [SerializeField] private MonoBehaviour[] _injectTargets;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterInstance(_playerStats);
        builder.Register<PlayerStatService>(Lifetime.Singleton).As<IPlayerStatService>();

        builder.RegisterBuildCallback(container =>
        {
            if (_injectTargets == null) return;
            for (int i = 0; i < _injectTargets.Length; i++)
            {
                if (_injectTargets[i] != null) container.Inject(_injectTargets[i]);
            }
        });
    }
}
