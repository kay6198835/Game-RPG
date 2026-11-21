#if UNITY_EDITOR
using System.Collections;
using System.Linq;
using UnityEngine;

namespace QA.Automation
{
    // Current HP / Mana of the player: reload, reduction, recovery, damage through the hurtbox, death.
    // The class is applied with SetCharacterData directly (the monument flow is covered by QA-PLY-*).
    // Death cases run last: the player has no way back from PlayerDeathState in this scene.
    public class QAVitalsSuite : QASuite
    {
        [SerializeField] private PlayerManager playerManager;
        [SerializeField] private PlayerData playerClass;

        private readonly QAPlayerRig _rig = new();
        private int _deathEvents;
        private int _changedEvents;

        private void OnEnable() => EventManager.Resgister(EventID.ON_PLAYER_DEATH, OnPlayerDeath);
        private void OnDisable() => EventManager.UnResgister(EventID.ON_PLAYER_DEATH, OnPlayerDeath);

        public override IEnumerator Run()
        {
            bool spawned = false;
            yield return WaitUntil(() => playerManager.Player != null, 3f, ok => spawned = ok);
            if (spawned)
            {
                _rig.Bind(playerManager.Player);
                _rig.Player.SetCharacterData(playerClass);
                _rig.Vital.CurrentStatsChanged += _ => _changedEvents++;
                yield return Frames(3);
            }

            yield return Case("QA-VIT-01", "VIT", QAPriority.P0, "ReloadComponent fills HP and Mana to max", Guard(CaseReloadFull()));
            yield return Case("QA-VIT-02", "VIT", QAPriority.P0, "Reduction lowers the value by exactly the amount and raises CurrentStatsChanged", Guard(CaseReductionExact()));
            yield return Case("QA-VIT-03", "VIT", QAPriority.P1, "Reduction larger than the current value clamps at 0", Guard(CaseReductionClamp()));
            yield return Case("QA-VIT-04", "VIT", QAPriority.P1, "Recovery below max restores exactly the amount", Guard(CaseRecoveryExact()));
            yield return Case("QA-VIT-05", "VIT", QAPriority.P1, "Recovery never pushes the value above max", Guard(CaseRecoveryClamp()));
            yield return Case("QA-VIT-06", "VIT", QAPriority.P0, "Damage through the player hurtbox lowers HP, player survives", Guard(CaseHurtbox()));
            // Added 2026-11-20 (coverage review): change events, max-HP changes, non-HP vitals, boundary damage.
            yield return Case("QA-VIT-09", "VIT", QAPriority.P1, "Recovery raises CurrentStatsChanged once; a zero amount changes nothing", Guard(CaseRecoveryEvent()));
            yield return Case("QA-VIT-10", "VIT", QAPriority.P1, "Lowering max HP below current clamps current; raising it leaves current", Guard(CaseMaxHpChange()));
            yield return Case("QA-VIT-11", "VIT", QAPriority.P1, "Mana at 0 does not kill the player", Guard(CaseManaZeroAlive()));
            yield return Case("QA-VIT-12", "VIT", QAPriority.P1, "Recovery / Reduction on Coin (not seeded) are safe and leave HP / Mana alone", Guard(CaseUnseededVital()));
            yield return Case("QA-VIT-13", "VIT", QAPriority.P1, "Damage of current HP minus 0.5 leaves the player alive at 0.5 HP", Guard(CaseBoundaryDamage()));
            yield return Case("QA-VIT-14", "VIT", QAPriority.P1, "A non-lethal hit enters PlayerTakeDamageState and returns to Idle / Move", Guard(CaseTakeDamageState()));
            // Added 2026-11-21 (part C): the receiver contract weapons look up, and the dash hurtbox toggle.
            yield return Case("QA-VIT-15", "VIT", QAPriority.P0, "Player and enemy hurtboxes implement INegativeReceiver, the interface weapons, projectiles and abilities look up", Guard(CaseReceiverContract()));
            yield return Case("QA-VIT-16", "VIT", QAPriority.P1, "InvicisibleHurtbox(true) disables the player hurtbox collider, false enables it again", Guard(CaseHurtboxToggle()));
            yield return Case("QA-VIT-07", "VIT", QAPriority.P0, "Lethal damage puts the player in PlayerDeathState and emits ON_PLAYER_DEATH", Guard(CaseDeath()));
            yield return Case("QA-VIT-08", "VIT", QAPriority.P1, "ON_PLAYER_DEATH is emitted once per death, not every frame", Guard(CaseDeathOnce()));
        }

