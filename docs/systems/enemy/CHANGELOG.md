# Enemy — Changelog

Newest first. Entries before 2026-10-05 reconstructed from `CLAUDE.md` history, `docs/CHANGELOG-DOCS.md` and `git log`.

## 2026-11-05 — Role clips use per-role sprite models
- **Commit:** none yet (working tree on `feature/new-asset-enemy`)
- **Changed:** 288 clips under `Assets/Animation/Enemy/Role/{Assasin,Tank,RangedCaster,Sniper}/` — sprite keys, `m_SampleRate` (30), `m_StopTime`; Die event times scaled. Events otherwise unchanged (128 per role before and after).
- **From → To:** Paladin placeholder sprites → Vampire (Assasin), Warrok (Tank), Maw (RangedCaster), Erika (Sniper). TrashMelee keeps Paladin. Idle / Walk / Run take the source length; Attack / SpinAttack / TakeDamage keep the role length; Die = min(source, 1.5 s).
- **Why:** owner decision — the demo is built around these five roles; art is temporary.
- **Open:** Vampire / Warrok / Maw sheets are 128 px vs Paladin 256 px, so those roles render at about half size against Paladin-sized colliders. Not checked in Play Mode.

## 2026-11-05 — Room presets limited to the five demo roles
- **Commit:** none yet (working tree on `feature/new-asset-enemy`)
- **Changed:** the eight presets in `Assets/SO/Database/Room/Role/` (all of `MapModel.fullRoomList`) — `enemiesOfRoom` rewritten; budgets unchanged; `RoomModel_Boss` untouched.
- **From → To:** nine roles across the presets → TrashMelee, Assasin, Tank, RangedCaster, Sniper only. FastSwarm, Brute, Elite, Scatter entries removed (assets kept). Each removed entry was replaced by the closest demo role (Scatter → RangedCaster, Brute → Tank or Assasin, FastSwarm → TrashMelee or Assasin, Elite dropped).
- **Why:** owner decision — the demo is built around these five roles.
- **Open:** Tank (Defense 43.4) is now in five presets; with sword stage damage lost to BUG-105 the Paladin may deal 0 damage to it. Damage calculation is pending (owner).

## 2026-11-05 — The 2026-11-04 working-tree entries are committed (doc-truth re-sync)
- **Commit:** `6094aa10` "fix bug, add asset room, enemy, data" (2026-11-04), merged into `sprint-21` by `ea00b3a4`. Every 2026-11-04 entry below that says "none yet (working tree)" landed in this commit.
- **Changed (README):** Attack paragraph — `EntityData.DefaultWeapon` / `WeaponSO` → `CharacterData.Weapon`, equipped through `Entity.SetCharacterData()` on every spawn.
- **Why:** not recorded in the commit.
- **Bugs:** BUG-106 (enemy half looks fixed in code), BUG-108; NOTE-20261101-2 / NOTE-20261104-1/-2/-5/-6 (see NOTE-20261105-1).

## 2026-11-04 — Lethal hit goes straight to death
- **Commit:** none yet (working tree)
- **Changed:** `NegativeReceiverBase.cs` (lethal check + abstract `OnLethalHit()`), `EntityNegativeReciver.cs`,
  `NegativeReciver.cs` (player)
- **From → To:** death detected by the basic state only while the 0.1 s take-damage flag was up → a hit with
  damage ≥ current HP changes to the death state immediately; hits on a 0 HP character are ignored
- **Why:** owner decision; QA saw an enemy stay alive at 0 HP (NOTE-20261101-2).
- **Also recorded:** enemies use no abilities in this phase (owner design); damage calculation
  (NOTE-20261104-3/-4) and crit (NOTE-20261104-2) pending.
- **Verified:** Assembly-CSharp compiles (Roslyn, outside the Editor). Not Play Mode.

## 2026-11-04 — EnemyRoleScaffolder updated
- **Commit:** none yet (working tree)
- **Changed:** `Assets/Editor/EnemyRoleScaffolder.cs`
- **From → To:**
  - templates `Knight.asset` / `KnightWeapon` / `ArcherWeapon` / `Arrow.prefab` → the TrashMelee (melee) and
    RangedCaster (ranged) role assets
  - one shared weapon tuning → per-role row: weight, rarity, sight, idle, stage damage / range / rate, shots,
    spread, projectile speed and lifetime (all ten roles listed with their current values)
  - blank clips → Paladin placeholder sprites when key times match
  - room models created once → new roles appended to `RoomModel_Roles` / `RoomModel_Boss`
  - warns when a stat profile has no `MaxRangeAttack` / `MinRangeAttack` formula
- **Verified:** Assembly-CSharp-Editor compiles (Roslyn, outside the Editor). Not run.

