# Project Review — 2026-10-08

> First run of `production/review-flow.md`. Review-only: **no code, asset or scene was changed.**

## Snapshot (R0)

| Field | Value |
|---|---|
| Branch / HEAD | `sprint-17` / `1b24ef7b` (tree clean) |
| Previous full read | Monthly audit delta, 2026-10-06, HEAD `8da09b1a` |
| Unreviewed commits | `5d1986db` "shop coding" (2026-10-06), `7f632021` "coding start room" (2026-10-07) — 8 `.cs` files, read in R3 below |
| Code size | 268 `.cs` files, ≈17.1k lines under `Assets/Script/`; 0 `.asmdef`; 0 test files |
| Evidence ceiling | **R2 NOT RUN** (no Editor in this session) · **R4 NOT RUN** (last playtest file 2026-06-12, **118 days**) → every verdict below is `STATIC` |

## 1. Verdict

**Demo milestone: AT RISK — static evidence only.**

The project has broad, real feature coverage: procedural dungeon, melee combat with combos, enemy AI
with pathfinding and a working damage/death chain, a composition-based ability framework with four
Paladin abilities, an item/drop system, a full menu → loading → gameplay UI flow with a HUD bound to
the live player, and a VContainer composition root. Architecture direction is sound and improving
(ADR-0005 unified player and enemy on shared bases; singleton count reduced).

What puts the demo at risk is not missing features but **missing verification and closure**:

1. The core loop cannot be completed in code as written: rooms with no spawn marker (5 of 20, incl.
   Start and Boss) never open (BUG-096, S1), and start/boss rooms are swapped (BUG-097).
2. Player death has a UI respawn path (scene reload) but no gameplay recovery owner; the death state
   still re-emits every frame and the player still slides (BUG-086, BUG-065, BUG-087 partial).
3. Nothing has been run in Play Mode for 118 days. "Done" across sprints 8–17 means "done in code".
4. Planned work loses to off-plan feature/asset work every sprint (planned-work velocity ≈0–15% for
   four sprints per `sprint-17.md`); no commit in this window touches a sprint-17 Must-Have item
   (S17-01…S17-08) — both are off-plan feature work (shop, start room).

## 2. Feature matrix (demo checklist, `CLAUDE.md`)

| # | Feature | State | Tier | Blocking items |
|---|---|---|---|---|
| 1–3 | Map compile, dungeon nav, level editor | Done | STATIC | — |
| 2/8 | Room load + room-clear opens doors | **Broken for marker-less rooms** | STATIC | BUG-096 (S1), BUG-097 (S2), Bug #15 (build), Bug #16 |
| 5/12 | Player melee + combo | Done | STATIC | BUG-095 *(CLAUDE.md meaning: sword stage damage lost — see §6 ID collision)* |
| 7/16/17 | Enemy deploy, targeting, damage/death | Done | STATIC | BUG-066/070 key guard |
| — | Enemy weapon attack (ADR-0005 "no weapon, no attack") | Done in code | STATIC | Never observed |
| — | Ranged weapon (`ProjectileBody`) | Done in code | STATIC | `Arrow.prefab` collider (bug-file BUG-095), cooldown dead (CLAUDE BUG-093) |
| 6/24 | Player death → restart | **Partial** | STATIC | BUG-065, BUG-086, BUG-087 (UI respawn only) |
| 9 | HUD | Mostly done | STATIC | EXP / minimap / skill tree have no gameplay source |
| 10 | Between-room upgrade (3 cards) | Not started | — | No GDD |
| 14 | Build-safe room JSON | Not started | — | Bug #15 — a player build cannot load rooms |
| 15 | Enemy spawn system | Done with known gaps | STATIC | `retry > 4` fallback, `overflowPercent` unread, BUG-098 doc drift |
| 18 | Reconcile ability frameworks v1/v2 | Not started | — | No ADR (TD-040) |
| 20/23 | Tests | **Impossible today** | — | BUG-084 (no `.asmdef`, `tests/` outside `Assets/`) |
| 21 | Abilities v2 deal damage | Projectile: done in code. Summon: one Inspector field | STATIC | BUG-072 (`Lightning.prefab` `layerMask`), BUG-092 residual (HoT/DoT one tick), **BUG-100 (new)** |
| 22 | Paladin per-direction animations | Partial | — | 3 of 4 abilities missing clips |
| 25 | UIFlow real providers | Player provider done | STATIC | Login / Inventory / Quest are stubs |
| — | Start room / Champion select / Shop (new, off-plan) | Started | STATIC | No GDD, no story; see R3 |

**Count:** 11 Done (all static), 5 Partial / broken, 5 Not started or blocked. Zero items `RUNTIME`.

## 3. Design state (R5)

**GDD coverage.** 8 system GDDs + concept; 1 Approved (`enemy-spawn-system.md`), the rest Designed /
reverse-documented / In Progress.

