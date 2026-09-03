using UnityEngine;

/// <summary>
/// Authoring half of a boss command: designer data only, no runtime state.
/// Runtime state lives in the object returned by <see cref="CreateRuntime"/> because a
/// ScriptableObject is a shared asset — writing per-fight values onto it leaks between
/// bosses and, in the Editor, survives exiting Play Mode (see NEW-4 in CLAUDE.md).
/// </summary>
public abstract class BossCommandSO : ScriptableObject
{
    [Header("Identity")]
    [SerializeField] private string displayName = "Command";

    [Header("Animation")]
    [Tooltip("Animator bool held true while this command runs. Must exist on the boss Animator.")]
    [SerializeField] private string animBoolName = GameConstants.AnimationName.ATTACK;

    [Header("Timing")]
    [Tooltip("Hard ceiling in seconds. The command ends even if its animation events never fire.")]
    [SerializeField][Range(0.1f, 30f)] private float maxDuration = 5f;

    [Tooltip("Seconds held after the command's effect resolves, before the next one is picked.")]
    [SerializeField][Range(0f, 10f)] private float recoveryTime = 0.4f;

    public string DisplayName => string.IsNullOrEmpty(displayName) ? name : displayName;
    public string AnimBoolName => animBoolName;
    public float MaxDuration => maxDuration;
    public float RecoveryTime => recoveryTime;

    public abstract IBossCommand CreateRuntime();
}
