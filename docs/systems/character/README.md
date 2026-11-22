# Character Core — Shared Base + Player

> **Status:** live · **Last verified:** 2026-11-22, `sprint-23` `1e5afb3c` (receiver interface, player spawn, player states, death flow and open issues only; vitals section not re-checked) · **Governing ADR:** ADR-0005 (Amendments 1-3)
> History: [CHANGELOG.md](CHANGELOG.md)

## Purpose

The component-hub + state-machine framework that both the player and enemies are built on, and
the player-specific states, input and vitals on top of it.

## Code

| Path | Contents |
|------|----------|
| `Assets/Script/Character/Base/` (20 files) | `BaseEntity`, `CharacterBase<TCore>`, `CharacterData` (SO), `CoreBase`, `CoreComponentBase<T>`, `StateMachine<TState>` (abstract, `[Serializable]`), `IState`, `StatusAnimation`, `StatusBase`, `DirectionResolver`, `CharacterInputBase`, `MovementBase`, `StatHandlerBase`, `VitalStatsBase`, `NegativeReceiverBase`, `WeaponHolderBase`, `AbilityHolderBase`, `Interface/{ICharacter, ICore, ICoreComponent}` |
| `Assets/Script/Character/Player/` (35 files) | `Player`, `PlayerState`, `PlayerStateMachine`, `PlayerData`, `Core/`, `CoreComponent/`, `States/`, `Input/`, `Animation/`, `Projectile/` |
| `Assets/Script/Interface/` | `ICharacterInput`, `IMovement`, `IStatService`, `IVitalComponent`, `IWeaponHolder`, `INegativeReceiver` and `INegativeReceiver<TCore>` (both in file `INegativeReciver.cs`), `IAimProvider` |

## How it works

**Lifecycle.** `BaseEntity` ticks `CurrentState.LogicUpdate()` in `Update()` and
`PhysicsUpdate()` in `FixedUpdate()`. `CharacterBase<TCore> : BaseEntity, ICharacter` adds the
`Core` and `Transform` accessors.

**Component hub.** `CoreBase.Setup()` pulls every `ICoreComponent` under it with
`GetComponentsInChildren` in `Awake()`; siblings resolve each other with
`Core.GetCoreComponent<T>(out var c)` or `Core.TryGetCapability<T>`. `GetCoreComponent` returns
`null` silently on a miss.

**Shared bases (ADR-0005).** Each concrete player/enemy component is a thin subclass:

| Base | Player subclass | Enemy subclass |
|------|-----------------|----------------|
| `StatHandlerBase<T> : IStatService` (max values, over `BaseStatsSO`) | `StatHandler` (`IPlayerStatService`) | `EntityStatsHandler` |
| `VitalStatsBase<T> : IVitalComponent` (current values, `Dictionary<StatType,float>`, `Reborn()`) | `VitalStatsComponent` (file `VitalComponent.cs`) | `EntityVitalStats` |
| `NegativeReceiverBase<T> : INegativeReceiver<T>` (`InvicisibleHurtbox(bool)`, `[RequireComponent(BoxCollider2D)]`) — ⚠️ since `8005d27e` it no longer implements `INegativeReceiver`, so no hit lookup finds it (**BUG-135**) | `NegativeReciver` | `EntityNegativeReciver` |
| `WeaponHolderBase<T> : IWeaponHolder` | `WeaponHolder` | `EntityWeaponHolder` |
| `AbilityHolderBase<T> : IAbilityOwner` | `AbilityHolder` | `EntityAbilityHolder` |
| `MovementBase<T> : IMovement` (velocity, speed multipliers, locks, knockback) | `PlayerMovement` | `EntityMovement` |
| `CharacterInputBase<T>` | `PlayerInputHandler` (file `PlayerInputHandle.cs`) | `EntityInput` |

`VitalStatsBase` also mirrors current HP into a serialized debug field `_currentHP` after every
change (`UpdateStatField()`).

**Data.** `CharacterData` (SO) holds `Stats` (`BaseStatsSO`, the authored asset — never written;
`CreateRuntimeStats()` returns a per-character clone used by `StatHandlerBase`), `AbilityBindings`,
`animationController` (`RuntimeAnimatorController`, `[FormerlySerializedAs("aima")]`) and `Weapon`
(a `Weapon` prefab component). `PlayerData` / `EntityData` derive from it.
`CharacterBase.ApplyCharacterData()` applies it: stats → weapon → abilities → vitals → effects
(`6094aa10`). *Was:* `DefaultWeapon` (`WeaponSO`).

