# Systems Index

> **Generated**: 2026-05-19 · **Status columns re-verified against source**: 2026-09-11 (HEAD `6d6a8e4`)
> **Game**: Unity Action Roguelike RPG
> **Review mode**: lean

This index is the master list of all systems needed to ship the demo. Systems are ordered
by design priority (Foundation → Core → Feature → Presentation). Update the Status
column as GDDs are authored.

Re-run `/map-systems` to add new systems or revise priorities.

---

## Systems Enumeration

| # | System | Category | Layer | Priority | Status | GDD File |
|---|--------|----------|-------|----------|--------|----------|
| 1 | Event Bus | Foundation | Foundation | MVP | Designed | *(in character-system.md + CLAUDE.md)* |
| 2 | Input System | Foundation | Foundation | MVP | Designed | *(in character-system.md)* |
| 3 | Damage & Health | Foundation | Foundation | MVP | Designed | *(in character-system.md)* |
| 4 | Animation System | Foundation | Foundation | MVP | Designed | design/gdd/animation-system.md |
| 5 | Character / Player Controller | Gameplay | Core | MVP | Designed | design/gdd/character-system.md |
| 6 | Enemy AI | Gameplay | Core | MVP | Designed | design/gdd/character-system.md |
| 7 | Dungeon Generation | Map | Core | MVP | In Progress | design/gdd/map-system.md |
| 8 | Melee Combat | Gameplay | Feature | MVP | Designed | design/gdd/weapons-system.md |
| 9 | Weapon System | Gameplay | Feature | MVP | Designed | design/gdd/weapons-system.md |
| 10 | Skill & Ability System | Gameplay | Feature | MVP | Designed | design/gdd/skill-ability-system.md |
| 11 | Room Progression | Map | Feature | MVP | In Progress | design/gdd/map-system.md |
| 12 | Death & Restart | Meta | Feature | MVP | Not Started | — |
| 13 | HUD | UI | Presentation | MVP | Partially built, undesigned | *(none — `UI/StatsUIController.cs`, `UI/UIController.cs`; `UIManager` still an empty stub)* |
| 14 | Per-Run Upgrades | Progression | Presentation | MVP | Not Started | — |
| 15 | Start Menu | UI/Meta | Presentation | MVP | Partially built, undesigned | *(none — `UI/UIController.cs` UI Toolkit main menu / settings / pause)* |
| 16 | Minimap | UI | Presentation | Vertical Slice | **Implemented** | *(in CLAUDE.md — `MapGridController`, DOTween)* |
| 17 | Object Pooling | Foundation | Foundation | Alpha | **Implemented** | *(none — `Assets/Script/System/PoolableService/`)* |
| 18 | Level Editor Tool | Tools | Tools | Alpha | Designed | *(implemented — editor-only)* |
| 19 | Stat System | Foundation | Foundation | MVP | Designed | design/gdd/stat-system.md |
| 20 | Enemy Spawn & Per-Room Management | Gameplay/Map | Feature | MVP | Approved | design/gdd/enemy-spawn-system.md |
| 21 | Attack Speed | Gameplay | Feature | MVP | Designed | design/gdd/attack-speed-system.md |
| 22 | Pathfinding (A*) | Gameplay | Core | MVP | **Implemented, undesigned** | *(none — `Assets/Script/System/Pathfinding/`, BUG-052)* |
| 23 | Character Core / Hub Layer | Foundation | Foundation | MVP | **Implemented, undesigned** | *(none — `Assets/Script/Character/Base/`, BUG-052)* |
| 24 | **Dependency Injection / Composition Root** | Foundation | Foundation | MVP | **Implemented, ADR only** | *(no GDD — `Assets/Script/System/LifetimeScope/`; **ADR-0004**)* |
| 25 | **Abilities v2 (composition-based)** | Gameplay | Feature | MVP | **Implemented, undesigned** | *(no GDD — `Assets/Script/System/Abilities/`; `skill-ability-system.md` covers v1 only)* |
| 26 | **Item / Depot / Drop Tables** | Progression | Feature | MVP | **Implemented, undesigned** | *(no GDD, no ADR — `Assets/Script/System/Item/`)* |

> **Three systems added 2026-09-11.** All three shipped between 2026-08-22 and 2026-09-09 without
> ever entering this index: DI (`aa4e620`), the Item system (`853fe39`, `9f1258c`) and Abilities v2
> (`9b8d40f`, `5c7afba`, promoted out of `prototypes/`). Only DI has any architecture document
> (ADR-0004, written retroactively on 2026-09-11).

---

## Dependency Map

```
Layer 1 — Foundation (no dependencies)
  Event Bus
  Input System
  Damage & Health
  Animation System
  Object Pooling

Layer 2 — Core
  Character / Player Controller  ← Input System + Animation System + Damage & Health
  Enemy AI                       ← Animation System + Damage & Health
  Dungeon Generation             ← Event Bus

Layer 3 — Feature
  Melee Combat                   ← Character + Enemy AI + Damage & Health
  Weapon System                  ← Character + Melee Combat
  Skill & Ability System         ← Character + Weapon System + Animation System
  Attack Speed                   ← Weapon System + Stat System + Animation System
  Room Progression               ← Dungeon Generation + Enemy AI + Event Bus
  Enemy Spawn & Per-Room Mgmt    ← Room Progression + Enemy AI + Event Bus + Stat System
  Death & Restart                ← Character + Event Bus

Layer 4 — Presentation
  HUD                            ← Character + Skill & Ability System + Damage & Health
  Minimap                        ← Dungeon Generation + Room Progression
  Per-Run Upgrades               ← Room Progression + Death & Restart
  Start Menu                     ← Death & Restart

Layer 5 — Tools (standalone)
  Level Editor Tool
```