        private IEnumerator Guard(IEnumerator body)
        {
            if (!_rig.Ready)
            {
                Fail("no player spawned by PlayerManager within 3s, or a vital/stat component is missing: " + (_rig.Player != null ? _rig.MissingComponents() : "Player null"));
                yield break;
            }
            yield return body;
        }

        private IEnumerator CaseReloadFull()
        {
            _rig.Vital.Reduction(VitalStatType.HP, _rig.MaxHp * 0.5f);
            _rig.Vital.Reduction(VitalStatType.Mana, _rig.MaxMana * 0.5f);
            _rig.Vital.ReloadComponent();
            yield return null;
            Check(Near(_rig.Hp, _rig.MaxHp), $"HP {_rig.Hp:0.#} = max {_rig.MaxHp:0.#}", $"HP {_rig.Hp:0.#}, max {_rig.MaxHp:0.#}");
            Check(Near(_rig.Mana, _rig.MaxMana), $"Mana {_rig.Mana:0.#} = max {_rig.MaxMana:0.#}", $"Mana {_rig.Mana:0.#}, max {_rig.MaxMana:0.#}");
            Check(_rig.MaxHp > 0f && _rig.MaxMana >= 0f, "max HP is positive", $"max HP {_rig.MaxHp}");
        }

        private IEnumerator CaseReductionExact()
        {
            _rig.Vital.ReloadComponent();
            yield return null;
            float before = _rig.Hp;
            int events = _changedEvents;
            _rig.Vital.Reduction(VitalStatType.HP, 10f);
            Check(Near(_rig.Hp, before - 10f), $"HP {before:0.#} -> {_rig.Hp:0.#}", $"HP {before:0.#} -> {_rig.Hp:0.#}, expected {before - 10f:0.#}");
            Check(_changedEvents == events + 1, "CurrentStatsChanged raised once", $"CurrentStatsChanged raised {_changedEvents - events} times");
            _rig.Vital.ReloadComponent();
        }

        private IEnumerator CaseReductionClamp()
        {
            _rig.Vital.ReloadComponent();
            yield return null;
            _rig.Vital.Reduction(VitalStatType.Mana, _rig.MaxMana + 500f);
            Check(Near(_rig.Mana, 0f), "Mana clamped at 0", $"Mana {_rig.Mana:0.#} after a reduction larger than max");
            _rig.Vital.ReloadComponent();
        }

        private IEnumerator CaseRecoveryExact()
        {
            _rig.Vital.ReloadComponent();
            yield return null;
            _rig.Vital.Reduction(VitalStatType.HP, 30f);
            float before = _rig.Hp;
            _rig.Vital.Recovery(VitalStatType.HP, 10f);
            Check(Near(_rig.Hp, before + 10f), $"HP {before:0.#} -> {_rig.Hp:0.#}", $"HP {before:0.#} -> {_rig.Hp:0.#}, expected {before + 10f:0.#}");
            _rig.Vital.ReloadComponent();
        }

        private IEnumerator CaseRecoveryClamp()
        {
            _rig.Vital.ReloadComponent();
            yield return null;
            _rig.Vital.Reduction(VitalStatType.HP, 5f);
            _rig.Vital.Recovery(VitalStatType.HP, 10f);
            Check(_rig.Hp <= _rig.MaxHp + 0.01f, $"HP {_rig.Hp:0.#} <= max {_rig.MaxHp:0.#}", $"HP {_rig.Hp:0.#} above max {_rig.MaxHp:0.#} after max-5 +10");
            _rig.Vital.ReloadComponent();
        }

