# Control Manifest — MUST / NEVER

> **Date**: 2026-08-17
> **Sources**: `architecture.md` (ratified), `.claude/rules/*.md`,
> `.claude/docs/technical-preferences.md`, `adoption-analysis.md`
> **Purpose**: flat rules sheet to consult while writing code. The *why* lives in
> `architecture.md`; this file is only the *what*.

---

## Part 1 — Work queue

### Blocked — decide before coding

| # | Decision | Blocks |
|---|---|---|
| 1 | **Health ownership**: `StatsSO` → runtime `StatsComponent`? (Rule 7 reversal) | UI Phase 0 |
| 2 | Is any `EntityStatsSO` asset referenced by more than one enemy prefab? | Whether step 3 is a bug fix or a refactor |
| 3 | Is mobile real? `technical-preferences.md` says PC-only / no touch | UI Phase 6 |

Only #1 actually blocks. Everything else in Phase 0 can proceed without it.

### Ordered work — each step leaves the game runnable

| # | Do this | Risk |
|---|---|---|
| 1 | Fix `EntityStatsSO.ModifiersAmor` self-reference (StackOverflow) | None |
| 2 | Resolve decision #1 above | — |
| 3 | `EntityStatsSO` → config only + runtime `EntityStatsComponent` | Low |
| 4 | `EventManager.Clear()` + scene-load reset | Low |
| 5 | Write the two HIGH-priority ADRs (Event Bus, Damage & Health) | None |
| 6 | Create `Presentation/` — migrate `StatsUI` to Presenter/View/ViewModel | Low |
| 7 | One `AttackCommand` — validate the flow with a single real caller | Medium |
| 8 | Split `PlayerInputHandler`: aim out first, then commands per action | Medium |
| 9 | `Gameplay/Systems/DamageSystem` — only once a second caller exists | Medium |
| 10 | Service layer for pool / scene / audio | Low |

---

## Part 2 — Rules by layer

### Gameplay

**MUST**
- Put new behaviour in a new `PlayerState` / `EntityState` subclass
- Keep state and capability in Entity/Component; keep mechanics in Systems
- Read all tuning values from ScriptableObjects
- Null-check `target` before `.position` / `.transform` / distance math
- Route all damage through `INegativeReceiver.TakeDamage(int, Vector2)`
- Pass `transform.position` as the second argument, always
- Use `Vector2` / `Vector2Int` for gameplay math; `Vector3` only at the Unity API boundary
- Cache `GetComponent<>()` in `Awake()`
- Use `Physics2D.OverlapCircleNonAlloc` with a pre-allocated buffer field

**NEVER**
- Inline `if/else` behaviour chains in `Update()`
- `new`, LINQ, or string concatenation in `Update()` / `LogicUpdate()` / `PhysicsUpdate()`
- `GameObject.Find()`, `FindObjectOfType()`, `SendMessage()`
- `public` fields on MonoBehaviours — use `[SerializeField] private` + property
- Mutate another entity's health field directly
- Hardcode layer indices — set LayerMask in the Inspector
- Physics or allocation in `LogicUpdate()`; that method is transitions and polling only
- `Vector3.one` as a 2D offset — it silently writes z

### Presentation

**MUST**
- Follow `Gameplay → Event/State → Presenter → ViewModel → View → Unity UI`
- Keep the Presenter to exactly three jobs: **Subscribe, Transform, Command**
- Make the View passive: `view.Render(viewModel)` and nothing else
- Put files under `Presentation/<Feature>/` per the ratified folder layout
- Subscribe in `OnEnable()`, unsubscribe in `OnDisable()` — never `Awake`/`Start` alone
- Use `TextMeshProUGUI`, never legacy `Text`
- Show/hide with `CanvasGroup.alpha`, not `SetActive`, for animated elements
- Pool list items (inventory slots, upgrade cards)

**NEVER**
- Let UI own or mutate game state
- Let the View know gameplay types — no `playerStats.GetStrength()` in a View
- Touch `Image` / `Text` / `Button` from a Presenter
- Put `CalculateDamage()`, `ApplyModifier()`, `FindTarget()`, `CanAttack()`,
  `CalculateStats()` in a Presenter — that logic belongs in a gameplay system
