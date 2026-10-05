# Project State

Updated **2026-10-05** (doc sync at HEAD `93ba6d8e`, branch `origin/feature/synce-doc-and-code`).
Every claim below was read from source or from `git diff c0067f4..93ba6d8e`. The previous update of
this file was **2026-09-11** (HEAD `6d6a8e4`) — it was skipped by the 2026-09-21, 09-22 and 09-25
passes, which updated `CLAUDE.md` only. Gaps between those dates are covered by `CLAUDE.md` history
entries and `docs/CHANGELOG-DOCS.md`.

Snapshot of actual code state. Long form: `CLAUDE.md`. Per-system current docs and change logs:
`docs/systems/<system>/README.md` + `CHANGELOG.md` (new layout, 2026-10-05).

---

## Build status

✅ **Compiles** at `93ba6d8e`. The build break of 2026-09-25 (BUG-092, `perTime` deleted but still
used) was fixed by re-declaring the field. Nothing in the project compiles code automatically — a
pre-push compile check is still missing (TD-048 / TD-051).

---

## Structural changes since the last update (2026-09-11 → 2026-10-05)

| # | Change | Commit(s) | Impact |
|---|--------|-----------|--------|
| R5 | Abilities v2 effect + runtime layers replaced (`SpawnEffectBase` / `StatsEffectBase`, `SpawnMono` controllers); Paladin ability set becomes the live content; enum `SkillState` → `AbilityState` | `8295539`…`7cceda2` (sprints 13-14) | Recorded in CLAUDE.md 2026-09-21 |
| R6 | **ADR-0005 Amendments 1-3** — shared character bases (`CharacterBase`, `VitalStatsBase`, `NegativeReceiverBase`, `WeaponHolderBase`, `AbilityHolderBase`), `IAbilityServices` reduced to `Pool`, `EntityAttack` deleted | 2026-09-24/25 | Recorded in CLAUDE.md 2026-09-25 |
| R7 | **Shared projectile layer** — `ProjectileBody` + `ProjectileConfig` + `IProjectilePayload`; `bullet.cs` and `BulletDataSO.cs` deleted | `b7a0af5e`, `5b035b73`, `7c637c0e` | Weapons and abilities fire the same pooled projectile |
| R8 | **Player spawned at runtime** by `PlayerManager` (`resolver.Instantiate`); `Player` / `StatHandler` / `AbilityHolder` no longer registered; `LevelManager` and `RoomGridController` registered | `a8820666`, `ac13ee4f` | The Player prefab must not be in the scene |
| R9 | **UIFlow** — 65-file UGUI menu/loading/in-game UI system with Mock/Real providers; 4 new scenes | `0c38633d`…`f8f180d0` | First real HUD; Real providers are stubs |
| R10 | Abilities v1 unhooked from player weapons (`AttackSO.ability`, `WeaponStats.AbilityWeapon/SkillWeapon`, `Weapon.currentAbilitySO` deleted) | `b7a0af5e` | v1 survives only in `EntityWeapon` |
| R11 | Room data rebuilt: 13 `NormalRoom_N` → 20 named rooms (Start → Boss); no-spawn rooms open doors at once | `213fa5a6`…`40d2c793` | `Maze_Storage` lists the new set |
| R12 | Ability input split: keys 1-4 → Primary/Secondary/Utility/Ultimate; Block (RMB) handler commented out | `3a395fe9` | Was `E` → Utility |
| R13 | **UIFlow runs the real game** — HUD bound via `ON_PLAYER_READY`; character creation, `StartScene`, `UISample`, `MainMenu.cs`, legacy `UIManager` stub deleted | `cb0de496` (merged 2026-10-05) | `MainGamePlay` is build index 0 |

---

## Systems completed since the last update

