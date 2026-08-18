# Game-RPG — Master Architecture

> **Status**: RATIFIED — this is the project's baseline architecture
> **Ratified**: 2026-08-17 by project owner
> **Engine**: Unity 2022.3.62f3 LTS
> **Current conformance**: see `docs/architecture/adoption-analysis.md`

Do not replace this with a different architecture — full Clean Architecture, system-wide
MVC/MVVM, system-wide ECS, or DDD — without a stated technical reason recorded as an ADR.

---

## 1. Overall model

Hybrid Game Architecture. Four concerns, with data/config underneath all of them.

```
                         GAME
                          │
        ┌─────────────────┼─────────────────┐
        │                 │                 │
     GAMEPLAY          SERVICES        PRESENTATION
        │                 │                 │
        │                 │           ┌─────┴─────┐
        │                 │           │           │
        │                 │          View     Presenter
        │                 │           │           │
        │                 │       Unity UI    ViewModel
        │                 │                 │
        └─────────────────┼─────────────────┘
                          │
                     DATA / CONFIG
```

Patterns in use: Component-Based, Systems-based gameplay, Data-Driven, Event-Driven,
State Machine, Command, Service Layer, Presenter + ViewModel, ScriptableObject for static
config.

**Apply a pattern only where it solves a real problem.** Rule 6 below is binding.

---

## 2. Dependency direction — the most important rule

```
                    ┌──────────────┐
                    │   GAMEPLAY   │
                    └──────┬───────┘
                           │ Events / State
                           ▼
                    ┌──────────────┐
                    │  PRESENTER   │
                    └──────┬───────┘
                           │ ViewModel
                           ▼
                    ┌──────────────┐
                    │     VIEW     │
                    └──────┬───────┘
                           ▼
                    ┌──────────────┐
                    │   UNITY UI   │
                    └──────────────┘
```

Interaction travels the other way, and only through commands:

```
Unity UI → View → Presenter → Command → Gameplay System
```

- Gameplay must never depend on Unity UI.
- Gameplay must never know `Text`, `Image`, `Button`, `VisualElement`, `UIDocument`.
- The View must never modify gameplay state directly.

---

## 3. Gameplay layer

Gameplay is the core. Target structure:

```
Gameplay
├── Entity        Player · Enemy · NPC
├── Components    Stats · Health · Combat · Movement · ...
├── Systems       CombatSystem · DamageSystem · StatSystem ·
│                 MovementSystem · AISystem · SpawnSystem · InventorySystem
├── StateMachine
├── Commands
└── Events
```

**Entity/Component holds state and capability. System processes mechanics.**

Never let `Player` or `Entity` become a God Object. The current split is correct and
should be preserved:

```
Player
├── MovementComponent
├── CombatComponent
├── StatsComponent
├── HealthComponent
└── StateMachine
```

### Project mapping [as of 2026-08-17]

| Architecture element | This project |
|---|---|
| Entity | `Character/Player/Player.cs`, `Character/Entity/Entity.cs` (both `BaseEntity`) |
| Component hub | `Core` / `EntityCore` (both `CoreBase`), `GetCoreComponent<T>()` |
| Components | `Character/Player/CoreComponent/`, `Character/Entity/CoreComponent/` |
| State machine | `PlayerStateMachine`, `EntityStateMachine`, `IState` |
| **Systems** | ❌ **does not exist** — logic currently lives inside states and components |
| **Commands** | ❌ **does not exist** — input drives states through boolean flags |
| Events | `Manager/EventManager.cs` (`EventID` enum) |

---

## 4. State machine

Manages behaviour and lifecycle of an entity: transitions, state lifecycle,
state-specific behaviour.

```
Player:  Idle · Move · Attack · Combo · Dodge · Hit · Dead
Enemy:   Idle · Patrol · Chase · Attack · Recover · Flee · Dead
```

**A state machine is not a gameplay system** (Rule 8). Do not push whole systems into
states. A state decides *when*; a system decides *what happens*.

---

## 5. Command pattern

A command represents a gameplay action, decoupled from whoever requested it.

```
Input / AI / Network
        │
        ▼
     Command  →  Command Handler  →  System  →  Gameplay
```

```
Keyboard ───────┐
AI ─────────────┼──> AttackCommand ──> CombatSystem
Network ────────┘
```

Examples: `AttackCommand`, `DodgeCommand`, `MoveCommand`, `UseSkillCommand`,
`EquipItemCommand`, `InteractCommand`.

The point is that gameplay logic never depends on the *source* of the action. This is
what makes AI, replay, and networked input possible later without touching combat code.

---

## 6. Events

Gameplay must not know what UI, audio, or VFX are doing.

```
CombatSystem → DamageEvent ─┬─> UI
                            ├─> Audio
                            ├─> VFX
                            └─> Quest
```

Examples: `DamageEvent`, `HealthChangedEvent`, `StatsChangedEvent`, `ItemEquippedEvent`,
`EnemyDiedEvent`, `AttackStartedEvent`, `AttackFinishedEvent`.

An event carries only the information the listener needs. **The event bus is not a global
dumping ground** (Rule 9) — every event needs an owner, a lifecycle, and clear semantics.

---

## 7. Service layer

For systems with a global responsibility or an infrastructure role:

```
Services
├── SaveService
├── AudioService
├── PoolService
├── SceneService
├── InputService
└── EventService
```

**Do not make these singletons by default** (Rule 5). Prefer dependency injection, a
composition root, or explicit dependencies. Where Unity's lifecycle genuinely forces
global access, the reason must be stated and the dependency controlled.

Currently permitted singletons: `MazeController`, `EnemyManager` (ADR-0002). Nothing else.

---

## 8. Data-driven design

Separate configuration from runtime state.