- Modify gameplay from a View — go `View → Presenter → Command → System`
- Hold a `GameObject` reference to Player or Enemy
- Poll `PlayerData` in `Update()` — bind to an event
- Grow a giant `UIManager`
- Hardcode pixel sizes or assume mouse hover exists (keeps the mobile door open)

### Data / ScriptableObject

**MUST**
- Keep SOs to configuration: base stats, formulas, curves, prefab refs, ranges
- Put runtime state (current HP, active modifiers, cooldowns, timers, target) in a
  runtime component or plain C# object
- Follow `ScriptableObject → Runtime Object → Gameplay System`
- Use `[Range(min, max)]` on numeric fields
- Preserve existing field-name typos (`attackDamege`) — renaming breaks serialization
- Reset player state through `PlayerData.Reborn()`

**NEVER**
- Write mutable runtime state into a ScriptableObject asset — **this is Rule 7, and it is
  already a live bug in `EntityStatsSO`**
- Assume an SO reference is per-instance: **no SO is instantiated anywhere in this
  codebase**, so every referencing object shares the same asset
- Merge `EnemySO` (spawn/drop) with `EntityData` (AI runtime)
- Modify the `ActivateSkill` base lifecycle — subclass and override instead
- Reset `PlayerData` by destroying and re-instantiating it

### Events

**MUST**
- Extend the `EventID` enum to add an event
- Pair every `Resgister` in `OnEnable()` with `UnResgister` in `OnDisable()`
- Match the source typos exactly: `Resgister` / `UnResgister`
- Give each event an owner, a lifecycle, and clear semantics
- Send only the information the listener needs

**NEVER**
- Add `static Action` fields to individual classes
- Use the bus as a global dumping ground (Rule 9)
- Pass a raw `object` payload without a defined type on new events
- Call `DoorController.OpenDoor()` directly from enemy death — go through the bus

### Services / Singletons

**MUST**
- Wire managers via Inspector reference
- State the reason whenever Unity's lifecycle genuinely forces global access

**NEVER**
- Add a singleton beyond `MazeController` and `EnemyManager` (ADR-0002)
- Reach for `GameManager.Instance` / `StatsManager.Instance` / `AudioManager.Instance`
- Find `UIManager` at runtime — Inspector reference only

---

## Part 3 — Migration prohibitions

These are things that look productive and are not. All from `adoption-analysis.md` §7.

**NEVER, during this migration**

| Don't | Why |
|---|---|
| Rewrite `Player` / `Entity` | 78 and 69 lines, already the target shape — they are not God Objects |
| Convert all 24 `EventManager` call sites at once | High risk, zero gameplay benefit; migrate opportunistically |
| Extract a System for a mechanic with one caller | Rule 6 — an abstraction must solve a concrete problem |
| Build the whole `Commands` layer up front | Introduce one command against one real caller, generalise on the second |
| Build the whole `Services` layer up front | Same reason; it is last in the queue for a reason |
| Refactor `MeleeWeapon`'s damage application now | Current code is correct and already zero-alloc — defer until a second damage source needs the rules |
| Replace the architecture with Clean Architecture / system-wide MVC / system-wide ECS / DDD | Ratified baseline — requires an ADR with a stated technical reason |
| Remove `MonoBehaviour` everywhere for purity | Unity's lifecycle is a legitimate part of the architecture |
| Change gameplay behaviour while refactoring | Migration is behaviour-preserving; a behaviour change is a separate story |
| Fix a bug silently inside a refactor commit | Report it separately — see `ModifiersAmor` |

---

## Part 4 — Rule conformance snapshot

| # | Rule | Status |
|---|---|---|
| 1 | No God Object | ⚠️ `PlayerInputHandler` (324 lines) |
| 2 | No UI dependency in Gameplay | ✅ |
| 3 | No gameplay logic in Presenter | n/a — no Presenter yet |
| 4 | No direct gameplay modification from View | ✅ |
| 5 | No singleton by default | ⚠️ `LevelManager`, `ObjectPoolManager` |
| 6 | No over-engineering | ✅ |
| 7 | Static data ≠ runtime state | ❌ **violated — live bug** |
| 8 | State machine is not a gameplay system | ⚠️ no Systems layer |
| 9 | Events are not a global dump | ⚠️ untyped payloads |
| 10 | Unity is not the whole architecture | ✅ |

Update this table when a status changes.
