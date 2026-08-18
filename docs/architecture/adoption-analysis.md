# Architecture Adoption Analysis

> **Date**: 2026-08-17
> **Baseline**: `docs/architecture/architecture.md` (ratified Hybrid Game Architecture)
> **Scope**: 157 `.cs` files under `Assets/Script/`
> **Method**: per §19 of the ratified architecture — map, find God Objects, find coupling
> and dependency-direction violations, propose incremental migration

**Behaviour-preserving.** Nothing below asks for a gameplay change. Where a real bug is
found it is reported, not silently folded into a refactor.

---

## 1. Verdict

The project is **closer to the target architecture than it looks**, and the gap is
concentrated in three places, not spread everywhere.

| Target element | State | Notes |
|---|---|---|
| Component-Based | ✅ **Good** | `CoreBase` + `GetCoreComponent<T>()` with type cache is a sound hub |
| State Machine | ✅ **Good** | `IState` / `BaseEntity` shared by Player and Entity — real reuse |
| Data-Driven | ⚠️ **Partial** | SOs everywhere, but config and runtime state are mixed |
| Event-Driven | ⚠️ **Partial** | Bus exists; payloads untyped, lifecycle unmanaged |
| **Systems layer** | ❌ **Missing** | Biggest structural gap — mechanics live inside states |
| **Command pattern** | ❌ **Missing** | Input reaches states through boolean flags |
| **Service layer** | ❌ **Missing** | Infrastructure is ad-hoc singletons |
| **Presentation** | ❌ **Missing** | 4 files, 2 of them empty |

The two foundations most worth having — component hub and state machine — are already
built and shared correctly between Player and Enemy. That is the expensive part, and it
is done. What is missing is mostly *additive*, which is why migration can be incremental.

---

## 2. Current layer map

```
        ┌───────────────────────────────────────────────────┐
  UI    │ StatsUI · StatSlot · UIManager(∅) · MainMenu(∅)   │  ← 4 files, 2 empty
        └───────────────────────┬───────────────────────────┘
                                │ reads StatsSO directly ⚠️ no Presenter
        ┌───────────────────────┴───────────────────────────┐
GAMEPLAY│ Player/Entity (BaseEntity, IState)                │
        │ Core/EntityCore (CoreBase)  +  CoreComponents     │
        │ States  ← mechanics live HERE instead of Systems  │
        │ Weapons · Skills · Projectile                     │
        └───────────────────────┬───────────────────────────┘
        ┌───────────────────────┴───────────────────────────┐
 WORLD  │ Maze · RoomGrid · Door · EnemySpawner · Pathfinding│
        └───────────────────────┬───────────────────────────┘
        ┌───────────────────────┴───────────────────────────┐
 FOUND. │ EventManager · AnimationEventManager · ObjectPool │
        │ StatSystem · GameConstants                        │
        └───────────────────────────────────────────────────┘
                                │
        ┌───────────────────────┴───────────────────────────┐
  DATA  │ PlayerData · EntityData · EntityStatsSO · AttackSO│
        │ StatsSO · EnemySO · WeaponSO · ItemOS            │  ⚠️ mixes config + runtime
        └───────────────────────────────────────────────────┘
```

---

## 3. God Objects

### `PlayerInputHandler` — 324 lines, the clearest violation of Rule 1

`Character/Player/CoreComponent/PlayerInputHandle.cs`. One class currently owns:

| Concern | Evidence |
|---|---|
| Raw input reading | `PlayerInput`, `moveVector`, `mouseVector`, `screenPos` |
| Aim resolution | `IAimProvider`, `directionMouseVector`, `angleRotationPlayer`, camera ref |
| 8-direction math | three parallel sets: keyboard / externality / mouse (angle + index each) |
| Intent flags | `isAttack`, `BufferIsAttack`, `isSkill`, `isDisadvantage`, `isTakeDamage`, `isEquip_Unequip`, `isInteractor` |
| Sub-state enums | `SkillState`, `SkillType`, `DisadvantageState` |
| Animation status | `statusAnimation` |

Input, aiming, intent buffering, and animation status are four separable concerns. The
input-buffer flags are exactly what the **Command** layer is meant to replace — this class
is the single highest-value target for the Command migration.

**No other God Object found.** `Player` (78 lines) and `Entity` (69 lines) are state-machine
hosts holding references — correct per the architecture, and explicitly the shape §3 of the
baseline wants preserved.

---

## 4. Rule 7 violation — the most damaging finding

**Static data and runtime state are stored in the same ScriptableObject assets, and no SO
is ever instantiated.**