**Player spawn.** The player is **not placed in the scene**. `PlayerManager` (see
[dependency-injection](../dependency-injection/README.md)) spawns it with
`IObjectPoolService.Spawn(playerPrefab, spawnPoint)`; the pool injects the instance with
`resolver.InjectGameObject()` (`Pool.cs:31`). *Was:* `resolver.Instantiate(...)` (changed in `be973605`).

**Player states.** `Player.Awake()` builds ten states (`PlayerDashState` added `91c42d79`, 2026-11-16):

```
PlayerBasicState → PlayerIdleState, PlayerMoveState, PlayerDashState
PlayerUseWeaponState → PlayerAttackState, PlayerSkillWeaponState, PlayerEquidUnequid,
                       PlayerIntertorState (`AnimationOnAction()` → `Interactor.Intertion()`, since `221d54be`), PlayerResourceReceiverState
PlayerDisadvantageState → PlayerTakeDamageState, PlayerDeathState
```

Animation events call `AnimationStart` / `AnimationTrigger` / `AnimationOnAction` /
`AnimationOffAction` / `AnimationFinishTrigger` / `AnimationEnd` on `Player`, each setting
`CurrentState.SetAnimationStatus(StatusAnimation.X)`; states branch on `Status`.

**Death (since `4097be4c`, 2026-11-05).** `NegativeReceiverBase.TakeDamage()` calls
`ICharacterInput.OnDeath()` on a lethal hit (else `OnTakeDamage()`); `CharacterInputBase.IsDeath` stays set
until `ReloadComponent()`. `PlayerBasicState`, `PlayerUseWeaponState` and `PlayerDisadvantageState` read it
first and change to `PlayerDeathState` (the enemy states do the same — see [enemy](../enemy/README.md)).
`PlayerDeathState.AnimationOnAction()` emits `ON_PLAYER_DEATH` (`49c58e35`, 2026-11-19); BUG-122 stays PARTIAL
(the state re-enters itself every frame by reading, and the Archer / Mage Die clips carry no event).
`CharacterInputBase.OnDeath()` now returns early when `isDeath` is already set.
`CharacterBase.Reborn()` calls `core.ReloadCore()`, which also reloads the input component.

**Input** (`PlayerInput.inputactions`):

| Action | Binding |
|--------|---------|
| Move | WASD |
| Attack | LMB |
| Block | RMB — handler **commented out** in `PlayerInputHandler` |
| Primary / Secondary / Utility / Ultimate ability | `1` / `2` / `3` / `4` → `AbilityHolder.TryDoAbility(slot)` |
| Equip / Interact / Dash | F / G / Space |

## Contracts and rules

- New behaviour = a new state class, never inline `if/else` in `Update`.
- `Status` is durable: the state that acts on a value writes a new one to consume it.
- `ICharacter` carries `Transform` and, since `221d54be` (on `sprint-18` 2026-10-10), `void SetCharacterData(CharacterData)`
  (abstract on `CharacterBase`; `Player` / `Entity` cast to `PlayerData` / `EntityData` unchecked — NOTE-20261009-6, Needs owner).
  ⚠️ ADR-0005 Amendment 1 still says "`Transform` only"; the ADR has not been amended. Never add component interfaces to it.
- Damage enters only through `INegativeReceiver.TakeDamage(float, Vector2)` (intended contract; broken by BUG-135).

## Open issues

| Bug | Summary |
|-----|---------|
| BUG-135 | S1 — `NegativeReceiverBase` implements only `INegativeReceiver<TCore>`; no weapon / projectile / ability can damage anyone |
| BUG-122 | S1 PARTIAL — death event now on `AnimationOnAction`; death state re-enters every frame, Archer / Mage Die clips have no event |
| BUG-130 | S2 — dash soft lock: class animator controllers have no Dash state |
| BUG-065 | `PlayerDeathState.Enter()` does not stop movement |
| BUG-086 | `ON_PLAYER_DEATH` fires more than once (status per its file: Open) |
| BUG-087 | PARTIAL — no `GameManager`, no player `Reborn()` caller |
| BUG-104 | `PlayerState.Enter()` logs every state change, unguarded (was cited here as BUG-094 — BUG-101 ID collision) |
| BUG-133 | S4 — hit knockback pushes the victim toward the attacker |

BUG-066 / BUG-070 (key guards) and BUG-071 (HoT / DoT stop) are FIXED in code per their files (2026-11-20 triage).
| Bug #6 | `PlayerData.currentHealth` never written; `PlayerData.Reborn()` has no caller |

## Related

`docs/architecture/adr-0005-unified-character-contract.md`, `design/gdd/character-system.md`,
`.claude/rules/gameplay-code.md`.
