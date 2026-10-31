#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UIFlow;
using UnityEngine;
using UnityEngine.InputSystem;

namespace QA.Automation
{
    // Player flow: spawn → pick each class at its monument → basic attack → the four abilities → asset
    // hygiene. Ported from Assets/Script/FastTest/QAProbe/QAPlayerFlowProbe.cs (sprint-20, ids A-/B-/C-/D-/E-)
    // to the stable QA-<SYS>-NN ids; see qa-automation/cases/case-catalog.md for the id map.
    [DefaultExecutionOrder(-10000)]
    public class QAPlayerFlowSuite : QASuite
    {
        [SerializeField] private PlayerManager playerManager;
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private PlayerData[] classes;
        [SerializeField] private Transform[] monuments;
        [SerializeField] private QAProbeTarget target;

        private readonly QAPlayerRig _rig = new();
        private QAInputDriver _input;

        private int _confirmPanelOpened;
        private ConfirnData _lastConfirnData;
        private int _doorEvents;
        private readonly Dictionary<BaseStatsSO, Dictionary<StatType, float>> _assetSnapshots = new();

        private static readonly Key[] SlotKeys = { Key.Digit1, Key.Digit2, Key.Digit3, Key.Digit4 };

        private Player P => _rig.Player;

        private void OnEnable()
        {
            UIEvents.OpenConfirnPanel += OnConfirmPanel;
            EventManager.Resgister(EventID.ON_OPEN_DOOR, OnOpenDoor);
        }

        private void OnDisable()
        {
            UIEvents.OpenConfirnPanel -= OnConfirmPanel;
            EventManager.UnResgister(EventID.ON_OPEN_DOOR, OnOpenDoor);
            _input?.Dispose();
            _input = null;
        }

        // Runs before the player's states tick (execution order), after input events are processed.
        private void Update() => _input?.ApplyAim();

        public override void Cleanup()
        {
            _input?.Dispose();
            _input = null;
        }

        public override IEnumerator Run()
        {
            _input = new QAInputDriver();
            foreach (PlayerData data in classes)
                if (data != null && data.Stats != null) _assetSnapshots[data.Stats] = data.Stats.FullStatsValue();

            // GameplayUI is loaded additively by CombatTestBootstrap.Start; give it time to register its listeners.
            yield return Wait(1.5f);

            yield return Case("QA-PLY-01", "PLY", QAPriority.P0, "Player spawns once at the spawn point", CaseSpawn());
            yield return Case("QA-PLY-02", "PLY", QAPriority.P2, "Before a class is picked: attack and ability input", Guard(CaseBeforePick()));

            for (int i = 0; i < classes.Length; i++)
            {
                PlayerData data = classes[i];
                string c = data.name;
                yield return Case($"QA-PLY-03.{c}", "PLY", QAPriority.P0, $"{c}: G at the monument opens the confirm panel", Guard(CaseOpenPanel(i)));
                yield return Case($"QA-PLY-04.{c}", "PLY", QAPriority.P1, $"{c}: nothing changes before Confirm", Guard(CaseBeforeConfirm(data)));
                // Always runs: every later case of this class needs the class applied.
                yield return Case($"QA-PLY-05.{c}", "PLY", QAPriority.P0, $"{c}: Confirm applies the class", Guard(CaseConfirm(data)));
                yield return Case($"QA-UI-01.{c}", "UI", QAPriority.P1, $"{c}: hotbar shows every bound ability", Guard(CaseHotbar(data)));

                Weapon weapon = _rig.WeaponHolder != null ? _rig.WeaponHolder.Weapon : null;
                if (weapon == null)
                    yield return Case($"QA-CMB-06.{c}", "CMB", QAPriority.P1, $"{c}: attack with no weapon", Guard(CaseNoWeapon()));
                else if (weapon.Stats.Type == WeaponType.MeleeWP)
                {
                    yield return Case($"QA-CMB-01.{c}", "CMB", QAPriority.P0, $"{c}: melee hit damage = PhysicalDamage + stage damage", Guard(CaseMeleeDamage()));
                    yield return Case($"QA-CMB-02.{c}", "CMB", QAPriority.P1, $"{c}: melee combo reaches every stage", Guard(CaseMeleeCombo()));
                    yield return Case($"QA-CMB-03.{c}", "CMB", QAPriority.P1, $"{c}: melee out of range deals no damage", Guard(CaseMeleeOutOfRange()));
                }
                else
                {
                    yield return Case($"QA-CMB-04.{c}", "CMB", QAPriority.P0, $"{c}: arrow hits and despawns", Guard(CaseRangedHit()));
                    yield return Case($"QA-CMB-05.{c}", "CMB", QAPriority.P1, $"{c}: shots are at least RecoveryTime apart", Guard(CaseRangedInterval()));
                }

                foreach (AbilityBinding binding in data.AbilityBindings)
                {
                    if (binding == null || binding.Ability == null) continue;
                    string name = AbilityName(binding.Ability);
                    string slot = $"{c}.{binding.Slot}";
                    QAPriority castPriority = binding.Slot == AbilitySlot.Primary ? QAPriority.P0 : QAPriority.P1;
                    yield return Case($"QA-ABL-01.{slot}", "ABL", castPriority, $"{c} {name}: cast pays cost, starts cooldown, applies effect", Guard(CaseCast(binding)));
                    yield return Case($"QA-ABL-02.{slot}", "ABL", QAPriority.P1, $"{c} {name}: no recast while on cooldown", Guard(CaseCooldownGate(binding)));
                    yield return Case($"QA-ABL-03.{slot}", "ABL", QAPriority.P2, $"{c} {name}: refused when the cost cannot be paid", Guard(CaseInsufficient(binding)));
                }
            }

            yield return Case("QA-STAT-10", "STAT", QAPriority.P0, "Class stat assets unchanged after a full player run (BUG-115)", CaseAssetsClean());
        }

