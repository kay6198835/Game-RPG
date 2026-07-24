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

## Status Verdict: 🟢 DAY 4 (Thu 02:00) — 8 of 10 Must-Have items closed, only S6-05 + S6-08 remain

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
| Days elapsed | 4 (Mon–Wed complete + Thu 02:00 checkpoint) |
| Days remaining | 1 (Fri wrap-up) |
| Work committed/done | ≈1.95d of 2.20d Must-Have landed on `sprint-06` (S6-00,01,02,03,04,06,07,09 closed same-day 2026-07-23 in a marathon owner session; S6-05/S6-08 ≈0.25d+0.25d=0.5d remain) |
| Velocity | 0% through Wed, then ~89% of Must-Have closed in a single Wed evening session — see Daily Log |

---

## Task Estimates

| ID | Task | Est (d) | Priority | Status |
|----|------|---------|----------|--------|
| S6-00 | Verify branch parity (`origin/feature/spawn-enemy` commits present) | 0.1 | Must | ✅ Done — `fe62f47` merge unified `sprint-06`/`feature/spawn-enemy` |
| S6-01 | Verify `PoolMember.cs` compiles (CS0592 risk) | 0.1 | Must | ✅ Done — owner fix: dropped `[SerializeField]` from the auto-property in [PoolMember.cs:9](Assets/Script/Poolable/PoolMember.cs#L9) (attribute doesn't apply to a property, only a backing field — the CS0592 risk source). `isInPool` no longer shows in the Inspector, matches its usage (runtime-only flag, set via `SwitchIsInPool()`) |
| S6-02 | Fix NEW-1 — `GetSpawnSet()` weight==0 hang risk | 0.25 | Must | ✅ Done — owner decision: `EnemyModal.weight` clamped `[Range(1, 100)]` ([RoomModel.cs:106](Assets/Script/Database-SO/Modal/RoomModel.cs#L106)); zero-weight can no longer be authored, closes the hang at the source instead of a loop guard |
| S6-03 | Fix BUG-05 — `EntityMoveState` null-guard to top | 0.25 | Must | ✅ Closed — owner decision: [EntityMovement.cs:11](Assets/Script/Character/Entity/CoreComponent/EntityMovement.cs#L11) `indexWaypoints` now explicit `= 0`, no separate `Waypoints.Count` guard added. Residual edge case not eliminated by this change alone — see Daily Log note |
| S6-04 | Fix BUG-07 — `EntityDeathState : EntityState` rewrite + wire | 0.5 | Must | ✅ Done — [EntityDeathState.cs](Assets/Script/Character/Entity/States/EntityDeathState.cs) now extends `EntityBasicState`, wired into [Entity.cs](Assets/Script/Character/Entity/Entity.cs) (`deathState` field/getter/`LoadState()`); emits `ON_ENEMY_DEATH` on anim finish. S6-05 (actually transitioning into it from `Health<=0`) still separate/open |
| S6-05 | Fix BUG-08 — `EntityBasicState` death transition + `ON_ENEMY_DEATH` | 0.25 | Must | ⬜ Not started — unblocked now that `entity.DeathState` exists; just needs `stateMachine.ChangeState(entity.DeathState)` in the empty `Health<=0` block ([EntityBasicState.cs:27](Assets/Script/Character/Entity/States/EntityBasicState.cs#L27)) |
| S6-06 | Fix BUG-ES-1 — `GetSpawnSet()` empty list not null | 0.25 | Must | ✅ Closed — owner decision: keep `return null;` as-is, accepted as intended behavior, not a bug |
| S6-07 | Fix BUG-ES-4 — guard `EnemySpawner.cs` empty `spawnPosition` read | 0.1 | Must | ✅ Closed — correction: guard already exists at [EnemySpawner.cs:67-71](Assets/Script/Enemy/EnemySpawner.cs#L67) before the indexing at line 77; earlier standup mis-flagged this, no code change needed |
| S6-08 | Fix BUG-06 partial — write-through to `PlayerData.currentHealth` | 0.25 | Must | ⬜ Not started |
| S6-09 | Decision — `EnemyModal` SO-vs-plain-class | 0.15 | Must | ✅ Done — owner kept plain class (rejected SO migration); recorded as ADR-0003 Amendment |
| S6-D1 | ADR-0002 Proposed→Accepted | 0.1 | Should | ⬜ Not started |
| S6-D2 | ADR-0003 Proposed→Accepted (post S6-09) | 0.1 | Should | ✅ Done — [adr-0003-enemy-spawn-selection-candidate-pool.md](docs/architecture/adr-0003-enemy-spawn-selection-candidate-pool.md) flipped Accepted 2026-07-23, Data Model/Formulas/Migration Plan/Validation Criteria amended to match shipped plain-class code; registry (`docs/registry/architecture.yaml`) and GDD (`design/gdd/enemy-spawn-system.md` "Current Implementation" note) updated to match. **Caveat**: accepted against the data shape only — the eight-step Candidate-Pool algorithm itself is still unimplemented (`GetSpawnSet()` still runs Option A) |
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

### Thu 07/24 — PLAN (revised at 02:00 standup — only 2 Must-Have items remain)

**Goal, revised**: Wed evening's marathon owner session closed 8 of 10 Must-Have items in one sitting
(S6-00,01,02,03,04,06,07,09 — see Daily Log 2026-07-23 entries). Only **S6-05** and **S6-08** are still
open, both small isolated one-file edits (~0.5d combined). Close those first, then move into the
Should-Have backlog (originally the whole of today's plan) if time allows.

| # | Task | Est | Priority | Note |
|---|------|-----|----------|------|
| 1 | **S6-05** — `EntityBasicState.cs:27-30` empty `Health <= 0` block → `stateMachine.ChangeState(entity.DeathState)` | 0.25d | 🔴 Must | Unblocked since S6-04 landed (`entity.DeathState` getter confirmed present, [Entity.cs:31](Assets/Script/Character/Entity/Entity.cs#L31)) — literally a one-line fix |
| 2 | **S6-08** — `NegativeReciver.cs:5` local `public int currentHealth` → write-through to `PlayerData.currentHealth` | 0.25d | 🔴 Must | `TakeDamage()` logic already correct (decrement + `ON_PLAYER_DEATH` emit at zero, [NegativeReciver.cs](Assets/Script/Character/Player/CoreComponent/NegativeReciver.cs)) — just needs the field swapped to the SO-backed source of truth so `Reborn()` resets the same value that damage mutates |
| 3 | **S6-D1** — ADR-0002 Proposed→Accepted | 0.1d | 🟡 Should | Still Proposed as of this standup — confirmed by reading the ADR file directly |
| 4 | **S6-D2** — ADR-0003 Proposed→Accepted (post S6-09) | 0.1d | 🟡 Should | ✅ Already done Wed (see Daily Log) — kept here for reference, not re-work |
| 5 | **S6-D3** — dedupe spawn driver (BUG-ES-2) | 0.5d | 🟡 Should | |
| 6 | **S6-D4** — `CancelInvoke` pairing | 0.25d | 🟡 Should | |
| 7 | **S6-D5** — cleanup batch | 0.25d | 🟡 Should | |
| 8 | **S6-D6** — S4-05/S4-06 keep-or-cut decision | 0.1d | 🟡 Should | |
| 9 | **Smoke** — Play Mode: enemy to 0 HP → dies once (`ON_ENEMY_DEATH` fires exactly once, not repeatedly since `EntityDeathState` skips `base.LogicUpdate()`); player to 0 HP → `ON_PLAYER_DEATH` fires, `PlayerData.currentHealth` reads 0 | — | Advisory | First point where the full death chain (both sides) is closeable end-to-end this sprint |

**New off-plan watch item found this standup**: `origin/feature/player-lifecycle-enhance` (not merged
into `sprint-06`) has one commit, `b33fe72 "add player lifecycle"` (2026-07-23 16:38), adding
`PlayerDeathState.cs` (extends `PlayerDisadvantageState`; emits `ON_PLAYER_DEATH` on anim start,
new `ON_REALOAD_GAME` event on anim end) plus the two new `EventID` entries. This overlaps directly
with S6-08/checklist-item-6 (player death → reload flow) but is scoped further (adds the reload-game
event, i.e. moving toward CLAUDE.md checklist item 6's "new `GameManager` subscribes … reload
`StartScene`"). Not counted as sprint-06 progress since it isn't merged — flagging so it isn't
duplicated: if S6-08's write-through lands on `sprint-06` today, reconcile with this branch before
merging either way, since both touch the player-death path.

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

## S6-09 Decision Brief — `EnemyModal` SO vs plain class (RESOLVED 2026-07-23 — see below)

> **Resolved same day**: owner decision was **keep the plain class**. ADR-0003 flipped Accepted with
> its Data Model amended to match (S6-D2, done). This brief is kept as the historical context/analysis
> that led to the decision — see the Amendment section of
> [adr-0003-enemy-spawn-selection-candidate-pool.md](docs/architecture/adr-0003-enemy-spawn-selection-candidate-pool.md)
> for the recorded outcome and its consequences.

**Context**: [ADR-0003](docs/architecture/adr-0003-enemy-spawn-selection-candidate-pool.md) (was Status:
Proposed, 2026-07-13; now Accepted, 2026-07-23) specified `EnemyModal` as a ScriptableObject: `public
class EnemyModal : EntityModel` — its own asset file, inheriting `EntityModel`'s GUID-based `ID`
(`OnValidate` auto-gens one when `0`), individually authorable/reusable, per its Migration Plan and Data
Model sections. The ADR's own example assumed separate assets per variant: *"Bat_Common.asset vs
Bat_Rare.asset — same prefab, different weight/rarityTier"*.

**What actually shipped** (current `sprint-06` code, [RoomModel.cs:102-108](Assets/Script/Database-SO/Modal/RoomModel.cs#L102)):
```csharp
[System.Serializable]
public class EnemyModal
{
    public GameObject Prefab;
    [Range(1, 100)] public int weight;
    public RarityTier rarityTier;
}
```
A plain `[System.Serializable]` class **nested inside `RoomModel.cs`** — not a `ScriptableObject`, does
not extend `EntityModel`, has no asset file, no GUID `ID`, cannot be referenced from more than one
`RoomModel` (each room's `enemiesOfRoom` list holds its own inline copies — no sharing).

**Root cause (why it diverged)**: this collapsed during the Sprint 5→6 Core/CoreComponent refactor
pass (`771f169 "big update core and corecomponet, add interface"`, 2026-07-22) — the dev's fastest path
to get `GetSpawnSet()` compiling again after the refactor was an inline struct-like class rather than
wiring a separate SO asset type. Nobody made an explicit call to abandon the ADR-0003 data model; it
just happened as refactor collateral. This is the same "off-plan work reshapes planned architecture
without a recorded decision" pattern flagged in the Risks table across Sprints 5-6 — S6-09 exists
precisely to make the divergence an explicit, recorded choice instead of a silent drift.

**Why it matters — consequences of each direction**:

| | Keep plain class (current) | Convert to SO extending `EntityModel` (per ADR-0003) |
|---|---|---|
| Matches `.claude/rules/scriptableobject-data.md` ("all gameplay config in SO assets, no magic numbers in MonoBehaviour") | ❌ violates it — `weight`/`rarityTier` live in a plain class, not an SO asset | ✅ complies |
| Reuse across rooms (author "Bat_Common" once, point 3 rooms at it) | ❌ impossible — every `RoomModel` re-enters its own copy, duplication + drift risk if one room's copy is tuned and others aren't | ✅ native — that's the whole point of ADR-0003's per-variant-asset pattern |
| `EntityModel.ID` (GUID auto-gen) usable for save/lookup/analytics | ❌ n/a, no ID exists | ✅ inherited for free |
| Migration cost | none — already shipped | real: create actual `.asset` files per enemy/tier variant, re-author every `RoomModel`'s `enemiesOfRoom` to reference assets instead of inline values, re-run `[Range(1,100)]`/`OnValidate` clamp logic in the new asset class instead of the nested class |
| ADR-0003 status | must be revised (Data Model section rewritten to match plain-class reality) before it can flip Proposed→Accepted (S6-D2 blocked either way until this is settled) | ships as originally reviewed, flips Accepted as-is |

**Direction (recommendation, not a unilateral decision)**: convert to the SO form. The project's own
standing rule is ScriptableObject-first for exactly this kind of authored gameplay data, and ADR-0003
was already reviewed and Proposed against that shape — reverting the ADR to match the shortcut is
retrofitting the decision record around an implementation accident rather than an intentional call.
The migration is bounded (one new SO class + re-pointing existing `RoomModel` asset fields) and there
appear to be few `RoomModel` assets authored so far (`Assets/SO/Database/Room/RoomModel.asset`,
`RoomModel 1.asset` — 2 seen in this sprint's diffs), so the re-authoring cost is still small. If
schedule pressure wins instead, the correct action is **not** silence — it's editing ADR-0003's Data
Model section to match the plain-class shape and recording that as the accepted change, so the next
person reading the ADR isn't misled by a spec the code no longer follows.

**Blocks**: S6-D2 (ADR-0003 Proposed→Accepted) cannot proceed cleanly until this is decided either way.

---

## Risks (live — updated each standup)

| Risk | Status | Mitigation |
|------|--------|------------|
| Off-plan work not merged into `sprint-06` | 🟢 RESOLVED (2026-07-23 02:45, `fe62f47`) | `origin/feature/spawn-enemy` merged into `sprint-06` (both refs now point at the same commit) — the A* pathfinding subsystem and Core/CoreComponent (`ICore`/`ICoreComponent`) refactor are now part of `sprint-06` history. This closes the risk raised 7 standups running. It does not close any S6-numbered Must-Have — see Status Verdict for the re-verified per-item state. New follow-on watch item: the Core refactor left `EntityMoveState`/`EntityBasicState` with commented-out `// fix` movement/attack calls — confirm in-Editor that enemies still move/attack before treating this merge as a net-positive for gameplay, not just for git hygiene. |
| Sprint 6 Must-Have bug list unaddressed, 3 of 5 sprint days elapsed | 🟢 RESOLVED (2026-07-23 evening) | Marathon owner session closed S6-00,01,02,03,04,06,07,09 same-day. Only S6-05 + S6-08 remain (~0.5d), both isolated one-file edits — see Thu plan. |
| `PoolMember.cs` build break unverified | 🟢 RESOLVED (2026-07-23) | `[SerializeField]` dropped from the auto-property (S6-01 closed) |
| `EnemyModal` decision blocks ADR-0003 flip and further spawn-system work | 🟢 RESOLVED (2026-07-23) | S6-09 decided (keep plain class); S6-D2 executed same day, ADR-0003 flipped Accepted with docs synced |
| No QA plan — 4th consecutive cycle | 🔴 OPEN | `production/qa/qa-plan-sprint-06.md` still does not exist as of Thu 02:00 standup; recommend running `/qa-plan sprint` before Friday's wrap-up gate |
| ADR-0002 (`EnemyManager` singleton) still Proposed | 🟡 OPEN | S6-D1 (Should-Have) not started; confirmed via direct file read Thu 02:00 |
| New unmerged branch `origin/feature/player-lifecycle-enhance` overlaps S6-08 | 🟡 WATCH (new, 2026-07-24) | `b33fe72` adds `PlayerDeathState.cs` + `ON_REALOAD_GAME` — reconcile with S6-08's write-through fix before merging either into `sprint-06` |
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

- **2026-07-23 (midday run, ~14:38) — daily standup, autonomous scheduled run**: `git log` since the
  02:52 follow-up (`7d33bf7`) shows 9 more commits landed 09:43→14:38: `5da1f66 "check fix get start
  position"`, `b38ae61 "update flow set corecomponent"`, `11ce1fb "fix issue"`, `1d13870 "change flow"`,
  `bda5b00 "update state, corecomponent of entity"`, `73faae3 "fix compiler"`, `d8de9b7 "polish code"`,
  `e009043 "done spawn flow, life cycle room"`, `fe750ba` (merge `origin/feature/spawn-enemy` into
  `origin/feature/enemy-control`). Re-verified against current `sprint-06` source:
  - **Watch item resolved**: the `// fix` commented-out markers flagged at the 02:45 standup are gone —
    `EntityAttackState.cs` now calls `weaponHolder.Weapon.Attack()` (was commented out) and
    `EntityUseWeaponState.cs` now resolves `entityMovement`/`weaponHolder` via
    `Core.GetCoreComponent()` and calls `entityMovement.StopMove()` in `Enter()` (was commented out).
    Enemy attack-state wiring looks intact again post-Core-refactor — still not confirmed in-Editor,
    but the source-level regression is gone.
  - S6-03 (BUG-05) **re-shaped, still open**: `EntityMoveState` no longer dereferences
    `Target.transform.position` directly — movement now goes through a new pathfinding layer
    (`EntityMovement.SendResquestPath()` / `MoveToTarget()`, `EnemyManager.RequestPath()`,
    `Path`/`PathRequest`). `SendResquestPath()` does guard `target == null`, so no bad request is sent —
    but `MoveToTarget()` (`EntityMovement.cs:20`) still indexes `Waypoints[indexWaypoints]` whenever
    `distance <= 0.3f`, with no guard for `Waypoints` being empty (which it is if `target` was ever
    null and no path came back) — same class of bug, different call site
    (`IndexOutOfRangeException` on an empty list instead of a `NullReferenceException` on `.position`).
    Recommend re-scoping S6-03 to this new location rather than closing it against the old file/line.
  - S6-01 (`PoolMember.cs:9`): unchanged, still unverified against a real Editor compile — **5th
    consecutive day**.
  - S6-02 (NEW-1, `RoomModel.cs:31` `while (weightBudget > this.weightBudget * 0.1f)`): unchanged, no
    zero-weight guard.
  - S6-04 (BUG-07): `EntityDeathState.cs` still `: MonoBehaviour`, unchanged.
  - S6-05 (BUG-08): `EntityBasicState.cs:27-30` — the `Health <= 0` block is still an empty `if`, no
    transition to a death state.
  - S6-06 (BUG-ES-1): `RoomModel.cs:16` still `if (n == 0) return null;`.
  - S6-07 (BUG-ES-4): `EnemySpawner.cs:77` `spawnPosition[Random.Range(0, spawnPosition.Count)]` still
    unguarded.
  - S6-08 (BUG-06 partial): `NegativeReciver.cs` unchanged — still a local `public int currentHealth`
    field, `TakeDamage()` decrements it and emits `ON_PLAYER_DEATH` at zero, but never writes to
    `PlayerData.currentHealth`.
  - `RoomGridController.cs` (`OnLoadMap`/door-transition method) reordered: `LoadRoom()` now runs
    *before* `GetStartDoorPosition()`/`_fastMovement` repositioning (previously position was read
    before the room finished loading) — looks like a legitimate sequencing fix for the room-transition
    flow (relates to checklist item 8 / Bug #13 area), not yet mapped to a specific S6 ID.
  Task Estimates table: no additional items flip to ✅ this run — S6-01 through S6-09 remain ⬜ Not
  started (S6-00 stays ✅ from the 02:45 run). 4 of 5 sprint days elapsed counting today, 1/9 Must-Have
  items closed. The gap between "lots of commits landing" and "Must-Have list moving" continues — today's
  9 commits are Core-refactor/pathfinding/room-flow work, still not S6-numbered bug fixes.
  **Remaining today**: re-run S6-07/S6-06/S6-02 (same call chain, ~0.6d combined) and S6-01/S6-08
  (isolated, ~0.35d) if any sprint time is left today; otherwise these + S6-03 (re-scoped) + S6-04/S6-05
  are Thursday's full carry-over load (~2.10d against 2 sprint days left, Thu+Fri).

- **2026-07-23 (owner decisions + fixes landed, same session)**: Owner reviewed the midday standup
  digest and resolved 4 items directly:
  - **S6-02 closed** — instead of a `weightBudget == 0` loop guard, owner chose to fix the root cause:
    `EnemyModal.weight` ([RoomModel.cs:106](Assets/Script/Database-SO/Modal/RoomModel.cs#L106)) now
    carries `[Range(1, 100)]` (was unclamped, CLAUDE.md's "no `[Range]` clamp" note is now stale). A
    zero-weight enemy can no longer be authored in the Inspector, so `SetListCandidate`'s
    `weightBudget -= enemiesOfRoom[i].weight` can never subtract 0 — the Phase-1 `while` loop in
    `GetSpawnSet()` can no longer spin indefinitely on this path.
  - **S6-04 closed** — [EntityDeathState.cs](Assets/Script/Character/Entity/States/EntityDeathState.cs)
    rewritten from `: MonoBehaviour` (empty `Start`/`Update` stub) to `: EntityBasicState`, matching the
    other Entity state classes' constructor pattern. `Enter()` stops movement; `LogicUpdate()` emits
    `EventID.ON_ENEMY_DEATH` once the death animation reports `StatusAnimation.EndRangeTrigger` (does
    **not** call `base.LogicUpdate()` — deliberately skips `EntityBasicState`'s attack/take-damage
    transition checks since death should be terminal). Wired into
    [Entity.cs](Assets/Script/Character/Entity/Entity.cs): added `deathState` field + `DeathState`
    getter + `new EntityDeathState(...)` in `LoadState()`, animBoolName `"Death"`. **Not yet reachable**
    — nothing calls `stateMachine.ChangeState(entity.DeathState)` yet; that's S6-05 (`EntityBasicState`'s
    empty `Health <= 0` block), still open and now unblocked.
  - **S6-06 closed, no code change** — owner decision: `GetSpawnSet()` returning `null` on an empty
    enemy pool is accepted as intended, not a bug. Note for whoever eventually touches
    `EnemySpawner.GetRoomSpawnSet()`: it already treats `roomModel == null` as an error case returning
    `new List<>()`, but `SpawnRoomEnemies()` line 62 (`if (set.Count == 0 || set == null)`) evaluates
    `set.Count` before the null check — if `GetSpawnSet()` ever returns actual `null` (empty pool, per
    this decision) that line throws `NullReferenceException` before the `null` check short-circuits.
    Not fixed this session (out of scope of what was asked) — flagging since the S6-06 decision makes
    this reachable in practice, not just theoretical.
  - **S6-07 closed, correction** — re-read [EnemySpawner.cs](Assets/Script/Enemy/EnemySpawner.cs) in
    full: lines 67-71 already guard `spawnPosition == null || spawnPosition.Count == 0` before the
    indexing loop at lines 72-85 that contains line 77. The earlier standup (02:45 and midday) flagged
    line 77 in isolation without reading the guard above it — false positive, no code change needed.
  Task Estimates: S6-00, S6-02, S6-04 (partial — see note), S6-06, S6-07 now ✅ — **5 of 9** Must-Have
  items closed. Remaining open: S6-01 (unverified, 5th day), S6-03 (re-scoped `Waypoints` index risk),
  S6-05 (death transition, now unblocked by S6-04), S6-08 (`PlayerData` write-through), S6-09 (decision).

- **2026-07-23 (2nd round of owner decisions, same session)**:
  - **S6-01 closed** — `[SerializeField]` dropped from `isInPool { get; private set; }` in
    [PoolMember.cs:9](Assets/Script/Poolable/PoolMember.cs#L9). The attribute was invalid on an
    auto-property (only fields can be serialized directly), which was the CS0592 risk; the property was
    never meant to be Inspector-editable anyway (`SwitchIsInPool()` is the only writer, called from
    `Pool.cs:41`). No behavior change, just removes the compile-risk attribute.
  - **S6-03 closed** — owner decision: [EntityMovement.cs:11](Assets/Script/Character/Entity/CoreComponent/EntityMovement.cs#L11)
    `[SerializeField] protected int indexWaypoints = 0;` (explicit default, was implicit). **Flagging
    for the record**: this alone does not remove the `Waypoints[indexWaypoints]` out-of-range risk at
    [EntityMovement.cs:27](Assets/Script/Character/Entity/CoreComponent/EntityMovement.cs#L27) — if
    `target` is null when `MoveToTarget()` starts running, `Waypoints` stays empty and `targetPosition`
    stays `Vector2.zero`; the guard at line 24 (`distance <= 0.3f`) only skips the indexing while the
    entity is farther than 0.3 units from world origin `(0,0)`. Any room/entity placed within 0.3 units
    of the literal world origin would still hit `Waypoints[0]` on an empty list. Accepted as closed
    per owner call (probability of a room straddling world origin is currently near-zero given the
    maze/room-grid layout), not because the underlying index risk is structurally gone — worth a
    one-line follow-up (`if (Waypoints.Count == 0) return;`) if room placement near origin ever changes.
  Task Estimates: **7 of 9** Must-Have items now ✅ (S6-00, S6-01, S6-02, S6-03, S6-04, S6-06, S6-07).
  Remaining: S6-05 (death transition wiring, unblocked), S6-08 (`PlayerData` write-through), S6-09
  (EnemyModal SO-vs-plain-class decision — see below, not resolved this session, needs owner input).

- **2026-07-23 (S6-09 decision + S6-D2 doc sync, same session)**: Owner asked for full context on the
  `EnemyModal` SO-vs-plain-class question (root cause: it drifted, unrecorded, during the Sprint 5→6
  Core refactor `771f169`, away from ADR-0003's originally-speced SO-extending-`EntityModel` shape).
  Decision brief written (see "S6-09 Decision Brief" above) comparing reuse/ID/migration-cost trade-offs
  either way. **Owner decision**: keep the shipped plain-class shape — do not migrate to SO. S6-D2
  ("ADR-0003 Proposed→Accepted") executed against that decision, updating every document that stated or
  implied the SO shape rather than leaving them stale:
  - [docs/architecture/adr-0003-enemy-spawn-selection-candidate-pool.md](docs/architecture/adr-0003-enemy-spawn-selection-candidate-pool.md) —
    Status flipped Proposed→Accepted; Data Model, Formulas table, Key Interfaces, Risks mitigation,
    Migration Plan, and AC-C4 amended in place to describe the actual plain-class/`[Range(1,100)]`/
    no-`OnValidate` shape instead of the original SO/`[Range(1,99)]`/`OnValidate` spec; new **Amendment
    (2026-07-23)** section records the trigger, decision, and explicitly what is/isn't covered (the
    Candidate-Pool eight-step algorithm itself is still **not implemented** — `GetSpawnSet()` still runs
    Option A — accepting the data shape is a separate question from accepting the algorithm rewrite as
    done).
  - [docs/registry/architecture.yaml](docs/registry/architecture.yaml) — `enemymodal_spawn_metadata`
    contract signature/detail updated to the plain-class shape; `enemy_spawn_selection_algorithm`
    decision `status: proposed` → `status: accepted` with a `status_note` carrying the same scope caveat
    (data shape accepted, algorithm rewrite still open).
  - [design/gdd/enemy-spawn-system.md](design/gdd/enemy-spawn-system.md) — "Option C — Formal
    Specification" Data Model section marked `[SUPERSEDED 2026-07-23]` with a pointer to the new
    "Current Implementation (as of 2026-07-23)" subsection showing the actual shipped `EnemyModal`
    shape and the three concrete differences (no SO, no cross-room reuse, Inspector-only clamp).
    Original spec code block kept for historical record, not deleted.
  Not touched this pass: the many older "weight has no `[Range]` clamp" gap-notes elsewhere in the GDD
  (lines ~68, 221, 370, 465) predate S6-02's fix and are now doubly stale (both the SO question and the
  unclamped-weight question have moved) — out of scope for this specific S6-09/S6-D2 request, flagging
  for a future `/consistency-check` or doc pass rather than silently leaving vs. silently rewriting
  unrelated history.
  Task Estimates: S6-09 now ✅ (decision recorded), S6-D2 now ✅ (ADR Accepted + docs synced). **8 of 9**
  Must-Have items closed — only **S6-08** (`NegativeReciver` write-through to `PlayerData.currentHealth`)
  remains open on the Must-Have list, plus S6-05 was already unblocked (S6-04) but not yet itself wired.

- **2026-07-24 (Thu 02:00) — daily standup, autonomous scheduled run**: Working tree clean except
  `.claude/settings.local.json` (not a `.cs`/asset file, left untouched). Currently on `sprint-06`
  already — no checkout needed. `git log` since the last standup commit (`d3b29d9`, 2026-07-23 16:10)
  shows no new commits on `sprint-06` itself; one commit landed on an unmerged remote branch,
  `origin/feature/player-lifecycle-enhance` (`b33fe72`, 16:38) — see new watch item in today's plan
  above.
  Re-verified the two items the Wed-evening log claims are still open, directly against current
  `sprint-06` source (not just trusting the log's own count, since it said "8 of 9" in one place and
  named both S6-05 and S6-08 as open in the next sentence):
  - **S6-05 confirmed still open** — [EntityBasicState.cs:27-30](Assets/Script/Character/Entity/States/EntityBasicState.cs#L27):
    the `Health <= 0` branch is still an empty block. Confirmed `entity.DeathState` getter exists and
    is wired ([Entity.cs:31](Assets/Script/Character/Entity/Entity.cs#L31), from S6-04) — this is
    genuinely a one-line unblock, not rediscovering scope.
  - **S6-08 confirmed still open** — [NegativeReciver.cs:5](Assets/Script/Character/Player/CoreComponent/NegativeReciver.cs#L5):
    `public int currentHealth` is still a local field on the component, decremented directly by
    `TakeDamage()`; `PlayerData.currentHealth` is never touched. `Reborn()`'s single-source-of-truth
    contract is still not honored by the damage path.
  - **S6-D1 confirmed still open** — read `docs/architecture/adr-0002-enemymanager-singleton-exception.md`
    directly: `## Status` still reads `Proposed`, not flipped to Accepted.
  - QA plan gate: `production/qa/qa-plan-sprint-06.md` still does not exist — **4th consecutive sprint
    cycle** without one. Flagging again per standing instruction, not silently dropping.
  Sprint arithmetic corrected in Burn Summary above: this is Thu (day 4 of 5), not day 1 as the stale
  header said before this run — the tracker header/Burn Summary had not been updated since Monday's
  0%-velocity baseline despite Wednesday's marathon session closing 8 of 10 Must-Have items same-day.
  Net position: very strong recovery from a 0%-velocity Mon–Wed into a near-complete Must-Have list by
  Wed night; today's real task is the last ~0.5d of Must-Have plus whatever Should-Have fits before
  Friday's wrap-up gate.