| System | Notes |
|--------|-------|
| **Character base layer** | One `INegativeReceiver` implementer (`NegativeReceiverBase<TCore>`); `VitalStatsBase<TCore>` shared by player and enemy (player inherits `Reborn()`); `ICharacter` is the root identity marker |
| **Enemy attack** | Enemies attack only through `EntityWeaponHolder` + a `Weapon` prefab (`EntityData.WeaponSO`). Optional `EntityAbilityHolder` casts Abilities v2 |
| **Ranged weapon** | Fully DI-wired (BUG-064 closed): `[Inject] Construct(IObjecPoolService)`, injected on equip by `WeaponHolderBase`. Player range-weapon assets under `SO/Weapons/RangeWeapons/Player Range Weapon/` |
| **Projectile abilities** | `SpawnProjectileBase` delegates to `ProjectileBody` (2D trigger, target/block masks, pooled despawn) |
| **Start-room teleport** | Bug #13 closed — `OnDoneLoadRoomGrid()` → `PlayerManager.SetPlayerPosition()` |
| **LevelManager singleton removed** | Bug #12 / TD-023 closed — injected via VContainer |
| **UI flow** | UIFlow: splash → login → main menu → save select → character creation → loading → gameplay + additive `GameplayUI` (HUD, hotbar, inventory, quests, skill tree, shop, dialogue, pause, game over, damage numbers) |
| **Room set** | 20 authored rooms; Start/Rest/Shop/Buff rooms no longer lock the player in |

---

## Open bugs (verified 2026-10-05)

| # | Sev | Description | Location |
|---|-----|-------------|----------|
| BUG-095 | S2 | NEW — `AttackSO.attackDamege` renamed `attackDamage` (2026-09-01, no `[FormerlySerializedAs]`); `SnS_State1-3.asset` still store `attackDamege: 55` → the player sword loses its 55 stage damage — `WeaponHolderBase.CalculateCurrentDamage()` (`:92-98`) computes `PhysicalDamage + attackDamage (+ CritDamage on crit)`, so a hit deals only the character's `PhysicalDamage`. Confirm in Play Mode | `AttackSO.cs:10`, `SnS_State*.asset:17` |
| BUG-092 | S2 | `perTime` and `timeCount` not serialized → every HoT/DoT runs one instant tick (build break itself fixed) | `RecoveryReductionPerTimeForDuration.cs:6-8` |
| BUG-072 | S2 | `Lightning.prefab` has no `layerMask` set → summon damage hits nothing. Code complete | `Lightning.prefab` |
| BUG-066 / BUG-070 | S2 | Unguarded `currentStats[statType]` in the shared vitals base (one fix closes both) | `VitalStatsBase.cs:35,47,49,53,61,63,67` |
| BUG-087 | S2 | PARTIAL — no `GameManager`, no player `Reborn()` caller; only UIFlow listens to `ON_PLAYER_DEATH` | `PlayerDeathState.cs` |
| BUG-086 | S2 | `ON_PLAYER_DEATH` emitted every frame from `PlayerDeathState.LogicUpdate()` | `PlayerDeathState.cs:16-19` |
| BUG-065 | S3 | `PlayerDeathState.Enter()` does not stop movement | `PlayerDeathState.cs:10` |
| BUG-084 | S2 | Zero `.asmdef` under `Assets/`; `tests/` outside `Assets/` — no test can be written | — |
| BUG-063 | — | ACCEPTED (deferred to demo prep): `Stat.modifiers` serialized under `#if UNITY_EDITOR` | `Stat.cs:63-66` |
| BUG-093 | S3 | NEW — `RangeWeapon.nextFireTime` never read (`RecoveryTime` dead); `OnHit()` ignores `finalDamage` (no PhysicalDamage/crit); gizmo math wrong | `RangeWeapon.cs:23,67` |
| BUG-094 | S4 | NEW — unguarded `Debug.Log` in `PlayerState.Enter()` | `PlayerState.cs:35` |
| BUG-068 | S3 | `CurrentActivationType` dereferences `currentAbility` unguarded | `AbilityHolderBase.cs:28` |
| BUG-071 | S3 | PARTIAL — HoT/DoT tick chain has no handle, cannot be stopped | `VitalStatsBase.cs` |
| BUG-073 / BUG-090 | S3 | `ShootSpirit` assets + `Has Enough Mana Condition.asset` reference deleted scripts | `Assets/SO/Skill/ShootSpirit/`, `Assets/SO/Skill/Conditions/` |
| BUG-079 | S3 | `AbilityInstance.Exit()` body commented out (layering defect) | `AbilityInstance.cs` |
| BUG-083 | S3 | `HoldTime` / `HoldRatio` always `0f` | `AbilityContext.cs` |
| BUG-052 | DOC | Live subsystems with no ADR — now also UIFlow | — |
| 14 | MEDIUM | `MazeController.Awake()` missing `return` after `Destroy` | `MazeController.cs:17` |
| 15 | BUILD | Room JSON via `File.ReadAllText(Application.dataPath…)` — Editor-only | `RoomGeneraterController.cs` |
| 16 | MEDIUM | `RoomType` never read; start/end rooms by list position | `RoomGeneraterController.cs` |
| 17 | LOW | Dead door-gating code | `DoorController.cs:29` |
| 6 | MEDIUM | `PlayerData.currentHealth` never written; `PlayerData.Reborn()` no caller | `PlayerData.cs` |

