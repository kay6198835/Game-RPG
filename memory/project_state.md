# Project State

Updated **2026-10-10** (doc-truth pass `/doc-sync --auto` at `origin/sprint-18` `617bb2bb`).
Bug statuses below follow `production/qa/bugs/BUG-*.md`, which are the source of truth (after the
2026-10-10 wrap-up triage). Claim sweep: `production/qa/doc-truth-2026-10-10.md`. Previous update:
**2026-10-09** (`sprint-17` `cdf68555`).

Snapshot of actual code state. Long form: `CLAUDE.md`. Per-system current docs and change logs:
`docs/systems/<system>/README.md` + `CHANGELOG.md`.

---

## Build status

✅ **Compiles** as far as last checked: the 2026-10-09 standup batchmode compile passed on `221d54be`,
which is the code now on `sprint-18` (no `.cs` change since). Nothing compiles code automatically
before a push (TD-048 / TD-051).

---

## Structural changes since the last update (2026-10-09 → 2026-10-10, on `sprint-18`)

Feature branch `221d54be` was merged into `sprint-18` by `42a81260` (`c6cbc57a`).

| # | Change | Commit(s) | Impact |
|---|--------|-----------|--------|
| R18 | `EventID.ON_CLEAR_ENEMY` renamed **`ON_OPEN_DOOR`** (same slot, still 24 values). Emitters: `RoomCell` (0 alive), `ChampitionController`, `LevelManagerEditor` button | `221d54be` | GDDs / ADR-0002 carry a read-as banner, body unchanged |
| R19 | `ChampitionController : InteractiveObjects` — start-room Champion; interact → `ConfirnPanel` → `ICharacter.SetCharacterData()` + `ON_OPEN_DOOR` | `221d54be` | Opens the start room only if a Champion is placed (BUG-096 stays open); BUG-102 |
| R20 | `UIFlow/Gameplay/Windows/ConfirnPanel.cs` + `ConfirnData`; `UIEvents.OnOpenConfirmPanel()` / `AccessAction()` | `221d54be` | BUG-102 |
| R21 | `ICharacter.SetCharacterData(CharacterData)`, abstract on `CharacterBase` | `221d54be` | ADR-0005 Amendment 1 ("`Transform` only") not amended; NOTE-20261009-6 (Needs owner) |
| R22 | `PlayerIntertorState.AnimationOnAction()` → `Interactor.Intertion()` | `221d54be` | Interact key now triggers interaction on the anim event |
| R23 | Main dev scene moved: `Assets/Scenes/Main/LoadRandomMap.unity` (was `Main/Test/`) | `221d54be` | Build settings already updated; loaded by name |

Earlier structural changes (R5–R17) are recorded in `CLAUDE.md` history entries.

---

## Open bugs (bug-file status, re-read 2026-10-10)

| # | Sev | Description | Location |
|---|-----|-------------|----------|
| BUG-096 | S1 | Marker-less rooms never open (fix `40d2c793` reverted). Start room opens only via the Champion | `RoomGeneraterController.cs:134` |
| BUG-087 | S1 | PARTIAL: one UIFlow `ON_PLAYER_DEATH` subscriber; no `GameManager`, no player `Reborn()` caller | `PlayerDeathState.cs` |
| BUG-105 | S2 | `attackDamege` → `attackDamage` rename without `[FormerlySerializedAs]`: sword stages load 0 | `AttackSO.cs:10` |
| BUG-097 | S2 | Alphabetical `Maze_Storage.asset`: start cell = Boss room | `RoomGeneraterController.cs:55` |
| BUG-095 | S2 | PARTIAL — `Arrow.prefab` has script + Rigidbody2D, still no `Collider2D` | `Arrow.prefab` |
| BUG-100 | S2 | Timed buff expiry strips every modifier from the same source (item buffs too) | `VitalStatsBase.cs:120` |
| BUG-092 | S3 | PARTIAL: `perTime` / `timeCount` not serialized → HoT/DoT runs one tick | `RecoveryReductionPerTimeForDuration.cs:6-8` |
| BUG-072 | S2 | `Lightning.prefab` `layerMask` unset | `Lightning.prefab` |
| BUG-066 / BUG-070 | S2 | Unguarded `currentStats[statType]` in `VitalStatsBase` | `VitalStatsBase.cs` |
| BUG-086 | S2 | `ON_PLAYER_DEATH` emitted every frame | `PlayerDeathState.cs` |
| BUG-084 | S2 | Zero `.asmdef`; `tests/` outside `Assets/` | — |
| BUG-103 | S3 | `RangeWeapon.nextFireTime` never read — `RecoveryTime` dead | `RangeWeapon.cs:67` |
| BUG-102 | S3 | `UIEvents` confirm events raised without `?.`; `ConfirnPanel` labels swapped (now on `sprint-18`) | `UIEvents.cs:57,63` |
| BUG-065 | S3 | `PlayerDeathState.Enter()` does not stop movement | `PlayerDeathState.cs:10` |
| BUG-099, BUG-101 | S3 | Doc drift / ID collision — live docs corrected; files still Open for the triage | — |
| BUG-068, 071, 073, 079, 083, 090 | S3 | Unchanged — see `CLAUDE.md` | — |
| BUG-104 | S4 | Unguarded `PlayerState.Enter()` log | `PlayerState.cs:35` |
| BUG-098 | S4 | GDD/ADR-0003 still say `RarityTier` (code: `RarityTierEnemy`) | `RoomModel.cs:110` |
| BUG-052 | DOC | Live subsystems with no ADR | — |
| BUG-063 | — | ACCEPTED (deferred to demo prep) | `Stat.cs:63-66` |
| 14, 15, 16, 17, 6 | — | Historical CLAUDE.md numbering, unchanged | see `CLAUDE.md` |