        private const string NoReceiver = "player has no component implementing INegativeReceiver: MeleeWeapon, RangeWeapon, Projectile and ability effects find no target on the player";

        private IEnumerator CaseReceiverContract()
        {
            yield return null;
            bool player = typeof(INegativeReceiver).IsAssignableFrom(typeof(NegativeReciver));
            bool enemy = typeof(INegativeReceiver).IsAssignableFrom(typeof(EntityNegativeReciver));
            Check(player, "NegativeReciver implements INegativeReceiver", "NegativeReciver (player hurtbox) does not implement INegativeReceiver: weapons and abilities cannot damage the player");
            Check(enemy, "EntityNegativeReciver implements INegativeReceiver", "EntityNegativeReciver (enemy hurtbox) does not implement INegativeReceiver: weapons and abilities cannot damage enemies");
            Check(_rig.Hurtbox != null, "spawned player has an INegativeReceiver", NoReceiver);
        }

        private IEnumerator CaseHurtboxToggle()
        {
            NegativeReciver receiver = _rig.Player.GetComponentInChildren<NegativeReciver>(true);
            if (receiver == null)
            {
                Skip("player has no NegativeReciver component");
                yield break;
            }
            BoxCollider2D box = receiver.GetComponent<BoxCollider2D>();
            if (box == null)
            {
                Fail("NegativeReciver has no BoxCollider2D (RequireComponent)");
                yield break;
            }
            yield return Frames(2);
            bool before = box.enabled;
            receiver.InvicisibleHurtbox(true);
            bool off = !box.enabled;
            receiver.InvicisibleHurtbox(false);
            bool on = box.enabled;
            box.enabled = before;
            Check(off, "collider disabled by InvicisibleHurtbox(true)", "collider still enabled after InvicisibleHurtbox(true)");
            Check(on, "collider enabled by InvicisibleHurtbox(false)", "collider still disabled after InvicisibleHurtbox(false)");
        }

        private IEnumerator CaseHurtbox()
        {
            if (_rig.Hurtbox == null)
            {
                Fail(NoReceiver);
                yield break;
            }
            _rig.Vital.ReloadComponent();
            yield return Frames(2);
            float before = _rig.Hp;
            float amount = Mathf.Max(5f, _rig.MaxHp * 0.1f);
            _rig.Hurtbox.TakeDamage(amount, (Vector2)_rig.Player.transform.position + Vector2.right);
            yield return Frames(2);
            float lost = before - _rig.Hp;
            Check(lost > 0f && lost <= amount + 0.01f, $"HP lost {lost:0.#} of {amount:0.#} sent", $"HP lost {lost:0.#} for {amount:0.#} damage sent");
            Check(_rig.Hp > 0f, "player alive", "player died from 10% damage");
            yield return Wait(1.5f);
            _rig.Vital.ReloadComponent();
        }

        private IEnumerator CaseRecoveryEvent()
        {
            _rig.Vital.ReloadComponent();
            yield return null;
            _rig.Vital.Reduction(VitalStatType.HP, 20f);
            int events = _changedEvents;
            float before = _rig.Hp;
            _rig.Vital.Recovery(VitalStatType.HP, 5f);
            Check(_changedEvents == events + 1, "Recovery raised CurrentStatsChanged once", $"Recovery raised CurrentStatsChanged {_changedEvents - events} times");
            Check(Near(_rig.Hp, before + 5f), $"HP {before:0.#} -> {_rig.Hp:0.#}", $"HP {before:0.#} -> {_rig.Hp:0.#}, expected +5");
            before = _rig.Hp;
            _rig.Vital.Reduction(VitalStatType.HP, 0f);
            _rig.Vital.Recovery(VitalStatType.HP, 0f);
            Check(Near(_rig.Hp, before), "zero-amount Reduction / Recovery leave HP unchanged", $"HP {before:0.##} -> {_rig.Hp:0.##} after zero amounts");
            _rig.Vital.ReloadComponent();
        }

