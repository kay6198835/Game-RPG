# Bug Inbox

> Suspected defects noted during the week. **No bug IDs are created here.** `/weekly-wrapup`
> triages every open note on Saturday night and is the only writer allowed to allocate a `BUG-NNN`.
> Rules: `.claude/rules/bug-inbox.md`. Started 2026-10-09.

## Open notes

| Note | Date | Source | Sev guess | Where | What | Evidence tier |
|---|---|---|---|---|---|---|

_(none — all open notes triaged by `/weekly-wrapup` on 2026-10-10)_

## Note text archive (triaged 2026-10-10)

| Note | Date | Source | Sev guess | Where | What | Evidence tier |
|---|---|---|---|---|---|---|
| NOTE-20261009-9 | 2026-10-09 | doc-sync | S4 | `production/qa/bugs/BUG-064.md`, `BUG-087.md` | Bug-file status lags the code. **BUG-064**: file says PARTIAL (sub-item 7 open), but at `sprint-17` `cdf68555` `RangeWeapon.cs:16-17` has `[Inject] Construct(IObjecPoolService)` and `WeaponHolderBase.cs:60` calls `InjectGameObject` → sub-7 looks fixed (`5b035b73`). **BUG-087**: file says Open; `RealPlayerDataProvider.cs:47` now subscribes to `ON_PLAYER_DEATH` → partial. `CLAUDE.md` now shows the file statuses; triage should decide whether to update the files | STATIC |
| NOTE-20261009-10 | 2026-10-09 | doc-sync | S3 | `Assets/Script/Weapons/RangeWeapon/RangeWeapon.cs:11,67` | `nextFireTime` is written (`:67`, `Time.time + stage.RecoveryTime`) but never read; `CanAttack()` (`:22-23`) has no time term, so `RangeAttackSO.RecoveryTime` has no effect. Also `OnDrawGizmosSelected` (`:82-83`) draws `firePoint + firePoint * attackRange`. One of the three unfiled defects from BUG-101 (mislabelled "BUG-093" in `CLAUDE.md` 2026-10-05); re-confirmed at `cdf68555` | STATIC |
| NOTE-20261009-11 | 2026-10-09 | doc-sync | S4 | `Assets/Script/Character/Player/PlayerState.cs:35` | `PlayerState.Enter()` calls `Debug.Log("Enter State: " + animBoolName)` with no `#if UNITY_EDITOR` guard: a string concat and a log on every player state change, shipped in builds. BUG-101 orphan (mislabelled "BUG-094"); re-confirmed at `cdf68555`. Relates to NOTE-20261009-2 (an NRE was logged at the same line from an uncommitted file) | STATIC |
| NOTE-20261009-12 | 2026-10-09 | doc-sync | S2 | `Assets/Script/Weapons/MeleeWeapon/AttackSO.cs:10`; `Assets/SO/Weapons/MeleeWeapons/SnS_State1-3.asset` | `attackDamege` was renamed to `attackDamage` in `ddcc0a5c` with no `[FormerlySerializedAs("attackDamege")]`. The three player sword stages still serialize `attackDamege: 55`, so Unity loads `attackDamage = 0` and a sword hit deals only `PhysicalDamage`. BUG-101 orphan (mislabelled "BUG-095"); re-confirmed at `cdf68555` (no attribute; 3 assets still carry the old key). Needs Play Mode to confirm the damage loss. `.claude/rules/scriptableobject-data.md` and the doc-sync skill cite "BUG-095" for this lesson; rewrite those once an ID exists | STATIC |