```
ScriptableObject  →  Runtime Object  →  Gameplay System
   (config)          (mutable state)
```

```
Data
├── CharacterData · EnemyData · WeaponData
├── SkillData · AttackData · AnimationData
└── ItemData
```

**Rule 7 is binding and currently violated — see §11.** A ScriptableObject asset is
shared by every instance that references it. Mutable runtime state written into an SO is
shared state, and in Unity it also survives between Editor Play sessions.

| Configuration data | Runtime state |
|---|---|
| Base stats, formulas, curves | Current HP, active modifiers, cooldowns |
| Prefab references, ranges, damage | Position, target, state timers |
| Lives in a ScriptableObject asset | Lives in a runtime component or plain C# object |

---

## 9. Presentation layer

```
Gameplay ──Event/State──> Presenter ──ViewModel──> View ──> Unity UI
```

### Presenter

Adapter between gameplay and UI. Exactly three responsibilities:

| Responsibility | Meaning |
|---|---|
| **Subscribe** | Receive events/state from gameplay |
| **Transform** | Convert gameplay data into presentation data |
| **Command** | Turn UI interaction into a Command |

A Presenter must **not**: calculate damage or stats, run combat or AI, touch `Image` /
`Text` / `Button` directly, hold business logic, or grow into a giant UIManager.

If a Presenter starts containing `CalculateDamage()`, `ApplyModifier()`, `FindTarget()`,
`CanAttack()`, or `CalculateStats()`, that logic belongs in a gameplay system.

### View

Presentation only. Receives data; never queries gameplay.

```csharp
view.Render(viewModel);      // correct
playerStats.GetStrength();   // wrong — View must not know gameplay
```

### ViewModel

Presentation state. Belongs to the Presentation layer — gameplay must never depend on it.

```
CharacterStatsViewModel
├── Strength   ┐
├── Dexterity  ├─ each a StatDisplayData { BaseValue, Bonus, Total }
├── Intelligence
├── Vitality
└── Luck
```

### Folder structure

```
Presentation/
├── Core/          IPresenter · IView · PresenterBase · ViewModelBase
├── Character/
│   ├── Stats/     CharacterStatsPresenter · View · ViewModel · StatDisplayData
│   └── Equipment/ EquipmentPresenter · View · ViewModel
├── HUD/           PlayerHUDPresenter · View · ViewModel
├── Inventory/     InventoryPresenter · View · ViewModel
└── Menu/          PauseMenuPresenter · View · ViewModel
```

### Both directions, concretely

```
UI → Gameplay:   User → Equip Button → InventoryView → InventoryPresenter
                      → EquipItemCommand → InventorySystem → Player Equipment

Gameplay → UI:   PlayerStats → StatsChangedEvent → CharacterStatsPresenter
                      → CharacterStatsViewModel → CharacterStatsView
```

Never: `Button → Player → Equipment.Add()`.

---

## 10. Unity-specific rule

Keep Unity-specific code at the outer layer. `MonoBehaviour`, `ScriptableObject`,
`Animator`, `Physics`, `Input`, `Scene`, `GameObject`, `Transform`, uGUI are all fine
where Unity is genuinely needed.

Do not be extreme about removing `MonoBehaviour` from everything — Unity's lifecycle is a
natural part of Unity architecture. The goal is simply:

> Unity-specific code where Unity is needed; gameplay logic where gameplay is needed.

---

## 11. Architectural rules

| # | Rule | Status in this project |
|---|---|---|
| 1 | No God Object | ⚠️ `PlayerInputHandler` (324 lines) |
| 2 | No UI dependency in Gameplay | ✅ holds today |
| 3 | No gameplay logic in Presenter | n/a — no Presenter exists yet |
| 4 | No direct gameplay modification from View | ✅ holds today |
| 5 | No singleton by default | ⚠️ `LevelManager`, `ObjectPoolManager` |
| 6 | No over-engineering | ✅ |
| 7 | **Static data ≠ runtime state** | ❌ **violated — see below** |
| 8 | State machine is not a gameplay system | ⚠️ no Systems layer exists |
| 9 | Events are not a global dump | ⚠️ untyped `Action<object>` payloads |
| 10 | Unity is not the whole architecture | ✅ |

### Rule 7 — the live violation

`EntityStatsSO` stores mutable runtime health (`health`, `modifiersHealth`) directly on
the ScriptableObject asset, and **no SO is ever instantiated anywhere in the codebase**.
Every enemy sharing an `EntityStatsSO` asset therefore shares one health value.

`PlayerData.currentHealth` and `StatsSO`'s runtime modifier stack have the same shape.

Resolution direction: SO holds base/config; a runtime component owns current values.
Tracked in `adoption-analysis.md`.

---

## 12. Goals this architecture serves

Gameplay independent of UI · UI independent of gameplay internals · Player and Enemy reuse
the same components and systems · Combat, stats, and AI extensible · animation changes do
not break combat · UI changes do not touch gameplay · new command sources (AI, network,
replay) can be added · infrastructure swappable · clear data-driven workflow · important
logic testable · debuggable · not over-engineered.

> **Simple enough to iterate, structured enough to scale.**
>
> Where several implementations are possible, prefer the one with the fewest abstractions
> that still preserves dependency direction and separation of responsibility.

---

## 13. Change process

- Any change to §2 (dependency direction), §8 (data/state split), or §9 (presentation
  contract) requires an ADR before code.
- Everything else is ordinary work.
- Update this file when the project mapping in §3 or the rule status in §11 changes.

---

## 14. Revision history

| Date | Change |
|---|---|
| 2026-08-17 | Created as template skeleton |
| 2026-08-17 | Replaced with the owner-ratified Hybrid Game Architecture; project mapping and rule-conformance status added |