        private IEnumerator Guard(IEnumerator body)
        {
            if (!_rig.Ready)
            {
                Fail("no player — QA-PLY-01 failed");
                yield break;
            }
            yield return body;
        }

        #region Cases — spawn

        private IEnumerator CaseSpawn()
        {
            bool spawned = false;
            yield return WaitUntil(() => playerManager.Player != null, 3f, ok => spawned = ok);
            Check(spawned, "PlayerManager spawned a player", "PlayerManager.Player is null after 3s");
            if (!spawned) yield break;

            _rig.Bind(playerManager.Player);
            _input.Bind(_rig.Input);
            Player[] all = FindObjectsOfType<Player>();
            Check(all.Length == 1, "exactly 1 Player in the scene", $"{all.Length} Player objects in the scene");
            float distance = Vector2.Distance(P.transform.position, spawnPoint.position);
            Check(distance < 0.1f, "at the spawn point", $"{distance:0.00} units from the spawn point");
            Check(Camera.main != null && Camera.main.transform.IsChildOf(P.transform),
                "camera follows the player", "Camera.main is not parented to the player");
            Check(_rig.Input != null && _rig.WeaponHolder != null && _rig.AbilityHolder != null && _rig.Stats != null && _rig.Vital != null,
                "input, weapon, ability, stat and vital components found", "missing component: " + _rig.MissingComponents());
            Note($"data before pick: {(P.Data != null ? P.Data.name : "none")}");
        }

        private IEnumerator CaseBeforePick()
        {
            Observe("design undecided: is an unpicked player meant to be unarmed?");
            Note($"weapon before pick: {(_rig.WeaponHolder.Weapon != null ? _rig.WeaponHolder.Weapon.name : "none")}");
            float hp = _rig.Hp, mana = _rig.Mana;
            bool attacked = false, casted = false;

            _input.AimAt((Vector2)P.transform.position + Vector2.right * 2f);
            _input.MouseDown();
            yield return WatchFor(0.6f, () => P.StateMachine.CurrentState is PlayerAttackState, v => attacked = v);
            _input.MouseUp();
            yield return Tap(SlotKeys[0]);
            yield return WatchFor(0.6f, () => P.StateMachine.CurrentState is PlayerSkillWeaponState, v => casted = v);

            Note($"LMB entered PlayerAttackState: {attacked}");
            Note($"key 1 entered PlayerSkillWeaponState: {casted}");
            Note($"HP {hp:0.#} -> {_rig.Hp:0.#}, Mana {mana:0.#} -> {_rig.Mana:0.#}");
            yield return WaitIdle(true);
        }

