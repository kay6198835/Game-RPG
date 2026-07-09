---
status: revised
source: owner spec (2026-07-08) + codebase audit (Assets/Script/Enemy, Map/Room, Manager)
date: 2026-07-08
<<<<<<< HEAD
revised: 2026-07-09 (Open Q#2 seed source resolved; Q#4 mechanism updated to RoomFile.roomData; prior: 2026-07-08 post /design-review — 4 specialist passes; owner decisions logged)
=======
revised: 2026-07-08 (post /design-review — 4 specialist passes; owner decisions logged)
revised: 2026-07-09 (reverse-synced to prototype code — Assets/Script/Database-SO/Modal + LevelManager.SpawnRoomEnemies)
>>>>>>> origin/claude/enemy-spawn-manager-review-7aq2wa
verified-by: Kiet
supersedes: map-system.md "Agreed spawn architecture (2026-07-02) [PLANNED]" (EncounterSO + RoomEnemySpawner)
---

# Enemy Spawn & Per-Room Management System

**Status**: Approved (design) · Prototype partial (see Current Implementation Status)
**Implements Pillar**: Room-clear progression · "each run is a fresh challenge" (run-to-run variety)

> **Architecture decision (2026-07-08):** This GDD adopts the data-driven **weight-budget** model
> (ScriptableObjects + `GetHybridEnemySet`/`GetSpawnSet` selection + a runtime driver) and
> **supersedes** the earlier `EncounterSO` + `RoomEnemySpawner` plan sketched in `map-system.md`.
> Propagated into `map-system.md` on 2026-07-09.

---

## Current Implementation Status (2026-07-09) — authoritative for "what is built"

> This section reverse-documents the **prototype actually in the repo** (commit `a420d5e`,
> `Assets/Script/Database-SO/Modal/*` + `LevelManager.SpawnRoomEnemies`). Where the Detailed
> Design below differs, **this section describes what exists today**; the rest of the GDD
> describes the **PLANNED** hardened target that the code has not reached yet. The prototype
> commit itself notes "need polish code and flow" — several planned invariants are not enforced
> (flagged as ⚠️ deviations here and re-listed as open issues at the end of the doc).

**Implemented ScriptableObjects** (`Assets/Script/Database-SO/Modal/`, note the `Modal` typo for
"Model"; all extend `EntityModel`):

| Class (code) | Planned name in this GDD | Fields (code) | Notes |
|--------------|--------------------------|---------------|-------|
| `EntityModel` (base) | — | `id` (int, private, `ID` getter), `nameEnity` (string) | `OnValidate` sets `id = Math.Abs(Guid.NewGuid().GetHashCode())` when `id == 0`. ⚠️ No reroll-if-zero loop and no public `EnsureId()` test hook. |
| `EnemyModal` | `EnemyData` | `prefab` (GameObject), `weight` (int) | ⚠️ `weight` has **no `[Range(1,99)]`/clamp** — the `≥ 1` invariant is not enforced at author time. |
| `MapModel` | `MapEnemyDatabase` | `mapName` (string), `idRooms` (List\<int\>), `totalWeight` (int) | ⚠️ Holds `idRooms`, **not** an `idEnemy` set. Diverges from the planned map→enemy-set role. |
| `RoomModel` | `RoomData` **+** the selection engine | `enemiesOfRoom` (List\<`EnemyModal`\>, **direct refs**), `weightBudget` (`[Range(0,500)]`), `randomRatio` (`[Range(0,1)]`, default `0.33`), `overflowPercent` (`[Range(0,1)]`, default `0.1`) | ⚠️ Holds **direct `EnemyModal` references**, not `id`s. The selection method lives here, **not** on a central `EnemyDatabase` (which does not exist). |

**Implemented selection** — `RoomModel.GetSpawnSet()` → `List<EnemySpawnEntry>` (`{enemy, count}`),
which calls private `RoomModel.GetHybridEnemySet()`:
- Candidates = `enemiesOfRoom` filtered to `e != null && e.weight > 0` (runtime guard only).
- **Phase 1 (random):** repeatedly picks a random eligible candidate (`UnityEngine.Random`) whose
  weight fits both `randomBudget` and `remaining`.
- **Phase 2 (fill):** while `remaining > 0`, picks a **random** candidate among those with
  `weight − remaining ≤ maxOverflow`. ⚠️ This is a **random** fill, **not** the planned
  deterministic `argmin |weight − remaining|` optimal fill.
- ⚠️ RNG is `UnityEngine.Random` (static), **not** an injected `System.Random` — so the
  determinism/tie-break acceptance criteria (AC-A3, AC-A4) **cannot be met** by this code as-is.

**Implemented driver** — `LevelManager.SpawnRoomEnemies()` (public), triggered by the Editor
button **"Spawn Enemy"** (`LevelManagerEditor`). Reads a single `[SerializeField] RoomModel roomModel`,
calls `GetSpawnSet()`, and `Instantiate`s each `entry.enemy.prefab` at a random position.
⚠️ There is **no `EnemyManager`, no room-combat lifecycle, no door lock, no alive-count, and no
events** yet — spawning is a manual editor action, not the `ON_LOAD_MAP`-driven runtime flow.
`LevelManager` is itself a singleton (map-system Bug #12).

**Still PLANNED (designed below, not in code):** `EnemyManager` runtime driver + state machine
(see ADR-0002 for the ratified singleton decision), `EnemyDatabase` central store + `GetByID`,
id-based (not ref-based) map/room data, injected-RNG deterministic selection, `argmin` optimal
fill, `weight ≥ 1` enforcement, `ON_ENEMY_DEATH`/`ON_ROOM_CLEAR` events, `Tile_Spawn` marker
parsing, entry-safety/jitter placement, and RoomType→RoomData routing.

---

## Overview

The enemy-spawn system decides **which enemies appear in each room and instantiates them**, then
manages the room's combat lifecycle: lock the doors on entry, track how many enemies are alive,
and unlock the doors when the room is cleared.

It is a **data layer** plus a **runtime driver**:

- **Data (definition only, no logic):** `EnemyData` (one enemy type: id, name, prefab, weight),
  `MapEnemyDatabase` (the id set belonging to one map), and `RoomData` (per-room allowed id subset
  + a single `weightBudget` difficulty dial).
- **Lookup + logic:** `EnemyDatabase` — the single project-wide store of every `EnemyData`, with a
  fast id lookup and the `GetHybridEnemySet` selection algorithm.
- **Runtime driver:** `EnemyManager` — listens for room-load, asks `EnemyDatabase` for a
  weight-budgeted enemy set, and spawns it at the room's spawn markers.

The player never touches this system directly; they **feel it** as the pacing and variety of each
room — a different, budget-appropriate mix of enemies every run, and the tension of doors sealing
until the room is clear.

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

> **Known limitation (owner-accepted 2026-07-08):** the current Phase-2 fill is a greedy
> `argmin` and structurally favours *fewer, heavier* enemies; a true "cheap swarm" is the rarer
> outcome. The full swarm-vs-heavy variety promised above is therefore **aspirational** for the
> demo. A composition-diversity pass (a comparison phase that decides *what kind* of cluster to
> build — e.g. "10 creeps vs 1 elite, which is the more interesting encounter here?") is a
> **Should-Have** enhancement, deferred (see Future Enhancements). Do not over-invest in variety
> tuning until that phase lands.

---

## Detailed Design

### Core Data Model

> **[PLANNED target.]** The table below is the *designed* model. For the classes that actually
> exist today (`EntityModel`/`EnemyModal`/`MapModel`/`RoomModel`) and how they differ, see
> **Current Implementation Status (2026-07-09)** above. Notably the code has **no `EnemyDatabase`**
> (selection lives on `RoomModel`), uses **direct refs** instead of `id`s, and `MapModel` carries
> `idRooms`/`totalWeight` rather than `idEnemy`.

| SO | Role | Fields | Contains logic? |
|----|------|--------|-----------------|
| `EnemyData` | One concrete enemy type | `id` (int), `enemyName` (string), `prefab` (GameObject), `weight` (int) | No |
| `EnemyDatabase` | Single project-wide store + selection engine | `allEnemies` (List\<EnemyData\>) | **Yes** — `GetByID`, `GetHybridEnemySet` |
| `MapEnemyDatabase` | The enemy set of one map | `mapName` (string), `idEnemy` (List\<int\>) | No |
| `RoomData` | One room's spawn config | `sourceMap` (MapEnemyDatabase), `idEnemy` (List\<int\> ⊆ sourceMap), `weightBudget` (int) | No |

**Design invariants:**
- `EnemyData` is **spawn metadata only**. Combat stats (HP, damage, defense…) live on the prefab's
  existing `EntityData` / `StatsSO` (per `stat-system.md`). `EnemyData` must **not** become a
  fourth stat store. `weight` is a spawn-budget cost, unrelated to the stat formula — though by
  convention it should track the enemy's power tier (creep < elite < champion).
- Definition SOs reference enemies by **`id` (int)**, never by direct object reference, so map/room
  data stays lightweight and decoupled.
- `EnemyDatabase` is the **only** object that holds real `EnemyData` references and the **only**
  place selection logic lives.
- **`weight` must be `≥ 1`, enforced — not merely documented.** `EnemyData.weight` uses
  `[Range(1, 99)]` and is clamped in `OnValidate` (`if (weight < 1) weight = 1`). A `weight` of
  `0` or negative would make the Phase-2 loop non-terminating (see Edge Cases → *weight ≤ 0*), so
  this is a hard invariant, not a soft convention. The default value of a newly created
  `EnemyData` asset must therefore be `1`, never the C# `int` default of `0`.

### `EnemyData.id` Generation

- `id` is generated by a public, test-callable method `EnsureId()` (invoked from `OnValidate()`),
  via a **reroll loop** so `0` is never committed as a real id:
  `while (id == 0) id = System.Guid.NewGuid().GetHashCode();`. Because `0` is the "unset" sentinel,
  a hash that legitimately lands on `0` must be rerolled, otherwise it would be indistinguishable
  from unset and silently regenerate later.
- Once non-zero, `id` is **never overwritten** — stable for the asset's life, so map/room
  references never break when the asset is edited.
- **Pre-first-validate hazard:** an `EnemyData` asset created head­lessly (import script, copied
  `.asset`) and referenced by `RoomData.idEnemy` **before its first `OnValidate` fires** would
  carry `id == 0` into version control, then reroll to a real hash later and orphan that reference.
  Authoring rule: never reference an `EnemyData` in a `RoomData`/`MapEnemyDatabase` until its `id`
  reads non-zero in the Inspector. `EnemyDatabase`'s duplicate/zero-id validation (below) logs any
  `id == 0` still present in `allEnemies`.
- `EnsureId()` is public so EditMode tests can force id assignment on an in-memory
  `ScriptableObject.CreateInstance<EnemyData>()` (Unity does not reliably fire `OnValidate` on
  purely in-memory SOs).
- `EnemyDatabase` validates its `allEnemies` list for **duplicate ids** and **zero ids** (see Edge
  Cases) so a hash collision or an un-validated asset can never silently corrupt lookups.

### `EnemyDatabase.GetByID(int id)`

- Backed by a lazily-built, cached `Dictionary<int, EnemyData>`. O(1) lookup. Returns `null` for an
  unknown id (caller logs + skips).
- **Cache invalidation is explicit, not count-based.** `EnemyDatabase` holds an `int _version`
  incremented in `OnValidate()`/`OnEnable()`; the dictionary is rebuilt whenever the cached version
  differs from `_version`. A naive count-diff would miss a same-count swap (asset A replaced by
  asset B) and serve a stale mapping — the version counter closes that hole.

### `EnemyDatabase.GetHybridEnemySet(List<int> idEnemy, int weightBudget, float randomRatio, float overflowPercent, System.Random rng)`

> **[PLANNED signature.]** The as-built method is `RoomModel.GetHybridEnemySet()` (parameterless,
> reads its own fields) exposed via `RoomModel.GetSpawnSet()`. It uses `UnityEngine.Random` (not an
> injected `System.Random`) and a **random** Phase-2 fill (not `argmin`). The design below is the
> hardened target; adopting it requires the injected-RNG + `argmin` rework noted in Current
> Implementation Status.

The RNG is an **injected parameter**, not constructed internally — this is what makes the method
deterministic and unit-testable (a test passes `new System.Random(fixedSeed)`). A thin runtime
overload `GetHybridEnemySet(idEnemy, weightBudget, randomRatio, overflowPercent)` constructs the
runtime RNG and forwards to the seedable core.

> ✅ **Runtime seed source — RESOLVED 2026-07-09** (see Open Questions #2). Per-room-from-run-seed,
> anchored by the room's own identity: `RoomFile` gains a `roomData` field (direct `RoomData`
> reference, author-assigned per room entry — see "Room → RoomData Resolution" below). The runtime
> overload derives `rng` by combining the dungeon run's global seed with that `RoomFile`'s identity
> (e.g. its list index or `roomName`), so the same run reproduces the same per-room enemy sets, but
> different runs vary. Tests are unaffected — they always call the explicit-`rng` core.

Runs **once** per room (no trial loops). Resolves `idEnemy` → candidate `EnemyData` via `GetByID`
(dropping unknown/null/zero ids), then two phases. Repetition of the same enemy type is allowed in
Phase 2. Returns the chosen `List<EnemyData>`.

**Phase 1 — RANDOM (variety):**
1. `randomBudget = weightBudget × randomRatio`.
2. Shuffle the candidate list with the **injected `rng`** (Fisher–Yates) — same `rng` → same order.
3. Walk the shuffled list **once**; add a candidate if its `weight` fits the remaining random
   sub-budget. Each candidate is considered at most once here.
4. Purpose: guarantee variety between runs.

**Phase 2 — OPTIMAL FILL with overflow (use the budget well):**
1. `remaining = weightBudget − (weight already spent in Phase 1)`.
2. `overflowCap = weightBudget × overflowPercent`.
3. **Pre-loop guard:** if `remaining ≤ 0`, skip Phase 2 entirely (the budget is already spent;
   overflow is for leftover budget, not for topping up an exact spend).
4. Each iteration, over **all** candidates (repetition allowed): consider those eligible, i.e.
   `weight ≤ remaining + overflowCap`; pick the one minimizing `|weight − remaining|`.
   **Tie-break:** on equal `|weight − remaining|`, pick the candidate **earliest in `idEnemy`
   order** (deterministic, independent of shuffle) — this is what preserves the determinism AC.
5. If no candidate is eligible → **stop**. Otherwise add it; `remaining −= weight`.
6. If `remaining ≤ 0` → **stop**.
7. Overflow lets total spend exceed `weightBudget` by at most `overflowCap`, so leftover budget is
   not wasted when no enemy fits exactly.

**Termination guarantee:** because `weight ≥ 1` is enforced (Design invariants), every added
candidate decreases `remaining` by at least 1, so the loop always reaches `remaining ≤ 0` or
"no eligible candidate" in finite steps. This guarantee is **void** if `weight ≤ 0` is ever allowed
— hence the hard clamp, not a soft convention.

### States and Transitions (per-room combat lifecycle) — **[PLANNED — not in code yet]**

> The lifecycle below is **not implemented**. Today spawning is a manual Editor action
> (`LevelManager.SpawnRoomEnemies()` via the "Spawn Enemy" button) with no state machine, door
> lock, alive-count, or events. `EnemyManager` is the ratified runtime driver (ADR-0002) but has
> not been written.

`EnemyManager` (singleton — ratified in ADR-0002) drives the room state:

| State | Enter condition | On enter | Exit condition |
|-------|-----------------|----------|----------------|
| `Idle` | boot / between rooms | nothing | `ON_LOAD_MAP` received |
| `Populating` | `ON_LOAD_MAP` for an uncleared room | resolve `RoomData` for the room → `GetHybridEnemySet` → instantiate at spawn markers → `RoomCell.CloseDoor()` (lock) → set `aliveCount` | spawn complete |
| `Fighting` | spawn complete, `aliveCount > 0` | listen for `ON_ENEMY_DEATH`, decrement `aliveCount` | `aliveCount == 0` |
| `Cleared` | `aliveCount == 0` (or room spawns 0 enemies) | `EventManager.Emit(ON_CLEAR_ENEMY)` (existing subscriber opens doors) + `Emit(ON_ROOM_CLEAR)`; mark `RoomCell.IsCleared = true` | room re-entered while cleared → stay `Cleared` (no re-lock, no re-spawn) |

A room that is **already cleared** on `ON_LOAD_MAP` skips `Populating` entirely (doors stay open).

### Room → `RoomData` Resolution (Open Q#3/#4 — RESOLVED 2026-07-08, mechanism updated 2026-07-09)

**Primary mechanism (2026-07-09): direct per-room reference.** `RoomFile` (the entry type in
`DungeonRoomSO.room`, alongside its existing `roomName`/`filePath`/`roomType` fields — see
`Assets/SO/Dungeon/DungeonRoomSO.cs`) gains a new field:

```csharp
public RoomData roomData;
```

Authored once per room-file entry, at the same time `roomType` is set. `EnemyManager`/`RoomCell`
read `RoomFile.roomData` directly — no `RoomType` enum lookup at runtime required. This also
anchors the per-room shuffle seed (see "Runtime seed source" above): the seed derives from the
run's global seed combined with the owning `RoomFile`'s identity, not from an unrelated global
counter.

- **Combat-bearing rooms**: `roomData` points to a `RoomData` asset with a real `weightBudget`.
- **Non-combat / start rooms**: `roomData` left unassigned (`null`) → treated as **zero budget**
  (safe default — an unconfigured room is empty, not randomly lethal). **`StartRoom` must be
  zero-budget** — the player must never be ambushed on spawn.

**Fallback (kept as a safety net, not the primary path): `RoomType` → `RoomData` lookup table.**
If a `RoomFile.roomData` is ever left unset for a combat-bearing room type, the previously-designed
`RoomType`-keyed table (unchanged from the 2026-07-08 resolution) may still supply a default. This
fallback is what previously **hard-depended on map-system Bug #16** (`RoomFile.roomType` not read at
runtime). Since the primary path now resolves `RoomData` via a direct author-time reference on
`RoomFile` itself, **Bug #16 no longer blocks enemy-spawn** — it remains a real bug (start/end room
still selected by list position elsewhere in map-system) but is no longer a hard dependency for this
system. Downgraded from BLOCKING to informational in Dependencies below.

