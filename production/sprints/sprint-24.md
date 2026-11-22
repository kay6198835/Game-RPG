# Sprint 24 — 2026-11-23 to 2026-11-27

**Status: OPEN.** Opened 2026-11-22 (`pm-weekly-kickoff`, autonomous run — no owner present).
Branch `sprint-24`, created on the remote from the `sprint-23` tip (`69f59a3b`, "Merge
pm/doc-sync-2026-11-22 into sprint-23"). `gh` CLI unavailable (`gh: program not found`) — draft PR
**not** auto-created; run manually:

```bash
gh pr create --draft --base sprint-23 --head sprint-24 --title "Sprint 24"
```

**Review mode:** lean — producer feasibility gate skipped.
**Tracker:** `sprint-24-daily-plan.md`.
**Inputs:** `sprint-23.md`, `sprint-23-daily-plan.md` (wrap-up handoff 2026-11-21),
`production/qa/bug-triage-2026-11-21.md`, `production/qa/open-bugs.md`, `git log --all` since
2026-11-16. No milestone file and no risk register exist — risks are tracked here.

---

## Sprint 23 Closure (recorded by this kickoff)

- **Verdict:** SLIPPED (2026-11-21 wrap-up, evidence RUNTIME (automated QA `20261121-2106`) + STATIC
  + LOG; no playtest sheet).
- **Done (planned):** S23-03 BUG-125 (`b92b6f2a`, QA-CMB-01 PASS), S23-04 BUG-097 + boss room (code +
  data), S23-07 Editor / data block, S23-08 BUG-117 / 120 / 123 (code), S23-F1 pathfinding branch merged
  (`8459e384`). S23-06 cut by owner decision.
- **Velocity:** 1.7d / 4d = **0.43** (Must-only 0.2d / 0.95d). Sprint 18 0.0, 19 0.01, 20 0.13,
  21 0.08, 22 0.16, 23 0.43 — best in seven sprints, but carried by Should / Feature work, not Must.
- **Off-plan owner output:** ≈2.0d (BUG-071 rework, map / spawn restructure, enemy investigate state,
  pathfinding tile centres, knockback / confirm UI, dash rework, ≈1.4k Erika animation assets).
- **Playtest done: no** — `playtest-2026-11-21.md` empty. Playtest at its **28th** carry.
- **New confirmed bugs:** BUG-129…134 (2026-11-20 triage), BUG-135 (S1, 2026-11-21 triage).
  BUG-129 FIXED (`be973605`). S1 count 6.
- **After the wrap-up:** owner commit `1e5afb3c` "add effect dash" (Sun 2026-11-22 22:18) — dash smoke
  VFX, `Player.controller`, `PlayerDashState`, `PlayerMovement`, `SpawnMono` / `ISpawn` changes.
  Off-plan; not triaged. It does not touch `NegativeReceiverBase`, so BUG-135 stands. Whether it
  changes BUG-130 needs the automated run (S24-02).
- **Carry-over:** S23-01 playtest, S23-02 remainder (BUG-122 / 086 / 065), S23-05 BUG-126, S23-09
  BUG-127, S23-10 remainder (BUG-104 `Exit()` / 103 / 102 / 068).

---

## Sprint Goal

**Restore damage (BUG-135) and finish death → Game Over, so the demo loop menu → run → death can be
played end to end — and record the first playtest sheet in 28 weeks.**

---

## Capacity

- Total days: 5
- Buffer (20%): 1 day
- Available: 4 days

Velocity 0.43 and ≈2d of off-plan output a week. Planned load: Must 0.95d + Should 0.65d + Feature
0.75d = **2.35d of 4.0d** — kept well below capacity on purpose; the remaining ≈1.6d absorbs the
owner's usual unplanned work.

0.1d is reserved on **Saturday 2026-11-28** for the weekly playtest (`tests/smoke/critical-paths.md`
+ the `playtest-sheet.sh` sheet). The Friday QA automation run is an input, not a substitute.

---

## Carryover from Previous Sprint

| Task | Times carried | Reason | New Estimate |
|------|---------------|--------|--------------|
| Playtest / Play Mode smoke (S23-01) | **28th** | Never recorded | 0.3d — Mon, first (S24-01) |
| BUG-122 remainder + BUG-086 + BUG-065 (S23-02) | 3rd / 10th / **16th** | Code landed Thu, death state re-enters itself; Archer / Mage clips carry no event | 0.2d (S24-03) |
| BUG-126 (S23-05) | 2nd | Not started (one line) | 0.05d (S24-05) |
| BUG-127 Archer + Mage (S23-09) | 2nd | Not started | 0.05d (Should, S24-06) |
| One-liners remainder (S23-10) | 6th–9th | BUG-104 half done | 0.15d (Should, S24-07) |
| BUG-087 design note (S23-D4) | **9th** | Deferred | 0.3d — promoted to Should (S24-08): the demo's restart loop needs it |
| S23-D1, D2, D3, D5, D6 | — | Unchanged | Kept deferred (S24-D1…D5) |

---

## Tasks

### Must Have (Critical Path)

| ID | Task | Agent/Owner | Est. Days | Dependencies | Acceptance Criteria |
|----|------|-------------|-----------|--------------|---------------------|
| S24-01 | **Weekly playtest / Play Mode smoke — first task Monday (28th carry).** `tests/smoke/critical-paths.md` through the menu; one sheet in `production/qa/playtests/`. Rows blocked by BUG-135 are recorded FAIL with "BUG-135", not skipped | owner (Kay) | 0.3 | None | Sheet filled PASS/FAIL/SKIP per row, **or** a signed accepted-risk note in this file |
| S24-02 | **BUG-135 (S1)** — hurtboxes / receivers implement `INegativeReceiver` again (`8005d27e`); **rerun the automated QA suite the same day** (also checks `1e5afb3c` against BUG-130) | gameplay-programmer | 0.1 | None | The 14 BUG-135 QA FAILs PASS; damage reaches player and enemies |
| S24-03 | **BUG-122 remainder + BUG-086 + BUG-065** — empty `LogicUpdate()` override in `PlayerDeathState` (no self re-entry), death event on Archer / Mage Die clips, movement stopped, event emitted once | gameplay-programmer | 0.2 | S24-02 (death needs damage) | QA-VIT-07 PASS; Game Over panel shows for all three classes; no slide |
| S24-04 | **BUG-130 (S2)** — dash soft lock: class animator controllers need a Dash state, `PlayerBasicState.cs:31` self re-entry. Check first whether `1e5afb3c` already covers it | gameplay-programmer | 0.2 | S24-02 run | QA-PLY-06 PASS for every class; Space never locks input |
| S24-05 | **BUG-126 (S2)** — guard `using UnityEditor;` in `TMP_TextInfoDebugTool.cs:4` | gameplay-programmer | 0.05 | None | QA-STATIC-05 PASS |
| — | Saturday 2026-11-28 weekly playtest | owner (Kay) | 0.1 | — | Sheet filled before `/weekly-wrapup` |

Must-Have total = **0.95d**.

### Should Have

| ID | Task | Agent/Owner | Est. Days | Dependencies | Acceptance Criteria |
|----|------|-------------|-----------|--------------|---------------------|
| S24-06 | BUG-127 (S3) — Archer + Mage bolt `speed` / `lifetime` data | owner (Kay) | 0.05 | None | QA-CMB-04/05 PASS |
| S24-07 | One-liners — BUG-104 `Exit()` log, BUG-103, BUG-102, BUG-068 `:28` | gameplay / ui-programmer | 0.15 | None | Each visible in `git diff` |
| S24-08 | BUG-087 design note — who owns `ON_PLAYER_DEATH` → Game Over → restart (`GameManager` vs UIFlow) and player `Reborn()` | game-designer / producer | 0.3 | S24-03 | Short note in `design/` or an ADR; next sprint can implement restart |
| S24-09 | BUG-096 remainder (S1) — rooms without a `Tile_Spawn_Enemy` marker open their doors | gameplay-programmer | 0.15 | None | Empty room opens doors on entry; no soft lock |

Should-Have total = **0.65d**. Must + Should = 1.6d.

### Feature story (owner, declared main work, capped)

| ID | Task | Agent/Owner | Est. Days | Dependencies | Acceptance Criteria |
|----|------|-------------|-----------|--------------|---------------------|
| S24-F1 | Erika animation / dash VFX asset work — commit asset imports **in their own commits**, separate from any code / interface change (Sprint 23 retro) | owner (Kay) | **0.75 cap** | S24-02 first | Asset commits contain no `.cs`; automated suite rerun after the last one |

### Nice to Have / Deferred

| ID | Task | Agent/Owner | Est. Days | Note |
|----|------|-------------|-----------|------|
| S24-D1 | Effect-source work (`SourceEffect`, plan `ef0d9c6f`) | owner (Kay) | ≤0.5 | 4th defer |
| S24-D2 | BUG-092 + BUG-066/070 verify, BUG-118, BUG-119, BUG-113, BUG-114, BUG-100, BUG-071 | gameplay-programmer | 0.9 | Effect / vitals code |
| S24-D3 | Missing-script assets BUG-073 / 090 / 112 / 128 | owner (Editor) | 0.25 | — |
| S24-D4 | BUG-111 `RegenComponent` | gameplay-programmer | 0.15 | — |
| S24-D5 | Git LFS for `.fbx`, TD-040 ADR, BUG-083, weapon architecture review (`b364c303`) | technical-director / lead-programmer | 1.0 | Carried since S19; LFS more urgent after ≈1.4k Erika assets |
| S24-D6 | BUG-110 weapon drop / pickup bookkeeping | gameplay-programmer | 0.15 | Deferred by owner decision (NOTE-20261116-3) |

---

## Risks

| Risk | Probability | Impact | Mitigation |
|------|------------|--------|------------|
| Playtest slips a 28th time | High | High | S24-01 first Monday; owner decision: record or sign accepted risk |
| Playtest before BUG-135 fix mostly FAILs | High | Low | Expected — the FAIL rows are evidence; S24-02 the same morning |
| Large mixed commit breaks combat again (`8005d27e` pattern) | Medium | High | S24-F1 asset commits separate from code; QA rerun same day |
| Off-plan work outruns Must (14th time) | High | Medium | Must kept under 1d; ≈1.6d unplanned slack |
| PM worktree `D:/Fork/Game-RPG-pm` dirty with `Assets/Sprite/Erika/**` (≈1.2k paths, not routine work) | Known | Medium | Routines run in no-checkout temp worktrees; owner should commit or clean it |
| No `gh` CLI | Known | Low | Manual PR command above |

## Dependencies on External Factors

- Owner Unity Editor time (S24-01, S24-06) — nothing can be runtime-verified without it.
- `gh` CLI install for automated PRs.

---

## Definition of Done for this Sprint

- [ ] S24-01 playtest sheet filled (or accepted-risk signed) — Monday
- [ ] S24-02 committed — automated suite rerun, BUG-135 FAILs PASS
- [ ] S24-03 committed — QA-VIT-07 PASS, Game Over for all classes
- [ ] S24-04, S24-05 committed — QA-PLY-06, QA-STATIC-05 PASS
- [ ] Saturday 2026-11-28 playtest sheet filled

## QA Plan

> ⚠️ **No QA Plan** (`production/qa/qa-plan-sprint-24.md` not found). Autonomous run: option [B]
> recorded by default. S24-01, the automated QA runs and the Saturday playtest are the de facto QA gate.

## Notes

- Playtest guard (`playtest done: no`) puts S24-01 first Monday. The wrap-up handoff suggested BUG-135
  first; the kickoff keeps the playtest first (routine rule) and places S24-02 immediately after.
- Open / needs-owner inbox notes (NOTE-20261115-7 item 3, NOTE-20261117-4, NOTE-20261122-1) are
  **not** tasks.
- `production/sprint-status.yaml` not updated (`.md`-only run).
