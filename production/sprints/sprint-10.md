# Sprint 10 — 2026-08-17 to 2026-08-21

**Opened:** 2026-08-16 (Sunday 22:00 `pm-weekly-kickoff`, on-slot autonomous run — no owner present).
Branch `sprint-10`, created from `sprint-09` tip (`7eb3378`, "chore(wrapup): weekly wrap-up 2026-08-15"),
after `git fetch origin sprint-09` confirmed the local ref matched `origin/sprint-09` exactly (process
fix adopted from `retro-sprint-09-2026-08-15.md`'s Process Improvements — never branch off an unfetched
local ref). `gh` CLI unavailable in this environment (`gh: command not found`) — draft PR
(`--base sprint-09 --head sprint-10`, title `Sprint 10`) was **not** auto-created; run manually if desired:
`gh pr create --draft --base sprint-09 --head sprint-10 --title "Sprint 10"`

**Review mode:** lean (from `production/review-mode.txt`) — producer feasibility gate (PR-SPRINT)
skipped per lean-mode rule.

---

## ⚠️ New finding this kickoff — uncommitted WIP on `origin/feature/fix-player-control`

Before this kickoff ran, the working directory was on a **different, non-sprint branch**
(`origin/feature/fix-player-control`) with 13+ uncommitted/modified files: `WeaponHolder.cs`,
`Player.cs`, `PlayerState.cs`, `PlayerAttackState.cs`, `MeleeWeapon.cs`, `RangeWeapon.cs`, `Weapon.cs`,
3 `Knight_ComboAttack` animation clips, `Assets/Scenes/Main/Test/LoadRandomMap.unity`,
`PlayerInputHandle.cs`, `PlayerInput.inputactions`, plus `StatSystem` changes (`Stat.cs`, `StatsSO.cs`,
`StatModifierTester.cs`, deleted `StatModifierGroupSO.cs` replaced by new untracked
`StatModifierGroup.cs`) and new untracked `Assets/Script/UI/` + `Assets/UI Toolkit/` content.

This touches the same attack/combo-flow files as the weapon-architecture refactor that closed BUG-041
on `sprint-09` this week (per `bug-triage-2026-08-15.md`) — it is very likely **further, newer**
attack/combo work than what already landed, sitting on an untracked branch outside any sprint. **Nothing
was lost**: this kickoff ran `git stash push -u` before switching branches
(`pre-kickoff-sprint-10: WIP on origin/feature/fix-player-control ...`), so the WIP is fully recoverable
via `git stash list` / `git stash show -p`. It was **not** applied to `sprint-10` — reconciling
unreviewed WIP onto a fresh sprint branch without owner review would repeat exactly the pattern S9-00
exists to prevent. See **S10-11** below.

---

## Sprint Goal

**Close the enemy-side combat chain (the sprint's sole remaining true blocker, 4th carry with zero
movement) and get the first-ever Play Mode confirmation of both attack directions.** Sprint 9 broke a
two-sprint zero-Must-Have streak by landing BUG-041 (player deals damage) for real, via a weapon
architecture refactor — but BUG-042/BUG-053/BUG-054 (enemy `TakeDamage()` chain) received zero code
movement across all 5 of Sprint 9's scheduled days, and S9-12 (Play Mode verification) has now gone
unreached for 3 consecutive sprints (S7-08, S8-12, S9-12). This sprint opens with that item as the
literal first task, per `retro-sprint-09-2026-08-15.md` Action Item #1.

---

## Capacity

- Total days: 5
- Buffer (20%): 1 day
- Available: 4 days

Must-Have load ≈ 1.0d — held at the same narrow scope as Sprint 9 (1.05d), which was the first cycle in
three to land real code. Widening scope now, before the enemy-side chain actually lands, would repeat
Sprint 8's overcommitment pattern.

---

## Carryover from Previous Sprint

Full detail: `production/sprints/sprint-09.md` (Status/closure section),
`production/retros/retro-sprint-09-2026-08-15.md`, `production/qa/bug-triage-2026-08-15.md`.

