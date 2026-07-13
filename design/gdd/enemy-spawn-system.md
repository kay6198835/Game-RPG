---
status: revised
source: owner spec (2026-07-08) + codebase audit (Assets/Script/Enemy, Map/Room, Manager)
date: 2026-07-08
revised: 2026-07-09 (Open Q#2 seed source resolved; Q#4 mechanism updated to RoomFile.roomData;
prior: 2026-07-08 post /design-review — 4 specialist passes; owner decisions logged)
revised: 2026-07-08 (post /design-review — 4 specialist passes; owner decisions logged)
revised: 2026-07-09 (reverse-synced to prototype code — Assets/Script/Database-SO/Modal + LevelManager.SpawnRoomEnemies)
revised: 2026-07-13 (second reverse-sync — code diverged further from the 2026-07-08 target rather
than converging on it; this revision makes the actual classes the primary reference and folds the
old target + a new candidate-pool proposal into Future Architecture Direction, evaluated together)
revised: 2026-07-13 (Option C locked — owner session resolved Open Q#8: Room Budget + Candidate Pool
+ `RarityTier` enum chosen over the original `Spawn Chance` float sketch; added the 8-step selection
flow, Formulas, Edge Cases, and Acceptance Criteria for Option C under Future Architecture Direction;
S5-A3's planned `weight`→`cost` rename is dropped)

verified-by: Kiet
supersedes: map-system.md "Agreed spawn architecture (2026-07-02) [PLANNED]" (EncounterSO + RoomEnemySpawner)
---

# Enemy Spawn & Per-Room Management System

**Status**: Approved (design) · Prototype partial — **code has diverged from the 2026-07-08 target,
not converged on it** (see Doc-sync note + Current Implementation)
**Implements Pillar**: Room-clear progression · "each run is a fresh challenge" (run-to-run variety)

> **Architecture decision (2026-07-08):** This GDD originally adopted a data-driven **weight-budget**
> model (idealized `EnemyData`/`EnemyDatabase`/`RoomData`/`MapEnemyDatabase` SOs + `GetHybridEnemySet`)
> and superseded the earlier `EncounterSO` + `RoomEnemySpawner` plan sketched in `map-system.md`. That
> supersession still holds. What changed 2026-07-13: the idealized class names were **never
> implemented** — the classes actually built (`EntityModel`/`EnemyModal`/`MapModel`/`RoomModel`) took a
> different shape and kept evolving independently after the 2026-07-09 sync. This revision makes the
> **real code the primary reference**, per project convention (docs follow code, not the reverse).

> **Doc-sync note (2026-07-13):** Second reverse-sync of this GDD (first: 2026-07-09). Between the two
> syncs the prototype moved **further from**, not closer to, the 2026-07-08 target:
> `RoomModel.GetSpawnSet()` was rewritten for zero-alloc (scratch buffers) but kept `UnityEngine.Random`
> and dropped the `argmin` idea entirely in favor of uniform-random-pick-from-fit-group in both phases;
> `MapModel` gained a `fullRoomList` + `GetRandomRoom()` bag-draw of whole `RoomModel` presets, replacing
> its earlier `idRooms`/`totalWeight` fields and superseding the `RoomFile.roomData` per-room mapping
> this GDD previously marked **RESOLVED** (that field was never added to `RoomFile` — see
> Room→RoomModel Resolution below); and the `Tile_Spawn_Enemy` marker parser this GDD previously listed
> as **not built** now exists in `RoomGeneraterController.LoadRoom()`. None of this was captured until
> this pass — see `production/epics/enemy-spawn/EPIC.md` and `docs/tech-debt-register.md` TD-031 for
> the parallel updates made alongside this one.

---

## Current Implementation (2026-07-13) — authoritative for "what is built"

> This section documents the **actual system running in the repo today**, using the project's real
> class names. It is the primary description of this system. The 2026-07-08 idealized target
> (`EnemyData`/`EnemyDatabase`/`RoomData`/`MapEnemyDatabase`, injected-RNG `argmin` selection,
> `RoomFile.roomData` mapping) was never built and is kept only as historical context inside
> **Future Architecture Direction**, alongside a newer candidate-pool proposal evaluated on the same
> footing.

**ScriptableObjects** (`Assets/Script/Database-SO/Modal/`, note the `Modal` typo for "Model"; all
extend `EntityModel`):

| Class (code) | Role | Fields | Notes |
|--------------|------|--------|-------|
| `EntityModel` (base) | Id + display name for any spawn-database asset | `id` (int, private, `ID` getter), `nameEnity` (string) | `OnValidate` sets `id = Math.Abs(Guid.NewGuid().GetHashCode())` when `id == 0`. ⚠️ No reroll-if-zero loop, no public `EnsureId()` test hook — unchanged since 2026-07-09. |
| `EnemyModal` | One concrete enemy type | `prefab` (GameObject), `weight` (int) | ⚠️ `weight` still has **no `[Range(1,99)]`/clamp** — a `0` or negative weight is still authorable in the Inspector and would make `RoomModel.GetSpawnSet()`'s fit-loop spin forever on that candidate. Unchanged since 2026-07-09. |
| `MapModel` | A pool of interchangeable room-difficulty presets | `fullRoomList` (List\<`RoomModel`\>, direct refs) + runtime-only `_pool` | **Changed since 2026-07-09** — no longer holds `idRooms`/`totalWeight`. `GetRandomRoom()` draws one `RoomModel` from `_pool` without replacement, refilling `_pool` from `fullRoomList` whenever it empties (a shuffle-bag). This makes `MapModel` a **room-profile draw pool**, not an enemy-id set scoped to a map — a different role than either the original design or the 2026-07-09 snapshot described. |
| `RoomModel` | One room's enemy-selection config **+** the selection engine | `enemiesOfRoom` (List\<`EnemyModal`\>, direct refs), `weightBudget` (`[Range(0,500)]`), `selectionWeight` (`[Range(1,100)]`, **new**), `randomRatio` (`[Range(0,1)]`, default `0.33`), `overflowPercent` (`[Range(0,1)]`, default `0.1`) | ⚠️ `selectionWeight` is declared "for `MapModel.GetRandomRoom`" (per its own code comment) but `GetRandomRoom()` actually does a uniform `Random.Range(0, _pool.Count)` — **the field is read nowhere**, dead weight in the Inspector. Selection logic still lives here, not on a central database (none exists). |

**Selection algorithm — `RoomModel.GetSpawnSet()`** → `List<EnemySpawnEntry>` (`{enemy, count}`).
Rewritten since 2026-07-09 for zero per-call allocation (reusable `_entries`/`_fitBuf` scratch arrays,
resized only when the enemy-type count changes):
- **Phase 1 (random, bounded by `randomBudget = weightBudget × randomRatio`):** loop — build the set
  of candidate indices whose `weight` fits the *remaining random sub-budget*; if none fit, stop; else
  `Random.Range` pick one uniformly, add it (repeats of the same enemy allowed), shrink both the
  random sub-budget and the overall `remaining`.
- **Phase 2 (fill, bounded by `remaining + overflowCap` where `overflowCap = weightBudget × overflowPercent`):**
  same loop shape against the wider threshold, continues until `remaining <= 0` or nothing fits.
- Both phases pick **uniformly at random** among whatever currently fits — there is no shuffle-once
  pass and no `argmin`/optimal-fill step. This is a real behavior change from the 2026-07-09 snapshot
  (which described Phase 1 as a single shuffled walk) — worth noting because it means the *current*
  code already resembles a repeated "gather eligible → pick one → shrink budget → repeat" loop, which
  is structurally close in spirit to the candidate-pool idea evaluated in Future Architecture Direction.
- RNG is still `UnityEngine.Random` (static), not an injected `System.Random` — determinism is still
  unreachable as-is (unchanged finding from 2026-07-09).
- `GetSpawnSet()` returns **`null`** (not an empty list) when `enemiesOfRoom.Count == 0` — this is
  **BUG-ES-1**, still unguarded by either caller (see Runtime drivers below).

**Runtime drivers — two parallel, both still live:**
1. **`EnemySpawner.cs`** (`Assets/Script/Enemy/`) — **no longer an empty stub.** `OnEnable`/`OnDisable`
   subscribe/unsubscribe `EventID.ON_GET_SPAWN_POSITIONS` → `OnDoneLoadRoomGrid(object obj)`. That
   handler casts `obj` to `List<Vector2Int>` (the marker positions — see Tile_Spawn_Enemy below), draws
   `roomModel = mapModel.GetRandomRoom()` from the bag, then calls `SpawnRoomEnemies()`, which
   `Instantiate`s each `EnemySpawnEntry.enemy.prefab` at a random position drawn from the marker list.
   `GetRoomSpawnSet()` null-checks `roomModel` but **not** the return of `roomModel.GetSpawnSet()`
   (BUG-ES-1 live here). An unused public `Spawn()` method (empty body) is still present — dead code.
2. **`LevelManager.SpawnRoomEnemies()`** — the original Editor-button driver, unchanged in spirit:
   its own `GetRoomSpawnSet()` has the same BUG-ES-1 gap, and it spawns at
   `transform.position + Random.insideUnitCircle * spawnRadius` instead of marker positions. Both
   drivers read a `[SerializeField] RoomModel roomModel` field independently — **BUG-ES-2** (duplicate
   spawn driver, neither routes through `EnemyManager`) is unchanged.

**`EnemyManager.cs`** — unchanged since 2026-07-09, still exactly:
```csharp
public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }
}
```
No `Awake`, no lifecycle, no alive-count, no event subscriptions. The unused `using System.Numerics;`
(BUG-EM-2) is also still present, though still harmless today since no `Vector3` is referenced in the
file yet.

**`Tile_Spawn_Enemy` marker parsing — now implemented (was "not built" as of 2026-07-09).**
`GameConstants.TileName.SPAWN = "Tile_Spawn_Enemy"`. `RoomGeneraterController.LoadRoom()` has a branch
(alongside its existing door-tile branch) that, for an uncleared room, appends every
`Tile_Spawn_Enemy` tile's position to a `spawnPositions` list, then emits
`EventManager.Emit(EventID.ON_GET_SPAWN_POSITIONS, spawnPositions)` — this is what `EnemySpawner`
subscribes to. **Only 1 of the 13 room JSONs (`NormalRoom_0.json`) currently authors a
`Tile_Spawn_Enemy` tile**; the other 12 emit an empty `spawnPositions` list. There is **no fallback**
for that case — `EnemySpawner.SpawnRoomEnemies()` indexes `spawnPosition[Random.Range(0, spawnPosition.Count)]`
directly, which throws on an empty list. This is a **new, unrecorded edge case** (not the same as
BUG-ES-1, though it fails the same way): loading any of the 12 marker-less rooms while an
`EnemyModal` pool is configured for it would throw at spawn time.

**`RoomCell.cs`** has `CloseDoor()`/`OpenDoors()` (the door-lock contract ADR-0002 expects) and
`ClearRoom()`. It does **not** have a `GetSpawnPosition()` method — the 2026-07-09 snapshot's claim
that one exists (calling an undefined `GetRoomSpawnSet()`) does not match current code; spawn
positions flow directly from `RoomGeneraterController` to `EnemySpawner` via the event payload,
bypassing `RoomCell` entirely. `RoomCell` has no alive-count field of any kind.

**`DungeonRoomSO.RoomFile`** (`Assets/SO/Dungeon/DungeonRoomSO.cs`) still has only `roomName`,
`filePath`, `roomType` — **no `roomData` field was ever added.** The 2026-07-09 revision of this GDD
marked the `RoomFile.roomData` direct-reference mechanism "RESOLVED"; that was aspirational, not a
report of shipped code, and is corrected below in Room→RoomModel Resolution.

**`EventID` enum** (`EventManager.cs`): `ON_PLAYER_ON_DOOR`, `ON_LOAD_MAZE_DONE`, `ON_LOAD_MAP`,
`ON_CLEAR_ENEMY`, `ON_GET_SPAWN_POSITIONS`, `ON_TEST`. Still **no** `ON_ENEMY_DEATH`, `ON_ROOM_CLEAR`,
or `ON_PLAYER_DEATH` (BUG-ES-3, unchanged).

**Still not built at all:** any room-combat lifecycle (lock-on-entry, alive-count, clear detection),
`EnemyManager`'s actual body, entry-safety/jitter/round-robin placement, and any mechanism that ties a
specific physical room to a specific enemy difficulty profile (the bag-draw in `MapModel` is
per-spawn-event random, not per-room-identity).

---

## Overview

The enemy-spawn system decides **which enemies appear in each room and instantiates them**. The
room-combat lifecycle this Overview originally promised (lock doors on entry, track alive count,
unlock on clear) is still the intended end state, but is **entirely unbuilt today** — see Current
Implementation above.

What exists is a **data layer** plus two ad-hoc runtime drivers:

- **Data (definition only, no logic):** `EnemyModal` (one enemy type: id, name, prefab, weight)
  and `RoomModel` (a room-difficulty preset: an enemy pool + a `weightBudget` dial + selection
  logic — see below).
- **Selection logic:** lives directly on `RoomModel.GetSpawnSet()` — there is no separate
  project-wide database/lookup class.
- **Room→preset assignment:** `MapModel` — a shuffle-bag of `RoomModel` presets, drawn at random
  each time a room emits its spawn-position event (not tied to which physical room it is).
- **Runtime drivers (two, duplicated):** `EnemySpawner` (event-driven, off `ON_GET_SPAWN_POSITIONS`)
  and `LevelManager.SpawnRoomEnemies()` (Editor-button-driven). Neither is `EnemyManager` — that
  class exists but has no body yet.

The player never touches this system directly; they are meant to **feel it** as the pacing and
variety of each room — a different, budget-appropriate mix of enemies every run, and eventually the
tension of doors sealing until the room is clear. Today only the "different mix of enemies"
half is reachable in Play Mode; the door-seal/clear half needs `EnemyManager` built first.

---

## Player Fantasy

Each room should feel like a **fresh, fair arena**. The doors seal behind you, a mix of enemies is
waiting, and you have to read the room and fight your way out. Because the mix is chosen against a
weight budget with a random component, **two runs of the "same" room rarely play identically** —
sometimes a swarm of cheap enemies, sometimes a couple of heavy ones — which keeps the roguelike
loop fresh and rewards adaptation over memorization.

The designer's fantasy matters too: tuning a room's difficulty is a **single number**
(`weightBudget`), not a hand-placed enemy list — so balancing the dungeon's difficulty curve is
fast and legible.

> **Known limitation, updated 2026-07-13:** the 2026-07-08 note here warned that a planned greedy
> `argmin` Phase-2 fill would favour *fewer, heavier* enemies. That specific algorithm was never
> built — `RoomModel.GetSpawnSet()`'s actual Phase 2 picks **uniformly at random** among whatever
> currently fits, which is a different (likely less biased, but **not verified**) distribution. No
> variety data has been collected either way. The full swarm-vs-heavy variety this Player Fantasy
> promises remains **unverified**, not confirmed working. A composition-diversity pass (a comparison
> phase that decides *what kind* of cluster to build — e.g. "10 creeps vs 1 elite, which is the more
> interesting encounter here?") is still a **Should-Have** enhancement — see Future Architecture
> Direction, which also evaluates a `Spawn Chance`-based candidate-pool alternative that targets this
> same goal directly. Do not over-invest in variety tuning until one direction is chosen.

---

## Detailed Design

### Core Data Model

See **Current Implementation** above for the authoritative field-by-field table. Summary:
`EntityModel` (base: id + name) → `EnemyModal` (one enemy type: prefab + weight) and `RoomModel`
(one difficulty preset: enemy pool + `weightBudget`/`randomRatio`/`overflowPercent` + the selection
method itself) → `MapModel` (a shuffle-bag of `RoomModel` presets). There is **no** central
project-wide enemy database and **no** id-based lookup anywhere in the runtime path — every
reference (`RoomModel.enemiesOfRoom`, `MapModel.fullRoomList`) is a direct Unity object reference.
Whether to introduce one is evaluated in **Future Architecture Direction**, not assumed here.

**Design invariants that hold today:**
- `EnemyModal` is **spawn metadata only**. Combat stats (HP, damage, defense…) live on the enemy
  prefab's existing `EntityData` / `StatsSO` (per `stat-system.md`). `EnemyModal` must **not**
  become a fourth stat store. `weight` is a spawn-budget cost, unrelated to the stat formula —
  though by convention it should track the enemy's power tier (creep < elite < champion).

**Design invariants that do NOT hold today (still just documentation intent):**
- ~~`weight` must be `≥ 1`, enforced~~ — `EnemyModal.weight` has no `[Range]`/clamp. `RoomModel`'s
  `OnValidate` only logs a warning for `weight <= 0`, it does not clamp or block the value. A
  `0`-or-negative `weight` asset can ship today and would hang `GetSpawnSet()`'s fit-loop.
- ~~ids are the only cross-reference mechanism~~ — every reference in the actual data model is a
  direct object reference; there is no id-based indirection anywhere yet.

### `RoomModel.GetSpawnSet()` — the actual selection method

This is the real, currently-running algorithm (see Current Implementation for the code-level walk).
Restated as design intent:

Runs **once** per call (no trial loops, no re-rolling on a bad outcome). Operates directly on
`enemiesOfRoom` (no id resolution step — the list already holds live references). Two phases,
repetition of the same enemy type allowed in both. Returns a `List<EnemySpawnEntry>` (`{enemy,
count}` pairs, not a flat list — this is a real difference from the 2026-07-08 target's
`List<EnemyData>` shape and is more memory-efficient for repeated picks of the same type).

**Phase 1 — RANDOM (variety), bounded by a sub-budget:**
1. `randomBudget = weightBudget × randomRatio`.
2. Loop: gather every candidate whose `weight` fits the *remaining* random sub-budget; if the set
   is empty, stop Phase 1. Otherwise pick one **uniformly at random** (`UnityEngine.Random.Range`,
   not seedable), add it, shrink both the sub-budget tracker and the overall `remaining`.
3. No shuffle-once pass exists — the fit-set is rebuilt and re-rolled every iteration instead
   (functionally similar outcome, different implementation shape from the original design).

**Phase 2 — FILL, bounded by budget-plus-overflow:**
1. `remaining` continues from wherever Phase 1 left it (may already be `≤ 0`, in which case Phase 2
   naturally does nothing since the fit-set will always be empty).
2. `overflowCap = weightBudget × overflowPercent`.
3. Loop: gather every candidate whose `weight` fits `remaining + overflowCap`; if empty, stop.
   Otherwise pick one **uniformly at random**, add it, shrink `remaining`.
4. There is **no `argmin`/optimal-fill step and no deterministic tie-break** — both phases use the
   same "gather eligible, pick uniformly, shrink budget, repeat" shape. This is a real behavioral
   change from what this GDD described as of 2026-07-09 (which still assumed a planned `argmin`
   Phase 2 that had not yet been built); it has now not been built in a different direction instead.

**Termination:** relies on every added candidate strictly reducing `remaining` (or the sub-budget
tracker) by at least its `weight`. This is **not guarded** — a `weight <= 0` candidate would make a
phase's loop non-terminating, since nothing enforces `weight ≥ 1` at any layer (see Edge Cases).

**Determinism:** none. `UnityEngine.Random` is a static, un-seedable-from-outside source in this
codebase's usage — there is no way to reproduce a given room's roll, and no EditMode test can drive
this method deterministically today.

### States and Transitions (per-room combat lifecycle) — **[PLANNED — not in code yet]**

> The lifecycle below is **not implemented**. Today spawning is a manual Editor action
> (`LevelManager.SpawnRoomEnemies()` via the "Spawn Enemy" button) with no state machine, door
> lock, alive-count, or events. `EnemyManager` is the ratified runtime driver (ADR-0002) but has
> not been written.

`EnemyManager` (singleton — ratified in ADR-0002) would drive the room state:

| State | Enter condition | On enter | Exit condition |
|-------|-----------------|----------|----------------|
| `Idle` | boot / between rooms | nothing | `ON_LOAD_MAP` received |
| `Populating` | `ON_LOAD_MAP` for an uncleared room | resolve a `RoomModel` for the room → `GetSpawnSet()` → instantiate at spawn markers → `RoomCell.CloseDoor()` (lock) → set `aliveCount` | spawn complete |
| `Fighting` | spawn complete, `aliveCount > 0` | listen for `ON_ENEMY_DEATH`, decrement `aliveCount` | `aliveCount == 0` |
| `Cleared` | `aliveCount == 0` (or room spawns 0 enemies) | `EventManager.Emit(ON_CLEAR_ENEMY)` (existing subscriber opens doors) + `Emit(ON_ROOM_CLEAR)`; mark `RoomCell.IsCleared = true` | room re-entered while cleared → stay `Cleared` (no re-lock, no re-spawn) |

A room that is **already cleared** on `ON_LOAD_MAP` would skip `Populating` entirely (doors stay
open). None of this table is implemented — `EnemyManager` has no `Awake`, no state, no subscriptions.

### Room → `RoomModel` Resolution (Open Q#4 — **REOPENED 2026-07-13**, was marked resolved in error)

> The 2026-07-09 revision of this section described a `RoomFile.roomData` direct-reference field as
> "RESOLVED" and implemented. **That field does not exist in `DungeonRoomSO.cs` today** — it was a
> design decision that was never carried into code. What actually shipped is a different mechanism,
> described below. Open Question #4 is reopened because the two are not equivalent and the actual
> one has a real gameplay consequence worth an explicit owner decision.

**What actually resolves a room's enemy set today: `MapModel.GetRandomRoom()`, a shuffle-bag.**
`MapModel.fullRoomList` holds every `RoomModel` preset available to a map. `GetRandomRoom()` draws
one at random from a runtime `_pool` (no replacement), refilling `_pool` from `fullRoomList`
whenever it runs dry. `EnemySpawner.OnDoneLoadRoomGrid()` calls this **every time any room emits**
`ON_GET_SPAWN_POSITIONS`, and assigns the result to `roomModel` for that spawn.

**Consequence — this decouples the enemy-difficulty profile from the room's identity entirely.**
Under the (unbuilt) `RoomFile.roomData` design, a specific physical room (e.g. `NormalRoom_3`)
would always resolve to the same `RoomModel` preset (or a fixed fallback by `RoomType`), so a level
designer could hand-tune "this room is easy, that one is hard." Under the shuffle-bag that actually
runs, **which preset a room gets is unrelated to which room it is** — the same physical room could
draw a trivial preset one run and a punishing one the next, and there is no way today to guarantee,
say, the start room always gets a zero-budget preset (see Edge Cases). This is a materially
different design than what this GDD previously described as decided.

**Not evaluated yet:** whether the shuffle-bag is an intentional simplification worth keeping (it is
arguably simpler and still delivers "different mix each run") or a regression from the per-room
authoring control the original design wanted. This is folded into the open decision in Future
Architecture Direction rather than resolved here — see Open Question #4b.

### Interactions with Other Systems

| Direction | System | Interface |
|-----------|--------|-----------|
| in | Event Bus | `EnemySpawner`/`LevelManager` subscribe or are triggered by `ON_GET_SPAWN_POSITIONS` (built) — `EnemyManager` subscribing `ON_LOAD_MAP`/`ON_ENEMY_DEATH` is still **planned**, not built |
| out | Event Bus | emits `ON_CLEAR_ENEMY` (existing enum value, not yet emitted by any spawn-system code) and `ON_ROOM_CLEAR` (**planned**, not in `EventID` yet) |
| out | Room Progression | `RoomCell.CloseDoor()`/`IsCleared` exist and are callable, but nothing in the spawn path calls them yet |
| in | Enemy AI | enemy death chain must emit `ON_ENEMY_DEATH` (requires Bugs #7/#8 fixed, and the event doesn't exist yet either); spawns framework-wired enemy prefabs |
| in | Dungeon / Room load | `RoomGeneraterController` parses `Tile_Spawn_Enemy` markers and emits `ON_GET_SPAWN_POSITIONS` with their positions — **built**, 1/13 rooms authored |
| out | Per-Run Upgrades | `ON_ROOM_CLEAR` would open the upgrade-card screen — consumer and event both unbuilt |

---

## Formulas

> Rewritten 2026-07-13 to match `RoomModel.GetSpawnSet()` as actually implemented. The `argmin`
> optimal-fill formula previously here described a Phase 2 that was never built; the real Phase 2
> is a uniform-random pick, formalized below.

### Random sub-budget

`randomBudget = weightBudget × randomRatio`

### Overflow cap

`overflowCap = weightBudget × overflowPercent`

### Phase 1 pick (each iteration)

`fitSet = { c ∈ enemiesOfRoom | c.weight ≤ randomBudget − usedRandom }`
`pick = uniform_random(fitSet)` — every eligible candidate has equal chance, not weighted by cost.
Stop when `fitSet` is empty. `usedRandom ← usedRandom + pick.weight`; `remaining ← remaining − pick.weight`.

### Phase 2 pick (each iteration)

`fitSet = { c ∈ enemiesOfRoom | c.weight ≤ remaining + overflowCap }`
`pick = uniform_random(fitSet)` — **no `argmin`, no tie-break rule** (removed 2026-07-13; the actual
code never had one). Stop when `fitSet` is empty or `remaining ≤ 0`.
`remaining ← remaining − pick.weight` after each selection.

**Variables:**

| Variable | Type | Range | Description |
|----------|------|-------|-------------|
| `weightBudget` | int | 0 – 500 (`RoomModel.weightBudget` `[Range]`) | Total spawn budget for the room (the difficulty dial) |
| `randomRatio` | float | 0.0 – 1.0 (`RoomModel.randomRatio` `[Range]`, default 0.33) | Fraction of budget spent in the random phase |
| `overflowPercent` | float | 0.0 – 1.0 (`RoomModel.overflowPercent` `[Range]`, default 0.1) | Max fraction of budget total spend may exceed `weightBudget`. No separate "safe" recommendation is enforced in code — `[Range(0,1)]` is the only guard |
| `randomBudget` | float | 0 – `weightBudget` | Budget available to Phase 1 |
| `overflowCap` | float | 0 – `weightBudget` | Absolute overshoot allowed in Phase 2 |
| `remaining` | int | ≤ `weightBudget` (may go slightly negative within `overflowCap`) | Budget left to spend |
| `weight` (per `EnemyModal`) | int | **unbounded — no `[Range]`, no clamp** | Spawn cost of one enemy type. `RoomModel.OnValidate` only warns on `weight <= 0`, does not block it |

**Output Range:** a list of `EnemySpawnEntry` (`{enemy, count}`), 0 – `weightBudget` total enemy
instances (fewer when weights are large). Total spend ∈ `[0, weightBudget + overflowCap]` **when
`weight ≥ 1` holds** — unverified today since nothing enforces that invariant.

**Worked example** (illustrative — actual outcome varies run to run since both phases are uniform
random, not deterministic): `weightBudget = 20`, `randomRatio = 0.5`, `overflowPercent = 0.1`;
candidates: Bat(w3), Rat(w2), Golem(w9).
- `randomBudget = 10`. Fit set = {Bat, Rat, Golem} (all ≤ 10). Suppose Golem is drawn:
  `usedRandom = 9`, `remaining = 11`. Next fit set = {Rat} (Bat's 3 > 1 remaining sub-budget, Golem's
  9 > 1) — draw Rat: `usedRandom = 11`, `remaining = 9`. Fit set now empty (Bat's 3 > −1 remaining
  sub-budget) → Phase 1 stops.
- Phase 2: `remaining = 9`, `overflowCap = 2` → threshold 11. Fit set = {Bat, Rat, Golem} (all ≤ 11).
  Suppose Bat is drawn: `remaining = 6`. Fit set still all three → suppose Rat: `remaining = 4`. →
  suppose Bat again: `remaining = 1`. Fit set = {Bat? no, 3 > 1+2=3, borderline-eligible} — depends
  on the exact draw; loop continues until the fit set empties.
- Result varies by run: composition is no longer reproducible from these inputs alone the way the
  old `argmin` design would have been.

---

## Edge Cases

> Legend: **[ACTUAL]** — this is what the current code really does, verified by reading it.
> **[PLANNED]** — this is the target behavior, not yet built; treat as a requirement for whoever
> implements it.

- **If `enemiesOfRoom` is empty: [ACTUAL, BUG-ES-1]** `RoomModel.GetSpawnSet()` returns **`null`**,
  not an empty list. Neither `EnemySpawner.GetRoomSpawnSet()` nor `LevelManager.GetRoomSpawnSet()`
  guards against this — both pass the result straight to a `.Count` read, which throws a
  `NullReferenceException`. **[PLANNED fix]:** return an empty `List<EnemySpawnEntry>` instead of
  `null`, and treat a resulting empty spawn as "room immediately cleared" once `EnemyManager` exists.
- **If `weightBudget ≤ 0`: [ACTUAL]** both phases' fit-sets are naturally empty from the first
  iteration (nothing fits a non-positive budget), so `GetSpawnSet()` already degrades safely to
  "spawn nothing" without special-case code. No explicit early-return exists, but the loop shape
  happens to produce the right outcome.
- **If any candidate's `weight ≤ 0`: [ACTUAL — unguarded, real hang risk]** `RoomModel.OnValidate`
  only **logs a warning** ("Pha 2 loop vô hạn") — it does not clamp or strip the value. A `0` or
  negative `weight` asset compiles, ships, and is authorable in the Inspector today. If one is ever
  referenced in `enemiesOfRoom`, that candidate is eligible in every iteration of whichever phase it
  qualifies for and never reduces `remaining`/the sub-budget — the fit-loop **never terminates**.
  **[PLANNED fix]:** enforce `weight ≥ 1` with `[Range(1,99)]` + an `OnValidate` clamp on
  `EnemyModal`, not just a warning on the containing `RoomModel`.
- **If every candidate's `weight` exceeds both phases' thresholds: [ACTUAL]** both fit-sets end up
  empty immediately; `GetSpawnSet()` returns whatever was collected so far (possibly an empty,
  non-null list if Phase 1 also collected nothing). No balance warning is logged for this case.
- **If `randomRatio = 0` or `= 1`: [ACTUAL]** both are legal `[Range(0,1)]` values and the loop
  shapes handle them the same way as any other ratio — no special-casing needed or present.
- **Duplicate/zero `id` across `EnemyModal` assets: [PLANNED, not applicable today]** there is no
  central database performing id-based lookups, so there is nothing to validate against yet. Every
  reference is a direct object reference; a "duplicate" `EnemyModal` is just two separate assets, not
  a lookup collision.
- **If an `EnemyModal.prefab` is null: [ACTUAL]** both `SpawnRoomEnemies()` implementations skip the
  entry (`if (entry.enemy == null || entry.enemy.prefab == null) continue;`) — this one guard **is**
  present and correct in both drivers today.
- **Placement (round-robin balance, entry-safety, jitter): [PLANNED — none of this exists]**
  `EnemySpawner.SpawnRoomEnemies()` picks a **uniformly random** position from the marker list for
  every enemy instance independently — no balancing, no distance-from-door check, no jitter. Multiple
  enemies can and will stack on the same marker; nothing prevents a spawn next to the entry door.
- **If a room has no `Tile_Spawn_Enemy` markers: [ACTUAL, new unrecorded bug]** there is **no
  fallback**. `EnemySpawner.SpawnRoomEnemies()` indexes directly into the (possibly empty)
  `spawnPosition` list and throws. This affects 12 of the 13 room JSONs today (only
  `NormalRoom_0.json` has a marker authored). **[PLANNED fix]:** fall back to room-centre and log a
  warning, as originally specified, before any placement math runs.
- **If `ON_ENEMY_DEATH` fires for an untracked enemy: [PLANNED, not applicable today]** the event
  doesn't exist yet and nothing tracks `aliveCount`.
- **Determinism for tests: [PLANNED, not reachable today]** `UnityEngine.Random` cannot be seeded
  from outside `RoomModel.GetSpawnSet()`'s call site, so no EditMode test can currently drive this
  method deterministically — see Future Architecture Direction for the injected-RNG option.

---

## Dependencies

| System | Role | Direction | Status |
|--------|------|-----------|--------|
| **Event Bus** (`EventManager`) | `ON_GET_SPAWN_POSITIONS` ✅ (built, drives current spawn), `ON_LOAD_MAP` ✅ (exists, not yet consumed by spawn code), `ON_ENEMY_DEATH` **[new, not added]**, `ON_CLEAR_ENEMY` ✅ (exists, not yet emitted by spawn code), `ON_ROOM_CLEAR` **[new, not added]** | Spawn ↔ EventManager | 2 new `EventID` values still needed |
| **Enemy AI** (`character-system.md`) | Death chain must emit `ON_ENEMY_DEATH`; spawns framework-wired enemy prefabs | Enemy → Spawn | **Blocked:** Bug #7 (`EntityDeathState` wrong base), Bug #8 (empty `Health<=0` transition); only `EnemyPrefab.prefab` wired (Bat/Crab broken) |
| **Room Progression** (`map-system.md`) | `RoomCell.CloseDoor()` / `OpenDoors()` for lock/unlock; `IsCleared` set on clear; room→preset mapping — see Room→RoomModel Resolution | Spawn → Map | `CloseDoor`/`OpenDoors` ✅ exist but nothing in the spawn path calls them yet. **No per-room mapping exists** — `MapModel.GetRandomRoom()`'s shuffle-bag is unrelated to room identity (see Room→RoomModel Resolution); `RoomFile.roomData` was never added to `DungeonRoomSO.cs`. map-system Bug #16 (`RoomFile.roomType` not read at runtime) is a live blocker again if a per-room mapping is chosen |
| **Dungeon / Room load** | `RoomGeneraterController` **parses `Tile_Spawn_Enemy` markers** and exposes their positions via `ON_GET_SPAWN_POSITIONS` | Map → Spawn | ✅ **Built** (2026-07-13 finding — previously recorded as not-built). `TileName.SPAWN = "Tile_Spawn_Enemy"`. **1 of 13 room JSONs authored** (`NormalRoom_0.json`); the other 12 emit an empty list, and there is no centre-fallback yet (see Edge Cases) |
| **Stat System** (`stat-system.md`) | Enemy combat stats stay on prefab `EntityData`/`StatsSO`; spawn system does not touch them | none (contract only) | No schema impact |
| **Object Pooling** | Reuse for enemy instances instead of raw `Instantiate` | Spawn → Pooling | Soft — Pooling is Alpha/not-started; `Instantiate` acceptable until then |
| **Per-Run Upgrades** | `ON_ROOM_CLEAR` opens the upgrade-card screen | Spawn → Progression | Consumer and event both unbuilt |

---

## Tuning Knobs

| Knob | Where | Safe range | Effect / failure at extremes |
|------|-------|-----------|------------------------------|
| `weightBudget` | `RoomModel` per preset | 0 – 500 (`[Range]`) | Room difficulty. Too high → overwhelming; too low → trivial/empty |
| `randomRatio` | `RoomModel` per preset | 0.0 – 1.0 (`[Range]`, default 0.33) | Variety vs. control. 1.0 → Phase 1 may consume the whole budget; 0.0 → all budget goes to Phase 2's fill loop |
| `overflowPercent` | `RoomModel` per preset | 0.0 – 1.0 (`[Range]`, default 0.1) | Budget-fill tightness. 0 → may under-spend budget; high → rooms overshoot intended difficulty. No "safe ≤0.3" ceiling is enforced in code |
| `selectionWeight` | `RoomModel` per preset | 1 – 100 (`[Range]`) | ⚠️ **Currently dead** — declared for room-picking but `MapModel.GetRandomRoom()` doesn't read it (uniform pick instead). Either wire it in or remove it; see Future Architecture Direction |
| `weight` | `EnemyModal` per enemy | **unbounded — no `[Range]`, author discipline only** | Enemy "cost". Should track power tier (creep < elite < champion). A `0`/negative value can hang the fit-loop (see Edge Cases) |
| `enemiesOfRoom` | `RoomModel` per preset | direct-ref list | Which enemies can appear in that preset. Empty → `GetSpawnSet()` returns `null` (BUG-ES-1) |
| `fullRoomList` | `MapModel` | direct-ref list | The pool `GetRandomRoom()` draws from. Empty → `GetRandomRoom()` returns `null`, uncaught downstream |
| spawn-marker count | room JSON (`Tile_Spawn_Enemy`) | **not yet authored (1/13 rooms)** | Spread of enemies. No round-robin/jitter/entry-safety logic exists yet to make marker count matter — see Edge Cases |
| `SPAWN_MIN_ENTRY_DISTANCE` | **[PLANNED]** not in `GameConstants` yet | 2 – 4 units (proposed default 3) | No-spawn radius around the entry door. Not implemented — no entry-safety check exists today |
| `SPAWN_JITTER_RADIUS` | **[PLANNED]** not in `GameConstants` yet | 0.25 – 0.75 (proposed default 0.5) | Offset when a marker hosts >1 enemy. Not implemented — spawns can stack exactly on top of each other today |

---

## Visual/Audio Requirements

Modest — this is a spawn/management layer, not a combat feel system.
- **Spawn telegraph (recommended):** a brief VFX/poof + SFX at each `Tile_Spawn_Enemy` marker when an
  enemy materializes, so enemies don't pop in silently. Pull from the `Pooling/` VFX system.
- **Door-lock feedback:** the door-seal on room entry should have an audible/visual cue — but that
  is owned by the Map/door system, not here; this system only calls `CloseDoor()`.
- **Room-clear cue:** a satisfying "clear" sting on `ON_ROOM_CLEAR` — owned by the upgrade/HUD layer.

> 📌 **Asset Spec** — after the art bible is approved, run `/asset-spec system:enemy-spawn-system`
> for the spawn-telegraph VFX/SFX.

---

## UI Requirements

No dedicated UI. An "enemies remaining" indicator, if desired, belongs to the **HUD** system and
would subscribe to `ON_ENEMY_DEATH` / `ON_ROOM_CLEAR` — do not build it here.

---

## Acceptance Criteria

> **[PLANNED target — still not met by the 2026-07-13 code, same as 2026-07-09.]** These ACs define
> "done" for a hardened system. **Caveat added 2026-07-13:** several ACs below (A3, A4, D1–D3)
> assume the specific injected-RNG, id-based-database shape from the 2026-07-08 target. That shape
> was never built and the code has since moved a different direction. Whether to still build toward
> that shape, harden the current direct-ref/uniform-random code instead, or adopt the candidate-pool
> proposal is an **open decision** — see Future Architecture Direction. Treat AC-A3/A4/D1–D3 as
> **conditional on that decision**, not as settled requirements; AC-A1, A5–A7, L1–L6, P1–P3 are
> direction-agnostic (some form of budget bound, variety, termination guard, and lifecycle is needed
> regardless of which selection algorithm is chosen) and remain the acceptance bar as-is.

> **Test isolation (applies to all ACs below):** every test constructs its own disposable
> `ScriptableObject.CreateInstance<…>()` for the relevant SOs (whichever set Future Architecture
> Direction settles on) — **never** the shipped project assets — mirroring the project `PlayerData`
> isolation rule. Algorithm tests that assume an injected-RNG design inject `new System.Random(fixedSeed)`.

### Algorithm — Logic (EditMode, BLOCKING)

- **AC-A1 (budget bound):** GIVEN candidate ids and `weightBudget = B`, WHEN `GetHybridEnemySet`
  runs, THEN total selected weight ≤ `B + overflowCap` and every selected id ∈ `RoomData.idEnemy`.
- **AC-A2 (eligibility boundary):** GIVEN a candidate whose `weight == remaining + overflowCap`,
  THEN it **is** selectable (`≤`, inclusive); GIVEN one whose `weight == remaining + overflowCap + 1`,
  THEN it is **not** — guards the off-by-one.
- **AC-A3 (determinism):** GIVEN identical `idEnemy`/`weightBudget`/`randomRatio`/`overflowPercent`
  and two `System.Random` instances built with the **same seed**, WHEN `GetHybridEnemySet` is called
  once per instance, THEN the two returned lists are equal in composition **and** count.
- **AC-A4 (tie-break determinism):** GIVEN two eligible candidates with equal `|weight − remaining|`,
  THEN the one earliest in `idEnemy` order is chosen, regardless of shuffle result.
- **AC-A5 (variety — concrete, no "statistically"):** GIVEN a fixture pool that admits ≥ 2 distinct
  fitting compositions (e.g. Bat w3 / Rat w2 / Golem w9, `weightBudget 20`, `randomRatio 0.5`),
  WHEN `GetHybridEnemySet` is called once per fixed seed in `{0,1,…,49}`, THEN the count of distinct
  compositions (by id+count, order-independent) is **≥ 2**. (Uses a variety-guaranteeing fixture,
  not an arbitrary real `RoomData`, so "low variance" can't be confused with "broken RNG".)
- **AC-A6 (termination / weight guard):** GIVEN `EnemyData.weight` set to 0 via reflection/test
  hook, WHEN validated, THEN it clamps to 1 (`OnValidate`); AND `GetHybridEnemySet` completes in
  bounded time (no hang) even if a `weight < 1` candidate is injected (defence-in-depth skip + log).
- **AC-A7 (degenerate inputs):** `weightBudget ≤ 0` → empty set; `randomRatio = 0` → Phase 1 skipped;
  `randomRatio = 1` with leftover → Phase 2 still runs under `overflowCap`; single candidate →
  no crash.

### Data Validation — Logic (EditMode, BLOCKING)

- **AC-D1 (duplicate id):** GIVEN two `EnemyData` with the same `id`, WHEN `EnemyDatabase` builds its
  lookup, THEN an Editor error names both assets and `GetByID` deterministically returns the one
  **first in `allEnemies` order**. (Test calls the public `EnsureId()` — does not rely on
  `OnValidate` firing on in-memory SOs.)
- **AC-D2 (zero id):** GIVEN an `EnemyData` with `id == 0` in `allEnemies`, WHEN the lookup builds,
  THEN an Editor error is logged and the entry is excluded from the map.
- **AC-D3 (subset strip):** GIVEN a `RoomData.idEnemy` entry not in `sourceMap.idEnemy`, WHEN
  `ValidateAgainstMap()` runs (public, called by `OnValidate`), THEN the entry is stripped and a
  warning logged. Test asserts on a pinned log message via `LogAssert.Expect`.

### Room Lifecycle — Integration (PlayMode, BLOCKING)

> Prerequisites for authoring these tests: (1) Open Question #1 resolved (EnemyManager
> singleton-vs-Inspector ADR — you cannot build the harness against an undecided architecture, though
> `EnemyManager` itself still needs to be built before these ACs are testable at all); (2) at least
> one `Tile_Spawn_Enemy`-populated fixture room JSON — **satisfied as of 2026-07-13**
> (`NormalRoom_0.json`, 1/13).

- **AC-L1 (spawn + lock on entry):** GIVEN an uncleared room, WHEN `ON_LOAD_MAP` fires, THEN enemies
  spawn at markers and all doors are locked (`CloseDoor()` called). *Not* blocked by Bugs #7/#8.
- **AC-L2 (clear events, isolated — unblocked today):** GIVEN a room populated with N enemies, WHEN
  `EventManager.Emit(ON_ENEMY_DEATH)` is called **directly** N times (no real AI), THEN
  `aliveCount` reaches 0 and `ON_CLEAR_ENEMY` **and** `ON_ROOM_CLEAR` are each emitted **exactly
  once** (subscribe counters; assert `== 1`) and doors open. This decouples the lifecycle test from
  the enemy-death bugs.
- **AC-L2b (real death chain — playtest, blocked on Bugs #7/#8):** a documented playtest confirming a
  real `EntityDeathState` death reaches `EnemyManager` and clears the room. Tracked separately; do
  **not** defer AC-L2's automated test until #7/#8 land.
- **AC-L3 (foreign / late event guard):** GIVEN a room in `Cleared` or `Idle` state, WHEN a stray
  `ON_ENEMY_DEATH` fires, THEN `aliveCount` is unchanged and `ON_CLEAR_ENEMY`/`ON_ROOM_CLEAR` are
  **not** re-emitted (prevents soft-lock and double-clear). Only `Fighting` decrements.
- **AC-L4 (zero-enemy room):** GIVEN a selection resolving to 0 enemies (empty pool or zero budget),
  WHEN it loads, THEN it is immediately `Cleared` — doors never lock, `ON_ROOM_CLEAR` fires once.
- **AC-L5 (cleared re-entry):** GIVEN a cleared room, WHEN re-entered, THEN `Populating` is skipped —
  no `Instantiate`, no `CloseDoor()`, doors stay open.
- **AC-L6 (null/unwired prefab):** GIVEN an `EnemyData.prefab` that is null/unwired, WHEN spawning,
  THEN it is skipped, an error with a **pinned message format** is logged, and it is **not** added to
  `aliveCount` (room can still clear).

### Placement — Integration (PlayMode, RECOMMENDED)

- **AC-P1 (round-robin balance):** GIVEN N enemies and M markers with N > M, WHEN spawning, THEN each
  marker hosts `floor(N/M)` or `ceil(N/M)` enemies and no two enemies share an identical world
  position.
- **AC-P2 (entry-safety):** GIVEN a marker within `SPAWN_MIN_ENTRY_DISTANCE` of the entry door, WHEN
  spawning, THEN no enemy is placed there; all spawned enemies are ≥ the min distance from the entry.
- **AC-P3 (parser):** GIVEN a room JSON containing `Tile_Spawn_Enemy` tiles, WHEN the room loads,
  THEN `RoomGeneraterController` exposes their world positions and the spawn driver spawns at them
  (not the centre-fallback). **Status update 2026-07-13: the parser itself is built** — `AC-P3`'s
  remaining gap is authoring markers into the other 12 room JSONs and adding the missing
  centre-fallback (see Edge Cases), not the parser code.

---

## Open Questions

> Table cleaned up 2026-07-13 — the previous revision had duplicate rows for #1/#2 with conflicting
> statuses (a merge artifact); resolved below to one row per question, and #4 reopened.

| # | Question | Status | Notes |
|---|----------|--------|-------|
| 1 | `EnemyManager` singleton violates "no new singletons" (only `MazeController` permitted). | ✅ RESOLVED (2026-07-09) | Ratified by **ADR-0002** — scoped singleton exception with mandated duplicate-guard + event-driven state. Unblocks the PlayMode lifecycle test harness (AC-L1…L6) once `EnemyManager` is actually built (still a stub as of 2026-07-13). |
| 2 | Runtime shuffle seed source — per-room from a global run seed, or fresh unseeded RNG? | ⬜ **REOPENED 2026-07-13** | Previously marked resolved via the `RoomFile.roomData` field, which was never implemented (see Q#4). With no per-room identity anchor, "per-room-from-run-seed" has nothing to derive from today. Moot unless/until Q#4 lands on a per-room mapping — otherwise this collapses to "seed from the run + a spawn-event counter," which is a materially weaker reproducibility guarantee. |
| 3 | Default values for `randomRatio` / `overflowPercent` (global vs per-`RoomModel`)? | ⬜ OPEN | Current code defaults live directly on each `RoomModel` asset (`0.33`/`0.1`) — no global default mechanism exists. Confirm during a balance pass. |
| 4 | How does a physical room resolve to a `RoomModel` preset? | ⬜ **REOPENED 2026-07-13** — was marked resolved in error | The `RoomFile.roomData` direct-reference design was never implemented. What runs today is `MapModel.GetRandomRoom()`, a shuffle-bag **decoupled from room identity** — see Room→RoomModel Resolution. Needs an explicit owner decision: keep the shuffle-bag (simpler, less authoring control), or build the per-room mapping this GDD previously assumed was done. Folded into Future Architecture Direction. |
| 5 | Should `weight`/cost be authored, or derived from the enemy's stat/rank tier? | ⬜ OPEN | Authored for now, and **still unenforced** (`weight ≥ 1` is not clamped anywhere). See Future Architecture Direction → weight↔stat cross-check. |
| 6 | Multi-wave rooms (second wave partway through)? | ⬜ POST-DEMO | Out of demo scope; schema allows a later extension. |
| 7 | Boss room: dedicated `RoomModel` (one boss + high budget) vs bypass the algorithm? | ⬜ POST-DEMO | Bosses are out of demo scope per `game-concept.md`. Likely a dedicated zero-random `RoomModel`; do not special-case the algorithm before the base loop is playtested. |
| 8 | Which selection-algorithm direction do we commit to: harden the current uniform-random `RoomModel.GetSpawnSet()`, revive the 2026-07-08 injected-RNG/`argmin`/id-database target, or adopt the Room Budget + Candidate Pool design? | ✅ **RESOLVED 2026-07-13** | **Option C chosen** — Room Budget + Candidate Pool + `RarityTier` (not the original `Spawn Chance` float sketch). Full spec: Future Architecture Direction → "Option C — Formal Specification (CHOSEN 2026-07-13)". `EnemyModal` gains `rarityTier` (enum: Common/Rare/Epic/Legendary); `weight` field name is kept, not renamed to `cost`. |
| 9 | **[NEW 2026-07-13]** Should `EnemyManager`/`EnemySpawner` own placement fallback (centre-of-room when no markers exist), or is authoring markers into all 13 rooms a hard prerequisite before Sprint 6 work starts? | ⬜ OPEN | 12/13 rooms currently have no fallback and would throw on load if spawn were wired further. Low-cost either way; recommend building the fallback regardless of the marker-authoring timeline, since it's also the demo's safety net if a future room ships without markers. |

---

## Future Architecture Direction — evaluated 2026-07-13

> Renamed from "Future Enhancements." Folds together the 2026-07-08 idealized target (never built),
> the current code's own trajectory (uniform-random, direct-ref, zero-alloc), and a new **Room
> Budget + Candidate Pool + Spawn Chance** proposal from the owner (2026-07-13, high-level sketch,
> not a final spec). All three are evaluated on the same footing — none is assumed to be the answer.
> Open Question #8 tracks the decision this section informs but does not make.

### Option A — Harden the current code (uniform-random, direct-ref)

Keep `EntityModel`/`EnemyModal`/`MapModel`/`RoomModel` as-is; add the missing guards (`weight ≥ 1`
clamp, null-safe `GetSpawnSet()`, centre-fallback for markerless rooms) without changing the
selection algorithm's shape.
- **Pros:** smallest change from what's shipping today; the zero-alloc scratch-buffer work in
  `GetSpawnSet()` is preserved; fastest path to closing BUG-ES-1/ES-2/ES-3.
  the direct-ref model.
- **Cons:** stays non-deterministic (`UnityEngine.Random`, unseedable) — EditMode algorithm tests
  stay out of reach; `MapModel`'s shuffle-bag stays decoupled from room identity unless Q#4 is
  separately resolved; no `Spawn Chance`/tier concept, so composition control stays coarse
  (budget + weight only).

### Option B — Revive the 2026-07-08 target (injected RNG, id-database, `argmin`)

Build the originally-designed `EnemyDatabase`/id-based lookup, inject `System.Random`, restore a
deterministic Phase-2 fill.
- **Pros:** the Acceptance Criteria section (AC-A3, AC-A4, AC-D1–D3) already exist for this shape;
  fully deterministic and testable; id-based refs decouple map/room assets from direct SO references
  (lighter, more refactor-safe).
- **Cons:** the most implementation work of the three options (new database class, id plumbing,
  migrating every existing `RoomModel`/`MapModel` asset off direct refs); the `argmin` fill was
  already flagged in the 2026-07-08 review as biasing toward *fewer, heavier* enemies — reviving it
  without also building the deferred composition-diversity pass reintroduces that known limitation.

### Option C — Room Budget + Candidate Pool + Spawn Chance (owner proposal, 2026-07-13)

The owner's sketch: each `RoomModel`-equivalent holds a `Room Type`, `Room Budget`, an `Enemy Pool`,
a `Budget Tolerance` (e.g. total cost within 90–110% of budget), and spawn points. Each
`EnemyModal`-equivalent gains `Cost` (≈ today's `weight`), `Spawn Chance`, and an `Enemy Tier`. Per
enemy spawned: build a fresh **Candidate Pool** by rolling `Spawn Chance` independently for every
enemy in the pool whose `Cost` still fits the remaining budget; pick from that pool (random/weighted/
other); if the pool comes up empty, fall back to *all* enemies that still fit, so a spawn attempt is
never blocked by bad luck alone. Repeat until the tolerance band is hit or nothing fits.

**Architecture evaluation** (requested 2026-07-13 — analysis only, no implementation):

- **Strengths.**
  - The per-pick fallback (empty Candidate Pool → take everything that still fits) directly fixes a
    real gap in the *current* code: today's fit-loop can legitimately empty out and silently under-
    spend the budget with candidates still technically available (see Formulas worked example) — this
    proposal's fallback guarantees forward progress except at the true boundary condition (nothing
    fits at all), which is a genuine improvement over both Option A and Option B as sketched.
  - `Spawn Chance` gives designers a **second knob** (chance to be considered) independent of `Cost`
    (budget weight) — today's `weight` conflates "how much of the budget this costs" with implicitly
    "how likely it is to appear" (anything that fits is equally likely). Separating them lets a
    designer make a cheap enemy *rare* or an expensive one *common*, which neither Option A nor B
    can express without adding a field very close to this one.
  - `Enemy Tier` as an explicit field (vs. today's convention-only "weight should track power tier")
    turns an implicit authoring convention into checkable data — closes part of Open Q#5.
  - Conceptually the closest of the three to what `RoomModel.GetSpawnSet()` **already does** (per
    Current Implementation's note that the current loop shape already resembles a candidate-pool
    pattern) — this is evolution, not a rewrite, which lowers implementation risk relative to Option B.

- **Weaknesses / open risks.**
  - **Termination is not obviously guaranteed.** Rolling `Spawn Chance` independently per candidate
    every pick means a pick round can legitimately produce an empty Candidate Pool even when eligible
    (cost-fitting) enemies exist — the proposal's own fallback step exists specifically to cover this,
    but the *fallback path itself* needs the same `cost ≥ 1`-style termination guarantee this GDD
    already had to add as a hard invariant for the current algorithm (see Formulas/Edge Cases) — that
    invariant isn't stated in the proposal and must be carried over, not re-derived from scratch.
  - **Budget Tolerance as a band (90–110%), not a hard cap, is a design change from today's
    `overflowCap`-style "may exceed by at most X%" — worth confirming it's intentional.** A band
    implies the algorithm might need to keep sampling *after* it would already stop under today's
    "stop at first `remaining ≤ 0`" rule, to reach the lower bound of the tolerance window; the
    proposal's step 8 loop condition ("until budget threshold OR no eligible enemy") doesn't fully
    specify what happens if the loop naturally stops below 90% with eligible-but-unlucky enemies
    remaining — is that an acceptable outcome, or does it need a forced top-up? This needs a Formulas
    section of its own before implementation, mirroring the rigor already in this GDD's existing
    Formulas section.
  - **Rebuilding the Candidate Pool from scratch every single enemy spawn is more `Spawn Chance` rolls
    than either current option** (current code does ~1 random draw per accepted candidate; this does
    up to `|EnemyPool|` chance-rolls per accepted candidate). For the pool sizes this project has today
    (single-digit enemy types per room), this is not a real performance concern — but it's worth
    stating explicitly per this project's Zero-Alloc Hot Path rule (`.claude/rules/engine-code.md`):
    the rolling step must not allocate per-candidate (a `foreach` + `Random.value` comparison against
    a pre-sized reusable buffer, mirroring `RoomModel.GetSpawnSet()`'s existing `_fitBuf` pattern, is
    the same shape already proven to work in this codebase — this is an implementation detail to
    carry forward, not a reason to avoid the design).
  - **`Room Type` (Normal/Elite/Boss/Treasure/Event) isn't consumed by any rule in the proposal's own
    flow** — it's declared as a field but the spawn algorithm section never branches on it. Either the
    algorithm needs a Room-Type-conditioned rule (e.g. Boss rooms skip the pool entirely, per this
    GDD's existing Open Q#7), or the field is aspirational for a later pass — should be explicit either
    way, not implied.
  - **No placement/marker guidance** — the proposal is entirely about *which* enemies, not *where*
    they spawn. This GDD's existing entry-safety/jitter/round-robin Edge Cases (still unbuilt in any
    option) would still be needed on top of whichever selection algorithm is chosen.

- **Balance implications.** The `Spawn Chance` + `Cost` split is the strongest lever of the three
  options for the Player Fantasy's stated goal ("sometimes a swarm of cheap enemies, sometimes a
  couple of heavy ones") — a designer can directly dial "Golem is powerful but rare" via low
  `Spawn Chance` + high `Cost`, independent of how often it *fits* the budget. Options A and B only
  have `Cost`/`weight` to work with, so composition variety is a side-effect of budget arithmetic
  rather than a directly-authored knob. This is the proposal's single biggest gameplay-facing
  advantage and the main reason it's worth prototyping over Option A/B as-is.

- **Recommendation (informational, not a decision):** Option C is the strongest direction of the
  three for the stated Player Fantasy, on the condition that (1) its termination/fallback guarantee
  is formalized with the same rigor as this GDD's existing invariants before implementation, (2)
  `Budget Tolerance` gets a precise Formulas-section treatment (not just "90–110%" prose), and (3) it
  reuses the zero-alloc scratch-buffer pattern already proven in `RoomModel.GetSpawnSet()` rather than
  reintroducing per-pick allocation. It is closer to the current code's actual trajectory than Option
  B is, which lowers migration cost.

### Option C — Formal Specification (CHOSEN 2026-07-13)

> **Decision**: Open Question #8 is resolved — **Option C is the selection-algorithm direction**.
> This section is the follow-up spec the Recommendation above called for: it satisfies condition (1)
> (termination/fallback guarantee, via a bounded retry cap) and (2) (`Budget Tolerance` as precise
> Formulas, not prose). It supersedes the `Spawn Chance` float field from the original owner sketch
> (Overview above) with a `RarityTier` enum, decided in the same 2026-07-13 owner session. This is
> the design target for the Sprint 5 Track A data refactor (S5-A3) and the Sprint 6 `GetSpawnSet()`
> rewrite — it is not yet implemented (see Current Implementation, which still describes Option A's
> shipped shape).
>
> **Implementation note — S5-A3 scope change:** the sprint plan's S5-A3 task originally specified a
> `weight`→`cost` rename (`[FormerlySerializedAs("weight")]`). That rename is **dropped** — the owner
> confirmed `weight` stays as the field name (it already means "cost"; no rename needed). S5-A3 is now
> "add the `RarityTier` enum + `rarityTier` field to `EnemyModal`," nothing else.

#### Data Model

```csharp
public enum RarityTier
{
    Common    = 50,  // roll chance, percent
    Rare      = 30,
    Epic      = 15,
    Legendary = 5
}

public class EnemyModal : EntityModel
{
    public GameObject prefab;
    public int weight;          // = Cost. Stays `weight` — not renamed (see Implementation note above)
    public RarityTier rarityTier;
}

public class RoomModel : EntityModel
{
    [SerializeField] private List<EnemyModal> enemiesOfRoom = new List<EnemyModal>();
    // No per-room override wrapper. Per-room variety is authored by pointing a room's
    // enemiesOfRoom at different EnemyModal asset variants of the same enemy — e.g.
    // Bat_Common.asset vs Bat_Rare.asset (same prefab, different weight/rarityTier) —
    // not by a dynamic per-room tier override on one shared asset.
}
```

`weight` (Cost) and `rarityTier` are fully independent by design. There is no code-enforced
correlation between them (no `OnValidate` cross-check flagging "high weight + high roll chance", for
example) — this is intentional, per owner decision: it lets a designer make an expensive enemy common
or a cheap enemy rare, which is the entire point of separating budget cost from appearance chance (see
Strengths, second bullet, above).

#### Candidate-Pool Selection Flow

Runs once per accepted pick — called repeatedly by the room-fill loop, the same per-candidate loop
shape `RoomModel.GetSpawnSet()` already uses today (see Current Implementation). Eight steps:

1. **Start pick round.** `retryCount = 0`.
2. **Build eligible set.** `eligibleSet = { e ∈ enemiesOfRoom | e.weight ≤ remaining }`. If empty,
   stop the fill loop entirely (see step 8's exit condition).
3. **Roll each eligible candidate independently.** For every `e ∈ eligibleSet`: `r = Random.value`;
   `e` passes if `r ≤ chance(e.rarityTier)`, where `chance(Common)=0.50`, `chance(Rare)=0.30`,
   `chance(Epic)=0.15`, `chance(Legendary)=0.05`.
4. **Collect passers.** `CandidatePool = { e ∈ eligibleSet | e passed step 3 }`.
5. **Empty-pool retry.** If `CandidatePool` is empty: `retryCount += 1`. If `retryCount ≤ 4`, return to
   step 3 and re-roll the same `eligibleSet`. If `retryCount > 4`, set `CandidatePool = eligibleSet`
   (fallback — every eligible candidate is accepted regardless of its roll, guaranteeing the pick
   round cannot stall indefinitely).
6. **Pick.** Choose one entry from `CandidatePool` uniformly at random — equal probability per entry,
   no weighting by tier or cost at this step (tier already did its job in step 3).
7. **Apply.** `remaining -= picked.weight`; append `picked` to the room's spawn result;
   `retryCount = 0`.
8. **Loop or stop.** Repeat steps 1–7 while `remaining` is outside the tolerance band (see Formulas)
   **and** `eligibleSet` (step 2) is non-empty. Stop when either condition fails.

#### Formulas

**Budget Tolerance band**

`ToleranceBand = [ B × 0.9, B × 1.1 ]`, where `B` = `RoomModel`'s Room Budget (the same role
`weightBudget` plays in Option A today).

**Important consequence of step 2's eligibility rule:** because a candidate must satisfy
`weight ≤ remaining` (not `remaining + an overflow cap`, unlike Option A's Phase 2), `totalSpend`
(`= B − remaining`) can **never exceed `B`** — the upper half of `ToleranceBand` (100–110%) is
therefore unreachable by construction, and is kept here only for band symmetry with the owner's
original "90–110%" phrasing. The **only practically meaningful bound is the 90% floor**, and even
that is a target, not a guarantee: per the Edge Cases below, the loop legitimately stops below 90% if
`eligibleSet` empties out first. This resolves the ambiguity the evaluation above flagged ("worth
confirming it's intentional") — confirmed: **under-spend is an accepted outcome, overspend is
structurally impossible.**

**Variables**

| Variable | Type | Range | Description |
|----------|------|-------|-------------|
| `B` (Room Budget) | int | mirrors today's `weightBudget` `[Range(0,500)]` | Total spend target for the room |
| `remaining` | int | starts at `B`, decreases each pick, never negative | Budget left to spend this fill loop |
| `totalSpend` | int | `B − remaining`, range `[0, B]` | Running total spent so far |
| `retryCount` | int | 0–4 (hard cap per pick round) | Consecutive empty-`CandidatePool` rolls |
| `chance(tier)` | float | `{0.50, 0.30, 0.15, 0.05}` | Fixed per-tier roll chance, not author-adjustable per instance |
| `weight` (per `EnemyModal`) | int | must be `≥ 1` (same invariant as Option A) | Cost consumed from `remaining` per pick |

**Termination guarantee.** Every accepted pick (step 7) strictly reduces `remaining` by at least 1
(given `weight ≥ 1`), so the outer loop (step 8) terminates in at most `B` iterations. Each pick round
(steps 3–5) terminates in at most 5 roll attempts (`retryCount` capped at 4, plus the forced
fallback) — never unbounded. This is the piece the evaluation above flagged as "not obviously
guaranteed" in the original owner sketch; the retry cap is what closes that gap.

**Worked example.** `B = 20`; candidates: Bat (`weight=3`, `Common`), Rat (`weight=2`, `Rare`), Golem
(`weight=9`, `Legendary`).
- Round 1: `remaining=20`, `eligibleSet={Bat,Rat,Golem}`. Roll: Bat `r=0.3≤0.50` PASS, Rat
  `r=0.6>0.30` FAIL, Golem `r=0.4>0.05` FAIL. `CandidatePool={Bat}` → pick Bat. `remaining=17`.
- Round 2: `eligibleSet={Bat,Rat,Golem}` again. All three FAIL on the roll, and again on 4 re-rolls
  (`retryCount` reaches 4). 5th attempt: fallback, `CandidatePool={Bat,Rat,Golem}` → pick Golem
  uniformly. `remaining=8`.
- Round 3: `eligibleSet={Bat,Rat}` (Golem's 9 no longer fits 8). Rat passes. `remaining=6`.
  `totalSpend=14` (70% of `B`) — below the 90% floor, but `eligibleSet` still has Bat → continue.
- Round 4: Bat passes. `remaining=3`. `totalSpend=17` (85%) — still below 90%. `eligibleSet={Bat}`
  (`3≤3`).
- Round 5: Bat passes. `remaining=0`. `totalSpend=20` (100%, inside `[18,22]`) → stop.

#### Edge Cases

- **If `eligibleSet` is empty at step 2 (no candidate fits `remaining`):** the fill loop stops
  immediately, even if `totalSpend` is below the 90% floor. Accepted outcome — a room can end up
  under-spent if its `enemiesOfRoom` pool has no cheap-enough candidate left; no forced top-up.
- **If any `EnemyModal.weight ≤ 0`:** same hang risk documented for Option A (see that Edge Cases
  entry) — `remaining` never drops below a non-positive-weight candidate, so it never leaves
  `eligibleSet`. The fix is shared, not re-derived per option: enforce `weight ≥ 1` via
  `[Range(1,99)]` + `OnValidate` clamp on `EnemyModal`.
- **If `retryCount` exceeds 4 (fallback triggers):** `CandidatePool` becomes the full `eligibleSet`,
  and the step-6 pick is uniform across it — a `Legendary` (5% chance) candidate has exactly the same
  odds as a `Common` (50% chance) one in a fallback round. Intentional: the fallback exists purely to
  guarantee forward progress, not to preserve tier weighting under bad luck.
- **If two entries in `enemiesOfRoom` share the same prefab with different `weight`/`rarityTier`**
  (the intended per-room authoring pattern, e.g. `Bat_Common.asset` and `Bat_Rare.asset`): both are
  independent entries, no dedup — both can be picked in the same room fill.
- **If `chance(tier)` were ever authored outside the fixed enum values** (not possible today —
  `RarityTier`'s four values are fixed): `chance = 1.0` needs no special handling (step 4/5 already
  handle a fully-populated `CandidatePool`); `chance = 0.0` means that candidate only ever enters
  `CandidatePool` via the retry-exhaustion fallback, never a genuine roll pass — a natural consequence
  of the model, not a case requiring extra code.

#### Acceptance Criteria (EditMode, BLOCKING)

- **AC-C1 (spend never exceeds budget):** GIVEN any `enemiesOfRoom` and Room Budget `B`, WHEN the fill
  loop runs to completion, THEN `totalSpend ≤ B` always (never exceeds — see Formulas' overspend
  note).
- **AC-C2 (bounded pick-round retries):** GIVEN a fixture where every candidate's roll is forced to
  FAIL (test hook or mocked `Random.value`), WHEN a pick round runs, THEN it completes in exactly 5
  roll attempts (4 re-rolls + 1 forced fallback) and returns a non-empty `CandidatePool` equal to
  `eligibleSet`.
- **AC-C3 (bounded outer-loop iterations):** GIVEN `weight ≥ 1` holds for all candidates, WHEN the
  fill loop runs, THEN it completes in at most `B` iterations (no hang).
- **AC-C4 (weight ≤ 0 guard):** GIVEN an `EnemyModal` with `weight = 0` injected via test hook, WHEN
  `OnValidate` runs, THEN it clamps to 1; AND the fill loop completes in bounded time even if a
  pre-clamp `weight < 1` candidate reaches `eligibleSet`.
- **AC-C5 (tier independence):** GIVEN candidates with high `weight` + high-chance `rarityTier` (or
  the inverse), WHEN any validation runs (`OnValidate`, asset import, etc.), THEN no warning or error
  is raised — `weight` and `rarityTier` are never cross-checked.
- **AC-C6 (tier distribution, statistical, non-fallback rounds only):** GIVEN a fixture pool of one
  candidate per tier all fitting `remaining`, WHEN 1000 pick rounds are run and only rounds that did
  **not** hit the retry-fallback are counted, THEN `Common` is selected into `CandidatePool` markedly
  more often than `Legendary` (directional check, not an exact ratio assertion — avoids flaking on
  RNG variance).

### Other deferred items (unchanged in spirit from 2026-07-08)

- **Run-depth difficulty escalation (Post-Demo balance).** `weightBudget`/`Room Budget` is static per
  preset regardless of which option is chosen; scaling it by run depth needs a maze-depth signal the
  map system does not currently expose.
- **`weight`/`Cost` ↔ stat cross-check (Open Q#5).** An editor validation comparing authored cost
  ordering against a derived power score from the prefab's `StatsSO`/rank tier — closes the "cost
  diverges from real threat" hazard regardless of which option is chosen.
- **Object pooling.** Replace `Instantiate` with the `Pooling/` system once it exists (currently
  Alpha / Not Started) — soft dependency, `Instantiate` acceptable for the demo.

---

## Prototype Deviations from the Planned Design (open — owner review)

> Updated 2026-07-13. The 2026-07-09 reverse-sync compared the prototype against the 2026-07-08
> target; since then the prototype moved **further from** that target rather than toward it, and new
> deviations appeared. Rows D1–D8 are the original findings (re-verified); D9–D12 are new. "Planned"
> below still means the 2026-07-08 idealized target — see Future Architecture Direction for whether
> that target is even the right one to converge on.

| # | Deviation | As-built (2026-07-13) | Planned (2026-07-08 target) | Risk if left as-is |
|---|-----------|----------|---------|--------------------|
| D1 | RNG source | `UnityEngine.Random` (static) — unchanged | Injected `System.Random` | Selection is non-deterministic → AC-A3/AC-A4 untestable; no reproducible runs |
| D2 | Phase-2 fill | Uniform random pick among eligible, rewritten for zero-alloc 2026-07-09→13 | Deterministic `argmin \|weight−remaining\|` + tie-break | Budget usage not "optimal"; behaviour not reproducible. Also no longer matches even the *prior* prototype snapshot — the implementation itself changed, not just its relation to the target |
| D3 | Enemy reference model | `RoomModel.enemiesOfRoom` = direct `EnemyModal` refs — unchanged | `id`-based via `EnemyDatabase.GetByID` | Heavier map/room assets; no central store; no dup/zero-id validation |
| D4 | `weight ≥ 1` enforcement | `RoomModel.OnValidate` logs a warning only, no clamp — unchanged | `[Range(1,99)]` + `OnValidate` clamp | A `0`/negative weight authored on `EnemyModal` still hangs the fit-loop |
| D5 | `MapModel` role | **Changed again** — now `fullRoomList` (List\<RoomModel\>) + `GetRandomRoom()` shuffle-bag; the 2026-07-09 snapshot's `idRooms`/`totalWeight` fields are **gone** | `mapName` + `idEnemy` (map's enemy set) | Map data doesn't scope a room's enemy pool by identity at all now — it's a random draw, further from the target than before |
| D6 | `id` generation | `Math.Abs(Guid.GetHashCode())`, no reroll, no public `EnsureId()` — unchanged | reroll-until-nonzero + public test hook | `id == 0` edge and in-memory-SO test path unhandled |
| D7 | Runtime driver | **Two** drivers now: `EnemySpawner` (event-driven, built out since 2026-07-09) **and** `LevelManager.SpawnRoomEnemies()` (Editor button, unchanged) | `EnemyManager` event-driven lifecycle (ADR-0002) | `EnemyManager` itself still has zero lifecycle; two parallel non-canonical drivers instead of one, neither routes through it (BUG-ES-2) |
| D8 | Class naming / location | `EntityModel`/`EnemyModal`/`MapModel`/`RoomModel` in `Database-SO/Modal/` (typo "Modal") — unchanged | `EnemyData`/`EnemyDatabase`/`MapEnemyDatabase`/`RoomData` | This GDD now treats the actual names as primary (see header) — this row is kept for historical traceability, not as an open action item |
| D9 | **[NEW]** Room→preset mapping | `MapModel.GetRandomRoom()` bag-draw, decoupled from room identity | `RoomFile.roomData` direct reference (this GDD marked it RESOLVED 2026-07-09; it was never implemented) | A designer cannot guarantee a specific room's difficulty; start room isn't guaranteed zero-budget (see Edge Cases) |
| D10 | **[NEW]** `selectionWeight` field | Declared on `RoomModel` `[Range(1,100)]`, documented in its own code comment as feeding `MapModel.GetRandomRoom` | N/A — not part of any prior design | Dead Inspector field; a designer tuning it observes no effect, which reads as a bug even though it's "just" unused |
| D11 | **[NEW]** Tile_Spawn_Enemy parser | **Built** — `RoomGeneraterController.LoadRoom()` parses the marker, `EnemySpawner` consumes it | Same — this GDD previously (incorrectly) recorded this as not built | None — this row flips from a gap to a resolved item; kept to correct the record |
| D12 | **[NEW]** Markerless-room fallback | **Missing** — `EnemySpawner.SpawnRoomEnemies()` throws on an empty marker list (12/13 rooms today) | Fall back to room-centre, log a warning | Loading almost any room with a configured `EnemyModal` pool throws at spawn time; this is the most player-visible risk in this table if spawning is wired further before it's fixed |
