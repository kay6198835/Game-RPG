# Sprint 6 — Daily Plan & Progress Tracker

> **Sprint**: 2026-07-21 (Mon) → 2026-07-25 (Fri)
> **Companion to**: `sprint-06.md` (formal plan) — this file is the day-by-day breakdown + live tracker
> **Maintained by**: PM assistant — updated each session/standup
> **Routines (Claude Code)**:
>   - **Mon–Fri 10:00** → `/daily-standup`
>   - **Sat 22:00** → `/weekly-wrapup`
>   - **Sun 22:00** → `/weekly-kickoff`
> **Opened**: 2026-07-20 (Sun 22:00 kickoff) — autonomous scheduled run, no user present. Branch
> `sprint-06` created from `sprint-05` tip (`cc543ba`).

---

## Status Verdict: 🟡 DAY 1 — sprint opened, Monday work not yet started, off-plan commit already logged

Sprint 5 closed CONCERNS/bordering-FAIL at 34% Must-Have (1.35d/3.95d) after 4 consecutive days of
off-plan work. Sprint 6 is deliberately scoped narrower (2.20d Must-Have vs 4d capacity) to make
landing the carried death-chain and spawn-stabilization bugs achievable even with some slippage.
As of the 2026-07-21 02:00 standup, zero Must-Have tasks (S6-00→S6-09) have landed — verified by
reading the actual code (`PoolMember.cs`, `EntityMoveState.cs`, `EntityDeathState.cs`,
`EntityBasicState.cs`, `NegativeReciver.cs`, `RoomModel.cs`, `EnemySpawner.cs`), not just the
tracker table. One commit landed overnight (`d247e09 update data tilemap`) that is **not** on the
Sprint 6 task list — see Daily Log and Risks below.

---

## Burn Summary

| Metric | Value |
|--------|-------|
| Total work estimated | Must-Have ≈ 2.20d + Should ≈ 1.20d + Nice ≈ stretch |
| Capacity (sprint) | 4 days (5 − 20% buffer) |
| Days elapsed | 0 |
| Days remaining | 4 |
| Work committed/done | 0d |
| Velocity | N/A — sprint has not started |

---

## Task Estimates

| ID | Task | Est (d) | Priority | Status |
|----|------|---------|----------|--------|
| S6-00 | Verify branch parity (`origin/feature/spawn-enemy` commits present) | 0.1 | Must | ⬜ Not started |
| S6-01 | Verify `PoolMember.cs` compiles (CS0592 risk) | 0.1 | Must | ⬜ Not started |
| S6-02 | Fix NEW-1 — `GetSpawnSet()` weight==0 hang risk | 0.25 | Must | ⬜ Not started |
| S6-03 | Fix BUG-05 — `EntityMoveState` null-guard to top | 0.25 | Must | ⬜ Not started |
| S6-04 | Fix BUG-07 — `EntityDeathState : EntityState` rewrite + wire | 0.5 | Must | ⬜ Not started |
| S6-05 | Fix BUG-08 — `EntityBasicState` death transition + `ON_ENEMY_DEATH` | 0.25 | Must | ⬜ Not started |
| S6-06 | Fix BUG-ES-1 — `GetSpawnSet()` empty list not null | 0.25 | Must | ⬜ Not started |
| S6-07 | Fix BUG-ES-4 — guard `EnemySpawner.cs` empty `spawnPosition` read | 0.1 | Must | ⬜ Not started |
| S6-08 | Fix BUG-06 partial — write-through to `PlayerData.currentHealth` | 0.25 | Must | ⬜ Not started |
| S6-09 | Decision — `EnemyModal` SO-vs-plain-class | 0.15 | Must | ⬜ Not started |
| S6-D1 | ADR-0002 Proposed→Accepted | 0.1 | Should | ⬜ Not started |
| S6-D2 | ADR-0003 Proposed→Accepted (post S6-09) | 0.1 | Should | ⬜ Not started |
| S6-D3 | Dedupe spawn driver (BUG-ES-2) | 0.5 | Should | ⬜ Not started |
| S6-D4 | `CancelInvoke` pairing | 0.25 | Should | ⬜ Not started |
| S6-D5 | Cleanup batch (AH-2/EM-2/WM-2/LM-1) | 0.25 | Should | ⬜ Not started |
| S6-D6 | Decision — S4-05/S4-06 keep-or-cut | 0.1 | Should | ⬜ Not started |
| S6-N1 | First playtest session | — | Nice | ⬜ Not started |
| S6-N2 | Start `EnemyManager` lifecycle body (stretch) | 1.0 | Nice | ⬜ Not started |

