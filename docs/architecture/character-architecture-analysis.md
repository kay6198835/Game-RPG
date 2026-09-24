# Character Architecture — Phase 1 Analysis

> **Date:** 2026-09-23 · **Branch:** `demo-architeture-1` (cut from `origin/feature/update-architecture-object` at `85bd612`)
> **Scope:** Player ↔ Entity (enemy) architecture, measured against the Map Grid/Cell system as precedent.
> **Companion documents:** [ADR-0005](adr-0005-unified-character-contract.md) (target design) ·
> [diagrams](../diagrams/character-architecture-diagrams.md) · [migration plan](character-migration-plan.md)
>
> Every finding below was read from source at `85bd612`. File:line references are to that revision.

---

## 1. Design principles of the Grid/Cell architecture

| # | Principle | Evidence in Map |
|---|---|---|
| P1 | **Two-tier interfaces**: a non-generic one to coordinate, a generic one for typed access | `IGrid` (Columns, Rows, AddCell, Setting, CaculateIndex) + `IGrid<T> : IGrid` (GetValue, SetValue, GetNext) — `Map/Interface/IGrid.cs` |
| P2 | **Abstract generic base with a type constraint** that owns all shared logic | `BaseGrid<T> : MonoBehaviour, IGrid<T> where T : MonoBehaviour, IGridItem` — `Map/BaseGrid.cs` |
| P3 | **Template method**: the base owns the flow, subclasses fill in the variable step | `BaseCell.AddCell()` → `abstract Setting()`, overridden by RoomCell / MapCell |
| P4 | **Pure data model, separate from MonoBehaviour**, shared by every grid | `Cell` is a `[Serializable]` class, not a component — `Map/Cell/Cell.cs` |
| P5 | **The coordinator only knows the interface** | `MazeController.SetCellData()` iterates `List<IGrid>` |
| P6 | **Concrete pairs are thin subclasses** | RoomGridController/RoomCell, MapGridController/MapCell |

### Weaknesses NOT to copy

