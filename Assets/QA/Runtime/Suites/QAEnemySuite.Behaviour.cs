#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;

namespace QA.Automation
{
    // Behaviour half of the enemy suite. The enemy is spawned free to move on the QAFloorPathGrid arena and the
    // player is parked out of its detection range, so each state is entered the way the AI enters it:
    // Idle, Move (wander and chase), TakeDamage, Death, the effect receiver, then the item drop on death and
    // the player's pickup (Q).
    public partial class QAEnemySuite
    {
        [SerializeField] private ItemSpawner itemSpawner;
        [SerializeField] private DepotItem depotItem;

        // Distance the player is parked at, to the left of the enemy spawn point (inside the floor arena).
        private const float ParkDistance = 24f;

        private Entity _entity;
        private EntityFindTarget _finder;
        private EntityInput _entityInput;
        private EntityMovement _movement;

        private QAInputDriver _input;
        private readonly List<GameObject> _collected = new();
        private int? _dropSeed;
        private GameObject _droppedItem;
        private ItemSO _droppedData;

        private Vector2 EnemyPosition => _enemy.transform.position;
        private Vector2 ParkPosition => (Vector2)spawnPoint.position + Vector2.left * ParkDistance;
        private string EnemyState => _entity != null ? _entity.StateMachine.CurrentState?.GetType().Name ?? "none" : "no Entity";
        private string PlayerState => _rig.Player.StateMachine.CurrentState?.GetType().Name ?? "none";
        private bool PlayerInBasicState => _rig.Player.StateMachine.CurrentState is PlayerIdleState or PlayerMoveState;

        private void OnCollectItem(object payload) => _collected.Add(payload as GameObject);

        private void DisposeInput()
        {
            _input?.Dispose();
            _input = null;
        }

        // The scene's ItemSpawner is not registered in the test scope, so the suite injects the pool itself.
        private void SetItemSpawner(bool on)
        {
            if (itemSpawner == null || itemSpawner.gameObject.activeSelf == on) return;
            if (on) itemSpawner.Construct(pool);
            itemSpawner.gameObject.SetActive(on);
        }

        #region Shared steps

        private IEnumerator EnsurePlayer(System.Action<bool> ready)
        {
            if (!_rig.Ready)
            {
                bool spawned = false;
                yield return WaitUntil(() => playerManager.Player != null, 3f, ok => spawned = ok);
                if (!spawned)
                {
                    Fail("PlayerManager spawned no player within 3s");
                    ready(false);
                    yield break;
                }
                _rig.Bind(playerManager.Player);
                if (!_rig.Ready)
                {
                    Fail("player components missing: " + _rig.MissingComponents());
                    ready(false);
                    yield break;
                }
                _rig.Player.SetCharacterData(playerClass);
                yield return Frames(3);
            }
            if (_rig.Player.StateMachine.CurrentState is PlayerDeathState)
            {
                Fail("player already dead (killed during an earlier case) — not judged");
                ready(false);
                yield break;
            }
            ready(true);
        }

        private void PlacePlayer(Vector2 position)
        {
            Rigidbody2D body = _rig.Player.RigidbodyPlayer;
            if (body != null)
            {
                body.position = position;
                body.velocity = Vector2.zero;
            }
            _rig.Player.transform.position = position;
            Physics2D.SyncTransforms();
        }

        // A fresh enemy that is free to move, with the player parked out of its detection range.
        private IEnumerator SpawnFree(System.Action<bool> ready)
        {
            if (enemyPrefab == null || pool == null || playerManager == null || playerClass == null || spawnPoint == null)
            {
                Fail("scene not wired: enemy prefab, pool, spawn point, PlayerManager or player class missing");
                ready(false);
                yield break;
            }
            bool playerReady = false;
            yield return EnsurePlayer(ok => playerReady = ok);
            if (!playerReady)
            {
                ready(false);
                yield break;
            }
            _rig.Vital.ReloadComponent();
            PlacePlayer(ParkPosition);

            if (_enemy != null && _enemy.activeSelf) pool.Release(_enemy);
            yield return Frames(1);
            yield return Spawn(null, false);
            _entity = _enemy != null ? _enemy.GetComponent<Entity>() : null;
            _finder = _enemy != null ? _enemy.GetComponentInChildren<EntityFindTarget>(true) : null;
            _entityInput = _enemy != null ? _enemy.GetComponentInChildren<EntityInput>(true) : null;
            _movement = _enemy != null ? _enemy.GetComponentInChildren<EntityMovement>(true) : null;
            if (_entity == null || _finder == null || _entityInput == null || _movement == null || _vital == null || _stats == null || _hurtbox == null)
            {
                Fail($"enemy not spawned or components missing: entity={_entity != null} findTarget={_finder != null} input={_entityInput != null} " +
                     $"movement={_movement != null} vital={_vital != null} stats={_stats != null} hurtbox={_hurtbox != null}");
                ready(false);
                yield break;
            }
            // The lock is released at 1.5 x the detection range (EntityFindTarget.loseTargetRangeMultiplier).
            if (_finder.Range * 1.5f >= ParkDistance)
            {
                Skip($"detection range {_finder.Range:0.##} is too large for the QA arena (the player is parked {ParkDistance:0} away)");
                ready(false);
                yield break;
            }
            ready(true);
        }