| Task | Reason | New Estimate |
|------|--------|-------------|
| BUG-042 / BUG-053 / BUG-054 (`EntityCore.TakeDamage()` chain) | 4th carry, zero movement all 5 of Sprint 9's scheduled days | 0.3d — see S10-01 |
| S9-00 → S10-02 process gate | 5th carry — still not drafted; Sprint 9's real (safe) feature-branch merge is the concrete example to cite | 0.15d |
| S9-12 → S10-03 Play Mode verify gate | 3rd carry as this exact item (S7-08, S8-12, S9-12) — no Unity CLI in this automated environment | 0.2d |
| BUG-033 (`EnemySpawner.cs:62` null-guard order) | 8th carry, one-line fix, session-availability pattern | 0.1d |
| BUG-044 (`PlayerDeathState.LogicUpdate()` orphaned) | 5th carry | 0.15d |
| S4-05/S4-06 keep-or-cut decision | 11th carry — oldest unresolved item in the project, decision-avoidance not estimation | 0.1d |
| BUG-043 (divergent enemy attack paths) | 5th carry, depends on BUG-042 | 0.3d — Should-Have |
| Bug #6 / S7-11 (player HP write-through) | 11th carry, deliberately Should-Have again per Sprint 9's own reasoning | 0.4d |
| ADR-0002 (`EnemyManager` singleton) Accept | 8th carry | 0.1d |
| QA plan | 10th consecutive cycle missing | — see QA Plan Gate below |
| Individual `production/qa/bugs/BUG-NNN.md` files | 5th+ cycle recommended (S7-D3/S8-D1/S9-D1), still only 2 files exist | 0.2d |
| First playtest | 11th cycle attempt, gated on S10-01 + S10-03 | — |

---

## Tasks

### Must Have (Critical Path)

| ID | Task | Agent/Owner | Est. Days | Dependencies | Acceptance Criteria |
|----|------|-------------|-----------|--------------|---------------------|
| S10-01 | Fix BUG-042 + BUG-053 + BUG-054 together, bundled per Sprint 9's own scoping — implement `EntityCore.TakeDamage()` for real (health decrement + death-state hookup); **delete** `EntityNegativeReciver.cs` (the duplicate, wrong-hub receiver with the never-initialized `currentHealth`) rather than patching it | ai-programmer | 0.3 | None | Player hit on enemy in Play Mode: no `NotImplementedException`; health decrements; exactly one `INegativeReceiver` implementer exists on the enemy prefab; no `ON_PLAYER_DEATH` emitted from enemy code |
| S10-02 | Land S9-00 as a minimal **pre-push compile check** (not a written-policy-only version, per retro Action Item #3) — a hook or equivalent script that gates merges onto `sprint-10`, so a safe merge (like this week's weapon-architecture refactor) can be told apart from a risky one (like Sprint 8's BUG-053-introducing merge) without manual re-review every cycle | producer / owner (Kay) | 0.15 | None | Either a working pre-push hook exists in the repo, or an equivalent compile-check script is committed and documented as required before merging into `sprint-10`. Owner sign-off required to adopt as binding. |
| S10-03 | **Verify** — Play Mode smoke check covering three things per `bug-triage-2026-08-15.md`'s Recommended Actions: (a) player melee damage actually lands (BUG-041's static fix, still unconfirmed live), (b) the enemy prefab's `holder`/`entityInput` wiring is non-null (BUG-032's remaining uncertainty), (c) the `statusAnimation` buffer-gate behaves correctly through a full combo | owner (Kay) / lead-programmer | 0.2 | S10-01 | Owner confirms in-Editor; do not treat S10-01/S9-01/BUG-032 as done without this — 3 consecutive sprints (7, 8, 9) have closed without any Play Mode confirmation |
| S10-04 | Fix BUG-033, 8th carry — `EnemySpawner.cs:62` swap to `set == null \|\| set.Count == 0` | gameplay-programmer | 0.1 | None | Empty-pool room spawns without `NullReferenceException` |
| S10-05 | Fix BUG-044, 5th carry — restore `PlayerDeathState.LogicUpdate()` body | gameplay-programmer | 0.15 | None | `PlayerDeathState` emits on death; no commented-out logic left |
| S10-06 | **Forced decision** — S4-05/S4-06 keep-or-cut, 11th carry, zero movement across every prior cycle | owner (Kay) | 0.1 | None | Written decision recorded in this file or a linked doc — make the call, do not carry a 12th time |