| Weakness | Location | Lesson |
|---|---|---|
| `GetNext` has no bounds check (TD-027) | `BaseGrid.cs:34-41` | Every keyed/indexed read needs a guard — the same defect as BUG-066/070 |
| Base holds per-traversal state (`_current`, `_next`) | `BaseGrid.cs:11-12` | A base holds the contract, not per-call runtime state |
| `"Room_"` name hard-coded for every grid | `BaseGrid.AddCell` | A base must not know subclass specifics |
| `MazeController` keeps concrete fields beside the interface list; `Awake` misses `return` (Bug #14) | `MazeController.cs:9-10,17-20` | A coordinator holds interfaces only; no new singletons |
| `BaseCell` carries `SpriteRenderer` + `[ExecuteInEditMode]` | `BaseCell.cs` | The contract base must not drag in a presentation concern |

## 2. Mapping the principles onto Player / Entity

| Map role | Character role (target) | State at `85bd612` |
|---|---|---|
| `IGrid` (P1) | `ICharacter` — Stats, Vital, DamageReceiver, Core, Transform | `ICharacter` empty, zero implementers |
| `IGrid<T>` (P1) | `ICharacter<TCore> : ICharacter` | absent |
| `BaseGrid<T>` (P2) | `CharacterBase<TCore> : BaseEntity, ICharacter<TCore>` | `BaseEntity` only ticks the state |
| `BaseCell.Setting()` (P3) | `StatHandlerBase<TCore>.ResolveProfile()`, `VitalStatsBase<TCore>` | each pair copy-pasted |
| `Cell` (P4) | `BaseStatsSO` profile (max) + the current-value dictionary | present, but enemies share one asset (§5) |
| `MazeController` (P5) | stat-changing systems: StatsEffectBase, items, weapon equip — see `ICharacter` only | hard-wired to Player |
| concrete pairs (P6) | `Player`/`Core`/`StatHandler`… and `Entity`/`EntityCore`/`EntityStatsHandler`… | present |

## 3. Core component comparison — Player ↔ Entity

| Concern | Player | Entity | Difference |
|---|---|---|---|
| **Stats (max)** | `StatHandler : CoreComponent<Core>, IPlayerStatService`; SO lazy from `core.Player.Data.Stats` | `EntityStatsHandler : EntityCoreComponent<EntityCore>, IPlayerStatService`; SO in `Setup()` from `core.Entity.Data.StatsSO` | **Byte-identical bodies.** Both implement an interface named "Player". `AddPrimaryPoint` / `GetLevelUpStatsBonus` only mean anything for the player |
| **Vital (current)** | `VitalStatsComponent : CoreComponent<Core>, IVitalComponent` (file `VitalComponent.cs`); `IsPrimary()` guard; `*PerTimeForDuration` (BUG-071); `BuffDebuffForDuration`; **no `Reborn()`** | `EntityVitalStats : EntityCoreComponent<EntityCore>` — **no interface**; `Reborn()` from `Start` + `OnEnable`; `DebuffForDuration` (zero external callers); no `IsPrimary()` guard; no PerTime | Interface, method names, guard and reset all diverge |
| **Damage** | `NegativeReciver : INegativeReceiver` on child `Takedamage` with a BoxCollider2D, **and** `ResourceReceiver : Interact, INegativeReceiver, IResourceReceiver` with **no collider** and an identical `TakeDamage` (BUG-081) | `EntityNegativeReciver : INegativeReceiver` on child `NegativeReciver` with a BoxCollider2D; Defense in `DamageCalculate()`; health bar refresh | Player has two receivers and no Defense. `SO/Database/EnemyPrefab.prefab` carries the *player* `NegativeReciver` |
| **Resources / items** | `ResourceReceiver` (IResourceReceiver); `vitalStatsComponent` assigned only inside `ReceverModifierGroup` (BUG-080) | — | Player only |
| **Input / AI** | `PlayerInputHandler : CoreComponent<Core>, IAimProvider` | `EntityInput : EntityCoreComponent<EntityCore>, IAimProvider`, `[Inject] Construct(IPlayerService)` | Both `IAimProvider`; stay per-type (out of scope) |
| **Movement** | `PlayerMovement` — `SetVeclocity` only | `EntityMovement` — A*, `EnemyManager.Instance` | Different in kind; stay per-type |
| **Weapon** | `WeaponHolder : Interact`; `Attack(Core.Player)` | `EntityWeaponHolder`, `EntityWeapon` (Abilities v1) | Stay per-type; only the equip modifier path must go through an interface |
| **Ability** | `AbilityHolder : CoreComponent<Core>, IAbilityOwner` | — | Enemies do not use v2 (out of scope) |
| **Hub** | `Core : CoreBase`, `Player { get; }` | `EntityCore : CoreBase`, `Entity { get; }` | Differ only in the typed owner |
| **Owner** | `Player : BaseEntity`, `private Core core` | `Entity : BaseEntity`, `protected EntityCore core` | Same serialized field name `core` |

### Known asymmetries — confirmed

- ✅ Both stat handlers implement `IPlayerStatService` (`StatHandler.cs:4`, `EntityStatsHandler.cs:4`).
- ✅ `EntityVitalStats` does not implement `IVitalComponent`; `DebuffForDuration` vs `BuffDebuffForDuration`.
- ✅ `AbilityContext` / `IAbilityServices` are player-bound: `Stats` is typed `IPlayerStatService`, and
  `Stats`/`Vital`/`ResourceReceiver` are the **caster's**, so every `StatsEffectBase` and
  `BuffDebuffStatsForDuration` can only affect the caster. The only "target" is
  `Services.NegativeReceiver`, a slot shared by every ability of one owner.
- ✅ `ICharacter` is empty (`Character/Base/Interface/ICharacter.cs`) with zero implementers.

### Prefab layout (parsed from YAML) — decides the lookup path

| Prefab | GameObject | Components |
|---|---|---|
| `Prefab/Player/PlayerTest.prefab` | root `PlayerTest` | Player, Rigidbody2D, BoxCollider2D, Animator |
| | `Takedamage` | **NegativeReciver + BoxCollider2D** (hurtbox) |
| | `ResourceReceiver` | ResourceReceiver (no collider) |
| | `Core`, `StatHandler`, `VitalHandler`, `AbilityHolder`… | one component per child |
| `Prefab/Enemy/EnemyPrefab.prefab` | root | Entity, Rigidbody2D, Animator (no collider) |
| | `NegativeReciver` | **EntityNegativeReciver + BoxCollider2D** (hurtbox) |
| | `WeaponHolder` | EntityWeaponHolder, EntityAttack, BoxCollider2D (attack box) |
| `SO/Database/EnemyPrefab.prefab` | `NegativeReciver` | **player** NegativeReciver — unreferenced by any asset/scene |

⇒ A hurtbox is always a **child** of its owner, and a character also has non-hurtbox colliders
(attack box, root body). The correct lookup from a hit collider is: "is this a hurtbox (carries an
`INegativeReceiver`)? then `GetComponentInParent<ICharacter>()`".