        // Raw damage that lands as finalDamage after the enemy's percent mitigation (x 100 / (100 + Defense)).
        private float RawFor(float finalDamage) =>
            finalDamage * (100f + Mathf.Max(0f, _stats.GetStatValue(StatType.Defense))) / 100f;

        private static string ClipInfo(Animator anim)
        {
            if (anim == null || !anim.isActiveAndEnabled || anim.runtimeAnimatorController == null) return "no active animator";
            AnimatorClipInfo[] clips = anim.GetCurrentAnimatorClipInfo(0);
            if (clips.Length == 0 || clips[0].clip == null) return $"controller {anim.runtimeAnimatorController.name}, no clip playing";
            return $"clip {clips[0].clip.name} [events: {string.Join(", ", clips[0].clip.events.Select(e => e.functionName))}]";
        }

        private bool AnimatorBool(string parameter) =>
            _entity.Anim != null && _entity.Anim.isActiveAndEnabled && _entity.Anim.GetBool(parameter);

        #endregion

        #region Cases — states

        private IEnumerator CaseIdle()
        {
            bool ready = false;
            yield return SpawnFree(ok => ready = ok);
            if (!ready) yield break;

            float idleDuration = _entity.Data.IdleDurationTime(_stats);
            Check(_entity.StateMachine.CurrentState is EntityIdleState, "starts in EntityIdleState", $"state after spawn is {EnemyState}");
            Check(!_finder.HasTarget, "no target locked with the player out of sight",
                $"locked the player {Vector2.Distance(EnemyPosition, ParkPosition):0.##} away (detection range {_finder.Range:0.##})");
            if (idleDuration < 0.3f)
            {
                Note($"IdleDurationTime {idleDuration:0.##}s is too short to watch the enemy stand still; only the start state is judged");
                yield break;
            }
            Check(AnimatorBool("Idle"), "animator Idle = true", "animator bool Idle is false while in EntityIdleState");

            Vector2 at = EnemyPosition;
            float watch = Mathf.Min(idleDuration * 0.5f, 1f);
            float drift = 0f;
            bool leftEarly = false;
            float end = Time.time + watch;
            while (Time.time < end)
            {
                yield return null;
                drift = Mathf.Max(drift, Vector2.Distance(at, EnemyPosition));
                if (_entity.StateMachine.CurrentState is not EntityIdleState) leftEarly = true;
            }
            Check(!leftEarly, $"stayed in Idle for {watch:0.##}s", $"left Idle within {watch:0.##}s (IdleDurationTime {idleDuration:0.##}s): now {EnemyState}");
            Check(drift < 0.05f, "stands still while idle", $"moved {drift:0.##} while idle");

            if (idleDuration > 20f)
            {
                Note($"IdleDurationTime {idleDuration:0.##}s is longer than the case allows; the Idle -> Move transition is not judged");
                yield break;
            }
            bool left = false;
            yield return WaitUntil(() => _entity.StateMachine.CurrentState is EntityMoveState, idleDuration + 1f, ok => left = ok);
            Check(left, $"left Idle for EntityMoveState after IdleDurationTime {idleDuration:0.##}s",
                $"still {EnemyState} {idleDuration + 1f + watch:0.##}s after spawn (IdleDurationTime {idleDuration:0.##}s)");
        }

