# Changelog — `systems-index.md`

Change log for [`design/gdd/systems-index.md`](../systems-index.md). The document itself is the **current official version**;
this file records **when** it changed, **what** changed (**from → to**) and **why**. Newest first.

Built 2026-10-05 from `git log --follow` and the per-document trail in `docs/CHANGELOG-DOCS.md`.
Where neither source states a reason, the entry says so instead of guessing. From 2026-10-05 on,
append entries by hand (template in `docs/systems/README.md`).

## 2026-09-11 — docs: full documentation/code re-synchronisation against HEAD 6d6a8e4
- **Commit:** `bbfb3028` (+29 / −11 lines)
- **Sections touched:** “Systems Index”, “Systems Enumeration”, “Recommended Design Order”, “GDD Progress”, “Next Systems to Design”
- **From → To:** `` > **Generated**: 2026-05-19 · **Status columns re-verified against source**: 2026-08-20 `` → `` > **Generated**: 2026-05-19 · **Status columns re-verified against source**: 2026-09-11 (HEAD `6d6a8e4`) ``
- **From → To:** `` | 17 | Object Pooling | Foundation | Foundation | Alpha | **Implemented** | *(none — `Assets/Script/Poolable/`)* | `` → `` | 17 | Object Pooling | Foundation | Foundation | Alpha | **Implemented** | *(none — `Assets/Script/System/PoolableService/`)* | ``
- **From → To:** `` | 22 | Pathfinding (A*) | Gameplay | Core | MVP | **Implemented, undesigned** | *(none — `Assets/Script/Pathfinding/`, BUG-052)* | `` → `` | 22 | Pathfinding (A*) | Gameplay | Core | MVP | **Implemented, undesigned** | *(none — `Assets/Script/System/Pathfinding/`, BUG-052)* | ``
- … 5 more hunks — `git show bbfb3028 -- design/gdd/systems-index.md`
- **Why:** commit message: “docs: full documentation/code re-synchronisation against HEAD 6d6a8e4”

