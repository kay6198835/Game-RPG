# Architecture Review Report

> **Date**: 2026-07-13
> **Engine**: Unity 2022.3.62f3 LTS
> **Mode**: `/architecture-review` (full)
> **GDDs Reviewed**: 8 · **ADRs Reviewed**: 3
> **Verdict**: **CONCERNS**

---

## Scope Note

Most systems in this project are **reverse-documented** from working code. Their
architecture is recorded informally in `.claude/rules/*.md` and the "Architecture
Decisions Log" in `.claude/docs/technical-preferences.md`, but only 3 formal ADRs
exist. This review therefore reads primarily as an **ADR-backfill map**, prioritized
by risk — not a catalogue of undecided architecture.

Support-file state at review time:
- No `docs/architecture/tr-registry.yaml` — all TR-IDs below are new (not yet persisted).
- No `docs/consistency-failures.md` — no reflexion log to append; no known conflict-prone areas.
- No `docs/architecture/architecture.md` — no master blueprint exists yet.
- Engine reference is `VERSION.md` only (no `breaking-changes.md` / `deprecated-apis.md` / `modules/`).

---

## Traceability Summary

- **Total technical requirements**: 59
- ✅ **Covered**: 8
- ⚠️ **Partial**: 1
- ❌ **Gaps**: 50

---

## Full Traceability Matrix

### Character / Player Controller + Enemy AI (`character-system.md`)

| TR-ID | Requirement | ADR | Status |
|-------|-------------|-----|--------|
| TR-char-001 | Hierarchical state machine shared by Player + Entity | — | ❌ Gap |
| TR-char-002 | Core/EntityCore component hub — self-register + `GetCoreComponent<T>()` | — | ❌ Gap |
| TR-char-003 | All damage/death/health flow through `INegativeReceiver.TakeDamage(int, Vector2)`; no direct health mutation | — | ❌ Gap (HIGH) |
| TR-char-004 | `ON_PLAYER_DEATH` → GameManager `Reborn()` + scene reload | — | ❌ Gap |
| TR-char-005 | Entity death → `EntityDeathState` (extends `EntityState`) → despawn + emit death event | — | ❌ Gap |
| TR-char-006 | Enemy detection via `Physics2D.OverlapCircle` per frame (must be NonAlloc in hot path) | — | ❌ Gap |
| TR-char-007 | Null-safe target handling in `EntityMoveState` | — | ❌ Gap |
| TR-char-008 | Animation-event-driven combat transitions (trigger/finished flags) | — | ❌ Gap |
| TR-char-009 | ScriptableObject-driven stats (`PlayerData`, `EntityData`, `EntityStatsSO`) | — | ❌ Gap |
| TR-char-010 | Enemy registered with room-clear tracking | — | ❌ Gap |

### Melee Combat + Weapon System (`weapons-system.md`)

| TR-ID | Requirement | ADR | Status |
|-------|-------------|-----|--------|
| TR-weap-001 | `WeaponMelee.Attack()` applies damage via `OverlapCircleAll` + `INegativeReceiver.TakeDamage` | — | ❌ Gap |
| TR-weap-002 | Combo chain from `List<AttackSO>` in `WeaponMeleeStats` SO | — | ❌ Gap |
| TR-weap-003 | Weapon carries two ability slots (RMB/E) registered with `AbilityHolder` | — | ❌ Gap |
| TR-weap-004 | Equip/unequip one weapon at a time via interaction | — | ❌ Gap |
| TR-weap-005 | Ranged projectile flow (cooldown, collision → `TakeDamage`) | — | ❌ Gap |
| TR-weap-006 | Attack hitbox layer masks set in Inspector | — | ❌ Gap |
| TR-weap-007 | Bullets pooled — no `Instantiate` per shot | — | ❌ Gap |

### Animation System (`animation-system.md`)