Must-Have total ≈ **1.0d** (0.3+0.15+0.2+0.1+0.15+0.1) — same narrow load as Sprint 9, which was the
first cycle in three to land real code against this kind of scope.

### Should Have

| ID | Task | Agent/Owner | Est. Days | Dependencies | Acceptance Criteria |
|----|------|-------------|-----------|--------------|---------------------|
| S10-07 | Fix BUG-043 — consolidate `EntityAttack.cs` and `EntityWeaponMelee.cs` into one enemy damage path | ai-programmer | 0.3 | S10-01 | One clear enemy-attack code path; cooldown gate functionally gates repeat attacks |
| S10-08 | Bug #6 / S7-11, 11th carry — write-through `NegativeReciver`→`PlayerData.currentHealth`, confirm `ON_PLAYER_DEATH` listener fires, EditMode test `TakeDamage_BelowZero_TriggersDeathState` | gameplay-programmer | 0.4 | None | Test passes; single HP source of truth; `Reborn()` contract intact — do not close without the test |
| S10-09 | Flip ADR-0002 (`EnemyManager` singleton) `Proposed → Accepted`, 8th carry, no excuse remains | producer | 0.1 | None | ADR-0002 Status reads Accepted; sign-off note recorded |
| S10-10 | File individual `production/qa/bugs/BUG-NNN.md` reports for the 9-item P1 table from `bug-triage-2026-08-15.md` | qa-lead | 0.2 | None | All 9 P1 items have individual files |
| S10-11 | **Reconcile the stashed `origin/feature/fix-player-control` WIP** — review the 13+ uncommitted files (attack/combo-flow + StatSystem work, see finding above), determine what is newer than what already landed via `sprint-09`'s weapon-architecture merge, and either cherry-pick reviewed pieces onto `sprint-10` or explicitly decide to discard/superseded them | owner (Kay) / lead-programmer | 0.3 | S10-01, S10-02 | Written decision recorded on each of the 13+ files: merged onto `sprint-10`, superseded/discarded, or deferred with reason; gated on S10-02 landing first so this reconciliation itself goes through the new process gate rather than repeating the pattern it exists to prevent |
| S10-12 | Fix BUG-046 — `EntityWeaponMelee.cs:29` still uses allocating `Physics2D.OverlapCircle`, not `OverlapCircleNonAlloc`, unlike the player-side melee weapon after this week's refactor | gameplay-programmer | 0.15 | None | `EntityWeaponMelee.Attack()` uses `OverlapCircleNonAlloc` against a cached buffer, matching `engine-code.md`'s zero-alloc rule |

### Nice to Have

| ID | Task | Agent/Owner | Est. Days | Dependencies | Acceptance Criteria |
|----|------|-------------|-----------|--------------|---------------------|
| S10-N1 | Fix Bug #14 — add `return` after `Destroy(gameObject)` in `MazeController.Awake()` | lead-programmer | 0.1 | None | Duplicate `MazeController` instance no longer overwrites `Instance` |
| S10-N2 | Fix Bug #15 — replace `File.ReadAllText(Application.dataPath...)` with `TextAsset` refs or StreamingAssets | lead-programmer | 0.5 | None | Room load works from a Player build |
| S10-N3 | First full playtest session — once S10-01/S10-03 land | producer / owner | — | S10-01, S10-03 | `/playtest-report` filed — 11th cycle attempt |
| S10-N4 | Re-verify BUG-032 prefab wiring explicitly (folded into S10-03, but track separately if S10-03 only partially covers it) | ai-programmer | 0.1 | S10-03 | Enemy prefab `EntityWeaponHolder` field confirmed wired in Inspector, not just code-guarded |

---