---

## High-Risk Systems (Bottlenecks)

| System | Risk | Why |
|--------|------|-----|
| **Event Bus** | HIGH | 12 of 20 systems route through it — misdesign cascades everywhere |
| **Character / Player Controller** | HIGH | Blocks Combat, Weapons, Skills, HUD, Death |
| **Damage & Health** | HIGH | Required by Combat, Enemy AI, HUD, Death — shared contract |
| **Room Progression** | MEDIUM | Connects dungeon generation to gameplay loop and upgrades |

---

## Recommended Design Order

Work top-to-bottom. Foundation systems first; do not start a system until its dependencies are designed.

| Order | System | Priority | Status | Blocks |
|-------|--------|----------|--------|--------|
| 1 | Event Bus | MVP | Designed ✅ | Everything |
| 2 | Input System | MVP | Designed ✅ | Character, Skills |
| 3 | Damage & Health | MVP | Designed ✅ | Combat, Enemy AI, HUD |
| 4 | Animation System | MVP | Designed ✅ | Character, Combat |
| 5 | Character / Player Controller | MVP | Designed ✅ | Combat, Weapons, Skills, HUD |
| 6 | Enemy AI | MVP | Designed ✅ | Room Progression, Melee Combat |
| — | Enemy Spawn & Per-Room Mgmt | MVP | Approved ✅ | Per-Run Upgrades |
| 7 | Dungeon Generation | MVP | In Progress ✅ | Room Progression, Minimap |
| 8 | Melee Combat | MVP | Designed ✅ | — |
| 9 | Weapon System | MVP | Designed ✅ | Skill & Ability |
| 10 | Skill & Ability System | MVP | Designed ✅ | HUD |
| 11 | Room Progression | MVP | In Progress ✅ | Per-Run Upgrades |
| 12 | Death & Restart | MVP | Not Started | Start Menu |
| 13 | HUD | MVP | Not Started | — |
| 14 | Per-Run Upgrades | MVP | Not Started | — |
| 15 | Start Menu | MVP | Not Started | — |
| 16 | Minimap | Vertical Slice | Designed ✅ | — |
| 17 | Object Pooling | Alpha | Implemented ✅ | — (unblocked ranged weapons and pooled enemy spawning) |
| 18 | Level Editor Tool | Alpha | Designed ✅ | — |

---

> **Audit note (2026-09-11, supersedes the 2026-08-20 note).** **Seven** systems are now built
> but carry no GDD: Pathfinding, the `Character/Base` hub layer, Object Pooling, the UI Toolkit
> menus + Stats UI, **Dependency Injection**, **Abilities v2**, and the **Item system**. The
> Dependency Map and Recommended Design Order sections were written on 2026-05-19 and do not
> yet account for Pathfinding, which every enemy now depends on at runtime
> (`EntityMovement.Start()` reads `EnemyManager.Instance.Grid`). Treat those two sections as
> historical planning, not as a current description.

## GDD Progress

- **Total systems**: 26 (21 planned + Pathfinding and the Character/Base hub layer from the
  2026-08-20 audit + Dependency Injection, Abilities v2 and the Item system from the
  2026-09-11 doc/code re-sync)
- **Designed / In Progress**: 14 (Event Bus, Input, Damage & Health, **Animation**, Character, Enemy AI, Melee Combat, Weapon, Skill & Ability, Minimap, **Dungeon Generation**, **Room Progression**, **Enemy Spawn**, **Attack Speed**)
- **With standalone GDD files**: 8 (animation-system.md, character-system.md, weapons-system.md, skill-ability-system.md, **map-system.md**, **stat-system.md**, **enemy-spawn-system.md**, **attack-speed-system.md**)
- **Not Started**: 3 MVP systems still need standalone GDDs (Death & Restart, HUD, Per-Run Upgrades)
- **Built without a GDD**: 7 — Pathfinding, Character/Base hub layer, Object Pooling, UI Toolkit menus + Stats UI, Dependency Injection (ADR-0004 only), Abilities v2, Item system (BUG-052, widened 2026-09-11)
- **Alpha/Tools (lower priority)**: 2 remaining

---

## Next Systems to Design

Priority order for next GDD sessions:

1. ~~**Dungeon Generation + Room Progression**~~ ✅ Done — `map-system.md` created; random load implemented
2. **Ability framework decision** → ⚠️ **now the top blocker.** One ADR deciding whether Abilities v1
   (`ActivateSkill`) migrates into v2 (`AbilityDefinition`) or the two coexist permanently. Until
   this lands, `skill-ability-system.md` cannot be made authoritative and v2 cannot be designed
   (one system = one GDD)
3. **Death & Restart** → short GDD, closely tied to Character system. Unblocked: the enemy death
   chain now works and `PlayerDeathState` emits `ON_PLAYER_DEATH`
4. **Item / Depot system** → shipped with no design doc at all; drop-rate tiers and effect
   composition are currently defined only in code and SO assets
5. **HUD** → depends on Character + Skills being stable. Note the *enemy* health bar is already
   built (`EntityUIController`) and is the pattern to follow
6. **Per-Run Upgrades** → depends on Room Progression (room-clear event flow now works)
