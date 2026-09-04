using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// One phase of a boss fight: which commands it may use, how often, and at what range.
/// Weight, cooldown and range live on the entry rather than on the command asset so the same
/// Slam can be rare and long-cooldown in phase 1 and the backbone of phase 3.
/// </summary>
[CreateAssetMenu(fileName = "BossPhase", menuName = "Boss/Phase")]
public class BossPhaseSO : ScriptableObject
{
    public enum SelectionMode
    {
        WeightedRandom,
        Sequence
    }

    [Header("Identity")]
    [SerializeField] private string phaseName = "Phase";

    [Header("Entry")]
    [Tooltip("The phase becomes active when health ratio drops to or below this. " +
             "Order phases descending, e.g. 1.0 / 0.66 / 0.33.")]
    [SerializeField][Range(0f, 1f)] private float enterAtHealthRatio = 1f;

    [Tooltip("Seconds of invulnerable transition animation when this phase starts. 0 skips it.")]
    [SerializeField][Range(0f, 6f)] private float transitionDuration;

    [Header("Behaviour")]
    [SerializeField] private SelectionMode selection = SelectionMode.WeightedRandom;

    [Tooltip("Played in order once, on entering the phase. Use for a scripted opening combo.")]
    [SerializeField] private List<BossCommandSO> openingSequence = new List<BossCommandSO>();

    [SerializeField] private List<BossCommandEntry> commands = new List<BossCommandEntry>();

    public string PhaseName => string.IsNullOrEmpty(phaseName) ? name : phaseName;
    public float EnterAtHealthRatio => enterAtHealthRatio;
    public float TransitionDuration => transitionDuration;
    public SelectionMode Selection => selection;
    public IReadOnlyList<BossCommandSO> OpeningSequence => openingSequence;
    public IReadOnlyList<BossCommandEntry> Commands => commands;
}

/// <summary>A command slotted into a phase, with the per-phase tuning that governs when it is picked.</summary>
[System.Serializable]
public class BossCommandEntry
{
    [SerializeField] private BossCommandSO command;

    [Tooltip("Relative pick chance among all currently valid commands.")]
    [SerializeField][Range(0f, 100f)] private float weight = 10f;

    [Tooltip("Seconds before this command may be picked again.")]
    [SerializeField][Range(0f, 30f)] private float cooldown;

    [Header("Range Gate")]
    [SerializeField][Range(0f, 30f)] private float minDistance;
    [SerializeField][Range(0f, 30f)] private float maxDistance = 30f;

    public BossCommandSO Command => command;
    public float Weight => weight;
    public float Cooldown => cooldown;

    public bool IsInRange(float distanceToTarget) =>
        distanceToTarget >= minDistance && distanceToTarget <= maxDistance;
}
