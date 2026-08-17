# Game-RPG — Master Architecture

> **Status**: TEMPLATE — skeleton seeded with verified project facts, awaiting completion
> **Last Updated**: 2026-08-17
> **Engine**: Unity 2022.3.62f3 LTS
> **Owner**: [name]

<!--
HOW TO USE THIS FILE

Sections are marked with one of:
  [VERIFIED]  — read from code or an existing doc; trust it, update when code changes
  [FILL]      — you need to decide or write this
  [DERIVED]   — assembled from other docs; re-check when those change

This is the master blueprint the 2026-07-13 architecture review says is missing
("docs/architecture/architecture.md does not exist ... no master blueprint tying the
Foundation layer together"). It sits ABOVE the ADRs: ADRs record individual decisions,
this file records how the pieces fit together. Keep it short — if a section grows past
a page it probably wants its own ADR or technical design doc.

Related templates already in the repo (do not duplicate them here):
  .claude/docs/templates/architecture-decision-record.md  — one decision
  .claude/docs/templates/technical-design-document.md     — one system, in depth
  .claude/docs/templates/architecture-traceability.md     — GDD requirement → ADR matrix
-->

---

## 1. Purpose and scope

**[FILL]** One paragraph: what this document is for and who reads it.

Suggested wording to adapt:
> This document describes how Game-RPG's systems fit together — the layers, the
> contracts they share, and the rules governing dependencies between them. It is the
> entry point for anyone touching more than one system. Individual decisions live in
> ADRs; individual systems live in technical design docs.

**Out of scope:** per-system internals, gameplay tuning, art and audio pipelines.

---

## 2. Constraints [VERIFIED — source: `.claude/docs/technical-preferences.md`]

| Constraint | Value |
|---|---|
| Engine | Unity 2022.3.62f3 LTS |
| Language | C# (.NET Standard 2.1) |
| Rendering | URP — 2D Renderer |
| Physics | Physics2D (Box2D) |
| Target platform | PC (Windows) |
| Input | Keyboard + Mouse. No gamepad, no touch |
| Target framerate | 60 FPS (16.7 ms frame budget) |
| Draw calls | ≤ 100 per frame |
| Memory ceiling | 256 MB |
| Per-enemy update budget | ≤ 0.1 ms per enemy per frame |

> ⚠️ **Open conflict:** the platform row says PC-only / no touch, but UI planning assumes
> "PC first, mobile later". Resolve before Phase 6 of `UI_ARCHITECTURE_PLAN.md`.

---

## 3. Layer map **[FILL — structure seeded, arrows need confirming]**

Draw the dependency direction. An arrow means "may reference"; absence means "must not".

```
        ┌─────────────────────────────────────────────┐
        │  Presentation:  UI · HUD · Minimap          │
        └──────────────────────┬──────────────────────┘
                               │ reads only
        ┌──────────────────────┴──────────────────────┐
        │  Gameplay:  Character (Player/Entity)       │
        │             Weapons · Skills · Items        │
        └──────────────────────┬──────────────────────┘
                               │
        ┌──────────────────────┴──────────────────────┐
        │  World:  Map/Dungeon · Enemy spawn          │
        └──────────────────────┬──────────────────────┘
                               │
        ┌──────────────────────┴──────────────────────┐
        │  Foundation:  EventManager · StatSystem     │
        │               Core hubs · Pooling · SO data │
        └─────────────────────────────────────────────┘
```

**Rule to state explicitly [FILL]:** which direction may references point, and what the
consequence is for a violation.

---

## 4. System inventory **[DERIVED — fill Status and ADR columns]**

| System | Entry point | ADR | Status |
|---|---|---|---|
| Event bus | `Script/Manager/EventManager.cs` | ❌ none — **HIGH priority gap** | Working |
| Animation bus | `Script/Manager/AnimationEventManager.cs` | ❌ none | Working |
| Player state machine | `Script/Character/Player/NewPlayer.cs` | ❌ none | Working |
| Enemy AI | `Script/Character/Entity/Entity.cs` | ❌ none | Working |
| Damage / health | `Script/Interface/INegativeReciver.cs` | ❌ none — **HIGH priority gap** | Broken |
| Stat system | `Script/StatSystem/StatsSO.cs` | ADR-0001 | Working, unconsumed |
| Dungeon generation | `Script/Map/Maze/MazeController.cs` | ❌ none | Working |
| Room progression | `Script/Map/Room/RoomGridController.cs` | ❌ none | Working |
| Enemy spawn | `Script/Enemy/EnemySpawner.cs` | ADR-0002, ADR-0003 | Partial |
| Weapons / skills | `Script/Weapons/`, `Script/Skill_Ability/` | ❌ none | Partial |
| Object pooling | `Script/Poolable/ObjectPoolManager.cs` | ❌ none | Working |
| UI | `Script/UI/` | ❌ none | Stub |

---

## 5. Cross-cutting contracts [VERIFIED]

These are touched by many systems. Changing one is a breaking change.

### 5.1 Event bus — `EventManager`
Static pub/sub. **12 of 20 indexed systems route through it** — the highest-risk single
component in the project.

- Extend by adding to the `EventID` enum only — never add `static Action` fields to classes
- Every `Resgister` in `OnEnable()` needs a matching `UnResgister` in `OnDisable()`
- Note the intentional typos: `Resgister` / `UnResgister`

**[FILL]** Payload typing rule, and lifetime/reset semantics across scene loads.

### 5.2 Damage — `INegativeReceiver`
`TakeDamage(int amountDamage, Vector2 attackPosition)`. All damage flows through this; no
MonoBehaviour mutates another entity's health directly.

**[FILL]** **Who owns the health value.** This is unresolved today and is the single
biggest source of confusion — see §6.

### 5.3 Component hubs — `Core` / `EntityCore`
Components self-register via `AddCoreComponent()`; consumers resolve via
`GetCoreComponent<T>(out var comp)`. These are the only permitted hubs.

### 5.4 Data — ScriptableObject-first
Gameplay numbers live in SO assets, never hardcoded in MonoBehaviours.

**[FILL]** Rule for runtime-mutable SOs — Editor Play sessions persist asset changes, so
state each SO's reset contract.

### 5.5 Singletons
Permitted: `MazeController`, `EnemyManager` (ADR-0002). Nothing else.
Known violations: `LevelManager`, `ObjectPoolManager` — tracked as TD-023 / TD-031.

---

## 6. Data ownership map **[FILL — highest-value section, currently the weakest area]**

One row per piece of mutable runtime state. "Owner" means the single place allowed to
write it; everything else reads or requests a change through the owner.

| State | Owner | Readers | Change signal |
|---|---|---|---|
| Player current health | ⚠️ **undecided — 3 competing stores** | HUD, death flow | none today |
| Player stats | `StatsSO` | UI, combat | `OnStatChanged` |
| Enemy health | [FILL] | [FILL] | [FILL] |
| Current room | [FILL] | Minimap, spawner | `ON_LOAD_MAP` |
| Run progress | [FILL] | [FILL] | [FILL] |

> Filling this table is the fastest way to prevent the class of bug the project keeps
> hitting: the same value stored in several places with no owner and no change signal.

---

## 7. Dependency rules **[FILL]**

State the ones that would otherwise be re-litigated per pull request. Starters, all
already enforced in `.claude/rules/`:

- UI reads game state; it never writes it, and holds no `GameObject` reference to Player or Enemy
- Core components depend on interfaces, never on concrete state classes
- No `GameObject.Find()` / `FindObjectOfType()` / `SendMessage()` in production code
- No allocation in `Update()` / `LogicUpdate()` / `PhysicsUpdate()`
- `Physics2D` NonAlloc variants only in hot paths

---

## 8. Open architecture decisions **[DERIVED — source: architecture review 2026-07-13]**

| # | Decision needed | Priority | Blocks |
|---|---|---|---|
| 1 | Event bus contract → ADR | **HIGH** | 12 of 20 systems |
| 2 | Damage & health ownership → ADR | **HIGH** | Combat, Enemy AI, HUD, Death |
| 3 | Accept ADR-0001 / 0002 / 0003 (all still `Proposed`) | **HIGH** | Sprint 5/6 stories |
| 4 | UI architecture → ADR | MEDIUM | `UI_ARCHITECTURE_PLAN.md` Phase 1+ |
| 5 | Core component-hub contract → ADR | MEDIUM | — |
| 6 | Animation second-bus rationale → ADR | MEDIUM | — |
| 7 | Platform scope: is mobile real? | MEDIUM | UI Phase 6 |
| 8 | Build-safe JSON loading (Bug #15) | LOW | Player builds |

Review verdict at last run: **CONCERNS** — 59 technical requirements, 50 gaps.

---

## 9. Change process **[FILL]**

- What size of change requires an ADR before code?
- Who reviews an architecture change on a solo project — self-review checklist, or a gate?
- When does this file get updated, and by whom?

Suggested minimum: any change to §5 (cross-cutting contracts) or §6 (data ownership)
requires an ADR first. Everything else is ordinary work.

---

## 10. Revision history

| Date | Author | Change |
|---|---|---|
| 2026-08-17 | — | Created as template, seeded from code, technical-preferences, and the 2026-07-13 review |
