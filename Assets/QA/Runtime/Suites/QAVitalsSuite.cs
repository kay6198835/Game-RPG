#if UNITY_EDITOR
using System.Collections;
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
                _rig.Vital.CurrentStatsChanged += () => _changedEvents++;
                yield return Frames(3);
            }

            yield return Case("QA-VIT-01", "VIT", QAPriority.P0, "ReloadComponent fills HP and Mana to max", Guard(CaseReloadFull()));
            yield return Case("QA-VIT-02", "VIT", QAPriority.P0, "Reduction lowers the value by exactly the amount and raises CurrentStatsChanged", Guard(CaseReductionExact()));
            yield return Case("QA-VIT-03", "VIT", QAPriority.P1, "Reduction larger than the current value clamps at 0", Guard(CaseReductionClamp()));
            yield return Case("QA-VIT-04", "VIT", QAPriority.P1, "Recovery below max restores exactly the amount", Guard(CaseRecoveryExact()));
            yield return Case("QA-VIT-05", "VIT", QAPriority.P1, "Recovery never pushes the value above max", Guard(CaseRecoveryClamp()));
            yield return Case("QA-VIT-06", "VIT", QAPriority.P0, "Damage through the player hurtbox lowers HP, player survives", Guard(CaseHurtbox()));
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

        private IEnumerator CaseHurtbox()
        {
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
