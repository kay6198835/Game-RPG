# Project State

Updated **2026-10-09** (doc-truth pass `/doc-sync --auto` at `origin/sprint-17` `cdf68555`).
Bug statuses below follow `production/qa/bugs/BUG-*.md`, which are the source of truth. Claim sweep:
`production/qa/doc-truth-2026-10-09.md`. Previous update: **2026-10-05** (HEAD `93ba6d8e`). That
version listed BUG-093/094/095 under the wrong defects (BUG-101) and called marker-less rooms fixed
(BUG-099). Both errors are corrected here.

Snapshot of actual code state. Long form: `CLAUDE.md`. Per-system current docs and change logs:
`docs/systems/<system>/README.md` + `CHANGELOG.md`.

> ⚠️ The owner's newest code (`221d54be`, Champion select, `ON_CLEAR_ENEMY` → `ON_OPEN_DOOR` rename,
> `LoadRandomMap.unity` moved) is on feature branches and **not** on `sprint-17`. This file describes
> `sprint-17`.

---

## Build status

✅ **Compiles.** The 2026-10-09 standup batchmode compile passed on `221d54be`. Nothing compiles
code automatically before a push (TD-048 / TD-051).

---

## Structural changes since the last update (2026-10-05 → 2026-10-09, on `sprint-17`)

| # | Change | Commit(s) | Impact |
|---|--------|-----------|--------|
| R14 | `StatModifierGroup.Apply()` / `.Remmove()` deleted; `VitalStatsBase` and `Weapon` call `AddModifiersFromSource(this, group.Modifiers)` / `RemoveModifiersFromSource(this)` directly | `7f632021` | ADR-0005 Context sentence is out of date; BUG-100 sits on this path |
| R15 | `System/StartGameSystem/ChampitionController.cs` added (3-field stub, not a MonoBehaviour); `PlayerData` gains an `AnimatorOverrideController` field | `7f632021` | Real Champion select is on `221d54be` only |
| R16 | `RoomCell.IsCleared` set in `OpenDoors()`; `DeleteDoorTileMap()` returns early on a cleared room; `UIFlow/Gameplay/Popup/PopupPanel.cs` empty stub | `5d1986db` | — |
| R17 | Paladin Knight PowerUp animation clips replaced | `f30b8343` | Asset-only |

Earlier structural changes (R5–R13) are recorded in `CLAUDE.md` history entries.

---

## Open bugs (bug-file status, re-read 2026-10-09)

| # | Sev | Description | Location |
|---|-----|-------------|----------|
| BUG-096 | S1 | Marker-less rooms (Start/Boss/Rest/Shop/Buff) never open; fix `40d2c793` reverted by `ac13ee4f` | `RoomGeneraterController.cs:134` |
| BUG-087 | S1 | Open: no `GameManager`, no player `Reborn()` caller (code now has one UIFlow `ON_PLAYER_DEATH` subscriber, but the file still says Open, see NOTE-20261009-9) | `PlayerDeathState.cs` |
| BUG-097 | S2 | Alphabetical `Maze_Storage.asset`: start cell = Boss room | `RoomGeneraterController.cs:55` |
| BUG-095 | S2 | PARTIAL — `Arrow.prefab` has script + Rigidbody2D, still no `Collider2D` | `Arrow.prefab` |
| BUG-100 | S2 | Timed buff expiry strips every modifier from the same source (item buffs too) | `VitalStatsBase.cs:120` |
| BUG-092 | S2 | `perTime` / `timeCount` not serialized → HoT/DoT runs one tick | `RecoveryReductionPerTimeForDuration.cs:6-8` |
| BUG-072 | S2 | `Lightning.prefab` `layerMask` unset | `Lightning.prefab` |
| BUG-066 / BUG-070 | S2 | Unguarded `currentStats[statType]` in `VitalStatsBase` | `VitalStatsBase.cs` |
| BUG-086 | S2 | `ON_PLAYER_DEATH` emitted every frame | `PlayerDeathState.cs` |
| BUG-084 | S2 | Zero `.asmdef`; `tests/` outside `Assets/` | — |
| BUG-064 | S1 | PARTIAL per file (sub-7). Code carries the fix (`RangeWeapon.cs:16-17`); file not updated, see NOTE-20261009-9 | `RangeWeapon.cs` |
| BUG-065 | S3 | `PlayerDeathState.Enter()` does not stop movement | `PlayerDeathState.cs:10` |
| BUG-102 | S3 | `UIEvents` confirm events raised without `?.` (code on `221d54be` only) | `UIEvents.cs:57` |
| BUG-099 | S3 | Doc drift: 2026-10-05 sync recorded BUG-096 fixed (live docs corrected 2026-10-09) | — |
| BUG-101 | S3 | Bug-ID collision 093/094/095 (`CLAUDE.md` corrected 2026-10-09; orphans are NOTE-10/11/12) | — |
| BUG-068, 071, 073, 079, 083, 090 | S3 | Unchanged — see `CLAUDE.md` | — |
| BUG-098 | S4 | GDD/ADR-0003 still say `RarityTier` (code: `RarityTierEnemy`) | `RoomModel.cs:110` |
| BUG-052 | DOC | Live subsystems with no ADR | — |
| BUG-063 | — | ACCEPTED (deferred to demo prep) | `Stat.cs:63-66` |
| 14, 15, 16, 17, 6 | — | Historical CLAUDE.md numbering, unchanged | see `CLAUDE.md` |

**Fixed in code per file (confirm in Play Mode):** BUG-093 (ability projectile direction), BUG-094
(ranged weapon root rotation), both in `b7a0af5`.