        // VitalStatsBase.OnMaxStatChanged: a lowered max clamps the current value down; a raised max leaves it.
        private IEnumerator CaseMaxHpChange()
        {
            _rig.Vital.ReloadComponent();
            yield return null;
            float max = _rig.MaxHp;
            float cut = Mathf.Max(1f, max * 0.25f);
            object source = new();
            _rig.Stats.AddModifiersFromSource(source, new[] { new StatModifier(StatType.HP, -cut, ModifierType.Flat) });
            yield return null;
            float lowered = _rig.MaxHp;
            Check(Near(lowered, max - cut, 0.05f), $"max HP {max:0.#} -> {lowered:0.#}", $"max HP {max:0.#} -> {lowered:0.#}, expected {max - cut:0.#}");
            Check(_rig.Hp <= lowered + 0.01f, $"current HP clamped to the new max ({_rig.Hp:0.#})", $"current HP {_rig.Hp:0.#} above the lowered max {lowered:0.#}");
            float current = _rig.Hp;
            _rig.Stats.RemoveModifiersFromSource(source);
            yield return null;
            Check(Near(_rig.MaxHp, max, 0.05f), "max HP restored", $"max HP {_rig.MaxHp:0.#}, was {max:0.#}");
            Check(Near(_rig.Hp, current, 0.05f), $"raising max leaves current at {current:0.#} (no free heal)", $"current HP {current:0.#} -> {_rig.Hp:0.#} when max was raised");
            _rig.Vital.ReloadComponent();
        }

        private IEnumerator CaseManaZeroAlive()
        {
            _rig.Vital.ReloadComponent();
            yield return null;
            _rig.Vital.Reduction(VitalStatType.Mana, _rig.MaxMana + 10f);
            bool died = false;
            yield return WatchFor(0.5f, () => _rig.Player.StateMachine.CurrentState is PlayerDeathState, seen => died = seen);
            Check(!died && _rig.Hp > 0f, "Mana 0, player alive", $"player entered PlayerDeathState with Mana 0 (HP {_rig.Hp:0.#})");
            _rig.Vital.ReloadComponent();
        }

        private IEnumerator CaseUnseededVital()
        {
            _rig.Vital.ReloadComponent();
            yield return null;
            float hp = _rig.Hp, mana = _rig.Mana;
            bool threw = false;
            try
            {
                _rig.Vital.Recovery(VitalStatType.Coin, 5f);
                _rig.Vital.Reduction(VitalStatType.Coin, 5f);
                Note($"GetCurrentStatValue(Coin) = {_rig.Vital.GetCurrentStatValue(VitalStatType.Coin):0.##} (coins live in WalletComponent)");
            }
            catch (System.Exception e)
            {
                threw = true;
                Fail($"{e.GetType().Name}: {e.Message}");
            }
            Check(!threw && Near(_rig.Hp, hp) && Near(_rig.Mana, mana), "no exception, HP and Mana unchanged", $"HP {hp:0.#} -> {_rig.Hp:0.#}, Mana {mana:0.#} -> {_rig.Mana:0.#}");
        }

        private IEnumerator CaseBoundaryDamage()
        {
            if (_rig.Hurtbox == null)
            {
                Fail(NoReceiver);
                yield break;
            }
            _rig.Vital.ReloadComponent();
            yield return Frames(2);
            float amount = _rig.Hp - 0.5f;
            _rig.Hurtbox.TakeDamage(amount, (Vector2)_rig.Player.transform.position + Vector2.right);
            bool died = false;
            yield return WatchFor(0.5f, () => _rig.Player.StateMachine.CurrentState is PlayerDeathState, seen => died = seen);
            Check(!died && Near(_rig.Hp, 0.5f, 0.02f), $"alive at {_rig.Hp:0.##} HP", died ? "a hit of current HP - 0.5 killed the player" : $"HP {_rig.Hp:0.##}, expected 0.5 (player receiver mitigates?)");
            yield return Wait(1.5f);
            _rig.Vital.ReloadComponent();
        }