**Closed since 2026-09-11:** BUG-043, BUG-064, BUG-067, BUG-069, BUG-074, BUG-075, BUG-076,
BUG-077, BUG-078, BUG-080, BUG-081, BUG-082, BUG-085, BUG-088, BUG-091, Bug #12, Bug #13.
BUG-089 closed by design (reopen at demo/release).

---

## EventID enum (current — 24 values)

`ON_PLAYER_ON_DOOR`, `ON_PLAYER_DEATH`, `ON_REALOAD_GAME`, `ON_LOAD_MAZE_DONE`, `ON_LOAD_MAP`,
`ON_CLEAR_ENEMY`, `ON_GET_SPAWN_POSITIONS`, `ON_DONE_SPAWN_ENEMY`, `ON_SPAWN_EXTRA_ENEMY`,
`ON_TEST`, `ON_ENEMY_DEATH`, `ON_ROOM_CLEAR`, `ON_OPEN_STATS_PLAYER_UI`,
`ON_CLOSE_STATS_PLAYER_UI`, `ON_INCREASE_STATS_BY_UI`, `ON_DECREASE_STATS_BY_UI`,
`ON_CHANGE_STATS_BY_UI_RUN_TIME`, `ON_UPDATE_STATS_BY_UI`, `ON_REVERT_STATS_BY_UI`,
`ON_RESTORE_STATS_BY_UI`, `ON_RESET_STATS_UI_SESSION`, `ON_DROP_ITEM`, `ON_COLLECT_ITEM`,
`ON_PLAYER_READY` (added 2026-10-05, `cb0de496`)

Still missing: `ON_PLAYER_TAKE_DAMAGE` (needed by the UIFlow HUD). `ON_ROOM_CLEAR` has no producer.
`ON_PLAYER_DEATH` has one subscriber (`UIFlow.RealPlayerDataProvider`). Note: UIFlow runs a second,
separate static bus (`UIFlow.UIEvents`) for UI-only requests.

---

## Key API changes to be aware of

