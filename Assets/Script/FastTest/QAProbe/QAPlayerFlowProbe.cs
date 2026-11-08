using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UIFlow;
using UnityEngine;
using UnityEngine.InputSystem;

// Automated player-flow probe (QA, sprint-20): spawn → pick each class at its monument → basic attack →
// the four abilities. Every case asserts numbers or state, never looks. Visual and feel checks are listed
// as PENDING in the report for a human pass. The report goes to production/qa/evidence/auto/.
// Drive it from Tools > QA > Player Flow, or from the command line via QAPlayerFlowRunner.RunFromCommandLine.
[DefaultExecutionOrder(-10000)]
public class QAPlayerFlowProbe : MonoBehaviour
{
    public const string CliFlagKey = "QA.PlayerFlow.Cli";
    public const string ExitCodeKey = "QA.PlayerFlow.ExitCode";

    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private PlayerData[] classes;
    [SerializeField] private Transform[] monuments;
    [SerializeField] private QAProbeTarget target;
    [SerializeField] private float caseTimeout = 40f;

    private enum Verdict { PASS, FAIL, SKIP, OBSERVED }

    private sealed class CaseResult
    {
        public string Id;
        public string Title;
        public Verdict Verdict = Verdict.PASS;
        public readonly List<string> Notes = new();
    }

    private readonly List<CaseResult> _results = new();
    private CaseResult _current;
    private QAInputDriver _input;

    private Player _player;
    private PlayerInputHandler _handler;
    private WeaponHolder _weaponHolder;
    private AbilityHolder _abilityHolder;
    private StatHandler _stats;
    private VitalStatsComponent _vital;

    private int _confirmPanelOpened;
    private ConfirnData _lastConfirnData;
    private int _doorEvents;
    private readonly Dictionary<BaseStatsSO, Dictionary<StatType, float>> _assetSnapshots = new();

    private static readonly Key[] SlotKeys = { Key.Digit1, Key.Digit2, Key.Digit3, Key.Digit4 };

    #region Unity

    private void OnEnable()
    {
        Application.logMessageReceived += OnLog;
        UIEvents.OpenConfirnPanel += OnConfirmPanel;
        EventManager.Resgister(EventID.ON_OPEN_DOOR, OnOpenDoor);
    }

    private void OnDisable()
    {
        Application.logMessageReceived -= OnLog;
        UIEvents.OpenConfirnPanel -= OnConfirmPanel;
        EventManager.UnResgister(EventID.ON_OPEN_DOOR, OnOpenDoor);
        _input?.Dispose();
    }

    private IEnumerator Start()
    {
        Application.runInBackground = true;
        _input = new QAInputDriver();
        foreach (PlayerData data in classes)
            if (data != null && data.Stats != null) _assetSnapshots[data.Stats] = new StatSheet(data.Stats).GetFullStat();

        // GameplayUI is loaded additively by CombatTestBootstrap.Start; give it time to register its listeners.
        yield return Wait(1.5f);

        yield return RunCase("A-01", "Player spawns once at the spawn point", CaseSpawn());
        yield return RunCase("A-02", "Before a class is picked: attack and ability input", CaseBeforePick());

        for (int i = 0; i < classes.Length; i++)
        {
            PlayerData data = classes[i];
            string c = data.name;
            yield return RunCase($"B-01 {c}", $"{c}: G at the monument opens the confirm panel", CaseOpenPanel(i));
            yield return RunCase($"B-03 {c}", $"{c}: nothing changes before Confirm", CaseBeforeConfirm(data));
            yield return RunCase($"B-02 {c}", $"{c}: Confirm applies the class", CaseConfirm(data));

            Weapon weapon = _weaponHolder != null ? _weaponHolder.Weapon : null;
            if (weapon == null)
                yield return RunCase($"C-05 {c}", $"{c}: attack with no weapon", CaseNoWeapon());
            else if (weapon.Stats.Type == WeaponType.MeleeWP)
            {
                yield return RunCase($"C-01 {c}", $"{c}: melee hit damage = PhysicalDamage + stage damage", CaseMeleeDamage());
                yield return RunCase($"C-02 {c}", $"{c}: melee combo reaches every stage", CaseMeleeCombo());
                yield return RunCase($"C-02b {c}", $"{c}: melee out of range deals no damage", CaseMeleeOutOfRange());
            }
            else
            {
                yield return RunCase($"C-03 {c}", $"{c}: arrow hits and despawns", CaseRangedHit());
                yield return RunCase($"C-04 {c}", $"{c}: shots are at least RecoveryTime apart", CaseRangedInterval());
            }

            foreach (AbilityBinding binding in data.AbilityBindings)
            {
                if (binding == null || binding.Ability == null) continue;
                string name = AbilityName(binding.Ability);
                yield return RunCase($"D-01 {c}/{binding.Slot}", $"{c} {name}: cast pays cost, starts cooldown, applies effect", CaseCast(binding));
                yield return RunCase($"D-02 {c}/{binding.Slot}", $"{c} {name}: no recast while on cooldown", CaseCooldownGate(binding));
                yield return RunCase($"D-03 {c}/{binding.Slot}", $"{c} {name}: refused when the cost cannot be paid", CaseInsufficient(binding));
            }
        }

        yield return RunCase("E-01", "Class stat assets unchanged after the run (BUG-115)", CaseAssetsClean());

        WriteReport();
        Finish();
    }

