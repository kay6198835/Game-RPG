# Bug Triage — 2026-10-05 (Sprint 16 wrap-up)

Run: `pm-weekly-wrapup`, autonomous, no owner present. **Late** — the Saturday 2026-10-03 slot did not
fire; this run executes Monday 2026-10-05, before any Sprint 17 kickoff.
Branch `sprint-16`, HEAD `ac13ee4` ("done load map."), in sync with `origin/sprint-16`, tree clean.
Window: `5d2dc8a` (last wrap-up) .. `ac13ee4` — 22 commits (3 chore), 26 `.cs` files (+419/-181).

Static analysis only. Nothing has been verified in Play Mode (10th sprint).

---

## Code review — this week's `.cs` (surface only, nothing fixed)

Files already reviewed by the 09-29 / 10-01 standups (`b7a0af5`, `7c637c0`, `9154763`, `e8d117e`) are
not repeated. New since the last standup read: `3a395fe`, `0bc3640`, `40d2c79`, `ac13ee4`.

| # | File:line | Sev | Finding | Filed |
|---|-----------|-----|---------|-------|
| 1 | `RoomGeneraterController.cs:134-139` | S1 | `ac13ee4` reverted `40d2c79`; rooms with no spawn markers never open their doors. 5/20 rooms have none, including Start and Boss | **BUG-096** |
| 2 | `RoomGeneraterController.cs:55-56` + `Maze_Storage.asset` | S2 | Room list is now alphabetical: start cell = Boss room, end cell = Start room. Bug #16 manifested | **BUG-097** |
| 3 | `RoomGeneraterController.cs:28`, `GameLifetimeScope.cs:21` | S3 | ✅ `LevelManager.Instance` singleton **removed** (grep `LevelManager.Instance` = 0) — closes Bug #12 / TD-023 in code. ⚠️ But `LevelManager` is injected as a **concrete** MonoBehaviour, not behind an interface (`engine-code.md` rule) — second instance after `RoomGridController` → `PlayerManager` | Not filed — note for `/doc-sync` |
| 4 | `Entity.cs:34-43` | S3 | `OnEnable()` initializes Idle unless already Idle, then `Start()` calls `Initialize(idleState)` again → on first spawn `EntityIdleState.Enter()` runs **twice** (timers reset twice; harmless today). A pooled enemy disabled while Idle skips re-entry on respawn and keeps stale idle timers | Not filed — note |
| 5 | `PlayerInputHandle.cs:118-143,272-317` | Info | Abilities rebound to keys **1-4** (Primary/Secondary/Utility/Ultimate) via a shared `OnAbility(ctx, slot)`. `Block` (RMB) and `SkillWeapon` handlers commented out → `OnAbilityWeapon()` is dead code; RMB is bound in `PlayerInput.cs:284-288` but does nothing. CLAUDE.md Input Bindings table is stale. Needs all four slots authored on `PlayerData.AbilityBindings` | Not filed — `/doc-sync` |
| 6 | `MapGridController.cs:20,26` | ✅ | Minimap `ON_PLAYER_ON_DOOR → Move` re-subscribed, Register/UnRegister paired | — |
| 7 | `PlayerState.cs` `Enter()` | S4 | `Debug.Log("Enter State: " + …)` leftover (reported 10-01) still present | Carried note |

---

## New this week

| ID | Sev | Pri | Title | Next |
|----|-----|-----|-------|------|
| BUG-096 | S1 | P1 | No-spawn rooms never open (revert of `40d2c79`) | Owner decision, then 1-branch restore |
| BUG-097 | S2 | P1 | Start ↔ Boss room swapped by alphabetical `Maze_Storage` | Reorder asset (stop-gap) or select by `RoomType` |

## Status changes this week

| ID | Change | Evidence |
|----|--------|----------|
| BUG-093 | ✅ Code done (`b7a0af5`) — Play Mode confirm pending | `SpawnProjectileEffect.Angle()` uses `_context.Forward` |
| BUG-094 | ✅ Code done (`7c637c0`) | `firePoint` is a `Vector2`, nothing rotated |
| BUG-095 | ⚠️ Partial, unchanged since 09-29 | `Arrow.prefab` untouched since `7c637c0`; still no `Collider2D` |
| Bug #13 | ✅ Code done (`9154763`, then `0bc3640`) | `OnDoneLoadRoomGrid()` → `SetPlayerPosition(StartDoorPosition + cell pos)` |
| Bug #12 / TD-023 | ✅ Code done (`0bc3640`) | `LevelManager.Instance` deleted, injected instead |
| Bug #16 | ❌ → live as BUG-097 | See above |

## Carried, unchanged (re-verified at `ac13ee4`)

`git log 5e1d81f..HEAD` shows no edit to `VitalStatsBase.cs`, `PlayerDeathState.cs`,
`RecoveryReductionPerTimeForDuration.cs`, `Lightning.prefab` or `Arrow.prefab` after the 10-01 read.

| ID | Sev | Carry | Evidence |
|----|-----|-------|----------|
| BUG-066 + BUG-070 | S2 | 7th / 5th | `VitalStatsBase` raw indexer; `e8d117e` added 4 more reads via `UpdateStatField()` |
| BUG-065 | S2 | 7th | `PlayerDeathState.Enter()` only `base.Enter()` |
| BUG-086 | S2 | 2nd | `ON_PLAYER_DEATH` re-emitted every frame |
| BUG-087 | S2 (feature) | — | 0 subscribers, no `GameManager` |
| BUG-072 | S2 | 3rd | `Lightning.prefab` last touched `fdc08d9`; no `layerMask` serialized |
| BUG-095 | S2 | 2nd | No collider on `Arrow.prefab` |
| BUG-092 residual | S3 | 2nd | `perTime` / `timeCount` still `private`, no `[SerializeField]` (`:6-8`) |
| BUG-071 | S3 | 3rd | No coroutine handle |
| BUG-073 / BUG-090 | S3 | 3rd | Missing-script assets + 4 deleted-bullet assets + `SpawnBat/SpawnCrab.asset` |
| BUG-068 | S3 | — | `AbilityHolderBase.cs:28` |
| BUG-079, BUG-083 | S3 | — | Dormant |
| BUG-084 | S3 | — | 0 `.asmdef` |
| BUG-052 | doc | — | Now also: `ProjectileBody` / `IProjectilePayload`, runtime-spawned Player, UIFlow |
| BUG-063 | — | accepted | Deferred to demo prep |

---

## Systemic Issues

1. **A verified fix was silently reverted** (BUG-096). The owner's map commit overwrote a one-branch fix
   made earlier the same day. With no Play Mode run and no review step, nothing noticed. This is the
   review-gate question (S16-12, 5th carry) in its sharpest form yet.
2. **Data order is load-bearing and undocumented** (BUG-097). Renaming room files alphabetically
   changed gameplay. Any code that reads `list[0]` / `list[last]` for meaning is a trap.
3. **Feature/content stream absorbed the whole sprint again** — 0 of 9 code Must-Have items moved.
4. **Binary growth without LFS**: 12 `.fbx` added this week, pack size 137.8 MiB, `.gitattributes` has
   only `* text=auto`.

## Recommendation for Sprint 17 (owner decision — this run does not create it)

Must, in order: **BUG-096 + BUG-097** (≈0.1d, they block every run) → BUG-092 residual → BUG-066/070 →
BUG-065 + BUG-086 → Editor block (BUG-072 layerMask, BUG-095 collider) → **Play Mode smoke**.
Total ≈1.1d — the same set as Sprint 16 plus the two map bugs.