| TR-ID | Requirement | ADR | Status |
|-------|-------------|-----|--------|
| TR-anim-001 | `AnimationEventManager` static bus — separate from `EventManager`; typed `AnimationEventId` | — | ❌ Gap (MED) |
| TR-anim-002 | Per-character `AnimationPlayerController` register OnEnable / unregister OnDisable | — | ❌ Gap |
| TR-anim-003 | State machines consume `isAnimationTrigger`/`isAnimationFinished` flags, reset each frame | — | ❌ Gap |
| TR-anim-004 | `runtimeAnimatorController` swap for skills (depth 1, no stacking) | — | ❌ Gap |
| TR-anim-005 | Per-attack 8-directional `AnimatorOverrideController` via `AttackSO.directionAttackAnimatorOV` | — | ❌ Gap |
| TR-anim-006 | Timing config in Animator inspector only (not code/SO) | — | ❌ Gap |

### Dungeon Generation + Room Progression (`map-system.md`)

| TR-ID | Requirement | ADR | Status |
|-------|-------------|-----|--------|
| TR-map-001 | DFS maze generation via `MazeController` singleton → `Cell[]` with door status | — | ❌ Gap |
| TR-map-002 | Two parallel grids (world + minimap) on `BaseGrid<T>` | — | ❌ Gap |
| TR-map-003 | Build-safe room-JSON loading (TextAsset/StreamingAssets) — Bug #15 | — | ❌ Gap (MED) |
| TR-map-004 | Door status contract (DISABLE/ENEBLE/BE_OPEN/OPEN/CLOSE); collider only on OPEN | — | ❌ Gap |
| TR-map-005 | Room transition via EventManager (`ON_PLAYER_ON_DOOR`/`ON_LOAD_MAP`/`ON_LOAD_MAZE_DONE`) + teleport | — | ❌ Gap |
| TR-map-006 | Room-clear locking — lock on entry, unlock on `ON_CLEAR_ENEMY` | — | ❌ Gap |
| TR-map-007 | Random room assignment from pool (Fisher-Yates `PickUniqueIndex`) | — | ❌ Gap |
| TR-map-008 | `RoomType` drives start/boss selection at runtime — Bug #16 | — | ❌ Gap |
| TR-map-009 | `GetNext` bounds check to prevent `IndexOutOfRangeException` | — | ❌ Gap |
| TR-map-010 | `MazeController` duplicate-instance guard (`return` after `Destroy`) — Bug #14 | ADR-0002 (ref) | ⚠️ Partial |
| TR-map-011 | `LevelManager` singleton — Bug #12 / TD-023 (ADR violation) | — | ❌ Gap (MED) |
| TR-map-012 | `ON_ENEMY_DEATH` + `ON_ROOM_CLEAR` events needed | — | ❌ Gap |

### Stat System (`stat-system.md`)

| TR-ID | Requirement | ADR | Status |
|-------|-------------|-----|--------|
| TR-stat-001 | Primary→derived formula (`baseConstant + level×perLevel + Σ primary×coeff`) | — | ❌ Gap |
| TR-stat-002 | `StatsSO` storage — serialized List + runtime Dictionary O(1) lookup | ADR-0001 | ✅ Covered |
| TR-stat-003 | `StatModifier` stack on top of base (never mutate base) | — | ❌ Gap |
| TR-stat-004 | Level-aware recalculation on stat/level change; `OnStatChanged` event | — | ❌ Gap |
| TR-stat-005 | Percentage stats clamp; `perLevel = 0` for pct/fixed | — | ❌ Gap |
| TR-stat-006 | Derived formulas reference only primary stats (no circular dependency) | — | ❌ Gap |

### Skill & Ability System (`skill-ability-system.md`)

| TR-ID | Requirement | ADR | Status |
|-------|-------------|-----|--------|
| TR-skill-001 | `ActivateSkill` SO 5-phase lifecycle driven by `AbilityHolder` | — | ❌ Gap |
| TR-skill-002 | `DoCast` vs `DoNonCast` type behaviour | — | ❌ Gap |
| TR-skill-003 | Cooldown reset in each ability's `Exit()` | — | ❌ Gap |
| TR-skill-004 | Weapon-bound skills (`abilityWeapon`/`skillWeapon`) | — | ❌ Gap |
| TR-skill-005 | Effect system — `IEffectable.ApplyEffect`; `EffectSkillSO` (DuringTime/OneTime) | — | ❌ Gap |
| TR-skill-006 | Projectile skills (`Projectile`/`Spell` raycast → `TakeDamage`) | — | ❌ Gap |
| TR-skill-007 | Passive upgrades — `InternalSkillSO` cards on `ON_ROOM_CLEAR`; SO-driven `TalentManager` | — | ❌ Gap |
| TR-skill-008 | Projectiles pooled | — | ❌ Gap |