Verified: `grep` for `Instantiate(...SO)` / `ScriptableObject.CreateInstance` across all
157 files returns **zero call sites**. Every consumer references the shared asset.

| Asset | Runtime state stored on it | Consequence |
|---|---|---|
| `EntityStatsSO` | `health`, `modifiersHealth`, `velocities`, `amor` | **All enemies sharing the asset share one health pool** |
| `PlayerData` | `currentHealth` (public field) | Persists across Editor Play sessions |
| `StatsSO` | modifier stack, `statViewDTOs` | Mitigated by `OnEnable()` clearing modifiers |

The `EntityStatsSO` case is a live gameplay bug, not a style issue: damage one bat and
every bat referencing that asset loses health.

**Direction:** SO holds base/config only; a runtime component owns current values, per §8
of the baseline (`ScriptableObject → Runtime Object → Gameplay System`).

### ⚠️ This reverses a decision made earlier today

The UI plan currently records **"health source of truth = `StatsSO`"**. Under Rule 7 that
is no longer the right answer — current HP is the canonical mutable runtime state, and
putting it on a shared asset is precisely the shared-state bug the rule exists to prevent.

**Recommended replacement:** `StatsSO` keeps base stats and formulas; a runtime
`StatsComponent` (Gameplay/Components) owns `CurrentHP` and the live modifier stack and
raises `HealthChangedEvent` / `StatsChangedEvent`. This satisfies both the original goal
(one owner, one change signal, equipment moves MaxHP) and Rule 7.

This needs owner confirmation — it changes Phase 0 of the UI plan.

---

## 5. Separate bug found during analysis

`EntityStatsSO.cs:45-55` — `ModifiersAmor` is self-referential in both accessors:

```csharp
public float ModifiersAmor
{
    get => ModifiersAmor;              // returns itself → infinite recursion
    set { if (ModifiersAmor != value)  // reads itself → infinite recursion
          { ModifiersAmor = value; ... } }
}
```

Reading or writing this property is an immediate `StackOverflowException`, which in Unity
kills the Editor process rather than throwing. It survives only because nothing calls it
yet. The backing field `modifiersAmor` (line 15) is never used.

Unrelated to the architecture migration — reported so it does not get discovered the hard
way. Fix is one line each: `get => modifiersAmor;` and assign `modifiersAmor`.

---

## 6. Per-class reviews

### `PlayerInputHandler`

| | |
|---|---|
| **Current responsibility** | Input reading, aim resolution, 8-direction math, intent flags, animation status |
| **Current dependencies** | `PlayerInput`, `Camera`, `Core`, `StatusAnimation` |
| **Current layer** | Gameplay / Component |
| **Problems** | God Object (Rule 1); intent flags are an ad-hoc command queue; states poll booleans, so gameplay is coupled to *this* input source |
| **Target layer** | split: **Services/InputService** (raw input) + **Gameplay/Components/AimComponent** + **Gameplay/Commands** |
| **Recommended responsibility** | Translate input into commands. Nothing else. |
| **Dependencies after** | `InputService` → emits `AttackCommand` etc. No knowledge of states |
| **Migration steps** | 1. Extract aim into `AimComponent` (keep `IAimProvider`). 2. Introduce one command (`AttackCommand`) alongside the existing flag. 3. Move `PlayerAttackState` to consume the command. 4. Delete the flag. 5. Repeat per action. |

### `EntityStatsSO`

| | |
|---|---|
| **Current responsibility** | Base stats **and** runtime health for an enemy |
| **Current dependencies** | none |
| **Current layer** | Data |
| **Problems** | Rule 7 violation — shared mutable state across all instances; self-referential property (§5) |
| **Target layer** | Data (config only) |
| **Recommended responsibility** | `baseHealth`, `baseVelocities`, `baseAmor`. Read-only at runtime. |
| **Dependencies after** | none |
| **Migration steps** | 1. Fix the `ModifiersAmor` recursion. 2. Add runtime `EntityStatsComponent` holding current values, initialised from the SO in `Awake`. 3. Point `EntityNegativeReciver` at the component. 4. Strip runtime fields from the SO. |

### `StatsUI` + `StatSlot`

