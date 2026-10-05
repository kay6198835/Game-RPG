# Character Core — Shared Base + Player

> **Status:** live · **Last verified:** 2026-10-05, HEAD `93ba6d8e` · **Governing ADR:** ADR-0005 (Amendments 1-3)
> History: [CHANGELOG.md](CHANGELOG.md)

## Purpose

The component-hub + state-machine framework that both the player and enemies are built on, and
the player-specific states, input and vitals on top of it.

## Code

| Path | Contents |
|------|----------|
| `Assets/Script/Character/Base/` (20 files) | `BaseEntity`, `CharacterBase<TCore>`, `CharacterData` (SO), `CoreBase`, `CoreComponentBase<T>`, `StateMachine<TState>` (abstract, `[Serializable]`), `IState`, `StatusAnimation`, `StatusBase`, `DirectionResolver`, `CharacterInputBase`, `MovementBase`, `StatHandlerBase`, `VitalStatsBase`, `NegativeReceiverBase`, `WeaponHolderBase`, `AbilityHolderBase`, `Interface/{ICharacter, ICore, ICoreComponent}` |
| `Assets/Script/Character/Player/` (35 files) | `Player`, `PlayerState`, `PlayerStateMachine`, `PlayerData`, `Core/`, `CoreComponent/`, `States/`, `Input/`, `Animation/`, `Projectile/` |
| `Assets/Script/Interface/` | `ICharacterInput`, `IMovement`, `IStatService`, `IVitalComponent`, `IWeaponHolder`, `INegativeReceiver` (file `INegativeReciver.cs`), `IAimProvider` |

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
| `NegativeReceiverBase<T> : INegativeReceiver` — the **only** implementer project-wide | `NegativeReciver` | `EntityNegativeReciver` |
| `WeaponHolderBase<T> : IWeaponHolder` | `WeaponHolder` | `EntityWeaponHolder` |
| `AbilityHolderBase<T> : IAbilityOwner` | `AbilityHolder` | `EntityAbilityHolder` |
| `MovementBase<T> : IMovement` (velocity, speed multipliers, locks, knockback) | `PlayerMovement` | `EntityMovement` |
| `CharacterInputBase<T>` | `PlayerInputHandler` (file `PlayerInputHandle.cs`) | `EntityInput` |

`VitalStatsBase` also mirrors current HP into a serialized debug field `_currentHP` after every
change (`UpdateStatField()`).

**Data.** `CharacterData` (SO) holds `Stats` (`BaseStatsSO`), `AbilityBindings`, `DefaultWeapon`
(`WeaponSO`). `PlayerData` / `EntityData` derive from it.

**Player spawn.** The player is **not placed in the scene**. `PlayerManager` (see
[dependency-injection](../dependency-injection/README.md)) spawns it with
`resolver.Instantiate(playerPrefab, spawnPoint)`, which injects the whole hierarchy before `Awake()`.

**Player states.** `Player.Awake()` builds nine states:

```
PlayerBasicState → PlayerIdleState, PlayerMoveState
PlayerUseWeaponState → PlayerAttackState, PlayerSkillWeaponState, PlayerEquidUnequid,
                       PlayerIntertorState, PlayerResourceReceiverState
PlayerDisadvantageState → PlayerTakeDamageState, PlayerDeathState
```

Animation events call `AnimationStart` / `AnimationTrigger` / `AnimationOnAction` /
`AnimationOffAction` / `AnimationFinishTrigger` / `AnimationEnd` on `Player`, each setting
`CurrentState.SetAnimationStatus(StatusAnimation.X)`; states branch on `Status`.

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
- `ICharacter` carries `Transform` only — never add component interfaces to it.
- Damage enters only through `INegativeReceiver.TakeDamage(float, Vector2)`.

## Open issues

| Bug | Summary |
|-----|---------|
| BUG-066 / BUG-070 | `VitalStatsBase` indexes `currentStats[statType]` with no key guard (`:35,47,49,53,61,63,67`) |
| BUG-065 | `PlayerDeathState.Enter()` does not stop movement |
| BUG-086 | `PlayerDeathState.LogicUpdate()` emits `ON_PLAYER_DEATH` every frame |
| BUG-087 | PARTIAL — no `GameManager`, no player `Reborn()` caller |
| BUG-094 | `PlayerState.Enter()` logs every state change, unguarded |
| BUG-071 | HoT/DoT tick chain in `VitalStatsBase` cannot be stopped |
| Bug #6 | `PlayerData.currentHealth` never written; `PlayerData.Reborn()` has no caller |

## Related

`docs/architecture/adr-0005-unified-character-contract.md`, `design/gdd/character-system.md`,
`.claude/rules/gameplay-code.md`.
