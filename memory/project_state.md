# Project State

Updated **2026-11-05** (doc-truth pass `/doc-sync --auto` at `origin/sprint-21` `028fdb90`).
Bug statuses below follow `production/qa/bugs/BUG-*.md`, which are the source of truth (unchanged
since the 2026-10-31 triage — no wrap-up has run since). Claim sweep:
`production/qa/doc-truth-2026-11-05.md`. Previous update: **2026-10-31** (`sprint-20` `e8baf251`).
This pass added the "2026-10-31 → 2026-11-05" table and build status; the sections below it are the
2026-10-31 snapshot plus that delta.

Snapshot of actual code state. Long form: `CLAUDE.md`. Per-system current docs and change logs:
`docs/systems/<system>/README.md` + `CHANGELOG.md`.

---

## Build status

✅ **Editor: COMPILED** — the 2026-11-05 standup batchmode run on the committed `ea00b3a4`
(`028fdb90` adds only `.md`). **Player build: not run.**
`CharacterData.cs` no longer imports `UnityEditor.Animations` (`6094aa10`), so BUG-116 looks fixed in
code; the bug file is still Open. A pre-push static gate now exists for pushes to `main`
(`3e3f6f10`, `c7fbd69e`, `1da76e9a`).

---

## Structural changes 2026-10-31 → 2026-11-05

| # | Change | Commit(s) | Impact |
|---|--------|-----------|--------|
| R34 | **Abilities v1 deleted** (`System/Skill_Ability/` + SO assets); `WeaponSO` → `System/Item/`; `EntityWeapon`, `EntityEffectStats`, `Spell`, `IEffectable` deleted | `6094aa10` | v2 is the only ability path; checklist 18 resolved in code, no ADR |
| R35 | `EventID` 24 → **25** (`ON_ROOM_CLEARED`, emitted by `RoomCell`, 0 subscribers) | `6094aa10` | — |
| R36 | `CharacterBase.ApplyCharacterData()`; `Entity.SetCharacterData()` called by `EnemySpawner` on every spawn (`EnemySpawnEntry.Data`) | `6094aa10` | BUG-106 enemy half, BUG-107 item 2 look fixed |
| R37 | `StatHandlerBase` on a per-character runtime clone (`CharacterData.CreateRuntimeStats()`); `IStatService<TCore>`; vitals clamp on a lowered max | `6094aa10` | BUG-115 runtime path looks fixed |
| R38 | `CharacterData.animationController` → `RuntimeAnimatorController` + `[FormerlySerializedAs("aima")]`; `EntityData.aima` / `movementVelocities` / `rangeCheckAttack` removed; `StatType.MaxRangeAttack` 112 / `MinRangeAttack` 113; move speed from `StatType.MoveSpeed` | `6094aa10` | BUG-116 looks fixed; BUG-108 |
| R39 | `AutoOpenRoomTypes` (Shop / Rest / Treasure) open doors on load; Buff room authored as `TreasureRoom` | `6094aa10` | BUG-096: only Boss left |
| R40 | `[FormerlySerializedAs("authoredModifiers")]` on `StatModifierGroup.statsModifiers` | `1370136c` | BUG-121 half fixed |
| R41 | `FastTest/QAProbe/` automated player-flow probe | `79ad79b2` | Test tooling only |

Triage hints for Saturday: inbox NOTE-20261105-1 (standup) and NOTE-20261105-2 (this pass).

---

## Structural changes since the last update (2026-10-10 → 2026-10-31)

| # | Change | Commit(s) | Impact |
|---|--------|-----------|--------|
| R24 | `IObjecPoolService` → **`IObjectPoolService`** (file name kept) | `cae75317` | ADR-0004 body not amended |
| R25 | `LightningController` → **`DamageArea`**, `RuneCircleController` → **`CircleSummon`**; Rain Arrows (`DamagesInManyPos`, `RainArrowsProjectile`) | `cae75317`, `5c8245c9` | BUG-072 now on `DamageArea`; BUG-109 |
| R26 | `StartGameSystem/` → **`RoomGameSystem/`**: `PedestalChampionController` (was `ChampitionController`), `UpgradeController`, empty `ShopController` | `041292ce` and earlier | BUG-096, BUG-102 |
| R27 | **`EffectReceiverBase`** + `PlayerEffectReceiver` / `EntityEffectReceiver` (`IEffectReceiver`): timed / per-tick stat effects moved out of `VitalStatsBase`; `SourceEffect` pooled visual | `786c031b`, `6e484f0c` | BUG-071, BUG-100, BUG-117, BUG-118 |
| R28 | `VitalStatsBase` keyed by `VitalStatType`; `IVitalComponent` = current values only; `CharacterBase.Reborn()` → `core.ReloadCore()`; `ON_PLAYER_READY` from `VitalStatsComponent.ReloadComponent()` | `786c031b`, `6e484f0c` | BUG-066/070 shape changed, BUG-114, BUG-119 |
| R29 | `RegenComponent` (`IRegenResource`), `WalletComponent` (`IWallet`), `VitalModifier` | `3d768b3d` | BUG-111, BUG-117 |
| R30 | Item effects: `StatModifierEffect` (abstract), `ModifierMaxStatsEffect`, `ModifierCurrentStatsEffect`, `CoinModifierEffect`; old three deleted | `3d768b3d` | BUG-120; NOTE-20261031-10 |
| R31 | `StatPointSession` extracted; stats panel in the GameplayUI Character tab (`StatsPanel`, `StatRowView`); `StatsUIController` unregistered | `1bc5d2ad`, `24f72474`, `da8275fc` | — |
| R32 | `StatModifierGroup.authoredModifiers` → `statsModifiers` (no `[FormerlySerializedAs]`) | `343a3830` | BUG-121 |
| R33 | `FastTest/CombatTest/` scene builder, `TrainingDummy`, `AttackFlowDebugger` | `87e053b4`, `fc98fa2a` | Test tooling only |