_(the four notes above were added by `/doc-sync --auto` on 2026-10-09, after that day's wrap-up triage)_

## Note text archive (triaged 2026-10-09)

| Note | Date | Source | Sev guess | Where | What | Evidence tier |
|---|---|---|---|---|---|---|
| NOTE-20261009-1 | 2026-10-09 | fix-survival | S1 | `Assets/Script/Map/Room/RoomGeneraterController.cs:134` | Fix `40d2c793` for BUG-096 (open doors at once in rooms with no spawn marker) is GONE at HEAD `221d54be`: 1 of 4 added lines remains. Removed by `ac13ee4f` "done load map." (owner, same evening, inside a large map commit). Intentional or accidental? Relates to BUG-096 / BUG-097. **Standup 2026-10-09 addendum:** `221d54be` "logic open door at start room" (feature branches only, not on `sprint-17`) renames `ON_CLEAR_ENEMY` → `ON_OPEN_DOOR` and has `ChampitionController.SetCharacterData()` emit it, so the **start room** now opens after a Champion is confirmed. `LoadRoom()` (`:134-144`) still has no zero-spawn branch, so Rest / Shop / Buff rooms (no `SPAWN` marker) still look sealed — partial, not fixed | STATIC |
| NOTE-20261009-2 | 2026-10-09 | editor-log | S3 | `Assets/Script/Character/Player/PlayerState.cs:35` | `NullReferenceException` with this as first project frame, once, in `Editor-prev.log` (session ending 2026-10-09 13:56). At HEAD line 35 is the `Debug.Log` in `PlayerState.Enter()`, which cannot throw — the trace is likely from an uncommitted version of the file; `player.Anim` (`:32`) is the plausible null | LOG |
| NOTE-20261009-3 | 2026-10-09 | editor-log | S3 | `Assets/Script/UIFlow/Gameplay/GameplayUIController.cs:55` | `error CS0246: 'InputSystemUIInputModule' could not be found` in `Editor-prev.log`. The 2026-10-09 17:00 batchmode compile of HEAD `221d54be` passed (0 errors), so this was likely a transient state of the owner's working tree — confirm it does not recur | LOG |
| NOTE-20261009-4 | 2026-10-09 | standup | S4 | `Assets/Script/Manager/EventManager.cs:46`, `ProjectSettings/EditorBuildSettings.asset` | `221d54be` renamed `EventID.ON_CLEAR_ENEMY` → `ON_OPEN_DOOR` (same enum slot) and moved `LoadRandomMap.unity` out of `Scenes/Main/Test/`. `CLAUDE.md` (Event System table, Map flow, Scene Map, main dev scene path) still names the old event and path — doc drift for `/doc-sync`, not a runtime defect | STATIC |
| NOTE-20261009-5 | 2026-10-09 | wrapup | S3 | `Assets/Script/UIFlow/Core/UIEvents.cs:57,63` (at `221d54be`) | `OpenConfirnPanel.Invoke(data)` and `ConfirnRequest.Invoke()` are raised without `?.` (review-flow R3.6); the only subscriber lives in the additive `GameplayUI` scene, so Play Mode on `LoadRandomMap` alone throws on Champion interact. `ResetStatics()` (`:46-53`) clears neither new event. `ConfirnPanel.SetConfirm()` writes the button labels into title/message | STATIC |
| NOTE-20261009-6 | 2026-10-09 | wrapup | S3 | `Player.cs:81-85`, `Entity.cs:78-82`, `ChampitionController.cs:40-44` (at `221d54be`) | `SetCharacterData()` only swaps the `data` reference and renames the GameObject. Nothing re-reads it: ability bindings are copied in `AbilityHolder.Start()`, stats live in the stat SO, the HUD binds once on `ON_PLAYER_READY` — so picking a Champion changes no stat, ability, animator or hotbar. The rename would break `FollowPlayer.Get()` (`GameObject.Find("PlayerTest(Clone)")`, TD-029) if called afterwards. `Entity.SetCharacterData` casts to `EntityData` unchecked. Secondary: `localScale = 2.5f` and Vietnamese dialogue strings hardcoded in `ChampitionController`; unused `using Unity.VisualScripting` | STATIC |
| NOTE-20261009-7 | 2026-10-09 | wrapup | S4 | `Assets/Script/Character/Player/States/PlayerIntertorState.cs:16,22` (at `221d54be`) | `AnimationOnAction()` dereferences `interactor` resolved by `GetCoreComponent(out …)`, which returns null silently when the component is absent | STATIC |
| NOTE-20261009-8 | 2026-10-09 | fix-survival | S3 | `fix-survival.sh 30` | PARTIAL rows: BUG-076 / BUG-078 fix `73ab8e77` (35% of added lines survive); BUG-093 / BUG-094 fix `b7a0af5e` (69%, later `7c637c0e`) | STATIC |

## Triaged notes

| Note | Triaged on | Outcome | Result (bug ID / commit / reason) |
|---|---|---|---|
| NOTE-20261009-1 | 2026-10-09 | Duplicate | Evidence appended to **BUG-096** (still Open on `sprint-17`; start-room half addressed off-branch in `221d54be`, Rest / Shop / Buff rooms still sealed) |
| NOTE-20261009-2 | 2026-10-09 | Needs owner | Not decidable from code: at every committed HEAD `PlayerState.cs:35` is a `Debug.Log` that cannot throw, so the trace came from an uncommitted file. Carried; re-check `editor-log.sh` next week — if it recurs on a committed line, confirm |
| NOTE-20261009-3 | 2026-10-09 | Resolved in week | `compile-check.sh` PASS on `221d54be` + working tree (2026-10-09 standup) and `Editor.log` clean — a transient working-tree state. New note only if it recurs |
| NOTE-20261009-4 | 2026-10-09 | Not a bug / by design | Doc drift, not a runtime defect — routed to `/doc-sync`. Code side of R3.5 is clean: no `.cs` at `221d54be` still names `ON_CLEAR_ENEMY` |
| NOTE-20261009-5 | 2026-10-09 | Confirmed | **BUG-102** (ID matched to the uncommitted `BUG-102.md` an interactive session had written for the same defect in the owner's folder) |
| NOTE-20261009-6 | 2026-10-09 | Needs owner | Prototype on a feature branch with no story, GDD or ADR: whether a Champion pick must re-seed stats / abilities / animator / HUD is a design decision. Carried to the Sprint 18 kickoff as a story candidate |
| NOTE-20261009-7 | 2026-10-09 | Not a bug / by design | Follows the project-wide `GetCoreComponent` convention (`CLAUDE.md` › Shared base layer); `Interactor` is a required player core component, so a null here is a prefab wiring error. Also checked: `InteractiveObjects.Awake()` is empty, so the missing `base.Awake()` in `ChampitionController` is harmless |
| NOTE-20261009-8 | 2026-10-09 | Not a bug / by design | Superseded rewrites, not regressions: `2a83469` rewrote the `73ab8e77` cost/cast code (BUG-076 / BUG-078 closed on it); `7c637c0e` moved flight into `ProjectileBody`. Substance present at HEAD: `SpawnProjectileEffect.cs:16` reads `context.Forward` (BUG-093), `RangeWeapon` no longer writes the root rotation (BUG-094) |
| NOTE-20261009-9 | 2026-10-10 | Resolved in week (status update) | Bug files updated on `pm/wrapup-2026-10-10`: **BUG-064** → FIXED in code, awaiting runtime (`5b035b73`: `RangeWeapon.cs:16-20` `[Inject]` + `WeaponHolderBase.cs:60` `InjectGameObject`); **BUG-087** → PARTIAL (`cb0de496`: `RealPlayerDataProvider.cs:47` subscribes, `Respawn()` reloads the scene; no in-place reset, never run) |
| NOTE-20261009-10 | 2026-10-10 | Confirmed | **BUG-103** (S3) — re-read at `sprint-18` `c6cbc57a`: `nextFireTime` written `:67`, never read; `CanAttack()` `:22-24` has no time term |
| NOTE-20261009-11 | 2026-10-10 | Confirmed | **BUG-104** (S4) — `PlayerState.cs:34` unguarded `Debug.Log` concat in `Enter()`, added `9154763f` |
| NOTE-20261009-12 | 2026-10-10 | Confirmed | **BUG-105** (S2) — `AttackSO.cs:10` has no `[FormerlySerializedAs]`; `SnS_State1-3.asset` still serialize `attackDamege`; consumer `WeaponHolderBase.cs:92-98` |
| NOTE-20261009-2 (re-check) | 2026-10-10 | Not reproduced — closed | `editor-log.sh` 2026-10-10: `Editor.log` (22:18) and `Editor-prev.log` (16:58), one Play Mode entry each, no exception with an `Assets/Script/` frame. The unguarded log line itself is filed as BUG-104. Re-open as a new note only if the NRE recurs |
| NOTE-20261009-6 (carry) | 2026-10-10 | Needs owner (carried again) | No code change since 2026-10-09; whether a Champion pick must re-seed stats / abilities / animator / HUD is still an open design decision (candidate for S18-F1) |