**Fixed in code per file (confirm in Play Mode):** BUG-064 (sub-7 `RangeWeapon` DI, `5b035b73`),
BUG-093 (ability projectile direction) and BUG-094 (ranged weapon root rotation), both in `b7a0af5`.

---

## EventID enum (current on `sprint-18` — 24 values)

`ON_PLAYER_ON_DOOR`, `ON_PLAYER_DEATH`, `ON_REALOAD_GAME`, `ON_LOAD_MAZE_DONE`, `ON_LOAD_MAP`,
`ON_OPEN_DOOR`, `ON_GET_SPAWN_POSITIONS`, `ON_DONE_SPAWN_ENEMY`, `ON_SPAWN_EXTRA_ENEMY`,
`ON_TEST`, `ON_ENEMY_DEATH`, `ON_ROOM_CLEAR`, `ON_OPEN_STATS_PLAYER_UI`,
`ON_CLOSE_STATS_PLAYER_UI`, `ON_INCREASE_STATS_BY_UI`, `ON_DECREASE_STATS_BY_UI`,
`ON_CHANGE_STATS_BY_UI_RUN_TIME`, `ON_UPDATE_STATS_BY_UI`, `ON_REVERT_STATS_BY_UI`,
`ON_RESTORE_STATS_BY_UI`, `ON_RESET_STATS_UI_SESSION`, `ON_DROP_ITEM`, `ON_COLLECT_ITEM`,
`ON_PLAYER_READY`

`ON_OPEN_DOOR` was `ON_CLEAR_ENEMY` until `221d54be`. Still missing: `ON_PLAYER_TAKE_DAMAGE`.
`ON_ROOM_CLEAR` has no producer.

---

## Key API changes to be aware of

| Contract | Was | Is now |
|---|---|---|
| Room-clear event | `ON_CLEAR_ENEMY` | **`ON_OPEN_DOOR`** (`221d54be`) |
| Character root | `ICharacter { Transform }` | + `SetCharacterData(CharacterData)` (`221d54be`) |
| Main dev scene | `Assets/Scenes/Main/Test/LoadRandomMap.unity` | `Assets/Scenes/Main/LoadRandomMap.unity` |
| Modifier bundles | `StatModifierGroup.Apply(add, source)` / `.Remmove(remove, source)` | `stats.AddModifiersFromSource(source, group.Modifiers)` / `RemoveModifiersFromSource(source)` (`7f632021`) |
| Room cleared flag | Set in `RoomCell.OnEnemyDeath()` at zero | Set in `RoomCell.OpenDoors()` (`5d1986db`) |
| Player in scene | Placed + registered | Spawned by `PlayerManager`; not registered |
| `IPlayerStatService` | `StatHandler` from hierarchy | Factory → `PlayerManager.StatService` |
| Ranged projectile | `bullet.cs` + `BulletDataSO` | `ProjectileBody` + `ProjectileConfig` + `IProjectilePayload.OnHit()` |
| `IAbilityServices` | Pool / Stats / ResourceReceiver / Vital / NegativeReceiver | `Pool` only; target via `AbilityContext.Target` |
| `AttackSO` damage field | `attackDamege` | `attackDamage` — old assets keep the old key (BUG-105) |

---

## Stubs / unimplemented

- `UIFlow/Gameplay/Popup/PopupPanel.cs` — empty `: UIPanel`
- `ChampitionController` dialogue path commented out (`UIEvents.RequestDialogue`); confirm-only
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

1. **BUG-096** — zero-spawn branch in `LoadRoom()` for Boss / Buff / Rest / Shop (the start room now opens via the Champion, if one is placed) (S1).
2. **BUG-105** — `[FormerlySerializedAs("attackDamege")]`; confirm sword damage in Play Mode (S2).
3. **BUG-097** — select start/end by `RoomType`, not list position.
4. **BUG-095** collider on `Arrow.prefab` and **BUG-072** `layerMask` on `Lightning.prefab`. Both are Inspector fields.
5. **BUG-092 residual** — `[SerializeField]` on `perTime` / `timeCount`.
6. **BUG-100** — use a distinct modifier source per timed buff.
7. **Play Mode smoke** — Champion confirm, Paladin abilities, ranged weapon, one enemy kill, one full room transition.
8. **BUG-066/070**, then death path BUG-086 → BUG-065 → BUG-087.
9. **Bug #15** before the first standalone build; **BUG-084** before TD-014.