    // Runs before the player's states tick (execution order), after input events are processed.
    private void Update() => _input?.ApplyAim();

    #endregion

    #region Cases — spawn

    private IEnumerator CaseSpawn()
    {
        bool spawned = false;
        yield return WaitUntil(() => playerManager.Player != null, 3f, ok => spawned = ok);
        Check(spawned, "PlayerManager spawned a player", "PlayerManager.Player is null after 3s");
        if (!spawned) yield break;

        BindPlayer(playerManager.Player);
        Player[] all = FindObjectsOfType<Player>();
        Check(all.Length == 1, "exactly 1 Player in the scene", $"{all.Length} Player objects in the scene");
        float distance = Vector2.Distance(_player.transform.position, spawnPoint.position);
        Check(distance < 0.1f, "at the spawn point", $"{distance:0.00} units from the spawn point");
        Check(Camera.main != null && Camera.main.transform.IsChildOf(_player.transform),
            "camera follows the player", "Camera.main is not parented to the player");
        Check(_handler != null && _weaponHolder != null && _abilityHolder != null && _stats != null && _vital != null,
            "input, weapon, ability, stat and vital components found",
            $"missing component: input={_handler != null} weapon={_weaponHolder != null} ability={_abilityHolder != null} stats={_stats != null} vital={_vital != null}");
        Note($"data before pick: {(_player.Data != null ? _player.Data.name : "none")}");
    }

    private IEnumerator CaseBeforePick()
    {
        Observe("design undecided (QA question 1): is an unpicked player meant to be unarmed?");
        Note($"weapon before pick: {(_weaponHolder.Weapon != null ? _weaponHolder.Weapon.name : "none")}");
        float hp = Hp, mana = Mana;
        bool attacked = false, casted = false;

        _input.AimAt((Vector2)_player.transform.position + Vector2.right * 2f);
        _input.MouseDown();
        yield return WatchFor(0.6f, () => _player.StateMachine.CurrentState is PlayerAttackState, v => attacked = v);
        _input.MouseUp();
        yield return Tap(SlotKeys[0]);
        yield return WatchFor(0.6f, () => _player.StateMachine.CurrentState is PlayerSkillWeaponState, v => casted = v);

        Note($"LMB entered PlayerAttackState: {attacked}");
        Note($"key 1 entered PlayerSkillWeaponState: {casted}");
        Note($"HP {hp:0.#} -> {Hp:0.#}, Mana {mana:0.#} -> {Mana:0.#}");
        yield return WaitIdle(true);
    }

    #endregion

    #region Cases — pick class