### Interactions with Other Systems

| Direction | System | Interface |
|-----------|--------|-----------|
| in | Event Bus | `EnemyManager` subscribes `ON_LOAD_MAP` (spawn trigger) and `ON_ENEMY_DEATH` (count) |
| out | Event Bus | emits `ON_CLEAR_ENEMY` (existing — opens doors) and `ON_ROOM_CLEAR` (new — triggers upgrade screen) |
| out | Room Progression | calls `RoomCell.CloseDoor()` on entry, sets `RoomCell.IsCleared` on clear |
| in | Enemy AI | enemy death chain must emit `ON_ENEMY_DEATH` (requires Bugs #7/#8 fixed); spawns framework-wired enemy prefabs |
| in | Dungeon / Room load | reads `Tile_Spawn` marker positions parsed from room JSON |
| out | Per-Run Upgrades | `ON_ROOM_CLEAR` opens the upgrade-card screen |

---

## Formulas

### Random sub-budget

`randomBudget = weightBudget × randomRatio`

### Overflow cap

`overflowCap = weightBudget × overflowPercent`

### Phase-2 pick (each iteration)

`pick = argmin over eligible candidates of |weight − remaining|`,
where a candidate is **eligible** iff `weight ≤ remaining + overflowCap`.
**Tie-break:** on equal `|weight − remaining|`, choose the candidate earliest in `idEnemy` order.

`remaining ← remaining − pick.weight` after each selection.
Phase 2 does not run at all when `remaining ≤ 0` on entry (pre-loop guard).

**Variables:**

| Variable | Type | Range | Description |
|----------|------|-------|-------------|
| `weightBudget` | int | 1 – ~50 | Total spawn budget for the room (the difficulty dial) |
| `randomRatio` | float | 0.0 – 1.0 | Fraction of budget spent in the random phase |
| `overflowPercent` | float | 0.0 – 0.5 (**safe ≤ 0.3**) | Max fraction of budget total spend may exceed `weightBudget`. Canonical range: hard-cap 0.5, recommended tuning ≤ 0.3 |
| `randomBudget` | float | 0 – `weightBudget` | Budget available to Phase 1 |
| `overflowCap` | float | 0 – `weightBudget×0.5` | Absolute overshoot allowed in Phase 2 |
| `remaining` | int | ≤ `weightBudget` (may go slightly negative within `overflowCap`) | Budget left to spend |
| `weight` (per `EnemyData`) | int | **1 – 99 (authored globally, `≥ 1` enforced)** | Spawn cost of one enemy type. Authored once on the `EnemyData` asset, **not** relative to any single room's budget |

**Output Range:** a list of 0 – `weightBudget` enemies (fewer when weights are large). Total spend
∈ `[0, weightBudget + overflowCap]`.

**Worked example:** `weightBudget = 20`, `randomRatio = 0.5`, `overflowPercent = 0.1`;
candidates: Bat(w3), Rat(w2), Golem(w9).
- `randomBudget = 10`. Shuffle → say [Golem, Rat, Bat]. Add Golem(9→remaining1=1), Rat(2>1 skip),
  Bat(3>1 skip). Phase 1 spent = 9.
- Phase 2: `remaining = 20 − 9 = 11`, `overflowCap = 2`. Pick argmin|w−11|: Golem(|9−11|=2) → add,
  remaining=2. Next argmin|w−2|: Rat(0) → add, remaining=0 → **stop**.
- Result: {Golem, Golem, Rat}, total weight 20.

---

## Edge Cases

- **If `idEnemy` is empty or no candidate resolves:** return an empty set; the room spawns 0 enemies
  and immediately enters `Cleared` → emit `ON_CLEAR_ENEMY` + `ON_ROOM_CLEAR` at once (doors open,
  no lock). Prevents the player being trapped in an enemy-less room.
- **If `weightBudget ≤ 0`:** spawn nothing; treat as a cleared room.
- **If any candidate `weight ≤ 0` (must never happen — `weight ≥ 1` is enforced):** this would make
  Phase 2 non-terminating (a `0`-weight candidate is always eligible and never reduces `remaining`;
  a negative weight makes `remaining` grow). It is prevented at the source by the `[Range(1, 99)]` +
  `OnValidate` clamp on `EnemyData.weight`. As defence-in-depth, `GetHybridEnemySet` skips any
  candidate whose resolved `weight < 1` and logs an Editor error rather than trusting the invariant.
- **If every candidate's `weight > weightBudget + overflowCap`:** Phase 2 selects none; return
  whatever Phase 1 produced (possibly empty). Log a balance warning in Editor.
- **If `randomRatio = 0`:** skip Phase 1 entirely; whole budget goes to optimal fill.
- **If `randomRatio = 1`:** Phase 1 may consume the whole budget; Phase 2 still runs on the leftover
  under `overflowCap`.
- **If two `EnemyData` share an `id` (hash collision or hand-edit):** `EnemyDatabase` detects the
  duplicate while building its dictionary and logs an Editor error naming both assets; the first
  entry wins the lookup so behavior is deterministic, not silent corruption.
- **If `RoomData.idEnemy` contains an id not in `sourceMap.idEnemy`:** `OnValidate` strips it and
  logs a warning — a room may only use enemies from its map.
- **If `RoomData.idEnemy` contains an id not in `EnemyDatabase`:** `GetByID` returns null; the id is
  dropped from candidates with a warning.
- **If an `EnemyData.prefab` is null or not framework-wired (no `Entity`/`EntityCore`):** skip that
  instance, log an error, and **do not** add it to `aliveCount` (so the room can still be cleared).
- **If chosen enemies outnumber spawn markers:** distribute round-robin so marker loads differ by at
  most one (`floor(N/M)` or `ceil(N/M)` enemies per marker). Co-located enemies get a **bounded
  jitter**: offset radius ≤ `SPAWN_JITTER_RADIUS` (default 0.5 units) **and** the jittered point must
  resolve to a walkable floor tile — if it lands on a wall/out-of-bounds tile, retry inward toward
  the marker; never place an enemy off the walkable set. Round-robin clumping is an
  *encounter-balance* concern, not just cosmetics (3 enemies on one marker reads as an alpha-strike):
  authoring guidance for marker count is in Tuning Knobs.
- **Entry-safety (no ambush on the seal):** no enemy may spawn within `SPAWN_MIN_ENTRY_DISTANCE`
  (default 3 units) of the door the player entered through. A `Tile_Spawn` marker inside that radius
  is skipped for this spawn (its share redistributes round-robin to the remaining markers). Authoring
  should also keep markers ≥ 3 tiles from any door.
- **If a room has no `Tile_Spawn` markers:** fall back to the room-centre position and log a warning.
  Note this is currently the **de-facto default for all 13 rooms** (markers not yet authored — see
  Dependencies), so the centre-fallback path must itself be entry-safe and jitter-bounded, not
  treated as a rare edge. The fallback runs **before** any round-robin/jitter math, guaranteeing a
  non-empty marker list so the distribution step can never divide by zero.
- **If `ON_ENEMY_DEATH` fires for an enemy the manager did not spawn (e.g. re-entry):** ignore when
  the room is `Cleared`/`Idle`; only decrement while `Fighting`.
- **Determinism for tests:** the shuffle RNG is seedable; a fixed seed makes `GetHybridEnemySet`
  fully deterministic for EditMode tests.

---

## Dependencies

| System | Role | Direction | Status |
|--------|------|-----------|--------|
| **Event Bus** (`EventManager`) | `ON_LOAD_MAP` ✅, `ON_ENEMY_DEATH` **[new]**, `ON_CLEAR_ENEMY` ✅, `ON_ROOM_CLEAR` **[new]** | Spawn ↔ EventManager | 2 new `EventID` values needed |
| **Enemy AI** (`character-system.md`) | Death chain must emit `ON_ENEMY_DEATH`; spawns framework-wired enemy prefabs | Enemy → Spawn | **Blocked:** Bug #7 (`EntityDeathState` wrong base), Bug #8 (empty `Health<=0` transition); only `EnemyPrefab.prefab` wired (Bat/Crab broken) |
| **Room Progression** (`map-system.md`) | `RoomCell.CloseDoor()` / `OpenDoors()` for lock/unlock; `IsCleared` set on clear; `RoomFile.roomData` direct reference (primary) + `RoomType` → `RoomData` table (fallback) — see Room→RoomData Resolution | Spawn → Map | `CloseDoor`/`OpenDoors` ✅; alive-count + lock-on-entry to be added. **New field `RoomFile.roomData` needed** in `Assets/SO/Dungeon/DungeonRoomSO.cs` (author task, not blocked on any bug). map-system Bug #16 (`RoomFile.roomType` not read at runtime) downgraded from BLOCKING to informational — only the fallback table path needed it |
| **Dungeon / Room load** | `RoomGeneraterController` must **parse `Tile_Spawn` markers** (new branch mirroring its `Tile_Door` handling) and **expose each marker's world position** to `EnemyManager`; markers must be **authored into all 13 room JSONs** | Map → Spawn | `TileName.SPAWN` constant ✅; **parser branch does not exist yet** (only `Tile_Door` handled); **markers absent from all 13 JSONs** → LD retrofit pass required (owner: level-design; est. separate task). Until done, every room uses the centre-fallback |
| **Stat System** (`stat-system.md`) | Enemy combat stats stay on prefab `EntityData`/`StatsSO`; spawn system does not touch them | none (contract only) | No schema impact |
| **Object Pooling** | Reuse for enemy instances instead of raw `Instantiate` | Spawn → Pooling | Soft — Pooling is Alpha/not-started; `Instantiate` acceptable until then |
| **Per-Run Upgrades** | `ON_ROOM_CLEAR` opens the upgrade-card screen | Spawn → Progression | Consumer not yet built |

---

## Tuning Knobs

| Knob | Where | Safe range | Effect / failure at extremes |
|------|-------|-----------|------------------------------|
| `weightBudget` | `RoomData` per room | 1 – ~50 | Room difficulty. Too high → overwhelming; too low → trivial/empty |
| `randomRatio` | `RoomData` (or global default) | 0.0 – 1.0 | Variety vs. control. 1.0 → maximal variety, less budget precision; 0.0 → deterministic optimal fill, less variety |
| `overflowPercent` | `RoomData` (or global default) | 0.0 – ~0.3 | Budget-fill tightness. 0 → may under-spend budget; high → rooms overshoot intended difficulty |
| `weight` | `EnemyData` per enemy | 1 – `weightBudget` | Enemy "cost". Should track power tier (creep < elite < champion). Mis-set → cheap enemies dominate or vanish |
| `idEnemy` | `RoomData` / `MapEnemyDatabase` | subset | Which enemies can appear. Empty → no spawns |
| spawn-marker count | room JSON (`Tile_Spawn`) | **3 – 6 per combat room** | Spread of enemies. Authoring rule: ≥ 3 markers in a combat room, each ≥ 3 tiles from any door, on walkable floor, ideally near natural cover/chokepoints. 0 markers → centre-fallback warning (a legal but poor experience) |
| `SPAWN_MIN_ENTRY_DISTANCE` | `GameConstants` | 2 – 4 units (default 3) | No-spawn radius around the entry door. Too low → ambush on the seal; too high → few valid markers in small rooms |
| `SPAWN_JITTER_RADIUS` | `GameConstants` | 0.25 – 0.75 (default 0.5) | Offset when a marker hosts >1 enemy. Must stay small enough to keep enemies on floor tiles |

---

## Visual/Audio Requirements

Modest — this is a spawn/management layer, not a combat feel system.
- **Spawn telegraph (recommended):** a brief VFX/poof + SFX at each `Tile_Spawn` when an enemy
  materializes, so enemies don't pop in silently. Pull from the `Pooling/` VFX system.
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

> **[PLANNED target — not all met by the 2026-07-09 prototype.]** These ACs define "done" for the
> hardened system. The current prototype (`RoomModel`/`LevelManager`) **cannot pass** the
> determinism/tie-break ACs (AC-A3, AC-A4 — no injected RNG, random fill), the `weight ≥ 1`
> enforcement AC (AC-A6 — no clamp), the data-validation ACs (AC-D1…D3 — no `EnemyDatabase`
> lookup / no subset strip), or the room-lifecycle ACs (AC-L1…L6, AC-P1…P3 — no `EnemyManager`).
> They remain the acceptance bar for the planned implementation.

> **Test isolation (applies to all ACs below):** every test constructs its own disposable
> `ScriptableObject.CreateInstance<…>()` for `EnemyDatabase` / `EnemyData` / `RoomData` /
> `MapEnemyDatabase` — **never** the shipped project assets — mirroring the project `PlayerData`
> isolation rule. Algorithm tests inject `new System.Random(fixedSeed)` into the seedable
> `GetHybridEnemySet` overload.

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
> singleton-vs-Inspector ADR — you cannot build the harness against an undecided architecture);
> (2) at least one `Tile_Spawn`-populated fixture room JSON (0/13 today).

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
- **AC-P3 (parser):** GIVEN a room JSON containing `Tile_Spawn` tiles, WHEN the room loads, THEN
  `RoomGeneraterController` exposes their world positions and `EnemyManager` spawns at them (not the
  centre-fallback). Requires the new parser branch.