        private IEnumerator CaseWander()
        {
            bool ready = false;
            yield return SpawnFree(ok => ready = ok);
            if (!ready) yield break;

            float idleDuration = _entity.Data.IdleDurationTime(_stats);
            if (idleDuration > 20f)
            {
                Skip($"IdleDurationTime {idleDuration:0.##}s: the enemy does not start to wander within the case time");
                yield break;
            }
            bool moving = false;
            yield return WaitUntil(() => _entity.StateMachine.CurrentState is EntityMoveState, idleDuration + 1.5f, ok => moving = ok);
            if (!moving)
            {
                Fail($"never entered EntityMoveState: still {EnemyState} {idleDuration + 1.5f:0.##}s after spawn");
                yield break;
            }
            float speed = _movement.MoveSpeed;
            Check(speed > 0f, $"MoveSpeed {speed:0.##}", "MoveSpeed is 0: the enemy cannot move");

            Vector2 start = EnemyPosition;
            Vector2 last = start;
            Vector2 anchor = _entityInput.SpawnPoint;
            float travelled = 0f, movingTime = 0f, displaced = 0f, farthest = 0f;
            bool animMove = false, locked = false;
            float end = Time.time + 6f;
            while (Time.time < end && !(displaced >= 0.5f && movingTime >= 0.5f))
            {
                yield return null;
                Vector2 now = EnemyPosition;
                travelled += Vector2.Distance(last, now);
                last = now;
                displaced = Mathf.Max(displaced, Vector2.Distance(start, now));
                farthest = Mathf.Max(farthest, Vector2.Distance(anchor, now));
                if (_entity.StateMachine.CurrentState is EntityMoveState)
                {
                    movingTime += Time.deltaTime;
                    animMove |= AnimatorBool("Move");
                }
                locked |= _finder.HasTarget;
            }
            Check(displaced >= 0.5f, $"wandered {displaced:0.##} from where it stood",
                $"moved only {displaced:0.##} in 6s of wandering: state {EnemyState}, MoveSpeed {speed:0.##}, CanMove {_movement.CanMove}, " +
                $"grid node under it {(EnemyManager.Instance.GetNodeByPositionWorld(EnemyPosition) != null)}");
            Check(animMove, "animator Move = true while moving", "animator bool Move was never true in EntityMoveState");
            if (movingTime >= 0.5f)
            {
                float average = travelled / movingTime;
                Check(average <= speed * 1.1f + 0.1f, $"average speed {average:0.##} <= MoveSpeed {speed:0.##}", $"average speed {average:0.##} is above MoveSpeed {speed:0.##}");
            }
            Check(!locked, "no target locked while wandering", "locked the player although it is parked out of sight");
            Note($"farthest point from its spawn point: {farthest:0.##}");
        }

        private IEnumerator CaseTakeDamageState()
        {
            bool ready = false;
            yield return SpawnFree(ok => ready = ok);
            if (!ready) yield break;

            float hit = MaxHp * 0.1f;
            float before = Hp;
            _hurtbox.TakeDamage(RawFor(hit), EnemyPosition + Vector2.right);
            bool entered = false;
            yield return WaitUntil(() => _entity.StateMachine.CurrentState is EntityTakeDamageState, 0.25f, ok => entered = ok);
            Check(entered, "entered EntityTakeDamageState", $"state {EnemyState} 0.25s after a non-lethal hit (IsTakeDamage {_entityInput.IsTakeDamage})");
            Check(Near(before - Hp, hit, 0.05f), $"lost {before - Hp:0.##} HP", $"lost {before - Hp:0.##} HP, expected {hit:0.##}");
            Check(Hp > 0f && !_entityInput.IsDeath, "still alive", $"HP {Hp:0.##}, IsDeath {_entityInput.IsDeath} after a hit of 10% HP");
            if (!entered) yield break;
            Check(AnimatorBool("TakeDamage"), "animator TakeDamage = true", "animator bool TakeDamage is false in EntityTakeDamageState");

            Vector2 at = EnemyPosition;
            float drift = 0f;
            bool left = false;
            float started = Time.time;
            string clip = ClipInfo(_entity.Anim);
            while (Time.time < started + 3f)
            {
                yield return null;
                if (_entity.StateMachine.CurrentState is not EntityTakeDamageState)
                {
                    left = true;
                    break;
                }
                drift = Mathf.Max(drift, Vector2.Distance(at, EnemyPosition));
                clip = ClipInfo(_entity.Anim);
            }
            Check(left, $"left the take-damage state after {Time.time - started:0.##}s",
                $"still in EntityTakeDamageState after 3s: {clip} (the state leaves on AnimationFinishTrigger)");
            Check(drift < 0.05f, "does not move while taking damage", $"moved {drift:0.##} in the take-damage state");
            if (left) Check(_entity.StateMachine.CurrentState is EntityMoveState, "returns to EntityMoveState", $"went to {EnemyState} instead of EntityMoveState");
        }

