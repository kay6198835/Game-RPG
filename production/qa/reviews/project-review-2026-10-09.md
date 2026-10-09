# Project Review — 2026-10-09

> Run 2 of `production/review-flow.md` (v2). Review-only: **no code, asset, scene, skill, hook or
> setting was changed.** Delta review on top of run 1 (`project-review-2026-10-08.md`); sections that
> did not change point back to it instead of repeating it.

## Snapshot (R0)

| Field | Value |
|---|---|
| Branch / HEAD | `origin/feature/update-bug-document` / `221d54be76749dc13398bba8f2aa29769a47c7b7` (tree clean at start) |
| Previous run | 2026-10-08, HEAD `1b24ef7b` |
| Window `1b24ef7b..221d54be` | 9 commits. 7 are framework/tooling (`df1d09d3`, `959268bd`, `7c90520b`, `d64047a8`, `ec44e9be`, two merges), 257 files under `.claude/`. 2 touch game code: `ada2c1a8` "Base logic access panel + prototype start room" (2026-10-08) and `221d54be` "logic open door at start room" (2026-10-09) |
| Unreviewed by any routine | `ada2c1a8` (after the 10-08 standup's HEAD), `221d54be` (landed today) |
| Code size | 269 `.cs` under `Assets/Script/`, ≈17.2k lines; 0 `.asmdef`; 0 tests |
| Evidence ceiling | **R1 RUN** (`skill-audit-2026-10-09.md`) · **R2 NOT RUN** · **R4 NOT RUN** (last playtest file 2026-06-12, **119 days**) → every verdict is `STATIC` |

## 1. Verdict

**Demo milestone: AT RISK — unchanged, static evidence only.**

The tooling side improved materially in one day: the CCGS v1.1.2 install closed every framework-level
finding from run 1 (missing agents, `src/` assumption, false-pass gating, missing entity registry,
commit-hook globs). The project side did not move on any blocker. The two new game commits are
off-plan feature work (champion select, confirm panel) and one of them **builds on top of BUG-097
instead of fixing it** (§4 #1): the champion monuments that open the first room's doors are authored
in `BossRoom_ThroneArena.json` — the file BUG-097 loads at the start cell. Fixing BUG-097 as filed will
now move champion select to the last room of the run.

## 2. Feature matrix — delta only

Full matrix: run 1 §2. Changes this window:

| Feature | Run 1 | Run 2 | Note |
|---|---|---|---|
| Room load + room-clear opens doors | Broken for marker-less rooms | **First room: works through a data workaround. Other marker-less rooms: still broken** | First cell opens when a champion is confirmed (`ChampitionController.cs:40-44` emits `ON_OPEN_DOOR`). Buff / Rest / Shop rooms — and the real `StartRoom_Entrance`, which BUG-097 puts at the end cell — still never open (BUG-096) |
| Start room / Champion select | Started, no GDD | **Playable in code** (interact → confirm panel → `SetCharacterData` → doors open) | Still no GDD, no story, no `systems-index.md` row |
| Confirm / popup UI | 7-line popup | Confirm panel with buttons and data struct | §4 #2–#4 |
| All other rows | — | Unchanged | No edits to the files they cite |

## 3. Design state (R5) — delta only

Unchanged from run 1 §3: 8 GDDs (1 Approved), 3 of 5 ADRs `Proposed` while implemented, module
scorecard 2 CRITICAL / 2 AT RISK / 4 HEALTHY. **Live systems without a GDD: 7 → 8** (champion select now
carries real behaviour: data swap via `ICharacter.SetCharacterData`, door gating, three prefabs spawned
from tiles). `design/registry/entities.yaml` now exists but is empty, so `/consistency-check` can run but
has nothing to compare — populating it is the cheapest way to make R5 automatic.

## 4. Code findings this window (R3)

Commit `221d54be` (8 `.cs`, +41/−12; plus `GameplayUI.unity` +1,055 lines, scene move
`Scenes/Main/Test/LoadRandomMap.unity` → `Scenes/Main/LoadRandomMap.unity`, `EditorBuildSettings`).
`ada2c1a8` was read for context only (its `.cs` changes are superseded by `221d54be` in the same files).

| # | File:line | Sev | Finding | Filed |
|---|---|---|---|---|
| 1 | `Assets/Data/Json/Room/BossRoom_ThroneArena.json` (only room JSON containing `Tile_Spawn_Champion_Monument_*`), `RoomGeneraterController.cs:55-56` | **S2** | Champion select — the start-of-run feature — is authored in the **Boss** room JSON, because BUG-097 loads `room[0]` (alphabetically `BossRoom_ThroneArena`) at the start cell. The workaround makes the first room playable but couples content to the bug: fixing BUG-097 by `RoomType` moves champion select to the end of the run, and the real `StartRoom_Entrance` (now loaded at the end cell, no spawn marker) stays locked forever (BUG-096). The owner must decide which JSON is the start room **before** BUG-097 is fixed | Added to BUG-097 as a re-verification note |
| 2 | `UIFlow/Core/UIEvents.cs:57` | S3 | `OpenConfirnPanel.Invoke(data)` has no null-conditional. The only subscriber is `GameplayUIController` (`:37`), which lives in the additive `GameplayUI` scene. Playing `LoadRandomMap` directly (the documented main dev scene) and interacting with a monument throws `NullReferenceException`; the siblings on the same bus use `?.Invoke` (`:36,38,43`). Same for `AccessAction()` `ConfirnRequest.Invoke()` (`:63`) | **BUG-102** |
| 3 | `UIFlow/Gameplay/Windows/ConfirnPanel.cs:19-20` | S4 | `SetConfirm()` writes the button labels into the panel's text fields: `tileText` (title) = `confirmText` ("Xác nhận"), `messText` (message) = `cancelText` ("Hủy"). The panel shows "Confirm / Cancel" as title and body; the buttons keep their scene labels and the question itself is never shown | Note in BUG-102 |
| 4 | `System/StartGameSystem/ChampitionController.cs:22-31` | S4 | Player-facing Vietnamese strings hardcoded in gameplay code (dialogue line and button labels); no string table exists. Also an unused `using Unity.VisualScripting;` (`:3`) in runtime code | Not filed — `/localize` input |
| 5 | `GameplayUIController.cs:47` | ✅ Fix | `OnDisable` previously did `+=` on `OpenConfirnPanel` (double-subscribe on every enable cycle); now `-=`. Correct | — |
| 6 | `Manager/EventManager.cs:46` | Info | `ON_CLEAR_ENEMY` renamed in place to `ON_OPEN_DOOR` (same enum position, so serialized ints are safe). All 4 code call sites updated. **35 Markdown files** still name `ON_CLEAR_ENEMY`; the living ones are `CLAUDE.md`, `.claude/rules/map-code.md`, `design/gdd/map-system.md`, `docs/systems/{map,enemy,event-system}/README.md`, `docs/ui/ui-ux-flow.md`. The event now has two meanings (room cleared, champion chosen) under one door-centric name | R6 drift |
| 7 | `ChampitionController.cs:40-44` | Info | Confirming any champion re-emits `ON_OPEN_DOOR`; safe because `DeleteDoorTileMap()` returns when `IsCleared` (`RoomGeneraterController.cs:185`). Choosing again replaces character data with no limit — a design question, not a defect | — |

**Rule conformance:** #4 conflicts with the localization guidance in `.claude/rules/ui-code.md` (no
hardcoded player-facing text); no new singleton, no `Find*`, no hot-path allocation.

## 5. Bug register deltas

| ID | Change | Evidence |
|---|---|---|
| BUG-096 | Still open; **first cell mitigated by data** (champion confirm emits `ON_OPEN_DOOR`). Buff / Rest / Shop / real Start room still lock | `RoomGeneraterController.cs:134-143` unchanged; `EnemySpawner.cs:42-46` unchanged |
| BUG-097 | Still open; **now load-bearing** — see §4 #1 | `Maze_Storage.asset` unchanged since `ac13ee4f` (first entry `BossRoom_ThroneArena`, last `StartRoom_Entrance`) |
| BUG-101 | Still open | `CLAUDE.md` Known Bugs BUG-093..095 rows unchanged |
| BUG-102 | **New, S3** | §4 #2 |
| Others | Unchanged | No edits in window to their cited files |

Register: **42 files**. Status parse: 20 open, 4 partial, 1 accepted (BUG-063), 2 fixed-pending-runtime
(BUG-093, BUG-094), 15 closed. The session banner's "Open bugs: 41/42" is a file count (skill audit N-1).

## 6. Documentation drift (R6) — new this window

| Claim | Location | Verdict |
|---|---|---|
| Main dev scene `Assets/Scenes/Main/Test/LoadRandomMap.unity` | `CLAUDE.md` §Unity Environment, Scene Map | **STALE** — moved to `Assets/Scenes/Main/LoadRandomMap.unity` in `221d54be` |
| `ON_CLEAR_ENEMY` is the room-clear event | `CLAUDE.md` Event System + Map flow, `map-code.md`, `map-system.md`, 3 system READMEs, `ui-ux-flow.md` | **STALE** — now `ON_OPEN_DOOR` |
| `EventID` "Room / map" group lists `ON_CLEAR_ENEMY` | `CLAUDE.md` Event System table | STALE (count of 24 unchanged) |
| Daily standup at 10:00 | `review-flow.md` v1 §7, `docs/skill-reference.md` | **FALSE** — cron `0 2 * * 1-5`. Corrected in the flow (v2); `skill-reference.md` not edited |
| Skill count 80, 6 team skills broken | `docs/skill-reference.md:6,134` | STALE — 81 skills, 0 broken |
| Run-1 drift list (BUG-099, `weapon-skill-code.md`, `map-code.md` Bug #13, `manager-event-code.md`, `systems-index.md`, `review-schedule.md`) | — | All still open; nothing in the window edited them |

`CLAUDE.md` is now **1,286 lines** — every session loads it first, and most of it is superseded history.

## 7. Process metrics

| Metric | Run 1 | Run 2 | Trend |
|---|---|---|---|
| Days since last playtest file | 118 | 119 | Worsening |
| Unreviewed game commits at review start | 2 | 2 | Flat |
| Commits touching a sprint-17 Must-Have | 0 | 0 | Flat |
| Open S1 | 1 (BUG-096) | 1 (BUG-096, partly mitigated) | Flat |
| Live systems without GDD | 7 | 8 | Worsening |
| Tooling findings open (skill audit) | 7 seams + 4 reference groups | 4 seams + 7 new (all S3/S4) | Improving |
| Missing agents | 13 | 0 | ✅ |
| Docs naming a renamed identifier | — | 35 files (`ON_CLEAR_ENEMY`) | New metric (R3.5) |

## 8. Ranked actions

Run 1's list stands; re-ranked with today's evidence:

| # | Action | Size | Owner role |
|---|---|---|---|
| 1 | One Play Mode run from `MainGamePlay` → New Game → champion → first door, logged with `/playtest-report` | 0.3d | Owner |
| 2 | **Decide the start-room JSON first**, then fix BUG-097 (select by `RoomType`) and BUG-096 (empty-spawn branch) together, moving the champion tiles to `StartRoom_Entrance` | 0.25d | Owner + gameplay-programmer |
| 3 | BUG-102: `?.Invoke` on `OpenConfirnPanel` / `ConfirnRequest`; fix `SetConfirm` field mapping | 0.05d | ui-programmer |
| 4 | BUG-101 + ID rule (next ID = max + 1, 3 digits) in project-own skills | 0.1d | PM |
| 5 | `/doc-sync` fed with §6 + run-1 §6 (scene path, `ON_OPEN_DOOR`, skill count) — after the claim sweep, not before | 0.2d | PM |
| 6 | Patch `/weekly-wrapup` with R3.2 regression + R3.5 rename sweeps; repoint `pm-weekly-wrapup` off `/weekly-sprint` | 0.2d | PM / tools-programmer |
| 7 | `/quick-design` spec for champion select + start room (what a champion changes, can it be re-chosen, which room hosts it) | 0.2d | game-designer |
| 8 | BUG-100, BUG-065 + BUG-086 (unchanged from run 1) | 0.5d | gameplay-programmer |
| 9 | BUG-084 decision + `tests/smoke/critical-paths.md` | 0.3d | technical-director |
| 10 | Condense `CLAUDE.md` to current state; history to `docs/CHANGELOG-DOCS.md` | 0.3d | PM (owner approval) |

## 9. What this review did not do

- Did not open the Unity Editor (R2) or enter Play Mode (R4). The champion flow, confirm panel and
  door opening are read from code and data only.
- Did not inspect the +1,055-line `GameplayUI.unity` diff beyond confirming the confirm panel exists.
- Did not re-run `/module-quality-audit`, `/consistency-check` or `/architecture-review`.
- Did not edit `CLAUDE.md`, rules, GDDs, skills, hooks or routines — drift is listed for `/doc-sync`
  and the owner.