Earlier structural changes (R5–R23) are recorded in `CLAUDE.md` history entries.

---

## Open bugs (bug-file status, 2026-10-31)

| # | Sev | Description | Location |
|---|-----|-------------|----------|
| BUG-116 | S1 | Runtime `CharacterData` imports `UnityEditor.Animations` — Player build fails | `CharacterData.cs:2,20` |
| BUG-108 | S1 | `CharacterData` serialized-key loss on enemy data (stats key restored in `fc98fa2a`; `weaponSO` still unread — NOTE-20261031-7) | `CharacterData.cs` |
| BUG-096 | S1 | Marker-less rooms never open; start room only via the Champion | `RoomGeneraterController.cs` |
| BUG-087 | S1 | PARTIAL: one UIFlow `ON_PLAYER_DEATH` subscriber; no `GameManager` | `PlayerDeathState.cs` |
| BUG-121 | S2 | `authoredModifiers` → `statsModifiers` rename loses authored modifiers | `StatModifierGroup.cs:23-24` |
| BUG-120 | S2 | Item stat effects apply nothing | `StatModifierEffect.cs` |
| BUG-117 | S2 | `VitalModifier` not serializable, sign inverted, reduction applied twice | `VitalModifier.cs`, `EffectReceiverBase.cs:44-52` |
| BUG-113 / 114 / 115 | S2 | Stat "max" has two meanings; vitals snapshot; player mutates the shared stat asset | `BaseStatsSO.cs`, `VitalStatsBase.cs` |
| BUG-110 | S2 | Weapon drop / pickup bookkeeping removed | `WeaponHolderBase.cs` |
| BUG-106 / 107 | S2 | Weapon + abilities only applied by `Player.SetCharacterData()`; 107 PARTIAL | `AbilityHolderBase.cs:77`, `WeaponHolderBase.cs` |
| BUG-109 | S2 | Rain Arrows reads `_context` before `Launch()`; destroys a pooled object | `DamagesInManyPos.cs` |
| BUG-105 | S2 | `attackDamege` → `attackDamage` without `[FormerlySerializedAs]` | `AttackSO.cs:10` |
| BUG-100 | S2 | Timed buffs share one modifier source (now `context.Caster`) | `EffectReceiverBase.cs:86-91` |
| BUG-097, 095, 072, 066/070, 086, 084 | S2 | Unchanged — see `CLAUDE.md` | — |
| BUG-118, 119, 111, 112 | S3 | HoT/DoT throws; heal not clamped; regen breaks once wired; NPC prefabs missing scripts | see `CLAUDE.md` |
| BUG-103, 102, 092, 071, 065, 099, 101, 068, 073, 079, 083, 090 | S3 | Unchanged / see row notes in `CLAUDE.md` | — |
| BUG-104, 098 | S4 | Unchanged | — |
| BUG-063 | — | ACCEPTED (deferred to demo prep) | `Stat.cs` |

---

## EventID enum (current on `sprint-20` — 24 values, unchanged)

`ON_PLAYER_ON_DOOR`, `ON_PLAYER_DEATH`, `ON_REALOAD_GAME`, `ON_LOAD_MAZE_DONE`, `ON_LOAD_MAP`,
`ON_OPEN_DOOR`, `ON_GET_SPAWN_POSITIONS`, `ON_DONE_SPAWN_ENEMY`, `ON_SPAWN_EXTRA_ENEMY`,
`ON_TEST`, `ON_ENEMY_DEATH`, `ON_ROOM_CLEAR`, `ON_OPEN_STATS_PLAYER_UI`,
`ON_CLOSE_STATS_PLAYER_UI`, `ON_INCREASE_STATS_BY_UI`, `ON_DECREASE_STATS_BY_UI`,
`ON_CHANGE_STATS_BY_UI_RUN_TIME`, `ON_UPDATE_STATS_BY_UI`, `ON_REVERT_STATS_BY_UI`,
`ON_RESTORE_STATS_BY_UI`, `ON_RESET_STATS_UI_SESSION`, `ON_DROP_ITEM`, `ON_COLLECT_ITEM`,
`ON_PLAYER_READY`