        private IEnumerator CaseDeathState()
        {
            bool ready = false;
            yield return SpawnFree(ok => ready = ok);
            if (!ready) yield break;

            // From Idle.
            Vector2 at = EnemyPosition;
            _deaths.Clear();
            _hurtbox.TakeDamage(RawFor(MaxHp * 2f), at + Vector2.left);
            bool entered = false;
            yield return WaitUntil(() => _entity.StateMachine.CurrentState is EntityDeathState, 0.25f, ok => entered = ok);
            Check(entered, "lethal hit: entered EntityDeathState", $"state {EnemyState} 0.25s after a lethal hit (IsDeath {_entityInput.IsDeath}, HP {Hp:0.##})");
            Check(Near(Hp, 0f), "HP 0", $"HP {Hp:0.##} after a lethal hit");
            if (_enemy.activeSelf) Check(AnimatorBool("Death"), "animator Death = true", "animator bool Death is false in EntityDeathState");

            _hurtbox.TakeDamage(RawFor(MaxHp), at + Vector2.left);
            float drift = 0f;
            float started = Time.time;
            string clip = ClipInfo(_entity.Anim);
            while (_deaths.Count == 0 && Time.time < started + 5f)
            {
                yield return null;
                drift = Mathf.Max(drift, Vector2.Distance(at, EnemyPosition));
                if (_enemy.activeSelf) clip = ClipInfo(_entity.Anim);
            }
            Check(_entity.StateMachine.CurrentState is EntityDeathState && !_entityInput.IsTakeDamage,
                "a hit on a dead enemy is ignored", $"a hit on a dead enemy changed it: state {EnemyState}, IsTakeDamage {_entityInput.IsTakeDamage}");
            Check(drift < 0.05f, "does not move while dying", $"moved {drift:0.##} in the death state");
            Check(_deaths.Count > 0, $"ON_ENEMY_DEATH followed after {Time.time - started:0.##}s",
                $"no ON_ENEMY_DEATH within 5s of the death state: {clip} (EntityDeathState emits on AnimationOnAction)");

            // From the take-damage state: a lethal hit landing during the hit reaction used to leave the enemy
            // alive at 0 HP (inbox NOTE-20261101-2).
            ready = false;
            yield return SpawnFree(ok => ready = ok);
            if (!ready) yield break;
            _hurtbox.TakeDamage(RawFor(MaxHp * 0.1f), EnemyPosition + Vector2.left);
            bool reacting = false;
            yield return WaitUntil(() => _entity.StateMachine.CurrentState is EntityTakeDamageState, 0.25f, ok => reacting = ok);
            if (!reacting)
            {
                Note($"the enemy did not enter the take-damage state ({EnemyState}); see QA-ENT-11 — death from TakeDamage not judged");
                yield break;
            }
            _hurtbox.TakeDamage(RawFor(MaxHp * 2f), EnemyPosition + Vector2.left);
            bool died = false;
            yield return WaitUntil(() => _entity.StateMachine.CurrentState is EntityDeathState, 0.25f, ok => died = ok);
            Check(died, "lethal hit during the take-damage state: entered EntityDeathState",
                $"lethal hit during the take-damage state left the enemy in {EnemyState} (HP {Hp:0.##}, IsDeath {_entityInput.IsDeath})");
        }

