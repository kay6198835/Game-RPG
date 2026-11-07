# Enemy — AI + Spawning

> **Status:** live · **Last verified:** 2026-11-05, `sprint-21` `028fdb90` (Attack paragraph; the owner's 2026-11-04 edits landed in `6094aa10`) · **Governing ADRs:** ADR-0002 (EnemyManager singleton exception), ADR-0003 (spawn candidate pool), ADR-0005
> History: [CHANGELOG.md](CHANGELOG.md)

## Purpose

Enemy characters (state-machine AI built on the shared character base) and the room-driven spawner
that places them.

## Code

| Path | Contents |
|------|----------|
| `Assets/Script/Character/Entity/` (26 files) | `Entity`, `EntityData` (SO, `: CharacterData`), `EntityState`, `EntityStateMachine`, `Core/EntityCore`, `CoreComponent/`, `States/` |
| `Character/Entity/CoreComponent/` | `EntityInput`, `EntityFindTarget`, `EntityMovement`, `EntityWeaponHolder`, `EntityAbilityHolder`, `EntityNegativeReciver`, `EntityStatsHandler`, `EntityVitalStats`, `EntityUIController` |
| `Assets/Script/System/Enemy/` | `EnemyManager` (pathfinding service, singleton per ADR-0002), `EnemySpawner`, `EnemySO` |
| `Assets/Script/Database-SO/Modal/` | `MapModel`, `RoomModel` (+ `EnemySpawnEntry`, `EnemyModal`, `RarityTier`) |

## How it works

**AI.** `Entity : CharacterBase<EntityCore>` builds Idle / Move / Attack / TakeDamage / Death (+ an
ability state when `EntityAbilityHolder` is present). `Entity.OnEnable()` re-initialises the state
machine to `EntityIdleState` unless it is already there, so a pooled enemy restarts clean.

```
EntityBasicState — direction, take-damage, death, attack check
  EntityIdleState   — timer or target → Move
  EntityMoveState   — null-guards target; chase / flee / wander; timeout → Idle
  EntityAttackState — weapon Attack() on anim event
  EntityTakeDamageState / EntityDeathState (emits ON_ENEMY_DEATH on EndRangeTrigger)
```

**Attack.** Only through `EntityWeaponHolder` + a `Weapon` prefab named by
`CharacterData.Weapon` (inherited by `EntityData`), equipped by `EnemySpawner` → `Entity.SetCharacterData()`
→ `ApplyCharacterData()` on every spawn (`6094aa10`). No weapon, no attack. Enemy `AttackSO`s need animator
overrides built on the enemy controller and a `LayerMask` that hits the player hurtbox.
Optional: `EntityAbilityHolder` casts Abilities v2 from `EntityData.AbilityBindings` (cast range =
attack range).

**Damage received.** `EntityNegativeReciver` (via `NegativeReceiverBase`) → `DamageCalculate()`
subtracts `StatType.Defense`, clamps at 0 → `EntityVitalStats.ReceiveReduction(HP)` →
`EntityInput.OnTakeDamage()` → `EntityUIController.UpdateUIHealth()`.

**Movement.** `EntityMovement` requests A* paths from `EnemyManager.Instance`;
`SetPositionToCheck()` lerps toward the target by `Random.Range(30,75)/100f`.

**Spawning.**

```
ON_GET_SPAWN_POSITIONS (from RoomGeneraterController.LoadRoom)
  → EnemySpawner.OnGetSpawnPositions()
      → mapModel.GetRandomRoom() → roomModel.GetSpawnSet()
      → ObjectPoolManager.Spawn() per entry at a random marker
      → Emit(ON_DONE_SPAWN_ENEMY, count)
RoomCell: ON_DONE_SPAWN_ENEMY → EnemyCount; ON_ENEMY_DEATH → -- ; 0 → ON_OPEN_DOOR
```

Rooms with no spawn marker still emit `ON_GET_SPAWN_POSITIONS` (with an empty list) — `LoadRoom()` has no
zero-spawn branch (re-checked at `sprint-18` `617bb2bb`). `OnGetSpawnPositions()` returns early, so `ON_DONE_SPAWN_ENEMY` and
`ON_OPEN_DOOR` never follow and the room stays sealed (BUG-096); see [map](../map/README.md).
*Corrected 2026-10-09 — was:* "Rooms with no spawn marker never emit `ON_GET_SPAWN_POSITIONS`".

**Prefab wiring.** `Entity` + child `EntityCore` + all core components as descendants; assign an
`EntityData`. A scene `EnemyManager` is required.

## Death on a lethal hit (2026-11-04)

`NegativeReceiverBase.TakeDamage()` compares the mitigated damage with current HP before reducing it. A hit
with damage ≥ current HP calls `OnLethalHit()`, which switches the character straight to its death state
(`EntityDeathState` / `PlayerDeathState`) from any state; a character at 0 HP ignores further hits. Before,
death was noticed only by the basic state while the 0.1 s take-damage flag was up, so a lethal hit during the
take-damage animation left the character alive at 0 HP (inbox NOTE-20261101-2).

## Content — role roster

> **Design (owner, 2026-11-04):** enemies do not use abilities in the current phase. `EnemyPrefab` carries
> no `EntityAbilityHolder` and every role's `abilityBindings` is empty — by design, not a gap.

> Added 2026-11-04, working tree on `origin/feature/add-effect-component` (uncommitted, not yet run in
> Play Mode). Replaces the single Knight enemy as the dungeon's enemy set.

Every enemy is the one shared prefab `Assets/Prefab/Enemy/EnemyPrefab.prefab`; the role comes from the
`EntityData` that `EnemySpawner` passes to `Entity.SetCharacterData()`. The prefab's default `data` is
`TrashMeleeEntityData`. `Core` also carries `EffectReceiver` (`EntityEffectReceiver`); there is no
`EntityAbilityHolder` on the prefab, so no role casts abilities.

**Animator (player style).** Each role has its own full controller, read from
`CharacterData.animationController` (the old `EntityData.aima` override field is commented out;
`[FormerlySerializedAs("aima")]` carries old assets over only when they have no `animationController` key).
The attack stage swaps in `<Role>_AttackStage1.overrideController`; `EntityAttackState.Exit()` and
`EntityAbilityState.Exit()` restore `animationController`.

| Role | Stats | Weapon | Room weight / rarity |
|------|-------|--------|----------------------|
| TrashMelee | `SO/Stat/Enemy/TrashMeleeStats` | melee | 5 / Common |
| FastSwarm | `FastSwarmStats` | melee | 3 / Common |
| Tank | `TankStats` | melee | 8 / Rare |
| Assasin | `AssasinStats` | melee | 6 / Rare |
| RangedCaster | `RangedCasterStats` | ranged (`RangedCasterProjectile`, trigger `BoxCollider2D`, speed 2); FOV 7, attack range 6 | 5 / Common |
| Boss | `Boss/BossStats` | melee | 50 / Legendary — **not spawned** (see below) |
| Brute | `BruteStats` | melee, stage 20 dmg, range 1.3, recovery 2.2 s | 7 / Rare |
| Sniper | `SniperStats` | ranged, 1 shot, 48 dmg, speed 6, recovery 2.5 s; FOV 9, attack range 8 | 6 / Rare |
| Scatter | `ScatterStats` | ranged, 3 shots / 45°, 27 dmg each, speed 2.5, recovery 1.8 s; FOV 6, attack range 4.5 | 5 / Common |
| Elite | `EliteStats` (level 8) | melee, stage 15 dmg, range 1.5, recovery 1.1 s | 15 / Epic |

**Balance targets for the 2026-11-04 roles** (static math against `PaladinStats`: HP 190, Defense 20,
PhysicalDamage 30; melee damage = `PhysicalDamage + stage`, ranged = stage only; both minus target Defense):

| Role | HP / Def | Paladin hits to kill | Damage per hit to Paladin | Hits to kill Paladin | Role in a room |
|------|----------|---------------------|---------------------------|----------------------|----------------|
| Brute | 187 / 14.6 | 13 | 40 | 5 | slow heavy hitter — punish standing still, long recovery opens a window |
| Sniper | 72.5 / 5.2 | 3 | 28 | 7 | glass cannon at range — forces the player to close distance |
| Scatter | 105 / 8.5 | 5 | 7 × 3 pellets | 10 volleys (all pellets hit) | area denial at mid range — punishes walking straight in |
| Elite | 252 / 18.8 | 23 | 40 | 5 | rare mini-boss — faster cadence than Brute, longer reach |

Crit is excluded from the table (NOTE-20261104-2: a crit adds `CritDamage` ≈ 150 flat; owner put it on hold).
Brute's MoveSpeed formula constant is 2.0 (others 2.5–2.7).

**Movement and attack range come from stats (2026-11-04).** `MovementBase.MoveSpeed` reads
`StatType.MoveSpeed`; `EntityMovement` moves at it (its serialized `speed`, 0 on `EnemyPrefab`, is commented
out) and `PlayerMovement.Move()` does the same for the player. `EntityFindTarget.MinRange` / `MaxRange` read
`StatType.MinRangeAttack` / `MaxRangeAttack` (the prefab's shared `minRange` 0.5 / `maxRange` 1 are commented
out). `EntityData.movementVelocities` and `rangeCheckAttack` are commented out — neither had a reader.

| Role | MaxRangeAttack | MinRangeAttack | Why |
|------|----------------|----------------|-----|
| TrashMelee | 1.5 | 0.4 | melee hit circle reaches 2 × stage range (2.0) from the body; attack well inside it |
| FastSwarm | 1.2 | 0.2 | hugs the player |
| Tank | 1.4 | 0.3 | |
| Assasin | 1.3 | 0 | never backs off |
| Brute | 2.0 | 0.6 | stage range 1.3 (reach 2.6); keeps swing room |
| Elite | 2.3 | 0.5 | stage range 1.5 (reach 3.0) |
| Boss | 1.6 | 0.5 | |
| RangedCaster | 6.1 | 3.0 | projectile travel 8 (speed 2 × 4 s); kites inside 3 |
| Sniper | 8.0 | 4.5 | travel 18; FOV 9 |
| Scatter | 4.5 | 2.0 | travel 6.25; FOV 6 |

Melee `MaxRangeAttack` is a role constant; ranged adds DEX × 0.02. **Sight range is
`EntityData.rangeCheckFieldOfView`** (owner decision 2026-11-04): `EntityFindTarget.range` — serialized 0 on
the prefab before — now reads it, so `IsInFOV()` and `FindTargetMethod()` use one value. It must stay ≥
`MaxRangeAttack`. Per role, by temperament: TrashMelee 5, FastSwarm 6.5 (alert pack), Tank 4 and Brute 4.5
(slow to notice), Assasin 7 and Elite 7 (hunters), Boss 10 (whole arena), RangedCaster 8, Sniper 11,
Scatter 6.5 (2–3 units beyond their attack range, so they spot the player before firing). Player profiles carry the stats too
(Paladin / Base / Test 1.5 / 0, Archer 8 / 2, Mage 7 / 2.5); no player code reads them yet.
The four roles were cloned from TrashMelee (melee) / RangedCaster (ranged) with fresh GUIDs; their
stat assets keep the source role's formula set.

Asset layout, one folder per role:

```
Assets/SO/Enemy/Role/<Role>/<Role>EntityData.asset
Assets/SO/Enemy/Role/<Role>/Weapon/<Role>WeaponStats.asset, <Role>AttackStage1.asset
Assets/Prefab/Enemy/Role/<Role>/<Role>Weapon.prefab
Assets/AnimationController/Enemy/Role/<Role>/<Role>.controller, <Role>_AttackStage1.overrideController
Assets/Animation/Enemy/Role/<Role>/<State>/<Role>_<State>_dir N.anim   (72 clips per role)
Assets/SO/Database/Room/Role/RoomModel_Roles.asset, RoomModel_T<N>_<Theme>.asset (7), RoomModel_Boss.asset
```

**Spawn chain.** `LoadRandomMap` → `EnemySpawner.mapModel` = `SO/Database/Map/MapModel.asset` →
`fullRoomList` = the eight room presets below. `GetRandomRoom()` draws them as a shuffle-bag, so each
combat room gets a different preset until all eight have been used.

**Room presets** (`Assets/SO/Database/Room/Role/`). Tier = `weightBudget`. A role's rarity is set per
preset, so one role can be common in one room and rare in another. **Since 2026-11-05 every preset uses
only the five demo roles** (TrashMelee, Assasin, Tank, RangedCaster, Sniper — owner decision); FastSwarm,
Brute, Elite and Scatter were removed from all presets (their assets are kept). Enemy counts are an
estimate (budget ÷ mean entry weight), not a simulation; the 2026-11-04 simulated counts no longer apply:

| Preset | Tier / budget | Entries (weight / rarity %) | Enemies (≈) | Theme |
|--------|---------------|-----------------------------|-------------|-------|
| `RoomModel_T1_SwarmNest` | T1 / 30 | TrashMelee 5/50, Assasin 6/30 | 5 | weak melee, an occasional flanker |
| `RoomModel_T1_Outpost` | T1 / 30 | TrashMelee 5/50, RangedCaster 5/50, Sniper 6/30 | 6 | first ranged mix |
| `RoomModel_Roles` | T2 / 50 | all five demo roles | 8 | mixed |
| `RoomModel_T2_Ambush` | T2 / 50 | Assasin 6/30, TrashMelee 5/50, Sniper 6/30 | 9 | flankers covered by a sniper |
| `RoomModel_T2_FiringLine` | T2 / 50 | RangedCaster 5/50, Sniper 6/50, Tank 8/30 | 8 | ranged line behind a tank |
| `RoomModel_T3_HeavySiege` | T3 / 70 | Tank 8/50, RangedCaster 5/30, Assasin 6/30 | 11 | tanks with support fire |
| `RoomModel_T3_Warband` | T3 / 70 | TrashMelee 5/50, RangedCaster 5/50, Assasin 6/30, Tank 8/30 | 12 | broad mix |
| `RoomModel_T4_ChampionGuard` | T4 / 90 | Tank 8/30, Sniper 6/30, TrashMelee 5/50, Assasin 6/30, RangedCaster 5/30 | 15 | every role, largest room |

⚠️ The draw ignores tier: a T4 preset can be the first combat room. Ordering presets by depth needs code
in `MapModel` / `EnemySpawner` (no depth input exists today).

**Placeholders.**
- Clips carry the original entity animation events. **Sprites are temporary art** taken from character
  models, one model per role (2026-11-05, owner mapping; the demo focuses on these five roles):

  | Role | Sprite model | Source clips |
  |---|---|---|
  | TrashMelee | Paladin | Paladin clips (2026-11-04, unchanged) |
  | Assasin | Vampire | `Sprite/Vampire/Animations2D/vampire_*_dir<N>.anim` |
  | Tank | Warrok | `Sprite/Warrok/Animations2D/warrok_*` |
  | RangedCaster | Maw | `Sprite/Maw/Animations2D/maw_*` |
  | Sniper | Erika (same art as the player Archer) | `Sprite/Erika/Animations2D/erika_*` (non-arrow sheets) |

  State mapping: Idle ← idle, Walk ← move, Run ← run (Erika) or move, Attack and SpinAttack State1-3 ←
  attack, TakeDamage ← takedamage, Die ← death. Timing rules: loop clips with no events (Idle / Walk / Run)
  take the source length; Attack, SpinAttack and TakeDamage keep the role clip length (events untouched,
  source frames sampled evenly, so they play faster than the source); Die is set to min(source, 1.5 s) and
  its event times are scaled by the same factor. All four models were checked against the project direction
  convention (dir0 = down-left, clockwise; dir3 = back, dir7 = front) and match. Brute, Elite, Boss,
  FastSwarm and Scatter still use Paladin art.
- ⚠️ Vampire, Warrok and Maw sheets are 128 px cells at 100 PPU; Paladin and Erika are 256 px. Those three
  roles render at about half the Paladin size while their prefab colliders were fitted to Paladin.
- Melee stage damage is 10 for every role; RangedCaster stage damage is 35 (copied from the Archer
  stage). FOV / attack range 3 / 3 for melee roles.
- `RoomModel_Boss` is referenced by nothing: `MapModel.GetRandomRoom()` ignores room type, and the boss
  room has no spawn marker (BUG-096), so there is no correct boss spawn path yet.
- **Adding a role:** author `Assets/SO/Stat/Enemy/<Role>Stats.asset` (with `MaxRangeAttack` /
  `MinRangeAttack` formulas), add a row to `EnemyRoleScaffolder.Roles` (`Assets/Editor/`) — weight, rarity,
  sight, idle, stage damage / range / rate, and for ranged roles shot count, spread, speed, lifetime — then run
  **Tools → Enemy → Scaffold Role Assets**. It builds the controller and clips (Paladin placeholder sprites,
  entity events kept), the attack override, a weapon copied from the TrashMelee / RangedCaster template, the
  EntityData copied from the same template, and appends the role to `RoomModel_Roles` (boss: `RoomModel_Boss`).
  Themed `RoomModel_T*` presets are edited by hand. Existing roles are skipped, so the table is also the
  record of every role's current tuning — keep it in step when you tune an asset by hand.

**Knight (retired).** `SO/Enemy/Knight/Knight.asset`, `KnightWeapon.prefab`, `Knight_Controller`
and `SO/Database/Room/RoomModel.asset` stay in the project but are no longer referenced by `MapModel`
or `EnemyPrefab`. Do not build new content on them.

## Open issues

| Bug | Summary |
|-----|---------|
| BUG-066 | Unguarded `currentStats[statType]` in the shared `VitalStatsBase` (same site as BUG-070) |
| BUG-ES-2 | Two parallel spawn drivers |
| — | `RoomModel.overflowPercent` serialized, never read; `retry > 4` fallback breaks ADR-0003's budget |
| TD-030 | `EnemySO` not consumed by `Entity` |

## Related

`design/gdd/enemy-spawn-system.md`, ADR-0002, ADR-0003, `.claude/rules/ai-code.md`.
