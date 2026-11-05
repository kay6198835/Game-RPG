#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace QA.Automation
{
    // A real enemy prefab spawned through ObjectPoolManager (as EnemySpawner does, so Pool.Spawn injects it):
    // components, default weapon, Defense-mitigated damage, death event, pooled respawn, asset hygiene.
    public class QAEnemySuite : QASuite
    {
        [SerializeField] private GameObject enemyPrefab;
        [SerializeField] private ObjectPoolManager pool;
        [SerializeField] private Transform spawnPoint;

        private GameObject _enemy;
        private EntityVitalStats _vital;
        private EntityStatsHandler _stats;
        private EntityNegativeReciver _hurtbox;
        private EntityWeaponHolder _weapon;
        private readonly List<GameObject> _deaths = new();
        private Dictionary<StatType, float> _assetSnapshot;
        private BaseStatsSO _statsAsset;

        private void OnEnable() => EventManager.Resgister(EventID.ON_ENEMY_DEATH, OnEnemyDeath);
        private void OnDisable() => EventManager.UnResgister(EventID.ON_ENEMY_DEATH, OnEnemyDeath);

        public override void Cleanup()
        {
            if (_enemy != null && pool != null) pool.Release(_enemy);
        }

        public override IEnumerator Run()
        {
            Entity entity = enemyPrefab != null ? enemyPrefab.GetComponentInChildren<Entity>(true) : null;
            _statsAsset = entity != null && entity.Data != null ? entity.Data.Stats : null;
            _assetSnapshot = _statsAsset != null ? _statsAsset.FullStatsValue() : null;

            yield return Case("QA-ENT-01", "ENT", QAPriority.P0, "Pool spawns the enemy prefab with its core components and full HP", CaseSpawn());
            yield return Case("QA-ENT-02", "ENT", QAPriority.P0, "Spawned enemy has its default weapon equipped", Guard(CaseWeapon()));
            yield return Case("QA-ENT-03", "ENT", QAPriority.P0, "Damage lowers HP by amount minus Defense", Guard(CaseDamage()));
            yield return Case("QA-ENT-04", "ENT", QAPriority.P0, "Lethal damage emits ON_ENEMY_DEATH once with the enemy as payload", Guard(CaseDeath()));
            yield return Case("QA-ENT-05", "ENT", QAPriority.P1, "Released then re-spawned enemy comes back with full HP", CaseRespawn());
            yield return Case("QA-ENT-06", "ENT", QAPriority.P2, "Enemy stat asset unchanged after spawn, damage and respawn", CaseAssetClean());
            yield return Case("QA-ENT-07", "ENT", QAPriority.P1, "Spawned enemy reads move speed and attack range from its stats", Guard(CaseStatDriven()));
        }

        private IEnumerator Guard(IEnumerator body)
        {
            if (_enemy == null || _vital == null || _stats == null || _hurtbox == null)
            {
                Fail("enemy not spawned or components missing (see QA-ENT-01)");
                yield break;
            }
            yield return body;
        }

        private IEnumerator Spawn()
        {
            _enemy = pool.Spawn(enemyPrefab, spawnPoint.position, Quaternion.identity);
            // Same as EnemySpawner.ApplyData() since 6094aa10: the spawner applies the EntityData after the pool
            // re-enabled the enemy, and that is what equips the default weapon and abilities.
            if (_enemy != null && _enemy.TryGetComponent(out Entity spawned) && spawned.Data != null)
                spawned.SetCharacterData(spawned.Data);
            yield return Frames(3);
            if (_enemy == null) yield break;
            _vital = _enemy.GetComponentInChildren<EntityVitalStats>(true);
            _stats = _enemy.GetComponentInChildren<EntityStatsHandler>(true);
            _hurtbox = _enemy.GetComponentInChildren<EntityNegativeReciver>(true);
            _weapon = _enemy.GetComponentInChildren<EntityWeaponHolder>(true);
            // Pin it: the suite measures damage, not chase behaviour.
            _enemy.GetComponentInChildren<IMovement>(true)?.AddSpeedMultiplier(this, 0f);
        }

        private float Hp => _vital.GetCurrentStatValue(VitalStatType.HP);
        private float MaxHp => _stats.GetStatValue(StatType.HP);

        private IEnumerator CaseSpawn()
        {
            if (enemyPrefab == null || pool == null)
            {
                Fail("scene not wired: enemy prefab or ObjectPoolManager missing");
                yield break;
            }
            yield return Spawn();
            Check(_enemy != null && _enemy.activeInHierarchy, "enemy spawned and active", "pool returned no active enemy");
            if (_enemy == null) yield break;
            Check(_enemy.GetComponentInChildren<Entity>(true) != null, "Entity found", "no Entity component");
            Check(_vital != null && _stats != null && _hurtbox != null,
                "vital, stat and hurtbox components found", $"vital={_vital != null} stats={_stats != null} hurtbox={_hurtbox != null}");
            if (_vital == null || _stats == null) yield break;
            Check(MaxHp > 0f && Near(Hp, MaxHp), $"HP {Hp:0.#}/{MaxHp:0.#} full", $"HP {Hp:0.#}/{MaxHp:0.#}");
        }

        private IEnumerator CaseWeapon()
        {
            bool armed = false;
            yield return WaitUntil(() => _weapon != null && _weapon.Weapon != null, 1f, ok => armed = ok);
            Check(armed, $"weapon = {(armed ? _weapon.Weapon.name : "-")}",
                _weapon == null ? "no EntityWeaponHolder on the enemy" : "EntityWeaponHolder.Weapon is null 1s after spawn (no weapon, no attack)");
        }

        private IEnumerator CaseDamage()
        {
            _vital.ReloadComponent();
            yield return null;
            float defense = _stats.GetStatValue(StatType.Defense);
            float amount = defense + Mathf.Max(5f, MaxHp * 0.1f);
            float before = Hp;
            _hurtbox.TakeDamage(amount, (Vector2)_enemy.transform.position + Vector2.left);
            yield return Frames(2);
            float lost = before - Hp;
            float expected = Mathf.Max(0f, amount - defense);
            Check(Near(lost, expected), $"lost {lost:0.##} = {amount:0.##} - Defense {defense:0.##}", $"lost {lost:0.##}, expected {expected:0.##} ({amount:0.##} - Defense {defense:0.##})");

            before = Hp;
            _hurtbox.TakeDamage(defense * 0.5f, (Vector2)_enemy.transform.position + Vector2.left);
            yield return Frames(2);
            Check(Near(before, Hp), "damage below Defense deals 0", $"damage below Defense changed HP {before:0.##} -> {Hp:0.##}");
        }

        private IEnumerator CaseDeath()
        {
            _deaths.Clear();
            _hurtbox.TakeDamage(MaxHp * 10f + _stats.GetStatValue(StatType.Defense), (Vector2)_enemy.transform.position + Vector2.left);
            bool died = false;
            yield return WaitUntil(() => _deaths.Count > 0, 5f, ok => died = ok);
            Check(died, "ON_ENEMY_DEATH emitted", "no ON_ENEMY_DEATH within 5s of lethal damage");
            Entity entity = _enemy.GetComponentInChildren<Entity>(true);
            if (!died && entity != null)
            {
                Animator anim = entity.Anim;
                bool hasController = anim != null && anim.runtimeAnimatorController != null;
                AnimatorClipInfo[] clips = hasController ? anim.GetCurrentAnimatorClipInfo(0) : new AnimatorClipInfo[0];
                Note($"HP {Hp:0.#}; state {entity.StateMachine.CurrentState?.GetType().Name}; animator {(hasController ? anim.runtimeAnimatorController.name : "none")}; clip {(clips.Length > 0 ? clips[0].clip.name : "-")}");
                if (clips.Length > 0)
                    Note("clip events: " + string.Join(", ", clips[0].clip.events.Select(e => e.functionName)) + " (EntityDeathState emits on StatusAnimation.OnActivate)");
            }
            if (!died) yield break;
            Check(_deaths[0] == _enemy, "payload is the enemy GameObject", $"payload {(_deaths[0] != null ? _deaths[0].name : "null")}");
            yield return Wait(1f);
            Check(_deaths.Count == 1, "emitted once", $"emitted {_deaths.Count} times");
        }

        private IEnumerator CaseRespawn()
        {
            if (_enemy == null)
            {
                Fail("no enemy to release (see QA-ENT-01)");
                yield break;
            }
            GameObject first = _enemy;
            pool.Release(_enemy);
            yield return Frames(2);
            Check(!first.activeSelf, "released enemy inactive", "released enemy still active");
            yield return Spawn();
            Check(_enemy == first, "pool reused the same instance", "pool instantiated a new enemy instead of reusing the released one");
            if (_vital == null || _stats == null) yield break;
            Check(Near(Hp, MaxHp), $"HP {Hp:0.#}/{MaxHp:0.#} full after respawn", $"HP {Hp:0.#}/{MaxHp:0.#} after respawn");
        }

        private IEnumerator CaseAssetClean()
        {
            yield return null;
            if (_statsAsset == null)
            {
                Skip("enemy prefab has no EntityData.Stats asset to compare");
                yield break;
            }
            Dictionary<StatType, float> now = _statsAsset.FullStatsValue();
            List<string> changed = _assetSnapshot.Where(p => !now.TryGetValue(p.Key, out float v) || !Mathf.Approximately(v, p.Value))
                .Select(p => $"{p.Key} {p.Value:0.##}->{(now.TryGetValue(p.Key, out float v) ? v.ToString("0.##") : "missing")}").ToList();
            Check(changed.Count == 0, $"{_statsAsset.name} unchanged", $"{_statsAsset.name} changed in memory: {string.Join(", ", changed)}");
        }

        // 6094aa10: MovementBase.MoveSpeed = StatType.MoveSpeed, EntityFindTarget.MaxRange / MinRange =
        // StatType.MaxRangeAttack / MinRangeAttack, EntityFindTarget.Range = EntityData.rangeCheckFieldOfView.
        // Was: serialized prefab fields left at 0 (inbox NOTE-20261104-1 / NOTE-20261104-6).
        private IEnumerator CaseStatDriven()
        {
            yield return Frames(2);
            EntityMovement movement = _enemy.GetComponentInChildren<EntityMovement>(true);
            EntityFindTarget finder = _enemy.GetComponentInChildren<EntityFindTarget>(true);
            if (movement == null || finder == null)
            {
                Fail($"components missing: movement={movement != null} findTarget={finder != null}");
                yield break;
            }
            float statMove = _stats.GetStatValue(StatType.MoveSpeed);
            float statMax = _stats.GetStatValue(StatType.MaxRangeAttack);
            float statMin = _stats.GetStatValue(StatType.MinRangeAttack);
            Check(movement.MoveSpeed > 0f && Near(movement.MoveSpeed, statMove),
                $"MoveSpeed {movement.MoveSpeed:0.##} = stat", $"MoveSpeed {movement.MoveSpeed:0.##}, stat {statMove:0.##}");
            Check(finder.MaxRange > 0f && Near(finder.MaxRange, statMax),
                $"MaxRange {finder.MaxRange:0.##} = stat", $"MaxRange {finder.MaxRange:0.##}, stat {statMax:0.##}");
            Check(Near(finder.MinRange, statMin) && finder.MinRange < finder.MaxRange,
                $"MinRange {finder.MinRange:0.##} below MaxRange", $"MinRange {finder.MinRange:0.##} (stat {statMin:0.##}), MaxRange {finder.MaxRange:0.##}");
            Check(finder.Range >= finder.MaxRange,
                $"detection range {finder.Range:0.##} >= MaxRange", $"detection range {finder.Range:0.##} below MaxRange {finder.MaxRange:0.##} (sees the player only inside attack range)");
        }

        private void OnEnemyDeath(object payload)
        {
            if (payload is GameObject go) _deaths.Add(go);
            else _deaths.Add(null);
        }
    }
}
#endif