        // The real AI path up to the attack: the player walks into the view cone, the enemy locks it, the player
        // steps back out of attack range, the enemy closes the gap on the floor grid and attacks.
        private IEnumerator CaseChase()
        {
            bool ready = false;
            yield return SpawnFree(ok => ready = ok);
            if (!ready) yield break;

            float min = _finder.MinRange, max = _finder.MaxRange, range = _finder.Range, speed = _movement.MoveSpeed;
            if (max <= 0f || range <= max || speed <= 0f)
            {
                Fail($"cannot chase: MaxRangeAttack {max:0.##}, detection range {range:0.##}, MoveSpeed {speed:0.##}");
                yield break;
            }

            float seeAt = Mathf.Lerp(max, range, 0.8f);
            float end = Time.time + 3f;
            while (Time.time < end && !_finder.HasTarget)
            {
                Vector2 look = _entityInput.DirectionLookVector;
                PlacePlayer(EnemyPosition + (look.sqrMagnitude > 0.0001f ? look.normalized : Vector2.left) * seeAt);
                yield return null;
            }
            Check(_finder.HasTarget, $"locked a player standing {seeAt:0.##} in front of it (detection range {range:0.##}, view cone {_finder.FieldOfView:0}°)",
                $"did not lock a player held {seeAt:0.##} in front of it for 3s: state {EnemyState}, detection range {range:0.##}, " +
                $"view cone {_finder.FieldOfView:0}°, CanDetectTarget {_finder.CanDetectTarget()}");
            if (!_finder.HasTarget) yield break;

            // Out of attack range, inside the lose-target range (1.5 x detection range).
            float gap = Mathf.Min(Mathf.Max(max + 1.5f, range * 1.2f), range * 1.4f);
            Vector2 away = (Vector2)_rig.Player.transform.position - EnemyPosition;
            Vector2 standAt = EnemyPosition + (away.sqrMagnitude > 0.0001f ? away.normalized : Vector2.left) * gap;
            if (gap <= max || EnemyManager.Instance.GetNodeByPositionWorld(standAt) == null)
            {
                Skip($"no room to step back: gap {gap:0.##}, attack range {max:0.##}, point {standAt} inside the arena: {(EnemyManager.Instance.GetNodeByPositionWorld(standAt) != null)}");
                yield break;
            }
            float timeout = Mathf.Clamp((gap - max) / speed * 3f + 2f, 3f, 20f);
            bool chased = false, reached = false, lostLock = false;
            float closest = gap;
            float started = Time.time;
            while (Time.time < started + timeout)
            {
                PlacePlayer(standAt);
                yield return null;
                float distance = Vector2.Distance(EnemyPosition, standAt);
                closest = Mathf.Min(closest, distance);
                if (_entity.StateMachine.CurrentState is EntityMoveState) chased = true;
                if (!_finder.HasTarget) lostLock = true;
                if (distance <= max)
                {
                    reached = true;
                    break;
                }
            }
            Check(chased, "entered EntityMoveState to chase", $"never entered EntityMoveState with the locked player {gap:0.##} away: state {EnemyState}");
            Check(reached, $"closed from {gap:0.##} to attack range {max:0.##} in {Time.time - started:0.##}s",
                $"closest {closest:0.##} after {timeout:0.#}s (attack range {max:0.##}, MoveSpeed {speed:0.##}): state {EnemyState}, target locked {_finder.HasTarget}");
            Check(!lostLock, "kept the target lock while chasing", $"lost the lock on a player {gap:0.##} away (lose range {range * 1.5f:0.##})");
            if (!reached) yield break;

            bool attacked = false;
            yield return WaitUntil(() => _entity.StateMachine.CurrentState is EntityAttackState, 4f, ok => attacked = ok);
            float stopAt = Vector2.Distance(EnemyPosition, _rig.Player.transform.position);
            Check(attacked, $"attacked after the chase, standing {stopAt:0.##} from the player (attack band {min:0.##}-{max:0.##})",
                $"did not enter EntityAttackState within 4s of reaching attack range: state {EnemyState}, distance {stopAt:0.##} " +
                $"(attack band {min:0.##}-{max:0.##}), in range {_finder.IsInRangeAttack()}, CallAttack {(_weapon != null && _weapon.CallAttack())}");

            if (_enemy.activeSelf) pool.Release(_enemy);
            if (_rig.Player.StateMachine.CurrentState is PlayerDeathState) Note("the player died during the chase; later cases are not judged");
            else _rig.Vital.ReloadComponent();
        }

        #endregion

        #region Cases — effect receiver

        // StatModifierGroup's lists are authored-only (private); QA fills them the way the Inspector would.
        private static StatModifierGroup MakeGroup(StatModifier stat, VitalModifier vital)
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            FieldInfo statsField = typeof(StatModifierGroup).GetField("statsModifiers", flags);
            FieldInfo vitalField = typeof(StatModifierGroup).GetField("vitalModifiers", flags);
            if (statsField == null || vitalField == null) return null;
            StatModifierGroup group = new();
            if (stat != null) ((List<StatModifier>)statsField.GetValue(group)).Add(stat);
            if (vital != null) ((List<VitalModifier>)vitalField.GetValue(group)).Add(vital);
            return group;
        }

        // EffectReceiverBase reads only context.Caster.Transform (where the effect's visual source spawns).
        private sealed class QAEffectCaster : IAbilityOwner
        {
            public QAEffectCaster(Transform transform) => Transform = transform;
            public Transform Transform { get; }
            public float GetCurrentStatValue(VitalStatType statType) => 0f;
            public float GetCurrentStatValue(StatType statType) => 0f;
            public void PayCost(VitalStatType statType, float amount) { }
            public Vector2 DirectorForward() => Vector2.right;
            public int DirectorForwardIndex() => 0;
            public Vector2 TargetPosition() => Transform.position;
        }

        private IEnumerator FindEffectReceiver(System.Action<EntityEffectReceiver> found)
        {
            bool ready = false;
            yield return SpawnFree(ok => ready = ok);
            if (!ready) yield break;
            EntityEffectReceiver receiver = _enemy.GetComponentInChildren<EntityEffectReceiver>(true);
            if (receiver == null) Skip("the enemy prefab has no EntityEffectReceiver");
            found(receiver);
        }

