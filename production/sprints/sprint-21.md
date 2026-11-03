# Sprint 21 — 2026-11-02 to 2026-11-06

**Status: OPEN.** Opened 2026-11-01 (`pm-weekly-kickoff`, autonomous run — no owner present).
Branch `sprint-21`, created on the remote from the `sprint-20` tip (`ed2f984b`, "chore(wrapup):
duplicate firing skipped 2026-11-01"). `gh` CLI unavailable (`gh: program not found`) — draft PR
**not** auto-created; run manually:

```bash
gh pr create --draft --base sprint-20 --head sprint-21 --title "Sprint 21"
```

**Review mode:** lean — producer feasibility gate skipped.
**Tracker:** `sprint-21-daily-plan.md`.
**Inputs:** `sprint-20.md`, `sprint-20-daily-plan.md` (Handoff to Sunday kickoff),
`production/qa/bug-triage-2026-10-31.md`, `production/qa/bug-inbox.md`, `production/qa/bugs/BUG-116…121.md`,
`production/session-state/routine-log.md` (QA automation rows 2026-11-01), `git log --all` since
2026-10-26. No milestone file and no risk register exist — risks are tracked here.

---

## Sprint 20 Closure (recorded by this kickoff)

- **Verdict:** SLIPPED (2026-10-31 wrap-up, evidence STATIC + LOG). Must-Have 0 / 1.3d.
- **Planned velocity:** 0.5d / 4d = **0.13** (S20-F1 only). Sprint 16 ≈0.01, 17 0.05, 18 0.0,
  19 0.01, 20 0.13.
- **Off-plan owner output:** ≈1.5d (effect-component follow-ups `6e484f0c` / `343a3830`, CombatTest
  harness `87e053b4` / `fc98fa2a` / `a787f234`, merge `2276049e`).
- **Playtest done:** no — no sheet for 2026-10-31 (16 unrecorded Play Mode entries in `Editor.log`).
  S20-01 carried for the 22nd time.
- **New confirmed bugs (2026-10-31 triage):** BUG-116 (S1), BUG-117 / 120 / 121 (S2), BUG-118 / 119 (S3).
- **After the wrap-up (2026-11-01):** a weekly QA automation harness (`qa/weekly-automation`,
  routine `qa-weekly-automation` Fri 23:00) ran automated Play Mode: run `20261101-0221` — 84 cases,
  53 PASS / 3 FAIL / 12 KNOWN / 16 SKIP. The 3 FAILs are open inbox notes **NOTE-20261101-1/2**
  (S1 guess: Paladin death never emits `ON_PLAYER_DEATH`; enemy at HP 0 never dies) and
  **NOTE-20261101-3** (S2 guess: Avatar of Light never reaches `Do`). They are **not tasks** until
  Saturday triage, but they are the first `RUNTIME` evidence in 17+ weeks.

---

## Sprint Goal

**A recorded playtest, a Player build that can compile, and a dungeon run that can be cleared.**
Playtest first on Monday (23rd carry — fill the sheet, or sign an accepted-risk note), then the new
S1 / P1 pair (BUG-116, BUG-121 — both one-liners), then the carried S1 dungeon blockers (BUG-108,
BUG-096) and the weapon trio.

---

## Capacity

- Total days: 5
- Buffer (20%): 1 day
- Available: 4 days

Planned-work velocity has been ≤0.13 for five sprints. Planned load stays at ≈1.45d Must and ≈0.95d
Should; owner feature work is an explicit story capped at 0.5d, gated behind S21-01 and S21-03.

0.1d is reserved on **Saturday 2026-11-07** for the weekly playtest (`tests/smoke/critical-paths.md`
+ the `playtest-sheet.sh` sheet). The Friday 23:00 QA automation run is an input to it, not a
substitute: the sheet covers the menu → dungeon flow the automation SKIPs.

---

## Carryover from Previous Sprint

| Task | Times carried | Reason | New Estimate |
|------|---------------|--------|--------------|
| Playtest / Play Mode smoke (S20-01) | **23rd** | Owner plays, never records | 0.3d — Mon, first (S21-01) |
| BUG-108 (S20-02) | 2nd | Partly fixed in code (`statsSO` attribute restored); `abilityBindings` / `Weapon` keys still unmapped | 0.1d (S21-04) |
| BUG-096 (S20-03) | 5th | Not started | 0.1d (S21-05) |
| Weapon trio BUG-106/107/110 (S20-04) | 2nd | Not started | 0.3d (S21-06) |
| BUG-097 (S20-05) | 5th | Not started | 0.1d (S21-07) |
| Editor / data block (S20-06) | 5th | Not started | 0.15d (S21-08) |
| Process gate decision (S20-07) | **11th** (TD-048 31st+) | Not started; third serialized-rename loss this month | 0.1d (S21-09) |
| BUG-065 + BUG-086 (S20-08) | 12th / 7th | Not started | 0.2d (Should, S21-11) |
| BUG-115 (S20-10) | 2nd | Not started | 0.3d (Should, S21-12) |
| One-liners (S20-11) | 3rd–6th | Not started | 0.15d (Should, S21-13) |
| BUG-092 + BUG-066/070 verify (S20-09) | 7th / 12th / 10th | Overlaps the new effect bugs | Deferred (S21-D1) |
| Missing-script assets (S20-12) | 2nd (BUG-073 8th) | Not started | Deferred (S21-D2) |
| S20-D1…D7 | — | Unchanged | Kept deferred (S21-D3…D7) |

---

## Tasks

### Must Have (Critical Path)

| ID | Task | Agent/Owner | Est. Days | Dependencies | Acceptance Criteria |
|----|------|-------------|-----------|--------------|---------------------|
| S21-01 | **Weekly playtest / Play Mode smoke — first task Monday (23rd carry).** Run `tests/smoke/critical-paths.md` through the menu (`MainGamePlay` → New Game → `LoadRandomMap` + `GameplayUI`); fill a sheet in `production/qa/playtests/` incl. section E. Use the QA run `20261101-0221` report for rows it already covers | owner (Kay) | 0.3 | None | Sheet filled with PASS/FAIL/SKIP per row, **or** a signed accepted-risk note in this file. No feature commit before this |
| S21-02 | **BUG-116 (S1, P1)** — remove `UnityEditor.Animations` / `AnimatorController` from runtime `CharacterData` (use `RuntimeAnimatorController`) | gameplay-programmer | 0.1 | None | No `using UnityEditor` in `Assets/Script/` runtime code; Player build compiles |
| S21-03 | **BUG-121 (S2, P1)** — `[FormerlySerializedAs]` for the `authoredModifiers` rename, **before** any stat / weapon / ability asset is re-saved | gameplay-programmer | 0.1 | None | Paladin weapon bonus and Blessed Slash data load non-empty |
| S21-04 | **BUG-108 (S1)** — finish: `[FormerlySerializedAs]` for the old `abilityBindings` / `weaponSO` keys on `CharacterData`; verify Knight enemies in Play Mode | gameplay-programmer + owner | 0.1 | S21-03 (same pattern) | Knight enemies have stats, take damage and attack |
| S21-05 | **BUG-096 (S1)** — zero-spawn branch in `RoomGeneraterController.LoadRoom()` so Rest / Shop / Buff / Boss rooms open | gameplay-programmer | 0.1 | None | Every non-combat room opens on load |
| S21-06 | **Weapon trio, one session** — BUG-106, BUG-107 item 2, BUG-110 in `WeaponHolderBase` | gameplay-programmer | 0.3 | S21-04 | Enemies attack; Mage confirm does not throw; picked-up weapon is the one that attacks |
| S21-07 | **BUG-097 (S2)** — start / boss order (closes Bug #16) | owner (Kay) | 0.1 | None | Run starts in `StartRoom_Entrance`, ends in `BossRoom_ThroneArena` |
| S21-08 | **Editor / data block** — BUG-105 `[FormerlySerializedAs("attackDamege")]`; BUG-095 `Collider2D` on `Arrow.prefab`; BUG-072 `layerMask` check | owner (Kay) | 0.15 | None | Visible in `git diff`; sword hit deals 55 + PhysicalDamage |
| S21-09 | **Process gate decision (11th carry)** — pick one: Editor compile + menu → dungeon load before merging to `sprint-NN`; TD-048 pre-push compile hook; a serialized-rename check (BUG-105 / 108 / 121); or an explicit signed drop | producer / owner | 0.1 | None | Decision recorded in this file |
| — | Saturday 2026-11-07 weekly playtest | owner (Kay) | 0.1 | — | Sheet filled before `/weekly-wrapup` |

Must-Have total = **1.45d**.

### Should Have

| ID | Task | Agent/Owner | Est. Days | Dependencies | Acceptance Criteria |
|----|------|-------------|-----------|--------------|---------------------|
| S21-10 | **BUG-117 + BUG-120 (S2, P2)** — vital modifiers serializable with the correct sign; item stat effects apply | gameplay-programmer | 0.3 | S21-03 | Heal / damage effect changes HP the right way; a stat item changes the stat |
| S21-11 | BUG-065 + BUG-086 — stop movement on death; emit `ON_PLAYER_DEATH` once | gameplay-programmer | 0.2 | S21-01 | No slide; event once; `RealPlayerDataProvider.Respawn()` still works |
| S21-12 | BUG-115 (S2) — runtime copy of `BaseStatsSO`; revert leaked modifiers in the player stat assets | gameplay-programmer + owner | 0.3 | S21-03 | Play Mode leaves `Assets/SO/Stat/*.asset` clean in `git status` |
| S21-13 | One-liners — BUG-104, BUG-103, BUG-102, BUG-068 `:28` | gameplay / ui-programmer | 0.15 | None | Each visible in `git diff` |

Should-Have total = **0.95d**. Must + Should = 2.4d of 4.0d.

### Feature story (owner, capped)

| ID | Task | Agent/Owner | Est. Days | Dependencies | Acceptance Criteria |
|----|------|-------------|-----------|--------------|---------------------|
| S21-F1 | Effect-source work (`SourceEffect` / `VitalModifier`, plan `ef0d9c6f`) — land with one Editor compile | owner (Kay) | **0.5 cap** | **S21-01**, **S21-03** | Merged, compiling (`compile-check.sh` or `Editor.log` 0 `error CS`) |

### Nice to Have / Deferred

| ID | Task | Agent/Owner | Est. Days | Note |
|----|------|-------------|-----------|------|
| S21-D1 | BUG-092 + BUG-066/070 verify, BUG-118, BUG-119, BUG-113, BUG-114, BUG-100, BUG-071 | gameplay-programmer | 0.9 | Effect / vitals code still moving (S21-F1) |
| S21-D2 | Missing-script assets BUG-073 / 090 / 112 | owner (Editor) | 0.2 | Carried from S20-12 |
| S21-D3 | BUG-087 design note — `GameManager` + player `Reborn()` | game-designer / producer | 0.3 | 6th carry, demoted |
| S21-D4 | BUG-109 Rain Arrows `_context` / `Destroy` on pooled object | gameplay-programmer | 0.15 | — |
| S21-D5 | BUG-111 `RegenComponent` defects | gameplay-programmer | 0.15 | Dormant |
| S21-D6 | Git LFS for `.fbx`, TD-040 ADR, BUG-084 decision, BUG-083 | technical-director | 0.9 | Carried since S19 |
| S21-D7 | Weapon architecture review (`b364c303`) | lead-programmer | 0.2 | Carried since S19 |

---

## Risks

| Risk | Probability | Impact | Mitigation |
|------|------------|--------|------------|
| Playtest slips a 23rd time | High | High | S21-01 first Monday; the QA automation report shortens it. A third skipped Saturday = owner decision (play or sign accepted risk) |
| NOTE-20261101-1/2 (S1 guess, RUNTIME 4 of 4 runs) are real: player death and enemy death broken | High | High | Not tasks until Saturday triage; S21-11 touches the same death path — read the notes before starting it |
| Off-plan feature work outruns Must (11th time) | High | Medium | S21-F1 capped, gated on S21-01 + S21-03 |
| Another serialized-rename loss (3 this month) | Medium | High | S21-03 first, S21-09 rename check candidate |
| BUG-115 keeps dirtying committed stat assets | High | Medium | Check `git status` for `Assets/SO/Stat/*.asset` after every Play Mode session until S21-12 lands |
| No `gh` CLI | Known | Low | Manual PR command above |

## Dependencies on External Factors

- Owner Unity Editor time (S21-01, S21-04, S21-07, S21-08) — nothing can be verified without it.
- `gh` CLI install for automated PRs.

## S21-09 Decision — process gate (2026-11-03, owner)

**Chosen: C + B, enforced as a committed git `pre-push` hook.** Closes the 11-sprint carry of the
gate decision. TD-048 / TD-051 move from Backlog to *In place — pending install on the owner's
machine*.

| Part | What it does | Blocks the push? |
|---|---|---|
| **C** `.githooks/static-check.sh` | (1) A serialized field removed or renamed while a committed `.asset` / `.prefab` / `.unity` still stores its key, with no `[FormerlySerializedAs("old")]` — the BUG-105 / 108 / 121 class. Also catches a deleted `[FormerlySerializedAs]` (BUG-108). (2) `using UnityEditor` / `UnityEditor.` outside `#if UNITY_EDITOR` in `Assets/Script/` runtime code — the BUG-116 class. Runs on the `.cs` files changed in the pushed range; ~0.1 s; no Unity | Yes, on any branch, for an ERROR. Warnings never block |
| **B** `.claude/scripts/compile-check.sh` | Headless Unity batchmode compile of this checkout (`PROJECT_PATH` = repo root). Skipped when a Unity Editor is open (project lock) | Yes, for pushes to `sprint-*`, `main`, `release/*`; warning only on other branches |

**Install (once per clone):** `git config core.hooksPath .githooks`

**Escape hatches:** `GATE_ALLOW_REMOVED=<field>[,<field>]` for a field deleted on purpose;
`GATE_SKIP_COMPILE=1` to skip part B; `git push --no-verify` for an emergency — record the reason in
this file.

**Validated against history (static analysis, 2026-11-03):**

| Commit | Bug | Result |
|---|---|---|
| `ddcc0a5c` | BUG-105 `attackDamege` | ERROR — would have blocked |
| `cae75317` | BUG-108 `statsSO`, `weaponSO` (+ `defaultWeapon` on `Mage.asset`) | ERROR — would have blocked |
| `343a3830` | BUG-121 `authoredModifiers` | ERROR — would have blocked. Also flags `impactType` (deliberate: recovery / reduction is now the sign of `VitalModifier.value`, BUG-117) — the case `GATE_ALLOW_REMOVED` exists for |
| `041292c` | BUG-116 `using UnityEditor.Animations` | ERROR — would have blocked |
| `f37d620..5f70bfd` | (clean range) | PASS |

**Known limits:** part C matches YAML keys by name, so a key shared by two classes is downgraded to a
warning unless an asset bound to the changed script (or a subclass) stores it. Part B cannot run
while the Editor is open, which is most of the owner's day — part C is the half that always runs.
The whole-tree mode `static-check.sh --all` currently reports one runtime ERROR: BUG-116
(`CharacterData.cs:2`), fixed by S21-02. A TextMesh Pro sample
(`TMP_TextInfoDebugTool.cs:4`) has the same unguarded `using UnityEditor` but is vendor code and out
of scope.

---

## Definition of Done for this Sprint

- [ ] S21-01 playtest sheet filled (or accepted-risk signed) — Monday
- [ ] S21-02, S21-03 committed — Player build compiles; renamed data loads
- [ ] S21-04, S21-05 on `sprint-21` — no S1 open on the dungeon loop
- [ ] S21-06 enemies attack; weapon pickup works
- [ ] S21-07, S21-08 committed
- [x] S21-09 decision recorded (2026-11-03 — see "S21-09 Decision" below)
- [ ] Saturday 2026-11-07 playtest sheet filled

## QA Plan

> ⚠️ **No QA Plan** (`production/qa/qa-plan-sprint-21.md` not found). Autonomous run: option [B]
> recorded by default. S21-01, the Friday QA automation run and the Saturday playtest are the de
> facto QA gate.

## Notes

- Open bug-inbox notes NOTE-20261031-7…10 and NOTE-20261101-1…3 are **not** tasks.
- `production/sprint-status.yaml` not updated (`.md`-only run).
