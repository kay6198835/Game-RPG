# Enemy — AI + Spawning

> **Status:** live · **Last verified:** 2026-10-10, `sprint-18` `617bb2bb` · **Governing ADRs:** ADR-0002 (EnemyManager singleton exception), ADR-0003 (spawn candidate pool), ADR-0005
> History: [CHANGELOG.md](CHANGELOG.md)

## Purpose

Enemy characters (state-machine AI built on the shared character base) and the room-driven spawner
that places them.

## Code

| Path | Contents |
|------|----------|
| `Assets/Script/Character/Entity/` (26 files) | `Entity`, `EntityData` (SO, `: CharacterData`), `EntityState`, `EntityStateMachine`, `Core/EntityCore`, `CoreComponent/`, `States/` |
| `Character/Entity/CoreComponent/` | `EntityInput`, `EntityFindTarget`, `EntityMovement`, `EntityWeaponHolder`, `EntityWeapon`, `EntityAbilityHolder`, `EntityNegativeReciver`, `EntityStatsHandler`, `EntityVitalStats`, `EntityUIController`, `EntityEffectStats` |
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
`EntityData.DefaultWeapon` / `WeaponSO`. No weapon, no attack. Enemy `AttackSO`s need animator
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

## Open issues

| Bug | Summary |
|-----|---------|
| BUG-066 | Unguarded `currentStats[statType]` in the shared `VitalStatsBase` (same site as BUG-070) |
| BUG-ES-2 | Two parallel spawn drivers |
| — | `RoomModel.overflowPercent` serialized, never read; `retry > 4` fallback breaks ADR-0003's budget |
| TD-030 | `EnemySO` not consumed by `Entity` |
| — | `EntityWeapon` still references Abilities v1 (`ActivateSkill currentAbilitySO`) |

## Related

`design/gdd/enemy-spawn-system.md`, ADR-0002, ADR-0003, `.claude/rules/ai-code.md`.
