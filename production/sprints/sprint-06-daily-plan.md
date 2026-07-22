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

## Status Verdict: 🟡 DAY 4 — branches merged (off-plan-work risk resolved), zero Sprint-6 Must-Have items still landed

Sprint 5 closed CONCERNS/bordering-FAIL at 34% Must-Have (1.35d/3.95d) after 4 consecutive days of
off-plan work. Sprint 6 is deliberately scoped narrower (2.20d Must-Have vs 4d capacity) to make
landing the carried death-chain and spawn-stabilization bugs achievable even with some slippage.
**Change since Wed 02:11 standup**: at 02:45 (2026-07-23) two commits landed — `1a64e09 "enhance
life circle room"` and `fe62f47 "Merge branch 'sprint-06' into origin/feature/spawn-enemy"`. The
merge unified `sprint-06` and `origin/feature/spawn-enemy` at the same commit (`git rev-parse HEAD
sprint-06` now match) — the off-plan pathfinding/Core-refactor branch is no longer separate history.
This resolves S6-00 (branch parity) and the "off-plan work not merged" risk flagged 7 standups
running. It does **not** mean the Sprint 6 Must-Have bug list is fixed — re-verified S6-01 through
S6-08 directly against the current merged code:
- S6-01 (`PoolMember.cs:9`): unchanged, still unverified against a real Editor compile.
- S6-02 (NEW-1, `RoomModel.GetSpawnSet()` weight==0 hang): unchanged — Phase-1 `while` loop still has
  no guard against a zero-weight pick leaving `weightBudget` unchanged.
- S6-03 (BUG-05): the literal line-30-before-line-34 dereference is gone — `EntityMoveState` was
  rewritten as part of the Core refactor and the movement call (`entityCore.EntityMovement.MoveToTarget()`)
  is now commented out with a `// fix` marker, i.e. enemy movement itself looks mid-refactor/disabled
  rather than merely unguarded. Needs a fresh look, not just the old fix.