    private IEnumerator CaseOpenPanel(int index)
    {
        Transform monument = monuments[index];
        yield return WaitIdle();
        _player.transform.position = monument.position + Vector3.down * 0.8f;
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
        Check(_player.Data != data, "class not applied before Confirm", "class already applied before Confirm");
        Check(_doorEvents == 0, "door did not open before Confirm", $"ON_OPEN_DOOR fired {_doorEvents} times before Confirm");
        Note("Cancel button itself is a manual check (MQ-03)");
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
            _player.SetCharacterData(data);
        }
        yield return Frames(3);

        Check(_player.Data == data, $"Player.Data = {data.name}", $"Player.Data = {(_player.Data != null ? _player.Data.name : "null")}");
        Check(_player.name == data.name, "GameObject renamed to the class", $"GameObject name '{_player.name}'");
        Check(_player.Anim.runtimeAnimatorController == data.animationController,
            "animator controller = class controller", $"animator controller '{Describe(_player.Anim.runtimeAnimatorController)}'");

        Weapon weapon = _weaponHolder.Weapon;
        if (data.Weapon != null)
        {
            Check(weapon != null && weapon.name.StartsWith(data.Weapon.name),
                $"weapon = {data.Weapon.name}", $"weapon = {(weapon != null ? weapon.name : "none")}");
        }
        else
        {
            Observe($"{data.name} has no default weapon in data (QA question 2)");
            Check(weapon == null, "no weapon equipped", $"weapon = {(weapon != null ? weapon.name : "none")}");
        }

        int weapons = _player.GetComponentsInChildren<Weapon>(false).Length;
        int expectedWeapons = data.Weapon != null ? 1 : 0;
        Check(weapons == expectedWeapons, $"{expectedWeapons} weapon under the player", $"{weapons} weapons under the player (old weapon not released)");

        Check(Mathf.Approximately(Hp, MaxHp) && Mathf.Approximately(Mana, MaxMana),
            $"HP {Hp:0}/{MaxHp:0}, Mana {Mana:0}/{MaxMana:0} full", $"HP {Hp:0}/{MaxHp:0}, Mana {Mana:0}/{MaxMana:0} not full");
        Check(_doorEvents == 1, "ON_OPEN_DOOR once", $"ON_OPEN_DOOR fired {_doorEvents} times");
        Check(Mathf.Approximately(Time.timeScale, 1f), "game unpaused after Confirm", $"Time.timeScale = {Time.timeScale} after Confirm (panel still open?)");
        // UIPanel hides with a CanvasGroup fade on unscaled time, never SetActive(false).
        float fadeEnd = Time.realtimeSinceStartup + 0.6f;
        while (Time.realtimeSinceStartup < fadeEnd) yield return null;
        CanvasGroup group = panel != null ? panel.GetComponent<CanvasGroup>() : null;
        Check(group == null || (group.alpha <= 0.01f && !group.blocksRaycasts),
            "confirm panel hidden", $"confirm panel still visible after Confirm (alpha {group?.alpha:0.##}, blocksRaycasts {group?.blocksRaycasts})");