Status legend: ⬜ Not started · 🟡 In progress · ✅ Done · ⏸️ Blocked · ✂️ Cut

---

## Day-by-Day Breakdown

### Mon 07/21 — PLAN
**Goal: Verify no build break, close the highest-severity carried bugs first (hang risk, entity death chain start).**

| # | Task | Est | Priority |
|---|------|-----|----------|
| 1 | **S6-00** — verify branch parity | 0.1d | 🔴 Must |
| 2 | **S6-01** — verify `PoolMember.cs` compiles, fix if broken | 0.1d | 🔴 Must |
| 3 | **S6-02** — fix NEW-1 hang risk (`weight == 0`) | 0.25d | 🔴 Must |
| 4 | **S6-03** — `EntityMoveState` null-guard (BUG-05) | 0.25d | 🔴 Must |

### Tue 07/22 — PLAN
**Goal: Finish the enemy death chain.**

| # | Task | Est | Priority |
|---|------|-----|----------|
| 1 | **S6-04** — `EntityDeathState : EntityState` rewrite + wire (BUG-07) | 0.5d | 🔴 Must |
| 2 | **S6-05** — `EntityBasicState` death transition + `ON_ENEMY_DEATH` (BUG-08) | 0.25d | 🔴 Must |
| 3 | **Smoke** — Play Mode: melee enemy to 0 HP → `EntityDeathState` reached, event fires once | — | Advisory |

### Wed 07/23 — PLAN
**Goal: Spawn stabilization + BUG-06 write-through fix.**

| # | Task | Est | Priority |
|---|------|-----|----------|
| 1 | **S6-06** — `GetSpawnSet()` empty list not null (BUG-ES-1) | 0.25d | 🔴 Must |
| 2 | **S6-07** — guard `EnemySpawner.cs` empty `spawnPosition` read (BUG-ES-4) | 0.1d | 🔴 Must |
| 3 | **S6-08** — `NegativeReciver` write-through to `PlayerData.currentHealth` (BUG-06 partial fix) | 0.25d | 🔴 Must |
| 4 | **S6-09** — `EnemyModal` SO-vs-plain-class decision | 0.15d | 🔴 Must |

### Thu 07/24 — PLAN
**Goal: Should-Have carry cleanup — ADR flips, driver dedupe, small fixes.**

| # | Task | Est | Priority |
|---|------|-----|----------|
| 1 | **S6-D1** — ADR-0002 Proposed→Accepted | 0.1d | 🟡 Should |
| 2 | **S6-D2** — ADR-0003 Proposed→Accepted (post S6-09) | 0.1d | 🟡 Should |
| 3 | **S6-D3** — dedupe spawn driver (BUG-ES-2) | 0.5d | 🟡 Should |
| 4 | **S6-D4** — `CancelInvoke` pairing | 0.25d | 🟡 Should |
| 5 | **S6-D5** — cleanup batch | 0.25d | 🟡 Should |
| 6 | **S6-D6** — S4-05/S4-06 keep-or-cut decision | 0.1d | 🟡 Should |

### Fri 07/25 — WRAPUP DAY
**Goal: Full-loop smoke-check + `/weekly-wrapup`.**

| # | Task | Est |
|---|------|-----|
| 1 | Full Play Mode pass: enemy death chain end-to-end; empty-pool + empty-spawn-position rooms load without throw; player damage → `Reborn()` restores the same HP field | — |
| 2 | `/weekly-wrapup` — review week's `.cs`, playtest log if S6-N1 ran, bug triage, light retro | — |
| 3 | Record carry-over + velocity in `sprint-06.md` if anything slips | — |
| 4 | Confirm `/qa-plan sprint` was run this cycle (blocking for next gate); flag again if not — 4th consecutive miss would be worth investigating as a process issue | — |
| 5 | If Must-Haves done early: **S6-N1** (playtest) or **S6-N2** (`EnemyManager` lifecycle start) | — |

---

## Risks (live — updated each standup)

| Risk | Status | Mitigation |
|------|--------|------------|
| Off-plan work recurs a 5th time | 🔴 CONFIRMED (2026-07-21) | `d247e09 "update data tilemap"` (all 13 room JSONs + `LoadRandomMap.unity`, not on the S6 task list) landed the same evening as kickoff. Not blocking Must-Have work yet — watch tomorrow's standup for whether it continues instead of S6-00→S6-03 |
| `PoolMember.cs` build break unverified | 🔴 WATCH | First task Monday, verify before anything else |
| `EnemyModal` decision blocks ADR-0003 flip and further spawn-system work | 🟡 WATCH | S6-09 scheduled Wed, before Thu's ADR cleanup |
| No QA plan — 3rd consecutive cycle | 🔴 OPEN | Flagged in `sprint-06.md`; recommend running `/qa-plan sprint` before S6-03 starts |
| No Unity CLI | 🟢 KNOWN | Play Mode smoke = manual in-Editor |