## Risks

| Risk | Probability | Impact | Mitigation |
|------|------------|--------|------------|
| S10-01 (BUG-042/053/054) receives zero movement a 5th consecutive cycle | High — 4 consecutive cycles at zero so far | High | Retro explicitly recommends this open the sprint as the literal first task; no other Must-Have item is scheduled ahead of it |
| No owner-in-Editor session happens, so S10-03 (Play Mode verify) goes unreached a 4th consecutive sprint | Medium-High — pattern across S7-08/S8-12/S9-12 | High | Flagged as Critical/owner-only in the task table; BUG-041's otherwise-solid fix stays unconfirmed until this runs |
| S10-02 (process gate) stays a written-policy-only version instead of an enforced hook | Medium | Medium | Retro Action Item #3 explicitly asks for the enforced version this time, citing Sprint 9's own safe-merge-with-no-way-to-verify-in-advance as the concrete case |
| Reconciling the `origin/feature/fix-player-control` WIP (S10-11) reintroduces unreviewed off-branch work before S10-02's gate exists | Medium | Medium | S10-11 explicitly depends on S10-02 landing first |
| No QA plan — 10th consecutive cycle | Confirmed | Medium | Flagged explicitly below; deferred to owner rather than silently dropped |
| No Unity CLI / no `gh` CLI in this environment | Known constraint | Low | All Play Mode smoke checks (S10-03) are manual in-Editor by the owner; PR creation noted as a manual follow-up |

---

## Dependencies on External Factors

- No Unity CLI — Play Mode smoke checks require manual in-Editor confirmation by the owner (S10-03).
- `gh` CLI unavailable — draft PR for `sprint-10` (base `sprint-09`) not auto-created; run manually:
  `gh pr create --draft --base sprint-09 --head sprint-10 --title "Sprint 10"`.

---

## Definition of Done for This Sprint

- [ ] S10-01: Enemy `TakeDamage()` no longer throws; single `INegativeReceiver` implementer on enemy prefab; `EntityNegativeReciver.cs` deleted (BUG-042 + BUG-053 + BUG-054)
- [ ] S10-02: Process gate landed as an enforced check, not just a written note
- [ ] S10-03: Play Mode gate actually confirmed by the owner (both attack directions + statusAnimation buffer-gate)
- [ ] S10-04 / S10-05: BUG-033, BUG-044 confirmed fixed
- [ ] S10-06: S4-05/S4-06 decision recorded
- [ ] QA Plan gate resolved (see below)

Everything else (BUG-043, Bug #6, ADR-0002, WIP reconciliation, BUG-046) is Should-Have — carried but not
blocking sprint close, consistent with Sprint 9's Definition of Done scoping.

---

## QA Plan

⚠️ **No QA plan exists** for Sprint 10 (`production/qa/qa-plan-sprint-10.md` not found) — **10th
consecutive sprint cycle** without one. This kickoff ran autonomously (no owner present); per the QA
plan gate, the choice requiring judgment (full plan now vs. defer) is deferred to the owner rather than
decided unattended, consistent with every prior cycle's handling of this same gate.

> ⚠️ This sprint was started without a QA plan. Run `/qa-plan sprint` before the last story is
> implemented. The Production → Polish gate requires a QA sign-off report, which requires a QA plan.
> Given S10-03 is an explicit Play Mode verification gate and S10-08 is EditMode-test-gated, a QA plan
> run early would meaningfully de-risk sign-off later — this recommendation has now repeated 10 times.

---

## Next Sprint Outlook (Sprint 11)

- If Must-Have closes clean: BUG-043 consolidation (if not done as Should-Have here), Bug #6 (if not
  done here), the enemy-death → room-clear condition (`ON_CLEAR_ENEMY`/`ON_ROOM_CLEAR`), `EnemyManager`
  lifecycle body (still a scaffold stub per `CLAUDE.md`), first playtest (if not already run).
- Bug #14/#15 and QA-plan process items, if not completed as Should-Have here.
- HUD health bar + between-room upgrade cards, once the death chain (both directions) is confirmed stable.