**Unfiled defects awaiting an ID (bug inbox):** NOTE-20261009-10 (`RangeWeapon.nextFireTime` never
read), NOTE-20261009-11 (unguarded `PlayerState.Enter()` log), NOTE-20261009-12 (`attackDamege`
rename: the sword stages load `attackDamage = 0`, S2).

---

## EventID enum (current on `sprint-17` — 24 values)

`ON_PLAYER_ON_DOOR`, `ON_PLAYER_DEATH`, `ON_REALOAD_GAME`, `ON_LOAD_MAZE_DONE`, `ON_LOAD_MAP`,
`ON_CLEAR_ENEMY`, `ON_GET_SPAWN_POSITIONS`, `ON_DONE_SPAWN_ENEMY`, `ON_SPAWN_EXTRA_ENEMY`,
`ON_TEST`, `ON_ENEMY_DEATH`, `ON_ROOM_CLEAR`, `ON_OPEN_STATS_PLAYER_UI`,
`ON_CLOSE_STATS_PLAYER_UI`, `ON_INCREASE_STATS_BY_UI`, `ON_DECREASE_STATS_BY_UI`,
`ON_CHANGE_STATS_BY_UI_RUN_TIME`, `ON_UPDATE_STATS_BY_UI`, `ON_REVERT_STATS_BY_UI`,
`ON_RESTORE_STATS_BY_UI`, `ON_RESET_STATS_UI_SESSION`, `ON_DROP_ITEM`, `ON_COLLECT_ITEM`,
`ON_PLAYER_READY`

Pending off-branch: `221d54be` renames `ON_CLEAR_ENEMY` → `ON_OPEN_DOOR` (same slot). Still missing:
`ON_PLAYER_TAKE_DAMAGE`. `ON_ROOM_CLEAR` has no producer.

---

## Key API changes to be aware of

| Contract | Was | Is now |
|---|---|---|
| Modifier bundles | `StatModifierGroup.Apply(add, source)` / `.Remmove(remove, source)` | **`stats.AddModifiersFromSource(source, group.Modifiers)`** / `RemoveModifiersFromSource(source)` (`7f632021`) |
| Room cleared flag | Set in `RoomCell.OnEnemyDeath()` at zero | Set in `RoomCell.OpenDoors()` (`5d1986db`) |
| Player in scene | Placed + registered | Spawned by `PlayerManager`; not registered |
| `IPlayerStatService` | `StatHandler` from hierarchy | Factory → `PlayerManager.StatService` |
| Ranged projectile | `bullet.cs` + `BulletDataSO` | `ProjectileBody` + `ProjectileConfig` + `IProjectilePayload.OnHit()` |
| `IAbilityServices` | Pool / Stats / ResourceReceiver / Vital / NegativeReceiver | `Pool` only; target via `AbilityContext.Target` |
| `AttackSO` damage field | `attackDamege` | `attackDamage` — old assets keep the old key (NOTE-20261009-12) |

---

## Stubs / unimplemented

- `System/StartGameSystem/ChampitionController.cs` — 3 serialized fields, no base class (on `sprint-17`)
- `UIFlow/Gameplay/Popup/PopupPanel.cs` — empty `: UIPanel`
- UIFlow `RealLoginService`, `RealInventoryProvider`, `RealQuestProvider` — stubs
- `Assets/Script/Interface/IReasourceReceiver` — empty extensionless orphan
- `SpawnProjectileBase.pierceCount`, `RoomModel.overflowPercent` — serialized, never read
- `PlayerUserItemState` — extends `MonoBehaviour` (TD-001)
- `PlayerData.Reborn()` — no caller; `EnemySO` not consumed by `Entity` (TD-030); `TalentManagger` hardcoded (TD-018)
- `Assets/Script/Character/Boss/`, `Assets/Script/Handler/` — `.meta`-only orphans
- `tests/` — only `.gitkeep` (TD-014, blocked by BUG-084)

---

## Undocumented systems (no GDD / no ADR)

| System | Location | Gap |
|--------|----------|-----|
| UIFlow | `Assets/Script/UIFlow/` | No GDD, no ADR |
| Start-game / Champion select | `Assets/Script/System/StartGameSystem/` | No GDD, no story (NOTE-20261009-6, needs owner) |
| Projectile layer | `ProjectileBody.cs`, `IProjectilePayload.cs` | No ADR |
| Item / Depot | `Assets/Script/System/Item/` | No GDD, no ADR |
| Abilities v2 | `Assets/Script/System/Abilities/` | No GDD of its own |
| Pathfinding, Object pooling | `System/Pathfinding/`, `System/PoolableService/` | No GDD (BUG-052) |

---

## Demo fix priority

1. **BUG-096** — zero-spawn branch in `LoadRoom()`. Without it the run cannot pass a marker-less room (S1).
2. **BUG-097** — select start/end by `RoomType`, not list position.
3. **NOTE-20261009-12** — needs an ID, then `[FormerlySerializedAs("attackDamege")]`. Confirm sword damage in Play Mode.
4. **BUG-095** collider on `Arrow.prefab` and **BUG-072** `layerMask` on `Lightning.prefab`. Both are Inspector fields.
5. **BUG-092 residual** — `[SerializeField]` on `perTime` / `timeCount`.
6. **BUG-100** — use a distinct modifier source per timed buff.
7. **Play Mode smoke** — Paladin abilities, ranged weapon, one enemy kill, one full room transition.
8. **BUG-066/070**, then death path BUG-086 → BUG-065 → BUG-087.
9. **Bug #15** before the first standalone build; **BUG-084** before TD-014.