## 2026-11-04 — Sight from rangeCheckFieldOfView; health bar on stat change; v1 removed
- **Commit:** none yet (working tree)
- **Changed:** `EntityFindTarget.cs` (`range` → `EntityData.rangeCheckFieldOfView`, superseding the
  MaxRangeAttack entry below); ten role `EntityData` sight values; `EntityUIController.cs` (subscribes to
  `IVitalComponent.CurrentStatsChanged` and the HP max-stat change), `EntityNegativeReciver.cs` (push call
  commented out); `EntityEffectStats` deleted and its `EffectStat` GameObject removed from both `EnemyPrefab`s;
  `EntityWeapon.cs` deleted (abstract, unused, v1-typed)
- **Why:** owner request.
- **Verified:** Assembly-CSharp compiles (Roslyn, outside the Editor); no asset references a deleted GUID. Not Play Mode.

## 2026-11-04 — Sight range from MaxRangeAttack; temporary Paladin sprites
- **Commit:** none yet (working tree)
- **Changed:** `EntityFindTarget.cs` (`range` field → `MaxRange` property); all 720 clips under
  `Assets/Animation/Enemy/Role/` (sprite keys copied from the matching Paladin clips)
- **From → To:** sight `range` serialized 0 (NOTE-20261104-6) → `MaxRangeAttack` stat; role clips with null
  sprite keys → Paladin sprites, same key times, entity events kept
- **Why:** owner decision; sprites are a stand-in so enemies are visible in testing.
- **Verified:** Assembly-CSharp compiles (Roslyn, outside the Editor); 0 null sprite keys, every sprite GUID
  resolves, Attack clips keep 5 events, Death keeps `AnimationOnAction`. Not Play Mode.

## 2026-11-04 — Move speed and attack range become stats
- **Commit:** none yet (working tree on `origin/feature/add-effect-component`)
- **Changed:** `StatType.cs` (+`MaxRangeAttack` 112, `MinRangeAttack` 113), `GameConstants.StatTypeName`,
  `MovementBase.cs`, `EntityMovement.cs`, `PlayerMovement.cs`, `PlayerMoveState.cs`, `EntityFindTarget.cs`,
  `EntityData.cs`; all 15 `BaseStatsSO` assets under `Assets/SO/Stat/` gain the two stats and their formulas
- **From → To:**
  - enemy speed `EntityMovement.speed` (serialized, 0 on `EnemyPrefab`) → `StatType.MoveSpeed`;
    player speed computed in `PlayerMoveState` → `PlayerMovement.Move()` reading the same stat
  - attack range `EntityFindTarget.minRange` / `maxRange` (one value for every role) → per-role stats
  - `EntityData.movementVelocities`, `rangeCheckAttack` (no readers) → commented out
- **Why:** owner request; closes inbox NOTE-20261104-1 in code (pending triage).
- **Verified:** Assembly-CSharp compiled with Unity's Roslyn outside the Editor, 0 errors. **Not** Play Mode.

## 2026-11-04 — Seven room presets in four tiers
- **Commit:** none yet (working tree on `origin/feature/add-effect-component`)
- **Changed:** new `SO/Database/Room/Role/RoomModel_{T1_SwarmNest,T1_Outpost,T2_Ambush,T2_FiringLine,T3_HeavySiege,T3_Warband,T4_ChampionGuard}.asset`;
  `MapModel.asset` lists them after `RoomModel_Roles`
- **From → To:** one room preset (all roles, budget 50) → eight presets, budgets 30 / 50 / 70 / 90, each with
  its own role subset and per-role rarity
- **Why:** owner request — more varied enemy mixes and tiers.
- **Not verified:** Play Mode. Counts in the README are a simulation of `GetSpawnSet()`, not observed spawns.
  The draw does not follow tier order.

## 2026-11-04 — Four more roles: Brute, Sniper, Scatter, Elite
- **Commit:** none yet (working tree on `origin/feature/add-effect-component`)
- **Changed:** new `SO/Stat/Enemy/{Brute,Elite,Sniper,Scatter}Stats.asset` and per-role folders under
  `Animation/`, `AnimationController/`, `SO/Enemy/`, `Prefab/Enemy/` `Role/<Role>/`; `RoomModel_Roles.asset`
  gains four entries; `EnemyRoleScaffolder.Roles` lists them
- **From → To:** five spawnable roles → nine. Each new role has its own stat profile, weapon stage, and
  EntityData FOV / attack range; balance targets in the README
- **Why:** owner request — more enemy variety with balanced data for the demo.
- **Not verified:** Play Mode. Clips have no sprites. Inbox notes NOTE-20261104-1…4 filed from the balance pass.

## 2026-11-04 — Role roster replaces Knight; enemy animator moves to CharacterData
- **Commit:** none yet (working tree on `origin/feature/add-effect-component`)
- **Changed:** `CharacterData.cs`, `EntityData.cs`, `Entity.cs`, `EntityAttackState.cs`, `EntityAbilityState.cs`,
  `EntityEffectReceiver.cs`; `EnemyPrefab.prefab`, `MapModel.asset`, `Knight.asset`; new assets under
  `*/Enemy/Role/` and `SO/Database/Room/Role/`; new editor tool `Assets/Editor/EnemyRoleScaffolder.cs`
