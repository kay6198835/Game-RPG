# Sprint 23 — 2026-11-16 to 2026-11-20

**Status: OPEN.** Opened 2026-11-15 (`pm-weekly-kickoff`, autonomous run — no owner present).
Branch `sprint-23`, created on the remote from the `sprint-22` tip (`37b56d76`, "chore(doc-sync):
routine log 2026-11-15"). `gh` CLI unavailable (`gh: program not found`) — draft PR **not**
auto-created; run manually:

```bash
gh pr create --draft --base sprint-22 --head sprint-23 --title "Sprint 23"
```

**Review mode:** lean — producer feasibility gate skipped.
**Tracker:** `sprint-23-daily-plan.md`.
**Inputs:** `sprint-22.md`, `sprint-22-daily-plan.md` (wrap-up handoff 2026-11-15),
`production/qa/bug-triage-2026-11-15.md`, `git log --all` since 2026-11-09. No milestone file and no
risk register exist — risks are tracked here.

---

## Sprint 22 Closure (recorded by this kickoff)

- **Verdict:** SLIPPED (2026-11-15 wrap-up, evidence RUNTIME (automated QA only) + LOG + STATIC).
- **Done (planned):** S22-05 BUG-124 (`b33dedf9`), S22-11 BUG-115 (closed by the stat split
  `a0e250fb`, QA-STATIC-04 PASS), S22-F1 `feature/polish-game` merged (`a5d671c2`).
- **Velocity:** 0.65d / 4d = **0.16**. Sprint 17 0.05, 18 0.0, 19 0.01, 20 0.13, 21 0.08, 22 0.16.
- **Off-plan owner output:** ≈2.5d (stat split, ItemSO key migration, asmdef + 64 EditMode tests →
  BUG-084 FIXED, Rain Arrows wiring → BUG-109 FIXED in code, QA cases) plus the unmerged owner branch
  `docs/gdd-pathfinding-ability` (`f6921dc9`, `3b02df7d`: bonfire, interact prompt, `PlayerDashState`
  unwired, GDD update).
- **Playtest done: no** — `playtest-2026-11-14.md` empty. Playtest at its **26th** carry.
- **Automated QA** run `20261114-2357`: PASS 150 / FAIL 5 / KNOWN 8 / SKIP 20 / OBSERVED 2.
- **New confirmed bugs (2026-11-15 triage):** BUG-125 (S2, P1), BUG-126 (S2, P2), BUG-127 (S3, P2),
  BUG-128 (S4, P3). Status changes: BUG-063, BUG-109, BUG-115, BUG-124 → FIXED in code; BUG-084 → FIXED.
- **Carry-over:** S22-01, S22-02 (BUG-122), S22-03 (BUG-096), S22-04 (BUG-097), S22-06
  (BUG-106 + BUG-110), S22-07, S22-09 (BUG-065 + BUG-086), S22-10 (BUG-117 + 120 + 123), S22-12.

---

## Sprint Goal

**Player death reaches Game Over, melee damage is correct, and the room path starts in the start
room — then merge the owner's bonfire / interact / dash branch as the declared feature work.**
Playtest (or a signed accepted-risk note) first on Monday.

---

## Capacity

- Total days: 5
- Buffer (20%): 1 day
- Available: 4 days

Planned-work velocity has been ≤0.16 for seven sprints while off-plan output runs 1.7–2.5d a week.
Per the sprint-22 retrospective, the owner's feature branch is now a **declared** story (S23-F1,
cap 1.0d) instead of competing with the plan. Planned load: Must 0.95d + Should 0.9d + Feature 1.0d
= **2.85d of 4.0d**.

0.1d is reserved on **Saturday 2026-11-21** for the weekly playtest (`tests/smoke/critical-paths.md`
+ the `playtest-sheet.sh` sheet). The Friday 23:00 QA automation run is an input, not a substitute.

---

## Carryover from Previous Sprint