        #endregion

        #region Cases — pick class

        private IEnumerator CaseOpenPanel(int index)
        {
            Transform monument = monuments[index];
            yield return WaitIdle();
            P.transform.position = monument.position + Vector3.down * 0.8f;
            Physics2D.SyncTransforms();
            _input.AimAt(monument.position);
            yield return Frames(3);

            _confirmPanelOpened = 0;
            _doorEvents = 0;
            _input.KeyDown(Key.G);
            bool opened = false;
            yield return WaitUntil(() => _confirmPanelOpened > 0, 3f, ok => opened = ok);
            _input.KeyUp();
            Check(opened, "confirm panel opened", "no OpenConfirnPanel event within 3s of pressing G");
            if (opened)
            {
                Note($"panel data: confirm='{_lastConfirnData.confirmText}', cancel='{_lastConfirnData.cancelText}'");
                Check(_confirmPanelOpened == 1, "opened once", $"opened {_confirmPanelOpened} times for one press");
            }
        }

        private IEnumerator CaseBeforeConfirm(PlayerData data)
        {
            yield return Frames(2);
            Check(P.Data != data, "class not applied before Confirm", "class already applied before Confirm");
            Check(_doorEvents == 0, "door did not open before Confirm", $"ON_OPEN_DOOR fired {_doorEvents} times before Confirm");
            Note("the Cancel button itself is a manual case (MAN-PLY-03)");
        }

        private IEnumerator CaseConfirm(PlayerData data)
        {
            ConfirnPanel panel = FindObjectOfType<ConfirnPanel>(false);
            if (_confirmPanelOpened > 0 && panel != null) panel.AccessConfirm();
            else if (_confirmPanelOpened > 0)
            {
                Fail("panel event fired but no active ConfirnPanel in the scene; confirmed through UIEvents.AccessAction()");
                UIEvents.AccessAction();
            }
            else
            {
                Fail("panel never opened; class applied directly with SetCharacterData so later cases still run");
                P.SetCharacterData(data);
            }
            yield return Frames(3);

            Check(P.Data == data, $"Player.Data = {data.name}", $"Player.Data = {(P.Data != null ? P.Data.name : "null")}");
            Check(P.name == data.name, "GameObject renamed to the class", $"GameObject name '{P.name}'");
            Check(P.Anim.runtimeAnimatorController == data.animationController,
                "animator controller = class controller", $"animator controller '{(P.Anim.runtimeAnimatorController != null ? P.Anim.runtimeAnimatorController.name : "null")}'");

            Weapon weapon = _rig.WeaponHolder.Weapon;
            if (data.Weapon != null)
                Check(weapon != null && weapon.name.StartsWith(data.Weapon.name),
                    $"weapon = {data.Weapon.name}", $"weapon = {(weapon != null ? weapon.name : "none")}");
            else
            {
                Observe($"{data.name} has no default weapon in data");
                Check(weapon == null, "no weapon equipped", $"weapon = {(weapon != null ? weapon.name : "none")}");
            }

            int weapons = P.GetComponentsInChildren<Weapon>(false).Length;
            int expectedWeapons = data.Weapon != null ? 1 : 0;
            Check(weapons == expectedWeapons, $"{expectedWeapons} weapon under the player", $"{weapons} weapons under the player (old weapon not released)");

            Check(Near(_rig.Hp, _rig.MaxHp) && Near(_rig.Mana, _rig.MaxMana),
                $"HP {_rig.Hp:0}/{_rig.MaxHp:0}, Mana {_rig.Mana:0}/{_rig.MaxMana:0} full", $"HP {_rig.Hp:0}/{_rig.MaxHp:0}, Mana {_rig.Mana:0}/{_rig.MaxMana:0} not full");
            Check(_doorEvents == 1, "ON_OPEN_DOOR once", $"ON_OPEN_DOOR fired {_doorEvents} times");
            Check(Mathf.Approximately(Time.timeScale, 1f), "game unpaused after Confirm", $"Time.timeScale = {Time.timeScale} after Confirm (panel still open?)");
            // UIPanel hides with a CanvasGroup fade on unscaled time, never SetActive(false).
            float fadeEnd = Time.realtimeSinceStartup + 0.6f;
            while (Time.realtimeSinceStartup < fadeEnd) yield return null;
            CanvasGroup canvasGroup = panel != null ? panel.GetComponent<CanvasGroup>() : null;
            Check(canvasGroup == null || (canvasGroup.alpha <= 0.01f && !canvasGroup.blocksRaycasts),
                "confirm panel hidden", $"confirm panel still visible after Confirm (alpha {canvasGroup?.alpha:0.##}, blocksRaycasts {canvasGroup?.blocksRaycasts})");
        }