        private IEnumerator CaseEffectMaxStats()
        {
            EntityEffectReceiver receiver = null;
            yield return FindEffectReceiver(r => receiver = r);
            if (receiver == null) yield break;

            const StatType stat = StatType.Defense;
            const float bonus = 5f;
            StatModifierGroup group = MakeGroup(new StatModifier(stat, bonus, ModifierType.Flat), null);
            if (group == null)
            {
                Fail("QA cannot fill a StatModifierGroup: field statsModifiers or vitalModifiers was renamed");
                yield break;
            }
            float before = _stats.GetStatValue(stat);
            bool clean = _stats.GetStat(stat).Modifiers.Count == 0;
            object source = new();
            receiver.AdjustMaxStats(group, null, source);
            float buffed = _stats.GetStatValue(stat);
            Check(clean ? Near(buffed, before + bonus) : buffed > before,
                $"AdjustMaxStats: {stat} {before:0.##} -> {buffed:0.##} (+{bonus:0.##} flat)", $"AdjustMaxStats: {stat} {before:0.##} -> {buffed:0.##}, expected +{bonus:0.##}");
            _stats.RemoveModifiersFromSource(source);
            Check(Near(_stats.GetStatValue(stat), before), "removing the source restores the stat", $"{stat} is {_stats.GetStatValue(stat):0.##} after removing the source, was {before:0.##}");

            const float duration = 0.5f;
            receiver.AdjustMaxForDuration(group, duration, new AbilityContext { Caster = new QAEffectCaster(_enemy.transform) });
            yield return Frames(2);
            buffed = _stats.GetStatValue(stat);
            Check(clean ? Near(buffed, before + bonus) : buffed > before,
                $"AdjustMaxForDuration: {stat} buffed to {buffed:0.##}", $"AdjustMaxForDuration: {stat} is {buffed:0.##} two frames after the buff, was {before:0.##}");
            yield return Wait(duration + 0.3f);
            Check(Near(_stats.GetStatValue(stat), before), $"the timed buff expired after {duration:0.##}s",
                $"{stat} is {_stats.GetStatValue(stat):0.##} {duration + 0.3f:0.##}s after a {duration:0.##}s buff, was {before:0.##}");
        }

        private IEnumerator CaseEffectCurrentStats()
        {
            EntityEffectReceiver receiver = null;
            yield return FindEffectReceiver(r => receiver = r);
            if (receiver == null) yield break;

            float chunk = Mathf.Max(1f, MaxHp * 0.2f);
            StatModifierGroup heal = MakeGroup(null, new VitalModifier(VitalStatType.HP, chunk, ModifierType.Flat));
            StatModifierGroup hurt = MakeGroup(null, new VitalModifier(VitalStatType.HP, -chunk, ModifierType.Flat));
            if (heal == null || hurt == null)
            {
                Fail("QA cannot fill a StatModifierGroup: field statsModifiers or vitalModifiers was renamed");
                yield break;
            }
            _vital.Reduction(VitalStatType.HP, chunk * 2f);
            float start = Hp;
            receiver.AdjustCurrentStats(heal, null);
            Check(Near(Hp, start + chunk, 0.05f), $"a +{chunk:0.##} HP vital modifier healed {Hp - start:0.##}",
                $"a +{chunk:0.##} HP vital modifier changed HP {start:0.##} -> {Hp:0.##}, expected {start + chunk:0.##}");
            start = Hp;
            receiver.AdjustCurrentStats(hurt, null);
            Check(Near(Hp, start - chunk, 0.05f), $"a -{chunk:0.##} HP vital modifier removed {start - Hp:0.##}",
                $"a -{chunk:0.##} HP vital modifier changed HP {start:0.##} -> {Hp:0.##}, expected {start - chunk:0.##} (a negative vital modifier does not damage)");
        }

        #endregion

        #region Cases — item drop and pickup

        // Kills the current enemy with the next drop roll fixed to the given seed and reports the items that appeared.
        private IEnumerator KillAndCollectDrops(int seed, List<ItemController> drops, System.Action<bool> died)
        {
            HashSet<ItemController> existing = new(FindObjectsOfType<ItemController>());
            _deaths.Clear();
            _dropSeed = seed;
            _hurtbox.TakeDamage(RawFor(MaxHp * 2f), EnemyPosition + Vector2.left);
            bool dead = false;
            yield return WaitUntil(() => _deaths.Count > 0, 5f, ok => dead = ok);
            _dropSeed = null;
            if (dead)
            {
                yield return Frames(2);
                drops.AddRange(FindObjectsOfType<ItemController>().Where(i => !existing.Contains(i)));
            }
            died(dead);
        }