| | |
|---|---|
| **Current responsibility** | Reads `StatsSO` directly, instantiates slots, toggles itself, formats text |
| **Current dependencies** | `StatsSO`, `EventManager`, `TextMeshProUGUI` |
| **Current layer** | Presentation — but talks straight to gameplay data |
| **Problems** | No Presenter (Rule 3/4 shape); View knows gameplay types; re-instantiates slots on every open; unchecked `(bool)obj` unbox |
| **Target layer** | `Presentation/Character/Stats/` |
| **Recommended responsibility** | `CharacterStatsView.Render(CharacterStatsViewModel)` — nothing else |
| **Dependencies after** | View → ViewModel only. Presenter subscribes to `StatsChangedEvent`. |
| **Migration steps** | Covered by `UI_ARCHITECTURE_PLAN.md` Phase 2, retargeted to Presenter/View/ViewModel |

### `EventManager`

| | |
|---|---|
| **Current responsibility** | Static `Dictionary<EventID, Action<object>>` bus |
| **Current dependencies** | none |
| **Current layer** | Foundation → should become **Services/EventService** |
| **Problems** | Rule 9 — untyped `object` payloads, no ownership, no lifecycle; never cleared, so listeners leak across scene reloads |
| **Target layer** | Services |
| **Recommended responsibility** | Typed publish/subscribe with a defined lifetime |
| **Dependencies after** | none |
| **Migration steps** | 1. Add `Clear()` + scene-load reset (fixes the leak without touching call sites). 2. Introduce typed event structs for *new* events only. 3. Migrate the 24 existing call sites opportunistically, never in one pass. |

### `MeleeWeapon` / `Weapon`

| | |
|---|---|
| **Current responsibility** | Owns the hitbox query **and** applies damage |
| **Current dependencies** | `Physics2D`, `INegativeReceiver`, `AttackSO`, `IAimProvider` |
| **Current layer** | Gameplay / Component |
| **Problems** | Damage application belongs in a `DamageSystem` (Rule 8) — currently every weapon re-implements it |
| **Target layer** | Component stays; damage resolution moves to `Gameplay/Systems/DamageSystem` |
| **Recommended responsibility** | Detect targets, raise a damage request |
| **Dependencies after** | Weapon → `DamageSystem`. No direct `TakeDamage` call. |
| **Migration steps** | Low priority — the current implementation is correct and uses `OverlapCircleNonAlloc` with a cached buffer. Defer until a second damage source needs the same rules. |

### `LevelManager`, `ObjectPoolManager`

| | |
|---|---|
| **Problems** | Rule 5 — singleton/global access without a stated reason (tracked as TD-023 / TD-031) |
| **Target layer** | `Services/SceneService`, `Services/PoolService` |
| **Migration steps** | Convert to injected services when the Service layer is introduced. Not urgent. |

---

## 7. Migration plan — incremental, behaviour-preserving

Ordered by value-per-risk. Each step leaves the game runnable.

| # | Step | Why first | Risk |
|---|---|---|---|
| 1 | Fix `ModifiersAmor` recursion | Latent Editor crash | None |
| 2 | Confirm the Rule 7 / health-ownership reversal with the owner | Blocks UI Phase 0 | — |
| 3 | `EntityStatsSO` → config + runtime `EntityStatsComponent` | Fixes a live shared-health bug | Low |
| 4 | `EventManager.Clear()` + scene reset | Unblocks menu→game→death→restart | Low |
| 5 | Introduce `Presentation/` with Presenter/View/ViewModel; migrate `StatsUI` | Proves the pattern on existing code | Low |
| 6 | Introduce `Gameplay/Commands` with **one** command (`AttackCommand`) | Validates the flow before committing | Medium |
| 7 | Split `PlayerInputHandler` — aim out, then commands per action | Largest God Object | Medium |
| 8 | Introduce `Gameplay/Systems` starting with `DamageSystem` | Only once two callers need it | Medium |
| 9 | Service layer for pool/scene/audio | Lowest urgency | Low |

**Steps 6-8 are deliberately late.** Commands and Systems are the two elements the project
lacks entirely, and both are tempting to build speculatively. Rule 6 says an abstraction
must solve a concrete problem — so each is introduced against one real caller first and
generalised only when a second appears.

### Not recommended

- Rewriting `Player` / `Entity` — they already match the target shape.
- Converting all 24 `EventManager` call sites at once — high risk, no gameplay benefit.
- Extracting Systems for mechanics that currently have exactly one caller.

---

## 8. Open questions for the owner

1. **Health ownership** (§4) — confirm the reversal from `StatsSO` to a runtime component.
   Blocks UI Phase 0.
2. **Enemy stats sharing** — is any `EntityStatsSO` asset currently referenced by more than
   one enemy prefab? If yes, step 3 is a bug fix, not a refactor, and should be prioritised.
3. **Platform scope** — `technical-preferences.md` says PC-only / no touch, which still
   conflicts with the mobile-later assumption in the UI plan.