### Enemy Spawn & Per-Room Management (`enemy-spawn-system.md`)

| TR-ID | Requirement | ADR | Status |
|-------|-------------|-----|--------|
| TR-spawn-001 | `EnemyManager` singleton drives room lifecycle (Idle→Populating→Fighting→Cleared) | ADR-0002 | ✅ Covered |
| TR-spawn-002 | Selection = Room Budget + Candidate Pool + `RarityTier` (Option C) on `RoomModel.GetSpawnSet()` | ADR-0003 | ✅ Covered |
| TR-spawn-003 | `EnemyModal` = spawn metadata only (not a fourth stat store) | ADR-0003 / ADR-0001 | ✅ Covered |
| TR-spawn-004 | Room→`RoomModel` resolution via `MapModel.GetRandomRoom()` shuffle-bag | — | ❌ Gap |
| TR-spawn-005 | `Tile_Spawn_Enemy` parsing → `ON_GET_SPAWN_POSITIONS`; centre-fallback for markerless rooms | — | ❌ Gap |
| TR-spawn-006 | `aliveCount` tracked vs `ON_ENEMY_DEATH`; emit `ON_CLEAR_ENEMY`/`ON_ROOM_CLEAR` | — | ❌ Gap |
| TR-spawn-007 | `weight ≥ 1` invariant (`[Range(1,99)]` + `OnValidate` clamp) — termination guard | ADR-0003 | ✅ Covered |
| TR-spawn-008 | Zero-alloc selection hot path (scratch buffers) | ADR-0003 | ✅ Covered |
| TR-spawn-009 | `EnemyManager` per-room state reset on `ON_LOAD_MAP` | ADR-0002 | ✅ Covered |
| TR-spawn-010 | PlayMode lifecycle test harness (AC-L1…L6) with deterministic reset | ADR-0002 | ✅ Covered |

---

## Coverage Gaps (Priority Fix List)

### Foundation layer — HIGH priority (write ADRs first)

- ❌ **TR-fnd-EVENT (Event Bus)** — the static `EventManager` pub/sub bus has **no ADR**, yet it is
  the highest-risk system in the index (12 of 20 systems route through it). The `EventID`-enum-only
  extension rule and register/unregister lifecycle are enforced in `manager-event-code.md` but never
  formalized architecturally.
  Suggested ADR: `/architecture-decision Event Bus` · Engine Risk: LOW
- ❌ **TR-char-003 (Damage & Health)** — the `INegativeReceiver.TakeDamage(int, Vector2)` contract and
  the question of *who owns the health value* (Core / EntityCore hub) has no ADR. This is a shared
  Foundation contract (Combat, Enemy AI, HUD, Death) and is currently **broken in code** (Bug #6:
  `Core.TakeDamage()` removed, `NegativeReciver.TakeDamage()` throws `NotImplementedException`).
  Suggested ADR: `/architecture-decision Damage & Health` · Engine Risk: LOW

### Foundation / Core layer — MEDIUM priority

- ❌ **TR-char-002 (Core component hub)** — `GetCoreComponent<T>()` discovery pattern. `engine-code.md`
  calls its public API "STABLE" but no ADR records the contract.
- ❌ **TR-char-001 (State machine pattern)** — recorded as "enforced" in the Decisions Log, no formal ADR.
- ❌ **TR-anim-001 (Animation second bus)** — running a *second* static bus separate from `EventManager`
  is a deliberate architectural decision worth an ADR.
