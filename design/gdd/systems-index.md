# Systems Index

> **Generated**: 2026-05-19
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
| 4 | Animation System | Foundation | Foundation | MVP | Not Started | — |
| 5 | Character / Player Controller | Gameplay | Core | MVP | Designed | design/gdd/character-system.md |
| 6 | Enemy AI | Gameplay | Core | MVP | Designed | design/gdd/character-system.md |
| 7 | Dungeon Generation | Map | Core | MVP | Not Started | — |
| 8 | Melee Combat | Gameplay | Feature | MVP | Designed | design/gdd/weapons-system.md |
| 9 | Weapon System | Gameplay | Feature | MVP | Designed | design/gdd/weapons-system.md |
| 10 | Skill & Ability System | Gameplay | Feature | MVP | Designed | design/gdd/skill-ability-system.md |
| 11 | Room Progression | Map | Feature | MVP | Not Started | — |
| 12 | Death & Restart | Meta | Feature | MVP | Not Started | — |
| 13 | HUD | UI | Presentation | MVP | Not Started | — |
| 14 | Per-Run Upgrades | Progression | Presentation | MVP | Not Started | — |
| 15 | Start Menu | UI/Meta | Presentation | MVP | Not Started | — |
| 16 | Minimap | UI | Presentation | Vertical Slice | Designed | *(in CLAUDE.md — implemented)* |
| 17 | Object Pooling | Foundation | Foundation | Alpha | Not Started | — |
| 18 | Level Editor Tool | Tools | Tools | Alpha | Designed | *(implemented — editor-only)* |

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
  Room Progression               ← Dungeon Generation + Enemy AI + Event Bus
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
| **Event Bus** | HIGH | 12 of 18 systems route through it — misdesign cascades everywhere |
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
| 4 | Animation System | MVP | Not Started | Character, Combat |
| 5 | Character / Player Controller | MVP | Designed ✅ | Combat, Weapons, Skills, HUD |
| 6 | Enemy AI | MVP | Designed ✅ | Room Progression, Melee Combat |
| 7 | Dungeon Generation | MVP | Not Started | Room Progression, Minimap |
| 8 | Melee Combat | MVP | Designed ✅ | — |
| 9 | Weapon System | MVP | Designed ✅ | Skill & Ability |
| 10 | Skill & Ability System | MVP | Designed ✅ | HUD |
| 11 | Room Progression | MVP | Not Started | Per-Run Upgrades |
| 12 | Death & Restart | MVP | Not Started | Start Menu |
| 13 | HUD | MVP | Not Started | — |
| 14 | Per-Run Upgrades | MVP | Not Started | — |
| 15 | Start Menu | MVP | Not Started | — |
| 16 | Minimap | Vertical Slice | Designed ✅ | — |
| 17 | Object Pooling | Alpha | Not Started | Ranged weapons |
| 18 | Level Editor Tool | Alpha | Designed ✅ | — |

---

## GDD Progress

- **Total systems**: 18
- **Designed**: 9 (Event Bus, Input, Damage & Health, Character, Enemy AI, Melee Combat, Weapon, Skill & Ability, Minimap — partial: some in CLAUDE.md)
- **With standalone GDD files**: 3 (character-system.md, weapons-system.md, skill-ability-system.md)
- **Not Started**: 6 MVP systems still need standalone GDDs (Animation, Dungeon Generation, Room Progression, Death & Restart, HUD, Per-Run Upgrades)
- **Alpha/Tools (lower priority)**: 2 remaining

---

## Next Systems to Design

Priority order for next GDD sessions:

1. **Dungeon Generation + Room Progression** → `/reverse-document design Assets/Script/Map/`
   *(These share heavy coupling — design together or back-to-back)*
2. **Death & Restart** → short GDD, closely tied to Character system
3. **HUD** → depends on Character + Skills being stable
4. **Per-Run Upgrades** → depends on Room Progression