Still missing: `ON_PLAYER_TAKE_DAMAGE`. `ON_ROOM_CLEAR` has no producer.

---

## Key API changes to be aware of

| Contract | Was | Is now |
|---|---|---|
| Pool service | `IObjecPoolService` | `IObjectPoolService` (file `IObjecPoolService.cs`) |
| Summon runtime | `LightningController`, `RuneCircleController` | `DamageArea`, `CircleSummon` |
| Champion | `StartGameSystem/ChampitionController` | `RoomGameSystem/PedestalChampionController` |
| Stat effects | `IVitalComponent.BuffDebuffForDuration()` etc. | `IEffectReceiver` (`EffectReceiverBase`) |
| Current values | `IVitalComponent` keyed by `StatType` | keyed by `VitalStatType`; missing key → `0` |
| Character reset | `VitalStatsComponent.Reborn()` | `ICharacter.Reborn()` → `CoreBase.ReloadCore()` → `ReloadComponent()` |
| Modifier list field | `StatModifierGroup.authoredModifiers` | `statsModifiers` + `vitalModifiers` (BUG-121, BUG-117) |
| Item effects | `Recovery` / `StatModifier` / `CurrencyEffectDefinition` | `StatModifierEffect` family, `CoinModifierEffect` |
| Room-clear event | `ON_CLEAR_ENEMY` | `ON_OPEN_DOOR` (`221d54be`) |
| `AttackSO` damage field | `attackDamege` | `attackDamage` — old assets keep the old key (BUG-105) |

---

## Stubs / unimplemented

- `EffectReceiverBase.AdjustCurrentPerTimeForDuration()` — `throw new NotImplementedException()` (BUG-118)
- `ShopController` — empty; `UpgradeController` — stub
- `RegenComponent.Regenning()` — no caller (BUG-111)
- `CoinModifierEffect`, `ModifierMaxStatsEffect`, `ModifierCurrentStatsEffect` — `abstract`, cannot be authored
- `UIFlow/Gameplay/Popup/PopupPanel.cs` — empty `: UIPanel`
- UIFlow `RealLoginService`, `RealInventoryProvider`, `RealQuestProvider` — stubs
- `SpawnProjectileBase.pierceCount`, `RoomModel.overflowPercent` — serialized, never read
- `PlayerUserItemState` — extends `MonoBehaviour` (TD-001); `TalentManagger` hardcoded (TD-018)
- `tests/` — only `.gitkeep` (TD-014, blocked by BUG-084)

---

## Undocumented systems (no GDD / no ADR)

| System | Location | Gap |
|--------|----------|-----|
| Effect receiver / source effect | `Character/Base/EffectReceiverBase.cs`, `Runtime/ImpactMono/` | Plan doc only (`CHANGELOG-DOCS` 2026-10-27); no ADR |
| Regen / Wallet | `RegenComponent.cs`, `WalletComponent.cs` | Wallet in the item GDD direction; no ADR |
| Room game system (Champion / Upgrade / Shop) | `Assets/Script/System/RoomGameSystem/` | No GDD |
| UIFlow | `Assets/Script/UIFlow/` | No GDD, no ADR |
| Projectile layer | `ProjectileBody.cs`, `IProjectilePayload.cs` | No ADR |
| Abilities v2 | `Assets/Script/System/Abilities/` | No GDD of its own |
| Pathfinding, Object pooling | `System/Pathfinding/`, `System/PoolableService/` | No GDD (BUG-052) |

---

## Demo fix priority

1. **BUG-116** — runtime `CharacterData` must not import `UnityEditor` (blocks every Player build).
2. **BUG-121 / BUG-105 / BUG-108 residual** — restore the lost serialized keys; decide a guard for the rename pattern.
3. **BUG-096** — zero-spawn branch in `LoadRoom()` for Boss / Buff / Rest / Shop.
4. **BUG-106 / BUG-110 / BUG-107** — weapon + ability setup outside `SetCharacterData()`; weapon swap bookkeeping.
5. **Stat-reduction epic** — BUG-115 → BUG-113 → BUG-114, with BUG-117 / BUG-119.
6. **BUG-120** and NOTE-20261031-10 — make item effects concrete.
7. **Play Mode smoke** (CombatTest scene now available) — Champion confirm, abilities, ranged weapon, one kill, one room transition.
8. **BUG-066/070**, then death path BUG-086 → BUG-065 → BUG-087.
9. **Bug #15** before the first standalone build; **BUG-084** before TD-014.