---

## Daily Log
> Owner reports what was done; PM updates estimates/status here each standup.

- **2026-07-20 (Sun 22:00) — kickoff, autonomous scheduled run**: Sprint 5 closed (`sprint-05.md`
  marked Status: COMPLETE). Branch `sprint-06` created from `sprint-05` tip (`cc543ba`) —
  `git checkout sprint-05 && git checkout -b sprint-06`. `gh` CLI unavailable in this environment;
  draft PR not created — run manually: `gh pr create --draft --base sprint-05 --head sprint-06
  --title "Sprint 6"`. Sprint plan (`sprint-06.md`) and this tracker written via `/sprint-plan`,
  scoped narrower than Sprint 5 (2.20d Must-Have vs 4d capacity) specifically because Sprint 5 closed
  at only 34% Must-Have across 4 days of off-plan work — the goal this cycle is closing the carry,
  not adding scope. QA plan gate still open (3rd consecutive cycle) — flagged, not silently dropped.
  No user was present for this run; scoping calls (task breakdown, estimates, day sequencing) made
  autonomously per the kickoff routine's standing authorization — review at Monday's standup.

- **2026-07-21 (Mon 02:00) — daily standup, autonomous scheduled run**: Checked out `sprint-06`
  (was on `origin/feature/enemy-control`). `git log` since kickoff shows exactly one commit —
  `d247e09 "update data tilemap"` (21:57 07-20) — touching all 13 `Assets/Data/Json/Room/*.json`
  files (+65,931/-8,705 lines total) plus `LoadRandomMap.unity` and removing `Level Manager.prefab`.
  This is **not** on the Sprint 6 task list (S6-00→S6-09, S6-D1→S6-D6) — flagged as the 5th
  recurrence of the "off-plan work" risk called out in this sprint's own risk register. Checked
  whether the tilemap update happened to add the missing `Tile_Spawn_Enemy` markers (Known Bug #16 /
  GDD item 15 — only 1/13 rooms has the marker) — it did not; still 1/13 after the commit, so this
  looks like a tile-art re-export, not spawn-marker authoring.
  Verified all Monday Must-Have targets against current code (not just the tracker table) — all
  still open exactly as described in `sprint-06.md`:
  - S6-01 (`PoolMember.cs:9`, CS0592 risk): `[SerializeField] public bool isInPool { get; private set; }`
    unchanged, still unverified against a real Editor compile.
  - S6-03 (BUG-05): `EntityMoveState.LogicUpdate()` line 30 still dereferences
    `entity.Input.Target.transform.position` before the null check at line 34.
  - S6-02 (NEW-1): `RoomModel.GetSpawnSet()` still has no `weight == 0` guard — an `EnemyModal` with
    `weight == 0` selected inside the Phase-1 loop leaves `weightBudget` unchanged, so the
    `while (weightBudget > this.weightBudget * 0.1f)` loop can spin indefinitely.
  - S6-06 (BUG-ES-1): `GetSpawnSet()` line 16 still `return null;` on an empty pool, not an empty list.
  - S6-07 (BUG-ES-4): `EnemySpawner.cs` line 61 (`spawnPosition[Random.Range(...)]`) still has no
    empty-list guard.
  - S6-08 (BUG-06 partial): `NegativeReciver.cs` still mutates its own local `currentHealth` field,
    not `PlayerData.currentHealth` — `Reborn()` write-through still broken.
  - S6-00 (branch parity): could not confirm via `git log sprint-06 | grep dce9be1|d653654` — neither
    hash appears literally (likely rebased/squashed into `sprint-05` history before the `sprint-06`
    branch point). Content-level check not run this session — **still needs a manual confirm that
    `RoomGeneraterController.cs`/`RoomGridController.cs` carry the `feature/spawn-enemy` changes**,
    task not marked done.
  No status cells in the Task Estimates table changed — everything is still accurately ⬜ Not started.
  This is expected for a 02:00 Monday run (the work day hasn't started yet); recorded here so today's
  actual progress has a clean baseline to diff against at tomorrow's standup.