| Contract | Was | Is now |
|---|---|---|
| Player in scene | Placed in the scene, registered in `GameLifetimeScope` | **Spawned by `PlayerManager`**; not registered |
| `IPlayerStatService` | `StatHandler` registered from the hierarchy | Factory → `PlayerManager.StatService` |
| `LevelManager` | `LevelManager.Instance` | **Injected** (`Construct(IPlayerService, LevelManager)`) |
| Ranged projectile | `bullet.cs` + `BulletDataSO` | **`ProjectileBody` + `ProjectileConfig` + `IProjectilePayload.OnHit()`** |
| Ability projectile hit | 3D `OnTriggerEnter` on `SpawnProjectileBase` | `ProjectileBody.OnTriggerEnter2D` → `SpawnProjectileBase.OnHit()` |
| `IAbilityServices` | Pool / Stats / ResourceReceiver / Vital / NegativeReceiver | **`Pool` only**; target via `AbilityContext.Target` |
| Ability input | `E` → Utility | `1/2/3/4` → Primary/Secondary/Utility/Ultimate |
| `AttackSO` damage field | `attackDamege` (intentional typo) | **`attackDamage`** (2026-09-01) — old assets still carry the old key (BUG-095) |
| Weapon ability (v1) | `WeaponStats.AbilityWeapon/SkillWeapon`, `AttackSO.ability` | Deleted |

---

## Stubs / unimplemented

- ~~`Manager/UI/UIManager.cs` empty stub (TD-017)~~ — deleted 2026-10-05 (`cb0de496`)
- UIFlow `Real*Provider` classes — TODO stubs; only the death event and save provider are wired
- `Assets/Script/Interface/IReasourceReceiver` — empty extensionless orphan file (new 2026-10-05)
- `SpawnProjectileBase.pierceCount` — serialized, not read
- `PlayerUserItemState` — extends `MonoBehaviour` (TD-001)
- `StatsCharacter`, `SwordAndShield`, `DualAbility`, `AnimationName.cs`, `AnimationEventManager` — unchanged since 2026-09-11
- `RoomModel.overflowPercent` — serialized, never read
- `PlayerData.Reborn()` — no caller
- `EnemySO` — not consumed by `Entity` (TD-030)
- `TalentManagger` — hardcoded stats (TD-018)
- `Assets/Script/Character/Boss/`, `Assets/Script/Handler/` — `.meta`-only orphans
- `tests/` — only `.gitkeep`; zero tests (TD-014, blocked by BUG-084). `UIFlowSmokeTest` is an Editor menu tool, not an NUnit test

---

## Undocumented systems (no GDD / no ADR)

| System | Location | Gap |
|--------|----------|-----|
| UIFlow | `Assets/Script/UIFlow/` | No GDD, no ADR; `docs/ui/ui-ux-flow.md` + beginner guide only |
| Projectile layer | `ProjectileBody.cs`, `IProjectilePayload.cs` | No ADR |
| Item / Depot | `Assets/Script/System/Item/` | No GDD, no ADR |
| Abilities v2 | `Assets/Script/System/Abilities/` | No GDD of its own |
| Pathfinding | `Assets/Script/System/Pathfinding/` | No GDD, no ADR (BUG-052) |
| Object pooling | `Assets/Script/System/PoolableService/` | No GDD |

---

## Demo fix priority

0. **BUG-095** — `[FormerlySerializedAs("attackDamege")]` on `AttackSO.attackDamage`, then confirm player melee damage in Play Mode (expect `PhysicalDamage + 55`).
1. **BUG-092 residual** — `[SerializeField]` on `perTime` and `timeCount`. Two attributes.
2. **BUG-072** — set `layerMask` on `Lightning.prefab`. One Inspector field.
3. **Play Mode smoke** — all four Paladin abilities + ranged weapon + one enemy kill, now that it compiles.
4. **BUG-066/070** — key guard in `VitalStatsBase` (audit seeding in `Reborn()` at the same time).
5. **Player death** — BUG-086 → BUG-065 → BUG-087 (`GameManager`, player `Reborn()`), then hook UIFlow `GameOverPanel` to it.
6. ~~**UIFlow Real providers**~~ — player provider done 2026-10-05 (`cb0de496`); Login / Inventory / Quest still stubs.
7. **BUG-093** — restore the ranged `RecoveryTime` gate.
8. **Bug #15** — build-safe room JSON loading before the first standalone build.
9. **BUG-084** — `.asmdef` + test location, so TD-014 can start.