- ❌ **TR-map-003 (Build-safe JSON loading)** — StreamingAssets vs TextAsset vs Addressables (Bug #15).
- ❌ **TR-map-011 (LevelManager singleton)** — Bug #12 / TD-023; ADR-0002 already names this as a
  structurally-identical unratified violation. Needs either a ratification ADR or a refactor.

### Feature / Presentation layer — LOW priority

The remaining ~41 gaps are ordinary design/implementation details (combo chains, skill lifecycle,
effect system, minimap, room transitions, etc.) that are already implemented or do not warrant an
architectural record. Backfill opportunistically, not blocking.

---

## Cross-ADR Conflicts

**None.** The 3 ADRs are mutually consistent and explicitly cross-referenced:

- **ADR-0002 ↔ ADR-0003** are declared siblings. ADR-0002 deliberately keeps the selection
  algorithm *off* the singleton so ADR-0003's `GetSpawnSet()` stays unit-testable. Complementary.
- **ADR-0001** is cited as the boundary contract by both 0002 and 0003 (`EnemyModal`/`EnemyData`
  must not become a fourth stat store; combat stats stay on `EntityData`/`StatsSO`). Reinforcing.

---

## ADR Dependency Order

All three ADRs declare `Depends On: None` → all Foundation-level, independently implementable. No cycles.

```
Foundation (no dependencies):
  1. ADR-0001  StatSystem dual data structure
  2. ADR-0002  EnemyManager singleton exception   → Enables PlayMode lifecycle harness
  3. ADR-0003  Enemy spawn selection (Option C)    → Enables S5-A3 + Sprint 6 GetSpawnSet() rewrite
```

⚠️ **Status blocker**: all three ADRs are still `Proposed`, none `Accepted`. ADR-0002 and ADR-0003
"Enable" downstream stories only once Accepted, so the Sprint 5/6 stories they unblock (PlayMode
lifecycle harness, S5-A3 `RarityTier` field, Sprint 6 `GetSpawnSet()` rewrite) cannot legitimately
start until these flip to Accepted.

---

## GDD Revision Flags

**None** — all GDD assumptions are consistent with verified engine behaviour. Engine risk is LOW
across every ADR. ADR-0001's only post-cutoff reference (Unity 6.0.0.5 Dictionary-inspector
behaviour) is correctly gated behind "Verification Required" and contradicts no GDD.

---

## Engine Compatibility

```
Engine: Unity 2022.3.62f3 LTS
ADRs with Engine Compatibility section: 3 / 3   (no blind spots)
Version consistency:       PASS — all 3 ADRs cite 2022.3.62f3 LTS
Post-cutoff API conflicts: none
Deprecated API references: none in any ADR
```

Minor code-standard note (not an ADR conflict): `VERSION.md` and `ai-code.md` mandate
`OverlapCircleNonAlloc` in per-frame paths, but `character-system.md` (TR-char-006) documents
`EntityInput.Update()` using allocating `Physics2D.OverlapCircle` every frame. This is a
gameplay-code compliance item for the AI-code owner, not an architecture decision.

---

## Architecture Document Coverage

`docs/architecture/architecture.md` does not exist. With 3 ADRs against 20 indexed systems, there
is no master blueprint tying the Foundation layer together. Recommend `/create-architecture` after
the two HIGH-priority Foundation ADRs (Event Bus, Damage & Health) are written.

---

## Verdict: CONCERNS

No blocking conflicts, engine-consistent, and the 3 existing ADRs are well-formed. But the two
highest-risk **Foundation** systems — Event Bus and the `INegativeReceiver` damage contract — have
zero ADR coverage, and the damage contract is currently broken in code (Bug #6). These decisions
exist informally (rules files + Architecture Decisions Log), which keeps this at CONCERNS rather
than FAIL, but they should be formalized before more systems build on top of them.

### Immediate actions (most foundational first)

1. `/architecture-decision Event Bus` — record the `EventManager` static-bus contract, the
   `EventID`-enum-only extension rule, and the register/unregister lifecycle.
2. `/architecture-decision Damage & Health` — record the `INegativeReceiver.TakeDamage(int, Vector2)`
   contract and health-value ownership (Core / EntityCore hub). Directly relevant to Bug #6.
3. Accept ADR-0001 / ADR-0002 / ADR-0003 (or return for revision) so the Sprint 5/6 stories they
   enable can start.
4. Backfill later: Core component-hub pattern, Animation second-bus, build-safe JSON loading
   (Bug #15), and the `LevelManager` / `ObjectPooling` singleton violations (TD-023 / TD-031).

### Gate guidance

When the two HIGH-priority Foundation ADRs are written and all three existing ADRs are Accepted,
run `/gate-check pre-production` to advance. Re-run `/architecture-review` after each new ADR to
verify coverage improves.