- **From → To:**
  - `EntityData.aima` (override of `Enemy.controller`) → `CharacterData.animationController`, one full
    controller per role, as the player classes do. `aima` is commented out, not deleted
  - `EntityEffectReceiver : MovementBase<EntityCore>` (copy-paste from `786c031b`) → `: EffectReceiverBase<EntityCore>`;
    added to `EnemyPrefab` > `Core` > `EffectReceiver`
  - `MapModel.fullRoomList` = [`RoomModel` (Knight)] → [`RoomModel_Roles`] (TrashMelee, FastSwarm, Tank,
    Assasin, RangedCaster)
  - `EnemyPrefab.data` default `Knight` → `TrashMeleeEntityData`
  - `Knight.asset.animationController` `null` → `Knight_Controller` (the asset already had a null
    `animationController` key, so `FormerlySerializedAs("aima")` did not apply); Knight is otherwise retired, files kept
- **Why:** owner decision — the six stat profiles are the enemy set; Knight is no longer used.
- **Not verified:** Play Mode. Clips have no sprites; Boss has no spawn path (BUG-096).
- **Bugs:** none filed.

## 2026-10-10 — Room-clear event renamed ON_OPEN_DOOR
- **Commits:** `221d54be` "logic open door at start room", merged into `sprint-18` by `42a81260` "update flow" (`c6cbc57a`)
- **Changed:** `RoomCell.cs:60` (emitter), README flow
- **From → To:** `0 alive → ON_CLEAR_ENEMY` → `0 alive → ON_OPEN_DOOR`. Spawn flow unchanged.
- **Why:** not recorded in the commit.
- **Bugs:** BUG-096

## 2026-10-09 — Correction: marker-less rooms do emit ON_GET_SPAWN_POSITIONS
- **Commit:** branch `pm/doc-sync-2026-10-09` (base `sprint-17` `cdf68555`)
- **Changed:** README only. No enemy code changed.
- **From → To:** "Rooms with no spawn marker never emit `ON_GET_SPAWN_POSITIONS`" → they do emit it, with an empty list. `EnemySpawner.OnGetSpawnPositions()` returns early, so the room never clears.
- **Why:** `ac13ee4f` reverted the zero-spawn branch in `LoadRoom()` (`40d2c793`), and the 2026-10-05 doc sync did not notice (BUG-099).
- **Bugs:** BUG-096, BUG-099

## 2026-09-28 … 10-05 — Pool-safe restart, serialized state machine
- **Commits:** `5b035b73`, `b7a0af5e`, `0bc36406`
- **Changed:** `Entity`, `EntityStateMachine`, `EntityVitalStats`
- **From → To:**
  - `stateMachine = new EntityStateMachine()` field initialiser → serialized field, `[Serializable]` class
  - No re-init on re-enable → `Entity.OnEnable()` re-initialises to `EntityIdleState` unless already there
- **Why:** pooled enemies came back in their last state (inferred from the code; commit message "range weapon").
- **Bugs:** none.

## 2026-09-25 — No weapon, no attack (ADR-0005 Amendment 3)
- **Commit:** `f7d98b19`, `b0b13379`
- **Changed:** `EntityAttack.cs` deleted; `EntityMovement.SetPositionToCheck()` lerp range
- **From → To:**
  - `EntityAttack.Attack()` hardcoding `TakeDamage(10, …)` → attack only via `EntityWeaponHolder` + `Weapon` prefab
  - `Random.Range(10,100)/100` (integer division, always 0) → `Random.Range(30,75)/100f`
- **Why:** one attack path for player and enemy; remove hardcoded damage.
- **Bugs:** closes BUG-043, BUG-088.

## 2026-09-24 / 09-25 — Shared bases; enemies can cast abilities
- **Commits:** `8c3c350a`, `59d871a9`, `83954bc6`, `4e05da46`, `e6d9603a`, `38c4b620`, `05e88133`
- **Changed:** enemy components rebuilt on `CharacterBase` / `VitalStatsBase` / `NegativeReceiverBase` / `WeaponHolderBase` / `AbilityHolderBase` / `MovementBase` / `CharacterInputBase`; `EntityData : CharacterData`; `EntityAbilityHolder` added
- **From → To:** enemy-only implementations → thin subclasses of shared bases
- **Why:** ADR-0005 — one contract for player and enemy.
- **Bugs:** BUG-066 merged with BUG-070 (same lines).

## 2026-09-06 — Enemy damage chain fixed
- **Commit:** `f3f5f08`
- **Changed:** `EntityNegativeReciver` rewritten
- **From → To:** player-only logic (PlayerInputHandler, ON_PLAYER_DEATH) on an enemy → Defense-aware `DamageCalculate()` → `EntityVitalStats` → `EntityUIController`
- **Why:** Sprint 12 entity/stat refactor.
- **Bugs:** closes BUG-053, BUG-042, NEW-2.