## 2026-08-21 — docs(registry): correct architecture.yaml, epics index and systems index
- **Commit:** `26244fba` (+18 / −7 lines)
- **Sections touched:** “Systems Index”, “Systems Enumeration”, “Recommended Design Order”, “GDD Progress”
- **From → To:** `` > **Generated**: 2026-05-19 `` → `` > **Generated**: 2026-05-19 · **Status columns re-verified against source**: 2026-08-20 ``
- **From → To:** `` | 13 | HUD | UI | Presentation | MVP | Not Started | — | `` → `` | 13 | HUD | UI | Presentation | MVP | Partially built, undesigned | *(none — `UI/StatsUIController.cs`, `UI/UIController.cs`; `UIManager` … ``
- **From → To:** `` | 15 | Start Menu | UI/Meta | Presentation | MVP | Not Started | — | `` → `` | 15 | Start Menu | UI/Meta | Presentation | MVP | Partially built, undesigned | *(none — `UI/UIController.cs` UI Toolkit main menu / … ``
- … 5 more hunks — `git show 26244fba -- design/gdd/systems-index.md`
- **Why:** commit message: “docs(registry): correct architecture.yaml, epics index and systems index”

## 2026-08-05 — design: add attack speed system GDD
- **Commit:** `b3a52e0c` (+5 / −3 lines)
- **Sections touched:** “Systems Enumeration”, “Dependency Map”, “GDD Progress”
- **From → To:** (nothing) → `` | 21 | Attack Speed | Gameplay | Feature | MVP | Designed | design/gdd/attack-speed-system.md | ``
- **From → To:** (nothing) → `` Attack Speed ← Weapon System + Stat System + Animation System ``
- **From → To:** `` - **Total systems**: 20 `` → `` - **Total systems**: 21 ``
- **Why:** commit message: “design: add attack speed system GDD”

## 2026-07-09 — fix doc
- **Commit:** `65c8228e` (+7 / −6 lines)
- **Sections touched:** “High-Risk Systems (Bottlenecks)”, “Recommended Design Order”, “GDD Progress”
- **From → To:** `` | **Event Bus** | HIGH | 12 of 18 systems route through it — misdesign cascades everywhere | `` → `` | **Event Bus** | HIGH | 12 of 20 systems route through it — misdesign cascades everywhere | ``
- **From → To:** `` | 4 | Animation System | MVP | Not Started | Character, Combat | `` → `` | 4 | Animation System | MVP | Designed ✅ | Character, Combat | ``
- **From → To:** (nothing) → `` | — | Enemy Spawn & Per-Room Mgmt | MVP | Approved ✅ | Per-Run Upgrades | ``
- … 1 more hunks — `git show 65c8228e -- design/gdd/systems-index.md`
- **Why:** commit message: “fix doc”

## 2026-07-08 — plan enemy spawn
- **Commit:** `ddbd54d7` (+5 / −3 lines)
- **Sections touched:** “Systems Enumeration”, “Dependency Map”, “GDD Progress”
- **From → To:** (nothing) → `` | 20 | Enemy Spawn & Per-Room Management | Gameplay/Map | Feature | MVP | Approved | design/gdd/enemy-spawn-system.md | ``
- **From → To:** (nothing) → `` Enemy Spawn & Per-Room Mgmt ← Room Progression + Enemy AI + Event Bus + Stat System ``
- **From → To:** `` - **Total systems**: 18 `` → `` - **Total systems**: 19 ``
- **Why:** commit message: “plan enemy spawn”

## 2026-07-07 — docs(statsystem): author stat-system GDD; move formula reference into ToolExcel
- **Commit:** `76803221` (+2 / −1 lines)
- **Sections touched:** “Systems Enumeration”, “GDD Progress”
- **From → To:** (nothing) → `` | 19 | Stat System | Foundation | Foundation | MVP | Designed | design/gdd/stat-system.md | ``
- **From → To:** `` - **With standalone GDD files**: 4 (character-system.md, weapons-system.md, skill-ability-system.md, **map-system.md**) `` → `` - **With standalone GDD files**: 5 (character-system.md, weapons-system.md, skill-ability-system.md, **map-system.md**, **stat-system.md**) ``
- **Why:** commit message: “docs(statsystem): author stat-system GDD; move formula reference into ToolExcel”

## 2026-06-04 — update document GDD map system
- **Commit:** `3c87f5af` (+9 / −10 lines)
- **Sections touched:** “Systems Enumeration”, “Recommended Design Order”, “GDD Progress”, “Next Systems to Design”
- **From → To:** `` | 7 | Dungeon Generation | Map | Core | MVP | Not Started | — | `` → `` | 7 | Dungeon Generation | Map | Core | MVP | In Progress | design/gdd/map-system.md | ``
- **From → To:** `` | 11 | Room Progression | Map | Feature | MVP | Not Started | — | `` → `` | 11 | Room Progression | Map | Feature | MVP | In Progress | design/gdd/map-system.md | ``
- **From → To:** `` | 7 | Dungeon Generation | MVP | Not Started | Room Progression, Minimap | `` → `` | 7 | Dungeon Generation | MVP | In Progress ✅ | Room Progression, Minimap | ``
- … 4 more hunks — `git show 3c87f5af -- design/gdd/systems-index.md`
- **Why:** commit message: “update document GDD map system”

## 2026-05-19 — Add animation system GDD and update systems index
- **Commit:** `f8321fbb` (+1 / −1 lines)
- **Sections touched:** “Systems Enumeration”
- **From → To:** `` | 4 | Animation System | Foundation | Foundation | MVP | Not Started | — | `` → `` | 4 | Animation System | Foundation | Foundation | MVP | Designed | design/gdd/animation-system.md | ``
- **Why:** commit message: “Add animation system GDD and update systems index”

## 2026-05-19 — Add design documentation scaffold from brownfield onboarding
- **Commit:** `f8da1430` (+130 / −0 lines)
- **Changed (from → to):** document created (+130 lines)
- **Why:** commit message: “Add design documentation scaffold from brownfield onboarding”