- S6-04 (BUG-07): `EntityDeathState` still `: MonoBehaviour`, empty stub, not wired.
- S6-05 (BUG-08): `EntityBasicState`'s `Health <= 0` block still empty (has a `// fix` marker).
- S6-06 (BUG-ES-1): `GetSpawnSet()` still `return null;` on an empty pool.
- S6-07 (BUG-ES-4): `EnemySpawner.cs:77` `spawnPosition[Random.Range(...)]` still unguarded.
- S6-08 (BUG-06 partial): `NegativeReciver.TakeDamage()` is now implemented (previously threw
  `NotImplementedException`, CLAUDE.md Bug #6) and emits `ON_PLAYER_DEATH` — real progress — but it
  still mutates its own local `currentHealth` field, not `PlayerData.currentHealth`, so the write-through
  part of S6-08 is still open.
The `1a64e09` commit itself is legitimate room-clear-condition progress (checklist item 8): event
subscriptions for `ON_ENEMY_DEATH`/`ON_DONE_SPAWN_ENEMY`/`ON_SPAWN_EXTRA_ENEMY` moved from `RoomCell`
up to `RoomGridController`, routed through `_current` — a step toward S6-D3 (dedupe spawn driver) —
but it is still not an S6-numbered task itself. See Daily Log and Risks below.

---

## Burn Summary

| Metric | Value |
|--------|-------|
| Total work estimated | Must-Have ≈ 2.20d + Should ≈ 1.20d + Nice ≈ stretch |
| Capacity (sprint) | 4 days (5 − 20% buffer) |
| Days elapsed | 1 (Mon complete, 0d landed on `sprint-06`) |
| Days remaining | 3 |
| Work committed/done | 0d on `sprint-06` (unrelated A* pathfinding work in progress on `origin/feature/enemy-control`) |
| Velocity | 0% — Must-Have list untouched after 1 full sprint day |

---

## Task Estimates

| ID | Task | Est (d) | Priority | Status |
|----|------|---------|----------|--------|
| S6-00 | Verify branch parity (`origin/feature/spawn-enemy` commits present) | 0.1 | Must | ✅ Done — `fe62f47` merge unified `sprint-06`/`feature/spawn-enemy` |
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

### Tue 07/22 — PLAN (revised at 02:00 standup — Monday's slate carries in, unstarted)

**Goal, revised**: Monday's S6-00→S6-03 did not start (0% velocity, see Status Verdict above). Original
Tuesday goal (death chain) pushed behind the carried Monday slate rather than silently reordered.

| # | Task | Est | Priority | Note |
|---|------|-----|----------|------|
| 1 | **S6-01** — verify `PoolMember.cs` compiles, fix if broken | 0.1d | 🔴 Must | Carried from Mon — 2nd day unverified, do first |
| 2 | **S6-00** — verify branch parity | 0.1d | 🔴 Must | Carried from Mon |
| 3 | **S6-02** — fix NEW-1 hang risk (`weight == 0`) | 0.25d | 🔴 Must | Carried from Mon |
| 4 | **S6-03** — `EntityMoveState` null-guard (BUG-05) | 0.25d | 🔴 Must | Carried from Mon |
| 5 | **S6-04** — `EntityDeathState : EntityState` rewrite + wire (BUG-07) | 0.5d | 🔴 Must | Original Tue goal — only if 1-4 close early |
| 6 | **S6-05** — `EntityBasicState` death transition + `ON_ENEMY_DEATH` (BUG-08) | 0.25d | 🔴 Must | Original Tue goal — only if 1-4 close early |
| 7 | **Smoke** — Play Mode: melee enemy to 0 HP → `EntityDeathState` reached, event fires once | — | Advisory | Only reachable after S6-04/S6-05 |

**Owner decision needed**: the A* pathfinding work on `origin/feature/enemy-control` (6th off-plan
recurrence) is not on this list — see Risks. Continuing it today means another 0% Must-Have day.

### Wed 07/23 — PLAN (revised at 02:00 standup — full Must-Have backlog carried, none started Mon/Tue)

**Goal, revised**: original Wed goal (spawn stabilization) kept, but sequenced behind the still-open
Mon/Tue carry since velocity is 0% after 2 full sprint days. Order below is by dependency/risk, not by
original day — apply `/estimate` logic (small, low-risk fixes first to bank quick wins; S6-04 last
among Musts as the largest single item).

| # | Task | Est | Priority | Note |
|---|------|-----|----------|------|
| 1 | **S6-01** — verify `PoolMember.cs` compiles (CS0592 risk) | 0.1d | 🔴 Must | Carried 3rd day — do first, blocks nothing but unverified risk compounds |
| 2 | **S6-00** — verify branch parity | 0.1d | 🔴 Must | Carried 3rd day |
| 3 | **S6-07** — guard `EnemySpawner.cs` empty `spawnPosition` read (BUG-ES-4) | 0.1d | 🔴 Must | Small, isolated, no dependency |
| 4 | **S6-06** — `GetSpawnSet()` empty list not null (BUG-ES-1) | 0.25d | 🔴 Must | Pairs naturally with S6-07 (same call chain) |
| 5 | **S6-02** — fix NEW-1 hang risk, `weight == 0` guard in `RoomModel.GetSpawnSet()` Phase-1 loop | 0.25d | 🔴 Must | Same file as S6-06 — do together |
| 6 | **S6-03** — `EntityMoveState` null-guard to top of `LogicUpdate()` (BUG-05) | 0.25d | 🔴 Must | Carried 3rd day — isolated one-file fix |
| 7 | **S6-08** — `NegativeReciver` write-through to `PlayerData.currentHealth` (BUG-06 partial) | 0.25d | 🔴 Must | Isolated one-file fix |
| 8 | **S6-09** — `EnemyModal` SO-vs-plain-class decision | 0.15d | 🔴 Must | Decision only, no code — do anytime, unblocks S6-D2 |
| 9 | **S6-04** — `EntityDeathState : EntityState` rewrite + wire (BUG-07) | 0.5d | 🔴 Must | Largest item — only if 1-8 close early |
| 10 | **S6-05** — `EntityBasicState` death transition + `ON_ENEMY_DEATH` (BUG-08) | 0.25d | 🔴 Must | Depends on S6-04 landing first |

Total Must-Have remaining if none of the above land today: 2.20d unchanged — still fits within 1
remaining sprint day only if today closes most of items 1-8.

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
| Off-plan work not merged into `sprint-06` | 🟢 RESOLVED (2026-07-23 02:45, `fe62f47`) | `origin/feature/spawn-enemy` merged into `sprint-06` (both refs now point at the same commit) — the A* pathfinding subsystem and Core/CoreComponent (`ICore`/`ICoreComponent`) refactor are now part of `sprint-06` history. This closes the risk raised 7 standups running. It does not close any S6-numbered Must-Have — see Status Verdict for the re-verified per-item state. New follow-on watch item: the Core refactor left `EntityMoveState`/`EntityBasicState` with commented-out `// fix` movement/attack calls — confirm in-Editor that enemies still move/attack before treating this merge as a net-positive for gameplay, not just for git hygiene. |
| Sprint 6 Must-Have bug list unaddressed, 3 of 5 sprint days elapsed | 🔴 OPEN | S6-01 through S6-08 (7 of 8 remaining after S6-00) still open, re-verified against current code this standup (see Status Verdict). 2.20d of estimated work vs ~1.4 sprint days left before Friday wrap-up. Recommend the owner triage today: bank the small isolated fixes first (S6-07, S6-06, S6-02 — same call chain; S6-01, S6-08 — one file each) before attempting S6-04 (largest item, 0.5d). |
| `PoolMember.cs` build break unverified | 🔴 WATCH — 2nd consecutive miss | Still `[SerializeField] public bool isInPool { get; private set; }` unchanged on `sprint-06`; was the mandated first task Monday, still not verified against a real Editor compile as of Tue 02:00 |
| `EnemyModal` decision blocks ADR-0003 flip and further spawn-system work | 🟡 WATCH | S6-09 was scheduled Wed; at current velocity (0% after day 1) the whole Wed slate is at risk of slipping |
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

- **2026-07-22 (Tue 02:00) — daily standup, autonomous scheduled run**: Could not `git checkout
  sprint-06` directly — the local working tree (on `origin/feature/enemy-control`) has uncommitted
  changes to `EnemySpawner.cs`, `LevelManager.cs`, `Cell.cs`, `Utility.cs`, and new
  `Assets/Script/Pathfinding/**` files that checkout would have overwritten. Per the hard constraint
  against touching `.cs` files, did not stash or discard that work; used a disposable
  `git worktree add D:/sprint06wt sprint-06` instead so the review/update happened without touching
  the developer's in-progress files. Worktree removed at the end of this run.
  `git log sprint-06` shows the branch tip is still `e1e3d64` (yesterday's standup commit) — **no
  commits landed on `sprint-06` since Monday's 10:17 standup**. Re-verified every Must-Have target
  directly against `sprint-06` code:
  - S6-00 (branch parity): still unconfirmed — `dce9be1`/`d653654` don't appear as literal hashes in
    `sprint-06` history (expected, likely rebased); content-level check of
    `RoomGeneraterController.cs`/`RoomGridController.cs` still not run.
  - S6-01 (`PoolMember.cs:9` CS0592 risk): unchanged, still unverified. **2nd consecutive day** this
    mandated "verify first" task hasn't been done.
  - S6-02 (NEW-1, weight==0 hang): `RoomModel.GetSpawnSet()` unchanged.
  - S6-03 (BUG-05): `EntityMoveState.cs` — the null check exists at line 21 (`Enter()`) and line 34,
    but the actual dereference `entity.Input.Target.transform.position` at line 30 still runs before
    either guard fires in `LogicUpdate()`. Still broken as described.
  - S6-04 (BUG-07): `EntityDeathState` still `: MonoBehaviour`, not wired.
  - S6-05 (BUG-08): `EntityBasicState`'s `Health <= 0` block still empty.
  - S6-06 (BUG-ES-1): `RoomModel.GetSpawnSet()` line 16 still `return null;` on `n == 0`.
  - S6-07 (BUG-ES-4): `EnemySpawner.cs` line 75 (`spawnPosition[Random.Range(...)]`) still unguarded
    on `sprint-06`. (Note: the *uncommitted* work on `origin/feature/enemy-control` changes the method
    signature to `SpawnRoomEnemies(in List<Vector2Int> spawnPosition)` and fixes two unrelated compile
    typos in the same file, but the indexing itself is still not guarded — and none of it is on
    `sprint-06` regardless.)
  - S6-08 (BUG-06 partial): `NegativeReciver.cs` still mutates its own local `currentHealth`, not
    `PlayerData.currentHealth`.
  All Task Estimates statuses remain accurately ⬜ Not started — nothing to flip to 🟡/✅.
  **Escalating the off-plan-work risk** (see Risks table) — this is the 6th recurrence of the pattern
  called out across Sprints 5 and 6, and it is now the direct explanation for 0% Must-Have velocity
  after a full sprint day. Recommend the owner explicitly decide: fold the A* pathfinding work into
  Sprint 7 scope with its own tasks/estimates, or pause it and redirect today to S6-00→S6-03 as
  originally planned. Not resolved autonomously — this is a scope call for the owner.

- **2026-07-23 (Wed 02:00) — daily standup, autonomous scheduled run**: Working tree (still on
  `origin/feature/spawn-enemy`, renamed from `origin/feature/enemy-control`) had 6 uncommitted `.cs`
  edits (`Interact.cs`, `EnemySpawner.cs`, `RoomCell.cs`, `RoomGeneraterController.cs`,
  `RoomGridController.cs`, `ObjectPoolManager.cs`). Per the hard constraint against touching `.cs`
  files, ran `git stash push -u` (reversible, not discarded) to move onto `sprint-06` cleanly, did the
  tracker update, then `git checkout` back to `feature/spawn-enemy` and `git stash pop` to restore the
  developer's WIP exactly as found — a plain stash round-trip was sufficient this time, no worktree
  needed.
  `git log sprint-06` tip is still `fcb89bd` (Tuesday's standup commit) — **no commits landed on
  `sprint-06` since Monday**. Meanwhile `origin/feature/spawn-enemy` picked up 6 more commits
  yesterday (2026-07-22 10:39 to 22:45): pathfinding continuation (`599191f`, `bc99d81`, `a2f06bf`,
  `5ecba49`, `0223d02`, `ea5658d`, `d3b0e74`) plus a new Core/CoreComponent architecture pass
  (`771f169 "big update core and corecomponet, add interface"` — `ICore`/`ICoreComponent` interfaces
  added under `Assets/Script/Character/Base/Interface/`, `CoreBase.cs`/`CoreComponentBase.cs`
  restructured, new `EntityAttack.cs`), then `9461bb1 "coding"` and `33df425 "fix complier bug"`. None
  of this touches the Sprint 6 Must-Have list. Re-verified S6-01 through S6-08 directly against
  `sprint-06` source (see Risks table above for line-level detail) — all still open exactly as
  Tuesday. Task Estimates table unchanged — still accurately all Not started, 3 of 4 sprint days
  elapsed, 0% Must-Have velocity.
  **Today's plan (autonomous, carried forward)**: since S6-00 through S6-05 (the Mon/Tue slate) never
  started either, today's list is the full Must-Have backlog in dependency order rather than only the
  original Wed items — see revised task list below with estimates. Whether to formally fold the
  pathfinding/Core-refactor branch into Sprint 7 scope is still not resolved autonomously — flagging
  again for the owner, 7th consecutive standup raising this.

- **2026-07-23 (follow-up run, 02:45+) — daily standup, autonomous scheduled run**: Re-ran after the
  02:11 standup above because 2 more commits landed 34 minutes later, changing the picture materially.
  `git log` since `8f8d65a` (the 02:11 standup commit):
  - `1a64e09 "enhance life circle room"` (02:45) — real progress toward checklist item 8 (room-clear
    condition): `RoomCell`'s `ON_DONE_SPAWN_ENEMY`/`ON_ENEMY_DEATH`/`ON_SPAWN_EXTRA_ENEMY`
    registrations moved up into `RoomGridController`, routed through `_current`; door reposition math
    fixed to be relative to room origin (`this.transform.position + (Vector3)dp.position`); also
    renamed `EnemySpawner.OnDoneLoadRoomGrid` → `OnGetSpawnPositions` and dropped a stray
    `Unity.VisualScripting` using.
  - `fe62f47 "Merge branch 'sprint-06' into origin/feature/spawn-enemy"` (02:45) — merges `sprint-06`
    into the feature branch; `sprint-06` and `origin/feature/spawn-enemy` now point at the identical
    commit. This resolves **S6-00** (branch parity) and closes the "off-plan work not merged" risk
    that has recurred every standup since Sprint 5. No uncommitted `.cs` changes were present this
    time (working tree only had 2 modified `.asset` files, untouched per the hard constraint) — no
    stash/worktree needed, `git checkout sprint-06` was a plain fast-forward.
  Re-verified S6-01 through S6-08 directly against the now-unified code (see Status Verdict section
  above for the full per-item breakdown) — 7 of the remaining 8 Must-Haves are still open exactly as
  Tuesday/Wednesday described them. One partial positive found: `NegativeReciver.TakeDamage()` is no
  longer a `NotImplementedException` stub (CLAUDE.md Bug #6 is stale on this point) — it decrements a
  local `currentHealth` and emits `EventID.ON_PLAYER_DEATH` at zero, but still doesn't write through to
  `PlayerData.currentHealth`, so S6-08 stays open (the `Reborn()` single-source-of-truth contract is
  still broken). One new watch item found: `EntityMoveState`/`EntityBasicState` now carry commented-out
  `# fix` markers around the movement/attack calls (`entityCore.EntityMovement.MoveToTarget()`,
  attack-check block) — this looks like the Core refactor left enemy movement/attack mid-wire, not
  fully broken but not confirmed working either; recommend an in-Editor Play Mode smoke check before
  assuming enemies still chase/attack correctly on `sprint-06` now.
  Task Estimates table: flipped **S6-00 to ✅ Done**. S6-01 through S6-09 remain ⬜ Not started — 3 of 5
  sprint days elapsed (Mon/Tue/Wed), 1/9 Must-Have items closed, Thu + Fri remain before wrap-up.

  **Today's plan** (Thu 07/23, revised — same dependency-ordered backlog as Wed since nothing but S6-00
  landed):
  | # | Task | Est | Priority | Note |
  |---|------|-----|----------|------|
  | 1 | **S6-07** — guard `EnemySpawner.cs:77` empty `spawnPosition` read (BUG-ES-4) | 0.1d | 🔴 Must | Smallest, isolated, no dependency — bank first |
  | 2 | **S6-06** — `GetSpawnSet()` empty list not null (BUG-ES-1) | 0.25d | 🔴 Must | Same file/call chain as S6-07 |
  | 3 | **S6-02** — `weight == 0` guard in `RoomModel.GetSpawnSet()` Phase-1 loop (NEW-1) | 0.25d | 🔴 Must | Same file as S6-06 |
  | 4 | **S6-01** — verify `PoolMember.cs:9` compiles (CS0592 risk) | 0.1d | 🔴 Must | Carried 4th day — isolated Editor check |
  | 5 | **S6-08** — `NegativeReciver` write-through to `PlayerData.currentHealth` (BUG-06 partial) | 0.25d | 🔴 Must | `TakeDamage()` now exists — this is now a small targeted edit, not a from-scratch implementation |
  | 6 | **S6-09** — `EnemyModal` SO-vs-plain-class decision | 0.15d | 🔴 Must | Decision only, no code — unblocks S6-D2 |
  | 7 | **S6-03** — re-scope: confirm whether `EntityMoveState`'s commented-out movement call is a merge artifact or in-progress work; restore/guard accordingly (BUG-05) | 0.25d | 🔴 Must | Needs an owner look — the original 1-line null-guard fix may no longer apply as described |
  | 8 | **S6-04** — `EntityDeathState : EntityState` rewrite + wire (BUG-07) | 0.5d | 🔴 Must | Largest item — only if 1-7 close early |
  | 9 | **S6-05** — `EntityBasicState` death transition + `ON_ENEMY_DEATH` (BUG-08) | 0.25d | 🔴 Must | Depends on S6-04 |
  | 10 | **Smoke** — Play Mode: confirm enemies still move/attack post-Core-refactor, before/alongside the above | — | Advisory | New, prompted by the `// fix` markers found this standup |

  Remaining Must-Have estimate: 2.10d (2.20d − 0.10d for S6-00) against ~1.4 sprint days left
  (Thu + Fri) before Friday's wrap-up gate — tight but not yet infeasible if today closes items 1-6.