        private IEnumerator CaseItemDrop()
        {
            if (itemSpawner == null || depotItem == null)
            {
                Fail("scene not wired: ItemSpawner or DepotItem missing");
                yield break;
            }
            bool ready = false;
            yield return SpawnFree(ok => ready = ok);
            if (!ready) yield break;

            // The roll is predicted on a copy of the drop table with the same seed the spawner will roll with.
            DepotItem table = Instantiate(depotItem);
            int hitSeed = -1, missSeed = -1;
            ItemSO expected = null;
            for (int seed = 1; seed <= 400 && (hitSeed < 0 || missSeed < 0); seed++)
            {
                Random.InitState(seed);
                if (table.TryRollItem(out ItemSO rolled))
                {
                    if (hitSeed < 0)
                    {
                        hitSeed = seed;
                        expected = rolled;
                    }
                }
                else if (missSeed < 0) missSeed = seed;
            }
            float dropRate = table.TotalDropRate;
            Destroy(table);
            Note($"{depotItem.name}: drop rate {dropRate:0.#}%");
            if (hitSeed < 0)
            {
                Skip($"{depotItem.name} never drops an item (400 rolls)");
                yield break;
            }

            SetItemSpawner(true);
            List<ItemController> drops = new();
            bool died = false;
            yield return KillAndCollectDrops(hitSeed, drops, ok => died = ok);
            if (!died)
            {
                Fail($"no ON_ENEMY_DEATH within 5s of lethal damage (state {EnemyState}); see QA-ENT-04");
                yield break;
            }
            Check(drops.Count == 1, $"one item dropped ({expected.name})", $"{drops.Count} items dropped for one death (the roll gives {expected.name})");
            if (drops.Count == 0) yield break;

            ItemController item = drops[0];
            float offset = Vector2.Distance(item.transform.position, EnemyPosition);
            Check(offset < 0.1f, "dropped at the death position", $"dropped {offset:0.##} away from the dead enemy");
            SpriteRenderer sprite = item.GetComponent<SpriteRenderer>();
            Check(item.name == expected.ItemName && sprite != null && sprite.sprite == expected.ItemSprite,
                $"carries the rolled item's name '{expected.ItemName}' and sprite",
                $"item shows name '{item.name}' sprite {(sprite != null && sprite.sprite != null ? sprite.sprite.name : "none")}, " +
                $"the roll gives name '{expected.ItemName}' sprite {(expected.ItemSprite != null ? expected.ItemSprite.name : "none")}");
            Collider2D pickupCollider = item.GetComponent<Collider2D>();
            ResourceReceiver pickup = _rig.Player.GetComponentInChildren<ResourceReceiver>(true);
            int layer = item.gameObject.layer;
            Check(pickupCollider != null && pickupCollider.enabled && pickup != null && (pickup.InteractableMask.value & (1 << layer)) != 0,
                $"the player can reach it: collider on layer {LayerMask.LayerToName(layer)}, inside the pickup mask",
                $"the player cannot pick it up: collider {(pickupCollider != null && pickupCollider.enabled)}, layer {LayerMask.LayerToName(layer)} ({layer}), " +
                $"pickup mask {(pickup != null ? pickup.InteractableMask.value.ToString() : "no ResourceReceiver")}");
            _droppedItem = item.gameObject;
            _droppedData = expected;

            if (missSeed < 0)
            {
                Note("the drop table always drops: a roll that drops nothing cannot be tested");
                yield break;
            }
            ready = false;
            yield return SpawnFree(ok => ready = ok);
            if (!ready) yield break;
            drops.Clear();
            yield return KillAndCollectDrops(missSeed, drops, ok => died = ok);
            if (died) Check(drops.Count == 0, "a roll in the no-drop band drops nothing", $"{drops.Count} items dropped on a roll in the no-drop band");
        }