**Live systems with no GDD (7):** Abilities v2 (the player's only ability path, most open ability
bugs), Item / Drop tables, UIFlow (has `docs/ui/ui-ux-flow.md`, a UX reference, not a GDD),
Pathfinding, Character Core / ADR-0005 bases, Death & Restart, and the new Start room / Champion /
Shop work. `design/gdd/systems-index.md` (last re-verified 2026-09-11) does not list Shop, Start room or
Champion select, and still describes HUD / Start Menu as `UIController` / `UIManager` stubs —
superseded by UIFlow on 2026-10-05.

**ADR state.** 5 ADRs: ADR-0003, ADR-0004 Accepted; **ADR-0001, ADR-0002, ADR-0005 `Proposed`** while
implemented and merged (ADR-0005 amended 3× and now governs character + weapon contracts).
Uncovered by any ADR: Pathfinding, Item system, Abilities v2 v1-vs-v2, UIFlow's static service
locator (`UIServices` bypasses VContainer), runtime player spawn.

**Status coherence.** GDD front-matter status (`authored`, `reverse-documented`, `revised`) vs
`systems-index.md` (`Designed`, `Approved`, `In Progress`) disagree in vocabulary for all 8 modules —
recorded in the October audit, still open.

**Module scorecard** (from `module-health-2026-10.md`, unchanged by this review): 2 CRITICAL
(character, map), 2 AT RISK (weapons, enemy-spawn), 4 HEALTHY.

## 4. Code findings this window (R3)

Commits `5d1986db`, `7f632021` (8 `.cs` files, +27/−37 excluding `.meta`).