| Task | Times carried | Reason | New Estimate |
|------|---------------|--------|--------------|
| Playtest / Play Mode smoke (S22-01) | **26th** | Never recorded | 0.3d — Mon, first (S23-01) |
| BUG-122 (S22-02) + BUG-065 / BUG-086 (S22-09) | 2nd / **14th** / 9th | Not started | 0.3d, one Must task (S23-02) |
| BUG-096 Boss + BUG-097 (S22-03 / 04) | **7th** | Not started | 0.15d (S23-04) |
| BUG-106 player half + BUG-110 (S22-06) | 4th | Not started | 0.2d (Should, S23-06) |
| Editor / data block (S22-07) | **7th** | Not started | 0.15d (Should, S23-07) |
| BUG-117 + BUG-120 + BUG-123 (S22-10) | 3rd | Not started; owner branch serializes `VitalModifier` without the sign fix | 0.35d (Should, S23-08) |
| One-liners (S22-12) | 5th–8th | Not started | 0.15d (Should, S23-10) |
| S22-D1…D7 | — | BUG-084 and BUG-109 closed; rest unchanged | Kept deferred (S23-D1…D6) |

---

## Tasks

### Must Have (Critical Path)

| ID | Task | Agent/Owner | Est. Days | Dependencies | Acceptance Criteria |
|----|------|-------------|-----------|--------------|---------------------|
| S23-01 | **Weekly playtest / Play Mode smoke — first task Monday (26th carry).** `tests/smoke/critical-paths.md` through the menu; one sheet in `production/qa/playtests/` incl. rows for the "fixed in code, awaiting runtime" bugs (BUG-105, 107, 116 build, 121, 124; 063, 109, 115) | owner (Kay) | 0.3 | None | Sheet filled PASS/FAIL/SKIP per row, **or** a signed accepted-risk note in this file. Stop the silent carry |
| S23-02 | **BUG-122 (S1) + BUG-065 + BUG-086 in one session** — `PlayerDeathState` reaches `ON_PLAYER_DEATH` without waiting for an `EndRangeTrigger` no Die clip sends; stop movement on enter; emit once | gameplay-programmer | 0.3 | None | QA-VIT-07 PASS; Game Over panel shows; no slide; event fires once |
| S23-03 | **BUG-125 (S2, P1)** — `Weapon.FinalDamage()` adds stage damage twice (`38d40925`) | gameplay-programmer | 0.05 | None | QA-CMB-01.Paladin PASS |
| S23-04 | **BUG-096 Boss remainder + BUG-097** — start / boss chosen by `RoomType`, boss room not sealed on load (closes Bug #16) | gameplay-programmer | 0.15 | None | Run starts in `StartRoom_Entrance`; boss room reachable and opens correctly |
| S23-05 | **BUG-126 (S2)** — guard `using UnityEditor;` in `TMP_TextInfoDebugTool.cs:4` | gameplay-programmer | 0.05 | None | QA-STATIC-05 PASS |
| — | Saturday 2026-11-21 weekly playtest | owner (Kay) | 0.1 | — | Sheet filled before `/weekly-wrapup` |

Must-Have total = **0.95d**.

### Should Have

| ID | Task | Agent/Owner | Est. Days | Dependencies | Acceptance Criteria |
|----|------|-------------|-----------|--------------|---------------------|
| S23-06 | BUG-106 player half + BUG-110 — armed without the Champion; drop / pickup bookkeeping | gameplay-programmer | 0.2 | None | Picked-up weapon is the one that attacks; player armed on direct scene load |
| S23-07 | Editor / data block — BUG-095 `Collider2D` on `Arrow.prefab`; BUG-072 `layerMask` on `Lightning.prefab`; BUG-108 dead `weaponSO:` key | owner (Kay) | 0.15 | None | Visible in `git diff`; arrows hit; Consecrate damages |
| S23-08 | BUG-117 + BUG-120 + BUG-123 — vital modifier sign + double reduction, item stat effects apply, coin effect authorable. **Before S23-F1 merges** (owner branch serializes `VitalModifier`) | gameplay-programmer | 0.35 | None | Heal / damage effect correct; stat item changes stat; coin item adds coins |
| S23-09 | BUG-127 (S3) — Mage bolt `speed` / `lifetime` data | owner (Kay) | 0.05 | None | QA-CMB-04/05.Mage PASS |
| S23-10 | One-liners — BUG-104, BUG-103, BUG-102, BUG-068 `:28` | gameplay / ui-programmer | 0.15 | None | Each visible in `git diff` |

Should-Have total = **0.9d**. Must + Should = 1.85d.

### Feature story (owner, declared main work, capped)

| ID | Task | Agent/Owner | Est. Days | Dependencies | Acceptance Criteria |
|----|------|-------------|-----------|--------------|---------------------|
| S23-F1 | Finish and merge `docs/gdd-pathfinding-ability` (bonfire, interact prompt, `PlayerDashState`, GDD update) into `sprint-23` | owner (Kay) | **1.0 cap** | S23-02, S23-03 (Mon fixes first); S23-08 preferred | Compiles; stray file `States/s` removed and dash wired or excluded (NOTE-20261115-7); interact prompt shows (NOTE-20261115-6) |

### Nice to Have / Deferred

| ID | Task | Agent/Owner | Est. Days | Note |
|----|------|-------------|-----------|------|
| S23-D1 | Effect-source work (`SourceEffect`, plan `ef0d9c6f`) | owner (Kay) | ≤0.5 | 3rd defer |
| S23-D2 | BUG-092 + BUG-066/070 verify, BUG-118, BUG-119, BUG-113, BUG-114, BUG-100, BUG-071 | gameplay-programmer | 0.9 | Effect / vitals code |
| S23-D3 | Missing-script assets BUG-073 / 090 / 112 / 128 | owner (Editor) | 0.25 | + BUG-128 (capture scenes) |
| S23-D4 | BUG-087 design note — `GameManager` + player `Reborn()` | game-designer / producer | 0.3 | 8th carry; needed right after S23-02 |
| S23-D5 | BUG-111 `RegenComponent` | gameplay-programmer | 0.15 | BUG-109 closed |
| S23-D6 | Git LFS for `.fbx`, TD-040 ADR, BUG-083, weapon architecture review (`b364c303`) | technical-director / lead-programmer | 1.0 | Carried since S19; BUG-084 closed |

---

## Risks

| Risk | Probability | Impact | Mitigation |
|------|------------|--------|------------|
| Playtest slips a 26th time | High | High | S23-01 first Monday; owner decision: record or sign accepted risk. Eight bugs wait on runtime |
| BUG-122 untouched a third sprint | Medium | High | Paired with BUG-125 on Monday; both are small single-file fixes |
| Owner branch merges `VitalModifier` serialization before BUG-117 sign fix | Medium | Medium | S23-08 before S23-F1, or fix BUG-117 on the owner branch |
| Off-plan work outruns Must (13th time) | High | Medium | Feature now declared (S23-F1); Must kept under 1d |
| Pre-push gate checks `main` only | Known | Medium | Accepted 2026-11-03 |
| No `gh` CLI | Known | Low | Manual PR command above |

## Dependencies on External Factors

- Owner Unity Editor time (S23-01, S23-07, S23-09) — nothing can be runtime-verified without it.
- `gh` CLI install for automated PRs.

---

## Definition of Done for this Sprint

- [ ] S23-01 playtest sheet filled (or accepted-risk signed) — Monday
- [ ] S23-02 committed — QA-VIT-07 PASS
- [ ] S23-03, S23-05 committed — QA-CMB-01.Paladin, QA-STATIC-05 PASS
- [ ] S23-04 on `sprint-23` — run starts in the start room; boss room reachable
- [ ] Saturday 2026-11-21 playtest sheet filled

## QA Plan

> ⚠️ **No QA Plan** (`production/qa/qa-plan-sprint-23.md` not found). Autonomous run: option [B]
> recorded by default. S23-01, the Friday QA automation run and the Saturday playtest are the de
> facto QA gate.

## Notes

- Open / needs-owner inbox notes (incl. NOTE-20261115-5 re-save 17 `EntityData` assets,
  NOTE-20261115-6/-7) are **not** tasks; they appear only as acceptance hints for S23-F1.
- Sprint-22 retro suggested BUG-122 + BUG-125 as Monday's first task; the playtest guard
  (`playtest done: no`) puts S23-01 first, with S23-02 / S23-03 immediately after.
- `production/sprint-status.yaml` not updated (`.md`-only run).