## 4. Call sites depending on a concrete type where an interface suffices

| File:line | Uses | Should use |
|---|---|---|
| `Weapons/Weapon.cs:87,100` | `weaponHolder.Core.Player.Data.Stats` — bypasses StatHandler entirely | `ICharacter.Stats` + `StatModifierGroup.Apply/Remmove` |
| `System/Item/ItemController.cs:34` | cast `(ResourceReceiver)interactor` | `interactor.Core.Character` |
| `ItemEffectDefinitionSO.cs:8` + Recovery/StatModifier/Currency | `Apply(ResourceReceiver)` | `Apply(ICharacter)` |
| `AbilityHolder.cs:45-47` | `Core.GetComponentInChildren<IPlayerStatService/IResourceReceiver/IVitalComponent>` | `Core.Character` |
| `AbilityContext.cs:20-47` | caster's `IPlayerStatService Stats`, `IVitalComponent Vital` | `ctx.Caster.Character`, `ctx.Target` |
| `StatEffectBase.cs:24`, `RecoveryReduction*.cs`, `BuffDebuffStatsForDuration.cs:11` | `context.Services.Vital` (always the caster) | the recipient's `ICharacter.Vital` |
| `AbilityDefinition.cs:64`, `AbilityEffectDefinition.cs:29` | `Services.Vital` for affordability | `ctx.CasterCharacter.Vital` |
| `SpawnProjectileBase.cs:31`, `LightningController.cs:27` | `TryGetComponent<INegativeReceiver>` | `TryGetCharacter` → `ctx.Target` |
| `bullet.cs:54`, `Projectile.cs:46`, `Spell.cs:10` | `GetComponentInChildren<INegativeReceiver>` (looks *down*) | inconsistent with the rest; left for a later pass |
| `MeleeWeapon.cs:31`, `EntityAttack.cs:31` | `TryGetComponent<INegativeReceiver>` | correct today (hurtbox carries receiver); optional later move |
| `EntityEffectStats.cs:20` (Abilities v1) | `Core.Entity.Data.StatsSO` — writes the **shared** asset | v1 maintenance path; out of scope, noted as residual |
| `GameLifetimeScope.cs:15`, `StatsUIController`, `StatsScreenUIController`, `StatPointAllocator` | `IPlayerStatService` | **keep** — these genuinely want the player's service |

**Legitimate and left alone:** state classes holding concrete types (`PlayerBasicState` →
`VitalStatsComponent`, `EntityBasicState` → `EntityVitalStats`) because state machines stay per-type;
siblings inside one character resolving each other through `GetCoreComponent`.

## 5. New finding with design impact

🔴 **Every enemy of one type shares one `BaseStatsSO` asset.** `EntityStatsHandler.Setup()` assigns
`core.Entity.Data.StatsSO` directly; nothing clones it (`Instantiate`/`CreateInstance` of a stat SO: 0 hits).

Once stat effects can target enemies:
- a debuff applied to one bat applies to **every live bat**;
- in the Editor the modifier leaks into the `.asset` (the BUG-063 class of defect);
- a pooled enemy disabled by `SetActive(false)` stops its coroutines, so `ApplyDebuffForDuration`
  never removes the modifier — the enemy re-spawns still debuffed.

Latent at `85bd612` (nothing modifies enemy stats); made live by the target design, so it is fixed
in the same migration (ADR-0005 §8).