| # | File:line | Sev | Finding | Filed |
|---|---|---|---|---|
| 1 | `Character/Base/VitalStatsBase.cs:43,118,120` | **S2** | Permanent buffs (`ApplyBuffDebuff`, used by item pickups via `StatModifierEffectDefinition.cs:12`) and timed buffs (`BuffDebuffForDuration`, used by the Blessing ability's buff effect) both use `this` as the modifier source. When any timed buff expires, `RemoveModifiersFromSource(this)` strips **all** of them — item stat bonuses vanish and overlapping buffs end early. Pre-existing semantics; `7f632021` rewrote these lines without changing the source | **BUG-100** |
| 2 | `System/StartGameSystem/ChampitionController.cs:5-9` | S4 | New class is a plain `class`, not a `MonoBehaviour` / `ScriptableObject`, yet declares three `[SerializeField]` fields — Unity will never serialize or show them; the class has no members and no caller. Stub for champion select. Name typo `Champition` will become a contract once referenced | Not filed — note |
| 3 | `Character/Player/PlayerData.cs:7` | S4 | New field `AnimatorOverrideController animationController;` is private with no `[SerializeField]` → never serialized, never assigned, no reader. Likely meant for the champion swap | Not filed — note |
| 4 | `System/StatSystem/StatModifierTester.cs:71` | S4 | `StatModifierGroup.ApplyTo/Apply/Remmove` were deleted; the debug tester's apply call is commented out rather than ported, so the tester's "apply" button silently does nothing (`Assets/Editor/StatModifierTesterEditor.cs` still drives it) | Not filed — note |
| 5 | `Map/Room/RoomCell.cs:132`, `RoomGeneraterController.cs:185` | Info | `IsCleared = true` moved from `OnEnemyDeath()` (after the `ON_CLEAR_ENEMY` emit) into `OpenDoors()`, and `DeleteDoorTileMap()` returns early if already cleared. Read as consistent: cleared ⇔ doors opened. Does **not** fix BUG-096 — `LoadRoom()` still has no `spawnPositions.Count == 0` branch, and `EnemySpawner.OnGetSpawnPositions()` (`:42-46`) returns on an empty list without emitting `ON_DONE_SPAWN_ENEMY` | — |
| 6 | `Weapons/Weapon.cs:90,112`, `StatModifierGroup.cs` | Info | API simplification compiles by reading: every caller now uses `StatModifierGroup.Modifiers` (`IReadOnlyList<StatModifier>`) with `IStatService.AddModifiersFromSource(object, IReadOnlyList<StatModifier>)` (`IStatService.cs:17`). No remaining caller of the deleted methods | — |
| 7 | `UIFlow/Gameplay/Popup/PopupPanel.cs` | Info | New 7-line popup panel for the shop; no design / UX doc | — |

**Rule conformance:** findings 2–3 violate `scriptableobject-data.md` ("new fields `[SerializeField]`").
No new singleton, no `Find*`, no hot-path allocation introduced.

## 5. Bug register deltas (R3.2 / R3.3)

| ID | Change this review | Evidence |
|---|---|---|
| BUG-096 | Still open (re-verified) | `RoomGeneraterController.cs:134-143` — only `!IsCleared` / else branches |
| BUG-099 | Still open | `CLAUDE.md` header still lists BUG-096's fix (`40d2c793`) among new facts |
| BUG-100 | **New, S2** | §4 #1 |
| BUG-101 | **New, S3** | §6 |
| Others | Unchanged from `bug-triage-2026-10-05.md` | No edits in the window to `PlayerDeathState.cs`, `RecoveryReductionPerTimeForDuration.cs`, `Lightning.prefab`, `Arrow.prefab` |

Register: 39 bug files (BUG-052…BUG-099) + 2 new = 41. Historical `#1–#17` numbering lives only in
`CLAUDE.md`.

## 6. Documentation drift (R6, partial sweep)

| Claim | Location | Verdict |
|---|---|---|
| BUG-093 = `RangeWeapon.nextFireTime` dead; BUG-094 = `PlayerState.Enter()` log; BUG-095 = `attackDamege` rename | `CLAUDE.md` header + Known Bugs table | **FALSE as IDs** — bug files BUG-093/094/095 are ability aim, `RangeWeapon` root rotation, `Arrow.prefab` collider. The three CLAUDE.md defects are real but have no bug file → **BUG-101** |
| BUG-096 fix recorded as landed | `CLAUDE.md` (2026-10-05 entry), `map-system.md` | FALSE — BUG-099 |
| `IAbilityServices` has 5 members | `.claude/rules/weapon-skill-code.md` | STALE (Pool only since ADR-0005) |
| Bug #13 open | `.claude/rules/map-code.md` | STALE (fixed `9154763f`) |
| UIManager stub to complete | `.claude/rules/manager-event-code.md`, `ui-code.md` | STALE (deleted `cb0de496`) |
| HUD / Start Menu = `UIController` / `UIManager` | `systems-index.md` rows 13, 15 | STALE (UIFlow) |
| Review cadence Mon/Fri | `production/review-schedule.md` | STALE (daily / Sat / Sun / monthly) |
| 80 skills | `docs/skill-reference.md` | STALE (79) |
| "Open bugs: 78" | session-start banner | FALSE (39 files, double-counted) |

`CLAUDE.md` is ~690 lines, most of it historical banners and superseded entries. As the file every
session loads first, its signal-to-noise is now a reliability risk: current truth and "previous
entry" text interleave in the same table cells (e.g. BUG-043, BUG-064, BUG-089 rows). Recommend a
condensed current-state `CLAUDE.md` with history moved to `docs/CHANGELOG-DOCS.md` (owner decision).

## 7. Process metrics (R7)

| Metric | Value | Trend |
|---|---|---|
| Days since last playtest file | **118** (2026-06-12) | Worsening |
| Play Mode smoke carry count (S17-04) | 11 | Worsening |
| Planned-work velocity (Must-Have) | ≈0–15% for 4 sprints | Flat |
| Open S1 | 1 (BUG-096) + BUG-063 accepted | — |
| Open S2 (bug files + new) | ≈9 | Flat |
| ADRs `Proposed` but implemented | 3 of 5 | Flat |
| Live systems without GDD | 7 | Worsening (+Shop/Start/Champion) |
| Tests | 0 (structurally impossible, BUG-084) | Flat |
| Compile breaks committed (Sept) | 2 in 4 days | — |
| Missed scheduled routines (30 days) | 2 (Sept monthly audit, Sat 10-03 wrap-up) | — |
| Binary growth | pack 137.8 MiB, no LFS (`bug-triage-2026-10-05.md`) | Worsening |

## 8. Ranked actions

| # | Action | Size | Owner role |
|---|---|---|---|
| 1 | One Play Mode run of `LoadRandomMap` from the menu, logged with `/playtest-report` — converts this whole report from STATIC to RUNTIME | 0.3d | Owner |
| 2 | BUG-096 + BUG-097 (restore empty-spawn branch; select start/end by `RoomType`) | 0.1–0.25d | Owner + gameplay-programmer |
| 3 | BUG-101: give the three CLAUDE.md-only defects real bug files (next free IDs) and correct CLAUDE.md; adopt the ID rule in `review-flow.md` §9 #2 | 0.1d | PM |
| 4 | BUG-100: per-application modifier source (e.g. a fresh `object` token per buff instance) | 0.1d | gameplay-programmer |
| 5 | BUG-065 + BUG-086, then design note for BUG-087 (who owns respawn: UIFlow reload vs `GameManager`) | 0.4d | gameplay-programmer + game-designer |
| 6 | Inspector block: BUG-072 `layerMask`, `Arrow.prefab` collider | 0.1d | Owner |
| 7 | Decide test location + add runtime/test `.asmdef` (BUG-084), then write `tests/smoke/critical-paths.md` | 0.3d | technical-director |
| 8 | Move ADR-0001/0002/0005 out of `Proposed` via `/architecture-review` | 0.3d | technical-director |
| 9 | Short GDDs (or `/quick-design` specs) before more code for Shop / Start room / Champion select; add them to `systems-index.md` | 0.3d | game-designer |
| 10 | Tooling fixes from `skill-audit-2026-10-08.md` (hooks, `consistency-check` registry, ID rule, R6 sweep in `/doc-sync`) | 0.5d | PM / tools-programmer |

## 9. What this review did not do

- Did not open the Unity Editor (R2) or enter Play Mode (R4).
- Did not run `/consistency-check`, `/architecture-review` or `/content-audit`; relied on the
  2026-10-05/06 module audit for module scores.
- Did not re-read every open bug's `file:line`; re-verified only those touched by the window.
- Did not read `.prefab`/`.asset` YAML except where cited by an earlier report.
- Did not update `CLAUDE.md`, rules or GDDs — drift is listed in §6 for the next `/doc-sync`.