        private IEnumerator CaseTakeDamageState()
        {
            if (_rig.Hurtbox == null)
            {
                Fail(NoReceiver);
                yield break;
            }
            _rig.Vital.ReloadComponent();
            bool idle = false;
            yield return WaitUntil(() => _rig.Player.StateMachine.CurrentState is PlayerIdleState or PlayerMoveState, 3f, ok => idle = ok);
            if (!idle)
            {
                Skip($"player is in {_rig.Player.StateMachine.CurrentState?.GetType().Name}, not Idle / Move");
                yield break;
            }
            _rig.Hurtbox.TakeDamage(Mathf.Max(1f, _rig.MaxHp * 0.05f), (Vector2)_rig.Player.transform.position + Vector2.right);
            bool hurt = false;
            yield return WaitUntil(() => _rig.Player.StateMachine.CurrentState is PlayerTakeDamageState, 0.5f, ok => hurt = ok);
            Check(hurt, "entered PlayerTakeDamageState", $"state {_rig.Player.StateMachine.CurrentState?.GetType().Name} 0.5s after a hit");
            bool back = false;
            yield return WaitUntil(() => _rig.Player.StateMachine.CurrentState is PlayerIdleState or PlayerMoveState, 3f, ok => back = ok);
            Check(back, "returned to Idle / Move", $"still in {_rig.Player.StateMachine.CurrentState?.GetType().Name} 3s after the hit");
            _rig.Vital.ReloadComponent();
        }

        private IEnumerator CaseDeath()
        {
            _rig.Vital.ReloadComponent();
            yield return Frames(2);
            _deathEvents = 0;
            _rig.Hurtbox.TakeDamage(_rig.MaxHp * 10f, (Vector2)_rig.Player.transform.position + Vector2.right);
            bool dead = false;
            yield return WaitUntil(() => _rig.Player.StateMachine.CurrentState is PlayerDeathState, 3f, ok => dead = ok);
            Check(Near(_rig.Hp, 0f), "HP = 0", $"HP {_rig.Hp:0.#} after lethal damage");
            Check(dead, "entered PlayerDeathState", $"state is {_rig.Player.StateMachine.CurrentState?.GetType().Name} 3s after lethal damage");
            bool emitted = false;
            yield return WaitUntil(() => _deathEvents > 0, 4f, ok => emitted = ok);
            Check(emitted, "ON_PLAYER_DEATH emitted", "ON_PLAYER_DEATH not emitted within 4s of death");
            Animator anim = _rig.Player.Anim;
            AnimatorClipInfo[] clips = anim != null && anim.runtimeAnimatorController != null ? anim.GetCurrentAnimatorClipInfo(0) : new AnimatorClipInfo[0];
            if (!emitted && clips.Length > 0)
                Note($"death clip {clips[0].clip.name}, events: {string.Join(", ", clips[0].clip.events.Select(e => e.functionName))} (PlayerDeathState emits on StatusAnimation.EndRangeTrigger)");
        }

        private IEnumerator CaseDeathOnce()
        {
            if (_deathEvents == 0)
            {
                Skip("the player never emitted ON_PLAYER_DEATH (see QA-VIT-07)");
                yield break;
            }
            int start = _deathEvents;
            yield return Wait(1f);
            int extra = _deathEvents - start;
            Check(start == 1 && extra == 0, "emitted exactly once", $"emitted {start} times by the time it was seen, +{extra} more in the next 1s");
        }

        private void OnPlayerDeath(object _) => _deathEvents++;
    }
}
#endif