        private IEnumerator CaseHotbar(PlayerData data)
        {
            yield return null;
            IReadOnlyList<SkillSlotData> hotbar = UIServices.Player?.GetHotbar();
            if (hotbar == null)
            {
                Fail("UIServices.Player.GetHotbar() returned null (GameplayUI not loaded?)");
                yield break;
            }
            int bound = data.AbilityBindings.Count(b => b != null && b.Ability != null);
            int shown = hotbar.Count(s => s != null && !string.IsNullOrEmpty(s.skillName));
            Check(shown == bound, $"hotbar shows {bound} abilities", $"hotbar shows {shown} named slots, {bound} bound");
            Note("hotbar: " + string.Join(", ", hotbar.Select(s => s == null ? "-" : $"[{s.keyLabel}] {s.skillName} ({s.cooldownSeconds:0.#}s, icon {(s.icon != null ? "yes" : "no")})")));
        }

        #endregion

        #region Cases — basic attack

        private IEnumerator CaseMeleeDamage()
        {
            AttackSO stage = _rig.WeaponHolder.Weapon.Stats.GetStage(0);
            yield return PlaceTarget(Mathf.Max(0.6f, stage.attackRange * 0.6f));
            _input.MouseDown();
            yield return Frames(2);
            _input.MouseUp();
            bool hit = false;
            yield return WaitUntil(() => target.Hits.Count > 0, 2.5f, ok => hit = ok);
            Check(hit, "the hit reached the target", "no hit within 2.5s");
            if (!hit) yield break;

            float physical = _rig.Stats.GetStatValue(StatType.PhysicalDamage);
            float crit = _rig.Stats.GetStatValue(StatType.CritDamage);
            float expected = physical + stage.attackDamage;
            float actual = target.Hits[0].Damage;
            bool normal = Near(actual, expected);
            bool critical = Near(actual, expected + crit);
            Check(normal || critical,
                $"damage {actual:0.##} = PhysicalDamage {physical:0.##} + stage {stage.attackDamage}{(critical ? $" + crit {crit:0.##}" : "")}",
                $"damage {actual:0.##}, expected {expected:0.##} (or {expected + crit:0.##} on crit)");
            Check(stage.attackDamage > 0, $"stage 0 damage {stage.attackDamage}", "stage 0 attackDamage = 0 (stage damage lost)");
            yield return WaitIdle(true);
        }

        private IEnumerator CaseMeleeCombo()
        {
            Weapon weapon = _rig.WeaponHolder.Weapon;
            AttackSO stage = weapon.Stats.GetStage(0);
            yield return PlaceTarget(Mathf.Max(0.6f, stage.attackRange * 0.6f));
            int maxStage = 0;
            for (int i = 0; i < 8; i++)
            {
                _input.MouseDown();
                yield return Frames(2);
                _input.MouseUp();
                yield return WatchFor(0.25f, () => { maxStage = Mathf.Max(maxStage, weapon.CurrentStageIndex); return false; }, _ => { });
            }
            int last = weapon.Stats.StageCount - 1;
            Check(maxStage == last, $"reached stage {maxStage + 1}/{last + 1}", $"highest stage reached {maxStage + 1}/{last + 1}");
            Note($"hits: {target.Hits.Count}");
            yield return WaitIdle(true);
        }

        private IEnumerator CaseMeleeOutOfRange()
        {
            AttackSO stage = _rig.WeaponHolder.Weapon.Stats.GetStage(0);
            yield return PlaceTarget(stage.attackRange + 2.5f);
            _input.MouseDown();
            yield return Frames(2);
            _input.MouseUp();
            yield return Wait(1.5f);
            Check(target.Hits.Count == 0, $"0 hits at {stage.attackRange + 2.5f:0.#} units", $"{target.Hits.Count} hits out of range");
            yield return WaitIdle(true);
        }

        private IEnumerator CaseRangedHit()
        {
            yield return PlaceTarget(4f);
            _input.MouseDown();
            yield return Frames(2);
            _input.MouseUp();
            bool hit = false;
            yield return WaitUntil(() => target.Hits.Count > 0, 2.5f, ok => hit = ok);
            Check(hit, $"arrow hit, damage {(hit ? target.Hits[0].Damage : 0):0.##}", "no arrow reached the target within 2.5s");
            yield return Wait(2f);
            int live = FindObjectsOfType<ProjectileBody>().Length;
            Check(live == 0, "no projectile left alive 2s later", $"{live} projectiles still active 2s later");
            yield return WaitIdle(true);
        }

        private IEnumerator CaseRangedInterval()
        {
            RangeAttackSO stage = _rig.WeaponHolder.Weapon.Stats.GetStage(0) as RangeAttackSO;
            if (stage == null)
            {
                Skip("stage 0 is not a RangeAttackSO");
                yield break;
            }
            yield return PlaceTarget(3f);
            for (int i = 0; i < 12; i++)
            {
                _input.MouseDown();
                yield return Frames(2);
                _input.MouseUp();
                yield return Wait(0.08f);
            }
            yield return Wait(1f);

            List<float> shots = new();
            foreach (QAProbeTarget.Hit hit in target.Hits)
                if (shots.Count == 0 || hit.Time - shots[^1] > 0.02f) shots.Add(hit.Time);
            if (shots.Count < 2)
            {
                Fail($"only {shots.Count} shot(s) landed in ~2s of clicking");
                yield break;
            }
            float min = float.MaxValue;
            for (int i = 1; i < shots.Count; i++) min = Mathf.Min(min, shots[i] - shots[i - 1]);
            Check(min >= stage.RecoveryTime - 0.02f,
                $"min interval {min:0.###}s >= RecoveryTime {stage.RecoveryTime:0.###}s",
                $"min interval {min:0.###}s < RecoveryTime {stage.RecoveryTime:0.###}s");
            Note($"{shots.Count} shots landed");
            yield return WaitIdle(true);
        }

        private IEnumerator CaseNoWeapon()
        {
            bool attacked = false;
            yield return PlaceTarget(1f);
            _input.MouseDown();
            yield return WatchFor(0.6f, () => P.StateMachine.CurrentState is PlayerAttackState, v => attacked = v);
            _input.MouseUp();
            Check(!attacked && target.Hits.Count == 0, "no attack, no damage, no exception", $"attack state entered={attacked}, hits={target.Hits.Count}");
            yield return WaitIdle(true);
        }

        #endregion

        #region Cases — abilities

        private IEnumerator CaseCast(AbilityBinding binding)
        {
            AbilityDefinition def = binding.Ability;
            yield return PrepareAbility(binding.Slot);
            yield return PlaceTarget(2.5f);

            // Start below max so a recovery effect has room to show; keep enough to pay the cost.
            (float hpNeed, float manaNeed) = ExpectedCost(def);
            _rig.Vital.Reduction(VitalStatType.HP, Mathf.Min(_rig.MaxHp * 0.3f, _rig.Hp - hpNeed - 1f));
            _rig.Vital.Reduction(VitalStatType.Mana, Mathf.Min(_rig.MaxMana * 0.3f, _rig.Mana - manaNeed - 1f));
            yield return null;

            Dictionary<StatType, float> statsBefore = _rig.Stats.GetFullStat();
            float hp0 = _rig.Hp, mana0 = _rig.Mana;
            List<(int frame, float hp, float mana)> vitalLog = new();
            void OnVital() => vitalLog.Add((Time.frameCount, _rig.Hp, _rig.Mana));
            float cooldown = -1f;
            void OnCooldown(AbilitySlot slot, float duration) { if (slot == binding.Slot) cooldown = duration; }
            _rig.Vital.CurrentStatsChanged += OnVital;
            _rig.AbilityHolder.AbilityCooldownStarted += OnCooldown;

            bool enteredSkill = false;
            try
            {
                yield return PressAbility(binding, v => enteredSkill = v);
                yield return WaitUntil(() => cooldown >= 0f, 3f, _ => { });
                yield return Wait(2.5f);
            }
            finally
            {
                _rig.Vital.CurrentStatsChanged -= OnVital;
                _rig.AbilityHolder.AbilityCooldownStarted -= OnCooldown;
            }
            Dictionary<StatType, float> statsDuring = _rig.Stats.GetFullStat();

            Note($"entered PlayerSkillWeaponState: {enteredSkill}");
            Check(cooldown >= 0f, $"cooldown started ({cooldown:0.##}s)", "no cooldown event — the ability never reached Do");
            if (cooldown >= 0f)
                Check(Mathf.Approximately(cooldown, def.Cooldown), $"cooldown = definition {def.Cooldown:0.##}s", $"cooldown {cooldown:0.##}s, definition {def.Cooldown:0.##}s");

            (float hpCost, float manaCost) = ExpectedCost(def);
            (float hpPaid, float manaPaid) = PaidInFirstFrame(vitalLog, hp0, mana0);
            Check(Near(hpPaid, hpCost) && Near(manaPaid, manaCost),
                $"cost paid HP {hpPaid:0.#} / Mana {manaPaid:0.#}",
                $"paid HP {hpPaid:0.#} / Mana {manaPaid:0.#}, definition costs HP {hpCost:0.#} / Mana {manaCost:0.#}");

            if (def.Effects == null || def.Effects.Count(e => e != null) == 0)
            {
                Skip("Effects list is empty — ability not implemented yet");
                yield return WaitIdle(true);
                yield break;
            }

            Note("effects: " + string.Join(", ", def.Effects.Where(e => e != null).Select(e => $"{e.name} ({e.GetType().Name})")));
            bool spawns = def.Effects.Any(e => e is SpawnEffectBase);
            if (spawns)
                Check(target.Hits.Count > 0, $"target hit {target.Hits.Count}x, first {(target.Hits.Count > 0 ? target.Hits[0].Damage : 0):0.##}",
                    "spawn effect dealt no damage to a target 2.5 units ahead within 2.5s");

            List<string> statChanges = statsBefore.Where(p => statsDuring.TryGetValue(p.Key, out float v) && !Mathf.Approximately(v, p.Value))
                .Select(p => $"{p.Key} {p.Value:0.##}->{statsDuring[p.Key]:0.##}").ToList();
            bool recovered = vitalLog.Count > 0 &&
                             (vitalLog.Any(v => v.hp > hp0 - hpCost + 0.01f && v.frame != vitalLog[0].frame)
                              || vitalLog.Any(v => v.mana > mana0 - manaCost + 0.01f && v.frame != vitalLog[0].frame));
            if (!spawns)
                Check(statChanges.Count > 0 || recovered,
                    $"stat effect visible: {(statChanges.Count > 0 ? string.Join(", ", statChanges) : "HP/Mana recovered after the cost")}",
                    "no stat change and no recovery within 2.5s");
            else if (statChanges.Count > 0) Note("stat changes: " + string.Join(", ", statChanges));

            yield return WaitIdle(true);
        }

        private IEnumerator CaseCooldownGate(AbilityBinding binding)
        {
            yield return PrepareAbility(binding.Slot);
            int casts = 0;
            void OnCooldown(AbilitySlot slot, float _) { if (slot == binding.Slot) casts++; }
            _rig.AbilityHolder.AbilityCooldownStarted += OnCooldown;
            try
            {
                yield return PressAbility(binding, _ => { });
                yield return WaitUntil(() => casts > 0, 3f, _ => { });
                if (casts == 0)
                {
                    Skip("first cast never started a cooldown (see QA-ABL-01)");
                    yield break;
                }
                yield return WaitIdle();
                AbilityInstance instance = Instance(binding.Slot);
                float remaining = instance != null ? instance.CooldownRemaining : 0f;
                if (remaining <= 0f)
                {
                    Observe($"cooldown {binding.Ability.Cooldown:0.##}s ended before the player was idle again — the cast animation outlasts the cooldown, so the gate cannot be reached by input");
                    yield break;
                }

                float hp = _rig.Hp, mana = _rig.Mana;
                yield return PressAbility(binding, _ => { });
                yield return Wait(0.3f);
                Check(casts == 1 && Near(hp, _rig.Hp) && Near(mana, _rig.Mana),
                    $"press with {remaining:0.##}s left: no cast, no cost", $"recast during cooldown: casts={casts}, HP {hp:0.#}->{_rig.Hp:0.#}, Mana {mana:0.#}->{_rig.Mana:0.#}");
            }
            finally
            {
                _rig.AbilityHolder.AbilityCooldownStarted -= OnCooldown;
            }
            yield return WaitIdle(true);
        }

        private IEnumerator CaseInsufficient(AbilityBinding binding)
        {
            AbilityDefinition def = binding.Ability;
            (float hpCost, float manaCost) = ExpectedCost(def);
            if (hpCost <= 0f && manaCost <= 0f)
            {
                Skip("ability has no cost");
                yield break;
            }
            yield return PrepareAbility(binding.Slot);
            if (hpCost > 0f) _rig.Vital.Reduction(VitalStatType.HP, _rig.Hp - (hpCost - 1f));
            if (manaCost > 0f) _rig.Vital.Reduction(VitalStatType.Mana, _rig.Mana - (manaCost - 1f));
            if (hpCost > 0f) Observe($"HP set to {_rig.Hp:0.#} below its {hpCost:0.#} cost (design open: should an HP cost be refused?)");
            float hp = _rig.Hp, mana = _rig.Mana;

            int casts = 0;
            void OnCooldown(AbilitySlot slot, float _) { if (slot == binding.Slot) casts++; }
            _rig.AbilityHolder.AbilityCooldownStarted += OnCooldown;
            try
            {
                yield return PressAbility(binding, _ => { });
                yield return Wait(1f);
            }
            finally
            {
                _rig.AbilityHolder.AbilityCooldownStarted -= OnCooldown;
            }

            Check(casts == 0 && _rig.Hp >= hp - 0.01f && _rig.Mana >= mana - 0.01f,
                $"refused at HP {hp:0.#} / Mana {mana:0.#}", $"cast went through: casts={casts}, HP {hp:0.#}->{_rig.Hp:0.#}, Mana {mana:0.#}->{_rig.Mana:0.#}");
            Check(_rig.Hp > 0f, "player alive", "HP reached 0 from an ability cost");
            _rig.Vital.ReloadComponent();
            yield return WaitIdle(true);
        }

        #endregion

        #region Cases — assets

        private IEnumerator CaseAssetsClean()
        {
            yield return null;
            foreach (KeyValuePair<BaseStatsSO, Dictionary<StatType, float>> pair in _assetSnapshots)
            {
                Dictionary<StatType, float> now = pair.Key.FullStatsValue();
                List<string> changed = pair.Value.Where(p => !now.TryGetValue(p.Key, out float v) || !Mathf.Approximately(v, p.Value))
                    .Select(p => $"{p.Key} {p.Value:0.##}->{(now.TryGetValue(p.Key, out float v) ? v.ToString("0.##") : "missing")}").ToList();
                Check(changed.Count == 0, $"{pair.Key.name} unchanged", $"{pair.Key.name} changed in memory: {string.Join(", ", changed)}");
            }
        }

        #endregion

        #region Helpers

        private bool IsIdle => P.StateMachine.CurrentState is PlayerIdleState || P.StateMachine.CurrentState is PlayerMoveState;

        // strict: called at the end of a case, where a player still locked in a state is that case's defect.
        private IEnumerator WaitIdle(bool strict = false)
        {
            bool idle = false;
            yield return WaitUntil(() => IsIdle, 4f, ok => idle = ok);
            if (idle) yield break;
            string message = $"player stuck in {P.StateMachine.CurrentState?.GetType().Name} for 4s after the action";
            if (strict) Fail(message);
            else Note($"(carried over) {message}");
        }

        private IEnumerator PlaceTarget(float distance)
        {
            yield return WaitIdle();
            target.Clear();
            Vector2 origin = P.transform.position;
            target.MoveTo(origin + Vector2.right * distance);
            _input.AimAt(target.transform.position);
            yield return Frames(2);
            target.Clear();
        }

        private IEnumerator PrepareAbility(AbilitySlot slot)
        {
            yield return WaitIdle();
            AbilityInstance instance = Instance(slot);
            if (instance != null) yield return WaitUntil(() => instance.CooldownRemaining <= 0f, 5f, _ => { });
            _rig.Vital.ReloadComponent();
            target.Clear();
        }

        private IEnumerator PressAbility(AbilityBinding binding, Action<bool> enteredSkill)
        {
            Key key = SlotKeys[(int)binding.Slot];
            bool entered = false;
            _input.KeyDown(key);
            float hold = binding.Ability.ActivationType == AbilityActivationType.Hold ? 0.5f : 0.05f;
            yield return WatchFor(hold, () => P.StateMachine.CurrentState is PlayerSkillWeaponState, v => entered |= v);
            _input.KeyUp();
            yield return WatchFor(0.3f, () => P.StateMachine.CurrentState is PlayerSkillWeaponState, v => entered |= v);
            enteredSkill(entered);
        }

        private IEnumerator Tap(Key key)
        {
            _input.KeyDown(key);
            yield return Frames(2);
            _input.KeyUp();
        }

        private AbilityInstance Instance(AbilitySlot slot)
        {
            FieldInfo field = typeof(AbilityHolderBase<Core>).GetField("_equipped", BindingFlags.Instance | BindingFlags.NonPublic);
            var equipped = field?.GetValue(_rig.AbilityHolder) as Dictionary<AbilitySlot, AbilityInstance>;
            return equipped != null && equipped.TryGetValue(slot, out AbilityInstance instance) ? instance : null;
        }

        private static (float hp, float mana) ExpectedCost(AbilityDefinition def)
        {
            float hp = 0f, mana = 0f;
            if (def.Costs == null) return (hp, mana);
            foreach (StatCost cost in def.Costs)
            {
                if (cost == null) continue;
                if (cost.statType == VitalStatType.HP) hp += cost.value;
                else if (cost.statType == VitalStatType.Mana) mana += cost.value;
            }
            return (hp, mana);
        }

        // Costs are paid in the activation frame; later changes (recovery, buffs) belong to the effect.
        private static (float hp, float mana) PaidInFirstFrame(List<(int frame, float hp, float mana)> log, float hp0, float mana0)
        {
            int first = -1;
            foreach (var entry in log)
                if (entry.hp < hp0 - 0.001f || entry.mana < mana0 - 0.001f) { first = entry.frame; break; }
            if (first < 0) return (0f, 0f);
            var last = log.Last(e => e.frame == first);
            return (hp0 - last.hp, mana0 - last.mana);
        }

        private static string AbilityName(AbilityDefinition def) => string.IsNullOrEmpty(def.DisplayName) ? def.name : def.DisplayName;

        private void OnConfirmPanel(ConfirnData data)
        {
            _confirmPanelOpened++;
            _lastConfirnData = data;
        }

        private void OnOpenDoor(object _) => _doorEvents++;

        #endregion
    }
}
#endif