        private IEnumerator CaseItemPickup()
        {
            if (_droppedItem == null || !_droppedItem.activeInHierarchy || _droppedData == null)
            {
                Fail("no dropped item to pick up (see QA-ITEM-02)");
                yield break;
            }
            bool playerReady = false;
            yield return EnsurePlayer(ok => playerReady = ok);
            if (!playerReady) yield break;
            ResourceReceiver pickup = _rig.Player.GetComponentInChildren<ResourceReceiver>(true);
            if (pickup == null || _rig.Input == null)
            {
                Fail($"player components missing: ResourceReceiver={pickup != null} input={_rig.Input != null}");
                yield break;
            }
            SetItemSpawner(true);
            if (_enemy != null && _enemy.activeSelf) pool.Release(_enemy);
            _rig.Vital.ReloadComponent();

            int unloadable = _droppedData.Effects.Count(e => e == null);
            Note($"item {_droppedData.name}: {_droppedData.Effects.Count} effect(s){(unloadable > 0 ? $", {unloadable} did not load (effect asset bound to an abstract script)" : "")}");

            _input ??= new QAInputDriver();
            _input.Bind(_rig.Input);
            PlacePlayer((Vector2)_droppedItem.transform.position + Vector2.left * 0.2f);
            bool idle = false;
            yield return WaitUntil(() => PlayerInBasicState, 2f, ok => idle = ok);
            if (!idle)
            {
                Fail($"player is in {PlayerState}, not Idle / Move, before the pickup");
                yield break;
            }

            _rig.Player.Core.TryGetCapability(out IWallet wallet);
            int coins = wallet != null ? wallet.GetCurrentCoin() : 0;
            float hp = _rig.Hp, mana = _rig.Mana;
            Dictionary<StatType, float> stats = _rig.Stats.GetFullStat();

            _collected.Clear();
            _input.KeyDown(Key.Q);
            bool entered = false;
            yield return WaitUntil(() => _rig.Player.StateMachine.CurrentState is PlayerResourceReceiverState, 1f, ok => entered = ok);
            Check(entered, "Q next to the item enters PlayerResourceReceiverState",
                $"player is in {PlayerState} 1s after Q: {pickup.NumFound} collider(s) found in radius {pickup.IntertionPointRadius:0.##}, " +
                $"IsResourceReceiver {_rig.Input.IsResourceReceiver}");
            bool collected = false;
            string clip = ClipInfo(_rig.Player.Anim);
            float started = Time.time;
            while (_collected.Count == 0 && Time.time < started + 3f)
            {
                yield return null;
                if (_rig.Player.StateMachine.CurrentState is PlayerResourceReceiverState) clip = ClipInfo(_rig.Player.Anim);
            }
            collected = _collected.Count > 0;
            _input.KeyUp();
            Check(collected, $"ON_COLLECT_ITEM emitted after {Time.time - started:0.##}s",
                $"no ON_COLLECT_ITEM within 3s of Q: player in {PlayerState}, {clip} (PlayerResourceReceiverState picks up on AnimationOnAction)");
            if (collected)
            {
                Check(_collected[0] == _droppedItem, "payload is the item", $"payload is {(_collected[0] != null ? _collected[0].name : "null")}");
                yield return Frames(2);
                Check(!_droppedItem.activeSelf, "the item is released to the pool", "the item is still in the scene after the pickup");
                Check(_collected.Count == 1, "collected once", $"ON_COLLECT_ITEM emitted {_collected.Count} times for one pickup");
            }
            bool back = false;
            yield return WaitUntil(() => PlayerInBasicState, 3f, ok => back = ok);
            Check(back, "player returns to Idle / Move", $"player is still in {PlayerState} 3s after the pickup: {ClipInfo(_rig.Player.Anim)}");

            // What the item gave is recorded, not judged: the item effects apply nothing today (BUG-120).
            Dictionary<StatType, float> now = _rig.Stats.GetFullStat();
            List<string> changed = stats.Where(p => now.TryGetValue(p.Key, out float v) && !Mathf.Approximately(v, p.Value))
                .Select(p => $"{p.Key} {p.Value:0.##}->{now[p.Key]:0.##}").ToList();
            Note($"effect of the pickup: coins {(wallet != null ? (wallet.GetCurrentCoin() - coins).ToString("+0;-0;0") : "no wallet")}, " +
                 $"HP {_rig.Hp - hp:+0.##;-0.##;0}, Mana {_rig.Mana - mana:+0.##;-0.##;0}, max stats {(changed.Count > 0 ? string.Join(", ", changed) : "unchanged")}");
            _droppedItem = null;
        }

        private IEnumerator CaseNoItemInReach()
        {
            bool playerReady = false;
            yield return EnsurePlayer(ok => playerReady = ok);
            if (!playerReady) yield break;
            if (_rig.Input == null)
            {
                Fail("player has no PlayerInputHandler");
                yield break;
            }
            SetItemSpawner(true);
            _input ??= new QAInputDriver();
            _input.Bind(_rig.Input);
            PlacePlayer(ParkPosition);
            bool idle = false;
            yield return WaitUntil(() => PlayerInBasicState, 2f, ok => idle = ok);
            if (!idle)
            {
                Fail($"player is in {PlayerState}, not Idle / Move");
                yield break;
            }
            _collected.Clear();
            _input.KeyDown(Key.Q);
            bool picking = false;
            yield return WatchFor(0.5f, () => _rig.Player.StateMachine.CurrentState is PlayerResourceReceiverState, seen => picking = seen);
            _input.KeyUp();
            Check(!picking, "no pickup state with nothing in reach", "entered PlayerResourceReceiverState with no item in reach");
            Check(_collected.Count == 0, "nothing collected", $"ON_COLLECT_ITEM emitted {_collected.Count} times with no item in reach");
            yield return Frames(2);
        }

        #endregion
    }
}
#endif