        IReadOnlyList<SkillSlotData> hotbar = UIServices.Player.GetHotbar();
        int bound = data.AbilityBindings.Count(b => b != null && b.Ability != null);
        int shown = hotbar?.Count(s => s != null && !string.IsNullOrEmpty(s.skillName)) ?? 0;
        Check(shown == bound, $"hotbar shows {bound} abilities", $"hotbar shows {shown} named slots, {bound} bound");
        if (hotbar != null)
            Note("hotbar: " + string.Join(", ", hotbar.Select(s => s == null ? "-" : $"[{s.keyLabel}] {s.skillName} ({s.cooldownSeconds:0.#}s, icon {(s.icon != null ? "yes" : "no")})")));
    }

    #endregion

    #region Cases — basic attack

    private IEnumerator CaseMeleeDamage()
    {
        AttackSO stage = _weaponHolder.Weapon.Stats.GetStage(0);
        yield return PlaceTarget(Mathf.Max(0.6f, stage.attackRange * 0.6f));
        _input.MouseDown();
        yield return Frames(2);
        _input.MouseUp();
        bool hit = false;
        yield return WaitUntil(() => target.Hits.Count > 0, 2.5f, ok => hit = ok);
        Check(hit, "the hit reached the target", "no hit within 2.5s");
        if (!hit) yield break;

        float physical = _stats.GetStatValue(StatType.PhysicalDamage);
        float crit = _stats.GetStatValue(StatType.CritDamage);
        float expected = physical + stage.attackDamage;
        float actual = target.Hits[0].Damage;
        bool normal = Mathf.Abs(actual - expected) < 0.01f;
        bool critical = Mathf.Abs(actual - (expected + crit)) < 0.01f;
        Check(normal || critical,
            $"damage {actual:0.##} = PhysicalDamage {physical:0.##} + stage {stage.attackDamage}{(critical ? $" + crit {crit:0.##}" : "")}",
            $"damage {actual:0.##}, expected {expected:0.##} (or {expected + crit:0.##} on crit)");
        Check(stage.attackDamage > 0, $"stage 0 damage {stage.attackDamage}", "stage 0 attackDamage = 0 — BUG-105 (stage damage lost)");
        yield return WaitIdle(true);
    }

    private IEnumerator CaseMeleeCombo()
    {
        Weapon weapon = _weaponHolder.Weapon;
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
        AttackSO stage = _weaponHolder.Weapon.Stats.GetStage(0);
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
        Check(hit, $"arrow hit, damage {(hit ? target.Hits[0].Damage : 0):0.##}", "no arrow reached the target within 2.5s (BUG-095?)");
        yield return Wait(2f);
        int live = FindObjectsOfType<ProjectileBody>().Length;
        Check(live == 0, "no projectile left alive 2s later", $"{live} projectiles still active 2s later");
        yield return WaitIdle(true);
    }

    private IEnumerator CaseRangedInterval()
    {
        RangeAttackSO stage = _weaponHolder.Weapon.Stats.GetStage(0) as RangeAttackSO;
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
            $"min interval {min:0.###}s < RecoveryTime {stage.RecoveryTime:0.###}s — BUG-103");
        Note($"{shots.Count} shots landed");
        yield return WaitIdle(true);
    }

    private IEnumerator CaseNoWeapon()
    {
        bool attacked = false;
        yield return PlaceTarget(1f);
        _input.MouseDown();
        yield return WatchFor(0.6f, () => _player.StateMachine.CurrentState is PlayerAttackState, v => attacked = v);
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
        _vital.Reduction(VitalStatType.HP, Mathf.Min(MaxHp * 0.3f, Hp - hpNeed - 1f));
        _vital.Reduction(VitalStatType.Mana, Mathf.Min(MaxMana * 0.3f, Mana - manaNeed - 1f));
        yield return null;

        Dictionary<StatType, float> statsBefore = _stats.GetFullStat();
        float hp0 = Hp, mana0 = Mana;
        List<(int frame, float hp, float mana)> vitalLog = new();
        void OnVital() => vitalLog.Add((Time.frameCount, Hp, Mana));
        float cooldown = -1f;
        void OnCooldown(AbilitySlot slot, float duration) { if (slot == binding.Slot) cooldown = duration; }
        _vital.CurrentStatsChanged += OnVital;
        _abilityHolder.AbilityCooldownStarted += OnCooldown;

        bool enteredSkill = false;
        yield return PressAbility(binding, v => enteredSkill = v);
        yield return WaitUntil(() => cooldown >= 0f, 3f, _ => { });
        yield return Wait(2.5f);
        Dictionary<StatType, float> statsDuring = _stats.GetFullStat();

        _vital.CurrentStatsChanged -= OnVital;
        _abilityHolder.AbilityCooldownStarted -= OnCooldown;

        Note($"entered PlayerSkillWeaponState: {enteredSkill}");
        Check(cooldown >= 0f, $"cooldown started ({cooldown:0.##}s)", "no cooldown event — the ability never reached Do");
        if (cooldown >= 0f)
            Check(Mathf.Approximately(cooldown, def.Cooldown), $"cooldown = definition {def.Cooldown:0.##}s", $"cooldown {cooldown:0.##}s, definition {def.Cooldown:0.##}s");

        (float hpCost, float manaCost) = ExpectedCost(def);
        (float hpPaid, float manaPaid) = PaidInFirstFrame(vitalLog, hp0, mana0);
        Check(Mathf.Abs(hpPaid - hpCost) < 0.01f && Mathf.Abs(manaPaid - manaCost) < 0.01f,
            $"cost paid HP {hpPaid:0.#} / Mana {manaPaid:0.#}",
            $"paid HP {hpPaid:0.#} / Mana {manaPaid:0.#}, definition costs HP {hpCost:0.#} / Mana {manaCost:0.#}");

        if (def.Effects == null || def.Effects.Count(e => e != null) == 0)
        {
            Skip("Effects list is empty — not implemented (QA question 3)");
            yield return WaitIdle(true);
            yield break;
        }

        string effects = string.Join(", ", def.Effects.Where(e => e != null).Select(e => $"{e.name} ({e.GetType().Name})"));
        Note($"effects: {effects}");
        bool spawns = def.Effects.Any(e => e is SpawnEffectBase);
        if (spawns)
            Check(target.Hits.Count > 0, $"target hit {target.Hits.Count}x, first {target.Hits[0].Damage:0.##}",
                "spawn effect dealt no damage to a target 2.5 units ahead within 2.5s");

        List<string> statChanges = statsBefore.Where(p => statsDuring.TryGetValue(p.Key, out float v) && !Mathf.Approximately(v, p.Value))
            .Select(p => $"{p.Key} {p.Value:0.##}->{statsDuring[p.Key]:0.##}").ToList();
        bool recovered = vitalLog.Any(v => v.hp > hp0 - hpCost + 0.01f && v.frame != vitalLog[0].frame)
                         || vitalLog.Any(v => v.mana > mana0 - manaCost + 0.01f && v.frame != vitalLog[0].frame);
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
        _abilityHolder.AbilityCooldownStarted += OnCooldown;

        yield return PressAbility(binding, _ => { });
        yield return WaitUntil(() => casts > 0, 3f, _ => { });
        if (casts == 0)
        {
            _abilityHolder.AbilityCooldownStarted -= OnCooldown;
            Skip("first cast never started a cooldown (see D-01)");
            yield break;
        }
        yield return WaitIdle();
        AbilityInstance instance = Instance(binding.Slot);
        float remaining = instance != null ? instance.CooldownRemaining : 0f;
        if (remaining <= 0f)
        {
            _abilityHolder.AbilityCooldownStarted -= OnCooldown;
            Observe($"cooldown {binding.Ability.Cooldown:0.##}s ended before the player was idle again — the cast animation outlasts the cooldown, so the gate cannot be reached by input");
            yield break;
        }

        float hp = Hp, mana = Mana;
        yield return PressAbility(binding, _ => { });
        yield return Wait(0.3f);
        _abilityHolder.AbilityCooldownStarted -= OnCooldown;
        Check(casts == 1 && Mathf.Approximately(hp, Hp) && Mathf.Approximately(mana, Mana),
            $"press with {remaining:0.##}s left: no cast, no cost", $"recast during cooldown: casts={casts}, HP {hp:0.#}->{Hp:0.#}, Mana {mana:0.#}->{Mana:0.#}");
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
        if (hpCost > 0f) _vital.Reduction(VitalStatType.HP, Hp - (hpCost - 1f));
        if (manaCost > 0f) _vital.Reduction(VitalStatType.Mana, Mana - (manaCost - 1f));
        if (hpCost > 0f) Observe($"HP set to {Hp:0.#} below its {hpCost:0.#} cost (QA question 4: should an HP cost be refused?)");
        float hp = Hp, mana = Mana;

        int casts = 0;
        void OnCooldown(AbilitySlot slot, float _) { if (slot == binding.Slot) casts++; }
        _abilityHolder.AbilityCooldownStarted += OnCooldown;
        yield return PressAbility(binding, _ => { });
        yield return Wait(1f);
        _abilityHolder.AbilityCooldownStarted -= OnCooldown;

        Check(casts == 0 && Hp >= hp - 0.01f && Mana >= mana - 0.01f,
            $"refused at HP {hp:0.#} / Mana {mana:0.#}", $"cast went through: casts={casts}, HP {hp:0.#}->{Hp:0.#}, Mana {mana:0.#}->{Mana:0.#}");
        Check(Hp > 0f, "player alive", "HP reached 0 from an ability cost");
        _vital.ReloadComponent();
        yield return WaitIdle(true);
    }

    #endregion

    #region Cases — assets

    private IEnumerator CaseAssetsClean()
    {
        yield return null;
        foreach (KeyValuePair<BaseStatsSO, Dictionary<StatType, float>> pair in _assetSnapshots)
        {
            Dictionary<StatType, float> now = new StatSheet(pair.Key).GetFullStat();
            List<string> changed = pair.Value.Where(p => !now.TryGetValue(p.Key, out float v) || !Mathf.Approximately(v, p.Value))
                .Select(p => $"{p.Key} {p.Value:0.##}->{(now.TryGetValue(p.Key, out float v) ? v.ToString("0.##") : "missing")}").ToList();
            Check(changed.Count == 0, $"{pair.Key.name} unchanged", $"{pair.Key.name} changed: {string.Join(", ", changed)}");
        }
    }

    #endregion

    #region Helpers — player

    private void BindPlayer(Player player)
    {
        _player = player;
        _handler = player.GetComponentInChildren<PlayerInputHandler>(true);
        _weaponHolder = player.GetComponentInChildren<WeaponHolder>(true);
        _abilityHolder = player.GetComponentInChildren<AbilityHolder>(true);
        _stats = player.GetComponentInChildren<StatHandler>(true);
        _vital = player.GetComponentInChildren<VitalStatsComponent>(true);
        _input.Bind(_handler);
    }

    private float Hp => _vital.GetCurrentStatValue(VitalStatType.HP);
    private float Mana => _vital.GetCurrentStatValue(VitalStatType.Mana);
    private float MaxHp => _stats.GetStatValue(StatType.HP);
    private float MaxMana => _stats.GetStatValue(StatType.Mana);

    private bool IsIdle => _player.StateMachine.CurrentState is PlayerIdleState || _player.StateMachine.CurrentState is PlayerMoveState;

    // strict: called at the end of a case, where a player still locked in a state is that case's defect.
    private IEnumerator WaitIdle(bool strict = false)
    {
        bool idle = false;
        yield return WaitUntil(() => IsIdle, 4f, ok => idle = ok);
        if (idle) yield break;
        string message = $"player stuck in {_player.StateMachine.CurrentState?.GetType().Name} for 4s after the action";
        if (strict) Fail(message);
        else Note($"(carried over) {message}");
    }

    private IEnumerator PlaceTarget(float distance)
    {
        yield return WaitIdle();
        target.Clear();
        Vector2 origin = _player.transform.position;
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
        _vital.ReloadComponent();
        target.Clear();
    }

    private IEnumerator PressAbility(AbilityBinding binding, Action<bool> enteredSkill)
    {
        Key key = SlotKeys[(int)binding.Slot];
        bool entered = false;
        _input.KeyDown(key);
        float hold = binding.Ability.ActivationType == AbilityActivationType.Hold ? 0.5f : 0.05f;
        yield return WatchFor(hold, () => _player.StateMachine.CurrentState is PlayerSkillWeaponState, v => entered |= v);
        _input.KeyUp();
        yield return WatchFor(0.3f, () => _player.StateMachine.CurrentState is PlayerSkillWeaponState, v => entered |= v);
        enteredSkill(entered);
    }

    private AbilityInstance Instance(AbilitySlot slot)
    {
        FieldInfo field = typeof(AbilityHolderBase<Core>).GetField("_equipped", BindingFlags.Instance | BindingFlags.NonPublic);
        var equipped = field?.GetValue(_abilityHolder) as Dictionary<AbilitySlot, AbilityInstance>;
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

    private static string Describe(RuntimeAnimatorController controller) => controller != null ? controller.name : "null";

    #endregion

    #region Helpers — waiting

    private static IEnumerator Frames(int count)
    {
        for (int i = 0; i < count; i++) yield return null;
    }

    private static IEnumerator Wait(float seconds)
    {
        float end = Time.time + seconds;
        while (Time.time < end) yield return null;
    }

    private static IEnumerator WaitUntil(Func<bool> condition, float timeout, Action<bool> result)
    {
        float end = Time.time + timeout;
        while (!condition())
        {
            if (Time.time >= end)
            {
                result(false);
                yield break;
            }
            yield return null;
        }
        result(true);
    }

    // Polls for the whole window and reports whether the condition was ever true.
    private static IEnumerator WatchFor(float seconds, Func<bool> condition, Action<bool> seen)
    {
        float end = Time.time + seconds;
        bool any = false;
        while (Time.time < end)
        {
            any |= condition();
            yield return null;
        }
        seen(any);
    }

    private IEnumerator Tap(Key key)
    {
        _input.KeyDown(key);
        yield return Frames(2);
        _input.KeyUp();
    }

    #endregion

    #region Case runner and report

    private IEnumerator RunCase(string id, string title, IEnumerator body)
    {
        _current = new CaseResult { Id = id, Title = title };
        _results.Add(_current);
        if (_player == null && id != "A-01")
        {
            Fail("no player — A-01 failed");
            _current = null;
            yield break;
        }

        float end = Time.realtimeSinceStartup + caseTimeout;
        float? pausedSince = null;
        Stack<IEnumerator> stack = new();
        stack.Push(body);
        while (stack.Count > 0)
        {
            if (Time.realtimeSinceStartup > end)
            {
                Fail($"timed out after {caseTimeout:0}s (real time, Time.timeScale = {Time.timeScale})");
                break;
            }
            // A panel left open pauses the game; every wait in the probe runs on game time and would hang.
            if (Time.timeScale == 0f)
            {
                pausedSince ??= Time.realtimeSinceStartup;
                if (Time.realtimeSinceStartup - pausedSince > 2f)
                {
                    Fail("game paused (Time.timeScale = 0) for 2s during the case — a UI panel left open; timeScale forced back to 1 so the run can continue");
                    Time.timeScale = 1f;
                    pausedSince = null;
                }
            }
            else pausedSince = null;
            IEnumerator top = stack.Peek();
            bool moved;
            try
            {
                moved = top.MoveNext();
            }
            catch (Exception e)
            {
                IEnumerable<string> frames = (e.StackTrace ?? "").Split('\n').Select(l => l.Trim()).Where(l => l.Length > 0).Take(4);
                Fail($"exception during the case {e.GetType().Name}: {e.Message} @ {string.Join(" <- ", frames)}");
                break;
            }
            if (!moved)
            {
                stack.Pop();
                continue;
            }
            if (top.Current is IEnumerator nested)
            {
                stack.Push(nested);
                continue;
            }
            yield return null;
        }

        _input.KeyUp();
        _input.MouseUp();
        Debug.Log($"[QA] {_current.Verdict} {_current.Id} — {_current.Title}");
        _current = null;
    }

    private void Check(bool condition, string pass, string fail)
    {
        if (condition) Note("ok: " + pass);
        else Fail(fail);
    }

    private void Fail(string message)
    {
        if (_current == null) return;
        _current.Verdict = Verdict.FAIL;
        _current.Notes.Add("FAIL: " + message);
    }

    private void Skip(string message)
    {
        if (_current == null) return;
        if (_current.Verdict == Verdict.PASS || _current.Verdict == Verdict.OBSERVED) _current.Verdict = Verdict.SKIP;
        _current.Notes.Add("SKIP: " + message);
    }

    private void Observe(string message)
    {
        if (_current == null) return;
        if (_current.Verdict == Verdict.PASS) _current.Verdict = Verdict.OBSERVED;
        _current.Notes.Add("OBSERVED: " + message);
    }

    private void Note(string message) => _current?.Notes.Add(message);

    private void OnLog(string condition, string stackTrace, LogType type)
    {
        if (type != LogType.Error && type != LogType.Exception && type != LogType.Assert) return;
        if (condition.StartsWith("[QA]")) return;
        string where = stackTrace?.Split('\n').FirstOrDefault(l => !string.IsNullOrWhiteSpace(l))?.Trim();
        string message = $"{type}: {condition.Split('\n')[0]}{(string.IsNullOrEmpty(where) ? "" : $" @ {where}")}";
        if (_current != null) Fail(message);
        else
        {
            CaseResult orphan = _results.LastOrDefault(r => r.Id == "LOG") ?? AddLogCase();
            orphan.Verdict = Verdict.FAIL;
            orphan.Notes.Add(message);
        }
    }

    private CaseResult AddLogCase()
    {
        CaseResult result = new() { Id = "LOG", Title = "Errors logged outside a case" };
        _results.Add(result);
        return result;
    }

    private void OnConfirmPanel(ConfirnData data)
    {
        _confirmPanelOpened++;
        _lastConfirnData = data;
    }

    private void OnOpenDoor(object _) => _doorEvents++;

    private string ReportPath()
    {
        string root = System.IO.Path.GetFullPath(System.IO.Path.Combine(Application.dataPath, ".."));
        string dir = System.IO.Path.Combine(root, "production", "qa", "evidence", "auto");
        Directory.CreateDirectory(dir);
        return System.IO.Path.Combine(dir, $"player-flow-{DateTime.Now:yyyyMMdd-HHmmss}.md");
    }

    private void WriteReport()
    {
        int Count(Verdict v) => _results.Count(r => r.Verdict == v);
        StringBuilder sb = new();
        sb.AppendLine("# Automated QA — Player Flow");
        sb.AppendLine();
        sb.AppendLine($"- Run: {DateTime.Now:yyyy-MM-dd HH:mm:ss}, Unity {Application.unityVersion}, scene `{gameObject.scene.path}`");
        sb.AppendLine($"- Probe: `Assets/Script/FastTest/QAProbe/QAPlayerFlowProbe.cs` — evidence tier RUNTIME (Play Mode, simulated input)");
        sb.AppendLine($"- Totals: PASS {Count(Verdict.PASS)} · FAIL {Count(Verdict.FAIL)} · SKIP {Count(Verdict.SKIP)} · OBSERVED {Count(Verdict.OBSERVED)}");
        sb.AppendLine("- OBSERVED = behaviour recorded where the design is undecided; SKIP = not testable as built (reason given).");
        sb.AppendLine();
        sb.AppendLine("| ID | Case | Result | Details |");
        sb.AppendLine("|---|---|---|---|");
        foreach (CaseResult r in _results)
            sb.AppendLine($"| {r.Id} | {Escape(r.Title)} | **{r.Verdict}** | {Escape(string.Join("<br>", r.Notes))} |");
        sb.AppendLine();
        sb.AppendLine("Manual (visual / feel) cases are not run here: see `production/qa/test-cases/s20-player-flow-manual-pending.md`.");

        string path = ReportPath();
        File.WriteAllText(path, sb.ToString());
        Debug.Log($"[QA] SUMMARY pass={Count(Verdict.PASS)} fail={Count(Verdict.FAIL)} skip={Count(Verdict.SKIP)} observed={Count(Verdict.OBSERVED)} report={path}");
    }

    private static string Escape(string text) => text.Replace("|", "\\|").Replace("\n", " ");

    private void Finish()
    {
#if UNITY_EDITOR
        int fails = _results.Count(r => r.Verdict == Verdict.FAIL);
        UnityEditor.SessionState.SetInt(ExitCodeKey, fails > 0 ? 1 : 0);
        if (UnityEditor.SessionState.GetBool(CliFlagKey, false)) UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    #endregion
}