---

## Open Questions

| # | Question | Status | Notes |
|---|----------|--------|-------|
<<<<<<< HEAD
| 1 | **`EnemyManager` singleton violates "no new singletons"** (only `MazeController` permitted). Owner chose singleton (2026-07-08). | ⬜ OPEN — **decide this sprint** | **Needs an ADR** to ratify the exception, or wrap as an Inspector-wired scene component. Run `/architecture-decision`. **Gates the PlayMode lifecycle test harness (AC-L1…L6)** — tests can't be authored against an undecided architecture. |
| 2 | **Runtime shuffle seed source** — per-room from a global run seed, or fresh unseeded `System.Random()`? | ✅ RESOLVED (2026-07-09) | Per-room-from-run-seed. Anchored by the new `RoomFile.roomData` field (see Q#4) — seed derives from the run's global seed + the owning `RoomFile`'s identity. See "Runtime seed source" callout under `GetHybridEnemySet`. |
=======
| 1 | **`EnemyManager` singleton violates "no new singletons"** (only `MazeController` permitted). Owner chose singleton (2026-07-08). | ✅ RESOLVED (2026-07-09) | Ratified by **ADR-0002** (`docs/architecture/adr-0002-enemymanager-singleton-exception.md`, Status: Proposed) — scoped singleton exception with mandated duplicate-guard + event-driven state. Unblocks the PlayMode lifecycle test harness (AC-L1…L6). Follow-up: update the "only `MazeController`" wording in the four rule files to include `EnemyManager`. |
| 2 | **Runtime shuffle seed source** — per-room from a global run seed, or fresh unseeded `System.Random()`? | ⬜ OPEN — **decide this sprint** | Tests are unaffected (always inject a seed). Recommend per-room-from-run-seed for reproducible runs + future daily-run support. Owner pended 2026-07-08; flagged in the sprint tracker for a daily nudge. |
>>>>>>> origin/claude/enemy-spawn-manager-review-7aq2wa
| 3 | Default values for `randomRatio` / `overflowPercent` (global vs per-`RoomData`)? | ⬜ OPEN | Suggest global defaults (`randomRatio 0.5`, `overflowPercent 0.1`) overridable per room; confirm during balance pass. |
| 4 | How is a `RoomCell` mapped to its `RoomData`? | ✅ RESOLVED (2026-07-08, mechanism updated 2026-07-09) | **Primary: `RoomFile.roomData` direct reference** (new field, author-assigned per room entry — no runtime `RoomType` read needed). **Fallback: `RoomType` → `RoomData` table**, non-combat + start rooms → zero budget. See "Room → RoomData Resolution". No longer hard-depends on map-system Bug #16 — only the fallback path does. |
| 5 | Should `weight` be authored, or derived from the enemy's stat/rank tier? | ⬜ OPEN | Authored for now (`≥ 1` enforced). See Future Enhancements → weight↔stat cross-check. |
| 6 | Multi-wave rooms (second wave partway through)? | ⬜ POST-DEMO | Out of demo scope; schema allows a later `List<wave>` extension. |
| 7 | Boss room: dedicated `RoomData` (one boss id + high budget) vs bypass the algorithm? | ⬜ POST-DEMO | Bosses are out of demo scope per `game-concept.md`. Likely a dedicated zero-random `RoomData`; do not special-case the algorithm before the base loop is playtested. |

---

## Future Enhancements (Should-Have / Post-Demo)

Deferred by owner decision (2026-07-08) — **do not implement for the demo**; recorded so the roadmap
carries them:

- **Composition-diversity phase (Should-Have).** The current greedy Phase-2 fill biases toward
  fewer-heavier sets. A future comparison phase would decide *what kind* of cluster to build (e.g.
  weigh "10 creeps vs 1 elite — which is the more interesting fight for this room?") and bias Phase 2
  accordingly, delivering the full swarm-vs-heavy variety the Player Fantasy promises. Until then,
  keep variety-tuning investment low.
- **Run-depth difficulty escalation (Post-Demo balance).** `weightBudget` is static per `RoomData`;
  the demo accepts that difficulty appears in random order (an early room may be harder than a late
  one). Scaling effective budget by run depth / rooms-cleared is a balance task for after the demo —
  it needs a maze-depth signal the map system does not currently expose.
- **`weight` ↔ stat cross-check (Open Q#5).** An editor validation comparing authored `weight`
  ordering against a derived power score from the prefab's `StatsSO`/rank tier, warning when a
  high-tier enemy is cheap or vice-versa — closes the "weight diverges from real threat" hazard.
- **Object pooling.** Replace `Instantiate` with the `Pooling/` system once it exists (currently
  Alpha / Not Started) — soft dependency, `Instantiate` acceptable for the demo.

---

## Prototype Deviations from the Planned Design (open — owner review)

The 2026-07-09 reverse-sync documented the prototype as-built. These are the points where the
prototype **diverges from the planned/reviewed design**; each is a decision for the owner —
either harden the code up to the design, or amend the design to accept the prototype's approach.

| # | Deviation | As-built | Planned | Risk if left as-is |
|---|-----------|----------|---------|--------------------|
| D1 | RNG source | `UnityEngine.Random` (static) | Injected `System.Random` | Selection is non-deterministic → AC-A3/AC-A4 untestable; no reproducible runs (Open Q#2) |
| D2 | Phase-2 fill | Random pick among eligible | Deterministic `argmin \|weight−remaining\|` + tie-break | Budget not used "optimally"; behaviour not reproducible |
| D3 | Enemy reference model | `RoomModel.enemiesOfRoom` = direct `EnemyModal` refs | `id`-based via `EnemyDatabase.GetByID` | Heavier map/room assets; no central store; no dup/zero-id validation (AC-D1…D3) |
| D4 | `weight ≥ 1` enforcement | Runtime `> 0` guard only | `[Range(1,99)]` + `OnValidate` clamp | A `0`/negative weight authored on `EnemyModal` still possible in the Inspector |
| D5 | `MapModel` role | `mapName` + `idRooms` + `totalWeight` | `mapName` + `idEnemy` (map's enemy set) | Map data does not scope a room's enemy pool as designed |
| D6 | `id` generation | `Math.Abs(Guid.GetHashCode())`, no reroll, no public `EnsureId()` | reroll-until-nonzero + public test hook | `id == 0` edge and in-memory-SO test path unhandled |
| D7 | Runtime driver | `LevelManager.SpawnRoomEnemies()` Editor button, random positions | `EnemyManager` event-driven lifecycle (ADR-0002) | No door lock / alive-count / room-clear / events; not a real run flow |
| D8 | Class naming / location | `EntityModel`/`EnemyModal`/`MapModel`/`RoomModel` in `Database-SO/Modal/` (typo "Modal") | `EnemyData`/`EnemyDatabase`/`MapEnemyDatabase`/`RoomData` | Naming drift between doc and code; prototype sits in production `Script/` tree, not `prototypes/` |
