using System.Collections.Generic;
using TMPro;
using UIFlow;
using UnityEngine;

// Stationary damage target for the CombatTest scene. Every weapon and ability resolves its receiver with
// TryGetComponent on the hit collider, so the Collider2D must sit on this GameObject, on a layer the
// weapon's LayerMask hits (Enemy).
[RequireComponent(typeof(Collider2D))]
public class TrainingDummy : MonoBehaviour, INegativeReceiver
{
    [SerializeField] private float maxHP = 1000f;
    [Tooltip("Subtracted from each hit, like EntityNegativeReciver: damage - defense, clamped at 0.")]
    [SerializeField] private float defense;
    [Tooltip("HP refills after this many seconds without a hit. 0 = never.")]
    [SerializeField] private float resetDelay = 3f;
    [SerializeField] private bool invulnerable = true;
    [SerializeField] private float dpsWindow = 3f;
    [SerializeField] private bool logHits = true;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private TextMeshPro label;
    [SerializeField] private Color hitColor = new(1f, 0.35f, 0.35f);
    [SerializeField] private float flashTime = 0.1f;

    private readonly Queue<(float time, float damage)> _recentHits = new();
    private float _currentHP;
    private float _totalDamage;
    private int _hitCount;
    private float _lastHitTime = float.NegativeInfinity;
    private float _flashUntil;
    private Color _baseColor = Color.white;

    private void Awake()
    {
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null) _baseColor = spriteRenderer.color;
        ResetDummy();
    }

    public void TakeDamage(float amountDamage, Vector2 attackPosition)
    {
        float finalDamage = Mathf.Max(0f, amountDamage - defense);
        _currentHP = Mathf.Max(invulnerable ? 1f : 0f, _currentHP - finalDamage);
        _totalDamage += finalDamage;
        _hitCount++;
        _lastHitTime = Time.time;
        _flashUntil = Time.time + flashTime;
        _recentHits.Enqueue((Time.time, finalDamage));

        UIEvents.ShowDamage(transform.position, finalDamage);
        if (logHits)
            Debug.Log($"[TrainingDummy] {name}: raw {amountDamage:0.#} -> final {finalDamage:0.#} " +
                      $"(def {defense:0.#}) | HP {_currentHP:0}/{maxHP:0} | from {attackPosition}", this);

        if (_currentHP <= 0f) Debug.Log($"[TrainingDummy] {name}: killed after {_hitCount} hits", this);
    }

    private void Update()
    {
        while (_recentHits.Count > 0 && Time.time - _recentHits.Peek().time > dpsWindow) _recentHits.Dequeue();

        if (resetDelay > 0f && _hitCount > 0 && Time.time - _lastHitTime > resetDelay) ResetDummy();

        if (spriteRenderer != null) spriteRenderer.color = Time.time < _flashUntil ? hitColor : _baseColor;
        if (label != null) label.text = BuildLabel();
    }

    [ContextMenu("Reset")]
    public void ResetDummy()
    {
        _currentHP = maxHP;
        _totalDamage = 0f;
        _hitCount = 0;
        _recentHits.Clear();
    }

    private string BuildLabel()
    {
        float windowDamage = 0f;
        foreach (var (_, damage) in _recentHits) windowDamage += damage;
        return $"HP {_currentHP:0}/{maxHP:0}  DEF {defense:0}\n" +
               $"DPS {windowDamage / dpsWindow:0.#}  Hits {_hitCount}  Total {_totalDamage:0}";
    }
}
