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
        // QA-ENT-08: the player is the target, every role EntityData is applied to the spawned enemy in turn.
        [SerializeField] private PlayerManager playerManager;
        [SerializeField] private PlayerData playerClass;
        [SerializeField] private EntityData[] roles;

        private readonly QAPlayerRig _rig = new();
        private const float AttackWindow = 8f;

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

            // Last: these move the player next to the enemy and let the AI hit it.
            if (roles == null || roles.Length == 0)
            {
                yield return Case("QA-ENT-08", "ENT", QAPriority.P0, "Enemy attacks the player in range", CaseNoRoles());
                yield break;
            }
            foreach (EntityData role in roles)
            {
                if (role == null) continue;
                string roleName = role.name.Replace("EntityData", "");
                yield return Case($"QA-ENT-08.{roleName}", "ENT", QAPriority.P0,
                    $"{roleName}: enemy attacks the player in range, player loses HP, attacks respect attackRate", CaseAttackPlayer(role));
            }
        }

        private IEnumerator CaseNoRoles()
        {
            yield return null;
            Fail("scene not wired: no role EntityData (Assets/SO/Enemy/Role)");
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

        private IEnumerator Spawn(EntityData data = null)
        {
            _enemy = pool.Spawn(enemyPrefab, spawnPoint.position, Quaternion.identity);
            // Same as EnemySpawner.ApplyData() since 6094aa10: the spawner applies the EntityData after the pool
            // re-enabled the enemy (EnemySpawnEntry.Data, or the prefab's own), and that is what equips the
            // default weapon and abilities.
            if (_enemy != null && _enemy.TryGetComponent(out Entity spawned) && (data != null || spawned.Data != null))
                spawned.SetCharacterData(data != null ? data : spawned.Data);
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
            Check(!_enemy.activeSelf, "released to the pool on the first emit (spawn-system contract)", "enemy still active after the spawn-system release");
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

        // QA-ENT-08: the real AI path — EntityBasicState sees the player in range, EntityAttackState plays the stage,
        // the clip's AnimationOnAction event calls MakeDamage(), the weapon hits the player hurtbox.
        // The enemy is pinned (speed 0) and the player is held in the middle of the role's attack range.
        private IEnumerator CaseAttackPlayer(EntityData role)
        {
            if (enemyPrefab == null || pool == null || playerManager == null || playerClass == null)
            {
                Fail("scene not wired: enemy prefab, pool, PlayerManager or player class missing");
                yield break;
            }
            if (!_rig.Ready)
            {
                bool spawned = false;
                yield return WaitUntil(() => playerManager.Player != null, 3f, ok => spawned = ok);
                if (!spawned)
                {
                    Fail("PlayerManager spawned no player within 3s");
                    yield break;
                }
                _rig.Bind(playerManager.Player);
                if (!_rig.Ready)
                {
                    Fail("player components missing: " + _rig.MissingComponents());
                    yield break;
                }
                _rig.Player.SetCharacterData(playerClass);
                yield return Frames(3);
            }
            if (_rig.Player.StateMachine.CurrentState is PlayerDeathState)
            {
                Fail("player already dead (killed during an earlier role) — not judged");
                yield break;
            }
            _rig.Vital.ReloadComponent();

            if (_enemy != null && _enemy.activeSelf) pool.Release(_enemy);
            yield return Frames(1);
            yield return Spawn(role);
            Entity entity = _enemy != null ? _enemy.GetComponent<Entity>() : null;
            EntityFindTarget finder = _enemy != null ? _enemy.GetComponentInChildren<EntityFindTarget>(true) : null;
            if (entity == null || _weapon == null || _stats == null || finder == null)
            {
                Fail($"enemy not spawned or components missing: entity={entity != null} weapon={_weapon != null} stats={_stats != null} findTarget={finder != null}");
                yield break;
            }
            bool armed = false;
            yield return WaitUntil(() => _weapon.Weapon != null, 1f, ok => armed = ok);
            if (!armed)
            {
                Fail($"{role.name}: no weapon equipped 1s after spawn (no weapon, no attack)");
                yield break;
            }

            float min = _stats.GetStatValue(StatType.MinRangeAttack);
            float max = _stats.GetStatValue(StatType.MaxRangeAttack);
            if (max <= 0f)
            {
                Fail($"MaxRangeAttack {max:0.##}: the enemy can never be in range");
                yield break;
            }
            float distance = Mathf.Clamp((min + max) * 0.5f, min + 0.05f, Mathf.Max(min + 0.05f, max - 0.05f));
            Vector2 holdAt = (Vector2)_enemy.transform.position + Vector2.left * distance;
            Rigidbody2D body = _rig.Player.RigidbodyPlayer;

            WeaponStats weaponStats = _weapon.Weapon.Stats;
            float minRate = weaponStats != null && weaponStats.StageCount > 0 ? weaponStats.AttackStages.Where(s => s != null).Select(s => s.attackRate).DefaultIfEmpty(0f).Min() : 0f;
            Note($"{role.name}: weapon {_weapon.Weapon.name} ({weaponStats?.Type}), range {min:0.##}-{max:0.##}, player held at {distance:0.##}, min attackRate {minRate:0.##}s");

            List<float> attacks = new();
            List<float> hits = new();
            float lastHp = _rig.Hp;
            float lost = 0f;
            bool wasAttacking = false;
            bool usedAbility = false;
            string attackClip = null;
            HashSet<int> projectiles = new();
            float closestProjectile = float.MaxValue;
            Vector2 closestAt = Vector2.zero;
            float end = Time.time + AttackWindow;
            while (Time.time < end && !(attacks.Count >= 2 && hits.Count >= 1))
            {
                if (body != null)
                {
                    body.position = holdAt;
                    body.velocity = Vector2.zero;
                }
                _rig.Player.transform.position = holdAt;
                yield return null;

                object state = entity.StateMachine.CurrentState;
                bool attacking = state is EntityAttackState;
                if (attacking && !wasAttacking) attacks.Add(Time.time);
                if (state is EntityAbilityState) usedAbility = true;
                if (attacking && attackClip == null && entity.Anim != null && entity.Anim.runtimeAnimatorController != null)
                {
                    // The stage's override controller is applied on Enter; the Attack clip shows a frame or two later.
                    AnimatorClipInfo[] clips = entity.Anim.GetCurrentAnimatorClipInfo(0);
                    if (clips.Length > 0 && clips[0].clip != null && clips[0].clip.name.Contains("Attack"))
                        attackClip = $"{clips[0].clip.name} [events: {string.Join(", ", clips[0].clip.events.Select(e => e.functionName))}]";
                }
                wasAttacking = attacking;

                foreach (ProjectileBody projectile in FindObjectsOfType<ProjectileBody>())
                {
                    projectiles.Add(projectile.GetInstanceID());
                    float d = Vector2.Distance(projectile.transform.position, holdAt);
                    if (d < closestProjectile)
                    {
                        closestProjectile = d;
                        closestAt = projectile.transform.position;
                    }
                }

                float hp = _rig.Hp;
                if (hp < lastHp - 0.001f)
                {
                    hits.Add(Time.time);
                    lost += lastHp - hp;
                }
                lastHp = hp;
                if (_rig.Player.StateMachine.CurrentState is PlayerDeathState) break;
            }

            if (usedAbility) Note("enemy also entered EntityAbilityState during the window");
            Check(attacks.Count >= 1,
                $"entered EntityAttackState {attacks.Count}x in {AttackWindow:0}s",
                $"never entered EntityAttackState in {AttackWindow:0}s: state {entity.StateMachine.CurrentState?.GetType().Name}; " +
                $"target locked {finder.HasTarget}; distance {finder.DistanceToPlayer():0.##} (range {min:0.##}-{max:0.##}); " +
                $"in range {finder.IsInRangeAttack()}; CallAttack {_weapon.CallAttack()}");
            if (attackClip != null) Note("attack clip " + attackClip + " (EntityAttackState deals damage on AnimationOnAction)");
            if (weaponStats != null && weaponStats.Type == WeaponType.RangeWP)
                Note(projectiles.Count == 0
                    ? "no ProjectileBody seen during the window (the weapon never fired)"
                    : $"{projectiles.Count} projectile(s) seen; closest {closestProjectile:0.##} from the player at {closestAt} (player at {holdAt}, enemy at {(Vector2)_enemy.transform.position})");

            Check(hits.Count >= 1,
                $"player lost {lost:0.##} HP in {hits.Count} hit(s)",
                attacks.Count == 0 ? "player HP unchanged (the enemy never attacked)" : $"player HP unchanged after {attacks.Count} attack(s): {MaskDiagnosis()}");

            if (attacks.Count >= 2)
            {
                float interval = attacks[1] - attacks[0];
                Check(interval + 0.02f >= minRate,
                    $"attacks {interval:0.##}s apart (>= attackRate {minRate:0.##}s)",
                    $"attacks only {interval:0.##}s apart, attackRate {minRate:0.##}s (no recovery between attacks)");
            }
            else if (attacks.Count == 1) Note($"only one attack in {AttackWindow:0}s: cadence not judged");

            if (_rig.Player.StateMachine.CurrentState is PlayerDeathState) Note("the player died during this role; later roles are not judged");
            else _rig.Vital.ReloadComponent();
        }

        private string MaskDiagnosis()
        {
            Component hurtbox = _rig.Hurtbox as Component;
            if (hurtbox == null) return "player has no hurtbox";
            int layer = hurtbox.gameObject.layer;
            WeaponStats stats = _weapon.Weapon != null ? _weapon.Weapon.Stats : null;
            if (stats == null) return "weapon has no WeaponStats";
            List<string> parts = new() { $"player hurtbox layer {LayerMask.LayerToName(layer)} ({layer})" };
            if (stats.Type == WeaponType.MeleeWP)
                parts.Add($"melee LayerMask {stats.LayerMask.value} hits it: {(stats.LayerMask.value & (1 << layer)) != 0}");
            foreach (AttackSO stage in stats.AttackStages)
            {
                if (stage is not RangeAttackSO ranged) continue;
                int mask = ranged.ProjectileConfig.targetMask.value;
                parts.Add($"{stage.name} targetMask {mask} hits it: {(mask & (1 << layer)) != 0}, " +
                          $"projectile collider: {(ranged.ProjectilePrefab != null && ranged.ProjectilePrefab.GetComponentInChildren<Collider2D>(true) != null)}");
                // ProjectileBody releases on the FIRST targetMask collider it enters; RangeWeapon.OnHit only damages
                // when that collider itself carries an INegativeReceiver.
                IEnumerable<string> targets = _rig.Player.GetComponentsInChildren<Collider2D>(true)
                    .Where(c => c.enabled && (mask & (1 << c.gameObject.layer)) != 0)
                    .Select(c => $"{c.gameObject.name} {c.GetType().Name} trigger={c.isTrigger} extents={c.bounds.extents.x:0.##}x{c.bounds.extents.y:0.##} receiver={c.TryGetComponent(out INegativeReceiver _)}");
                parts.Add("player colliders in targetMask: " + string.Join(" | ", targets));
            }
            return string.Join("; ", parts);
        }

        // Mirrors EnemySpawner.ReleaseEnemy(): by design (owner, 2026-11-05) the spawn system releases a dead enemy
        // on the first ON_ENEMY_DEATH, and that release is what stops the looping death clip from emitting again.
        // Any spawner, custom or test, must do the same.
        private void OnEnemyDeath(object payload)
        {
            if (payload is GameObject go)
            {
                _deaths.Add(go);
                if (pool != null) pool.Release(go);
            }
            else _deaths.Add(null);
        }
    }
}
#endif
