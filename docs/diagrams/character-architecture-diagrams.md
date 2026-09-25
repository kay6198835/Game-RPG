# Character Architecture — Diagrams

Companion to [ADR-0005](../architecture/adr-0005-unified-character-contract.md). Before = `85bd612`;
after = branch `demo-architeture-1`, including ADR-0005 Amendment 1 (2026-09-24).

## 1. Class diagram — before

```mermaid
classDiagram
    class BaseEntity { <<abstract>> }
    class ICharacter { <<interface>> }
    class ICore { <<interface>> +GetCoreComponent~T~() }
    class CoreBase { <<abstract>> }
    class Core { +Player Player }
    class EntityCore { +Entity Entity }
    class Player
    class Entity
    class IPlayerStatService { <<interface>> }
    class IVitalComponent { <<interface>> }
    class INegativeReceiver { <<interface>> }
    class IResourceReceiver { <<interface>> }
    class StatHandler
    class EntityStatsHandler
    class VitalStatsComponent
    class EntityVitalStats
    class NegativeReciver
    class ResourceReceiver
    class EntityNegativeReciver
    class AbilityServices { +Stats +Vital +NegativeReceiver }

    BaseEntity <|-- Player
    BaseEntity <|-- Entity
    ICore <|.. CoreBase
    CoreBase <|-- Core
    CoreBase <|-- EntityCore
    Player o-- Core
    Entity o-- EntityCore
    IPlayerStatService <|.. StatHandler
    IPlayerStatService <|.. EntityStatsHandler : misnamed
    IVitalComponent <|.. VitalStatsComponent
    EntityVitalStats ..> IVitalComponent : does NOT implement
    INegativeReceiver <|.. NegativeReciver
    INegativeReceiver <|.. ResourceReceiver : duplicate BUG-081
    IResourceReceiver <|.. ResourceReceiver
    INegativeReceiver <|.. EntityNegativeReciver
    AbilityServices --> StatHandler : caster only
    AbilityServices --> VitalStatsComponent : caster only
    note for ICharacter "empty, zero implementers"
```

## 2. Class diagram — after (ADR-0005 + Amendment 1)

`ICharacter` is identity only. The shared contract is the set of component interfaces both sides
implement; systems reach them with the `GetComponent` family.

```mermaid
classDiagram
    class ICharacter { <<interface>> +Transform }
    class ICore { <<interface>> +GetCoreComponent~T~() +TryGetCapability~T~() }
    class BaseEntity { <<abstract>> }
    class CharacterBase["CharacterBase~TCore~"] { <<abstract>> #core: TCore +Core: TCore }
    class Player
    class Entity
    class CoreBase { <<abstract>> }
    class Core
    class EntityCore
    class IStatService { <<interface>> }
    class IPlayerStatService { <<interface>> +AddPrimaryPoint() +GetLevelUpStatsBonus() }
    class IVitalComponent { <<interface>> +Reborn() }
    class INegativeReceiver { <<interface>> }
    class StatHandlerBase["StatHandlerBase~TCore~"] { <<abstract>> #ResolveProfile() }
    class VitalStatsBase["VitalStatsBase~TCore~"] { <<abstract>> }
    class StatHandler
    class EntityStatsHandler { -runtimeClone }
    class VitalStatsComponent
    class EntityVitalStats
    class NegativeReciver { hurtbox collider }
    class EntityNegativeReciver { hurtbox collider }

    BaseEntity <|-- CharacterBase
    ICharacter <|.. CharacterBase
    CharacterBase <|-- Player
    CharacterBase <|-- Entity
    ICore <|.. CoreBase
    CoreBase <|-- Core
    CoreBase <|-- EntityCore
    Player o-- Core
    Entity o-- EntityCore
    IStatService <|-- IPlayerStatService
    IStatService <|.. StatHandlerBase
    StatHandlerBase <|-- StatHandler
    StatHandlerBase <|-- EntityStatsHandler
    IPlayerStatService <|.. StatHandler
    IVitalComponent <|.. VitalStatsBase
    VitalStatsBase <|-- VitalStatsComponent
    VitalStatsBase <|-- EntityVitalStats
    INegativeReceiver <|.. NegativeReciver
    INegativeReceiver <|.. EntityNegativeReciver
```

### Lookup rule

| Context | Lookup |
|---|---|
| Collider hit carries the capability (hurtbox) | `hit.TryGetComponent(out INegativeReceiver r)` |
| From a child of the character (holder, interactor, hurtbox) to the root | `GetComponentInParent<ICharacter>()` |
| From the root to a capability with no collider | `character.Transform.GetComponentInChildren<IVitalComponent / IStatService>()` |
| Sibling inside the same character | `Core.GetCoreComponent<T>` / `Core.TryGetCapability<T>` |

## 3. Correspondence with the Map precedent

```mermaid
flowchart LR
    subgraph Map
      IG[IGrid] --> IGT["IGrid&lt;T&gt;"] --> BG["BaseGrid&lt;T&gt;"] --> RG[RoomGridController / MapGridController]
      BC["BaseCell.Setting()"] --> RC[RoomCell / MapCell]
      C[Cell data] -.-> BC
      MC[MazeController] -->|List of IGrid| IG
    end
    subgraph Character
      IC[ICharacter identity] --> CB["CharacterBase&lt;TCore&gt;"] --> PE[Player / Entity]
      SB["StatHandlerBase.ResolveProfile()"] --> SH[StatHandler / EntityStatsHandler]
      SO[BaseStatsSO + current values] -.-> SB
      SYS[StatsEffect / Item / Weapon] -->|GetComponent family| IFC[IVitalComponent / IStatService / INegativeReceiver]
      PE -.implements via components.-> IFC
    end
```

## 4. Flow — stat effect on a target (player or enemy, same path)

```mermaid
sequenceDiagram
    participant AI as AbilityInstance
    participant FX as SpawnProjectileEffect
    participant SM as SpawnProjectileBase
    participant CX as AbilityContext
    participant SE as SubEffect: StatsEffectBase (recipient=Target)
    participant V as IVitalComponent (hit character)
    AI->>FX: Apply(ctx) [Do]
    FX->>SM: Pool.Spawn + Launch(lifetime, ctx, Execute)
    SM->>SM: OnTriggerEnter2D(other): other.TryGetComponent(out INegativeReceiver _)
    SM->>CX: ctx.Target = other (hurtbox) or null  (set)
    SM->>FX: Execute(ctx)  (then invoke)
    FX->>FX: ctx.Target.TryGetComponent(out INegativeReceiver r) → r.TakeDamage
    FX->>SE: ApplyOnHitEffects → Apply(ctx)
    SE->>CX: TryGetRecipientComponent<IVitalComponent>(Target)
    CX->>CX: Target.GetComponentInParent<ICharacter>() → .Transform.GetComponentInChildren<IVitalComponent>()
    CX-->>SE: vital
    SE->>V: Reduction / Recovery
    Note over SE,V: no "is Player" / "is Entity" — IVitalComponent only
```

## 5. Flow — damage (signature unchanged)

```mermaid
sequenceDiagram
    participant W as MeleeWeapon / Projectile / Lightning
    participant H as Hurtbox collider
    participant R as INegativeReceiver (same GameObject)
    participant V as IVitalComponent
    W->>H: OverlapCircleNonAlloc / OnTriggerEnter2D
    W->>H: TryGetComponent(out INegativeReceiver r)
    H-->>W: r (only if this collider is a hurtbox)
    W->>R: TakeDamage(amount, position)
    R->>V: Reduction(HP, final)  (enemy subtracts Defense, refreshes its health bar)
```

## 6. Flow — item pickup

```mermaid
sequenceDiagram
    participant PS as PlayerResourceReceiverState
    participant RR as ResourceReceiver (Interact, range check)
    participant IC as ItemController
    participant E as ItemEffectDefinition
    participant V as IVitalComponent
    PS->>RR: Intertion()
    RR->>IC: Interact(this)
    IC->>IC: target = interactor.GetComponentInParent<ICharacter>()
    IC->>E: Apply(target)
    E->>E: target.Transform.GetComponentInChildren<IVitalComponent>()
    E->>V: Recovery(HP) / ApplyBuffDebuff(group)
    IC->>IC: Emit(ON_COLLECT_ITEM)
```

## 7. Flow — weapon equip

```mermaid
sequenceDiagram
    participant WH as WeaponHolder
    participant W as Weapon
    participant S as IStatService
    WH->>W: Equid(holder)
    W->>W: holder.GetComponentInParent<ICharacter>().Transform.GetComponentInChildren<IStatService>()
    W->>S: StatModifiers.Apply(AddModifiersFromSource, this)
    Note over W,S: UnEquid → StatModifiers.Remmove(RemoveModifiersFromSource, this)
```

## 8. Flow — DI and pooled enemy spawn

```mermaid
sequenceDiagram
    participant LS as GameLifetimeScope
    participant ES as EnemySpawner
    participant P as Pool
    participant EN as Entity
    participant VS as EntityVitalStats
    participant SH as EntityStatsHandler
    LS->>ES: inject IObjecPoolService (scene service)
    ES->>P: Spawn(prefab)
    alt first spawn
        P->>EN: Instantiate + InjectGameObject (cross-system services only)
    else reuse
        P->>EN: SetActive(true)
    end
    EN->>VS: OnEnable → Reborn()
    VS->>SH: ResetRuntimeModifiers() (lazy clone of EntityData.StatsSO on first use)
    VS->>VS: current = max
    Note over VS,SH: siblings resolve each other through the core hub — never DI
```

## 9. Component bases (ADR-0005 Amendment 2)

Every pair derives from `CoreComponentBase<TCore>`; the base holds the shared logic, the subclass only
the side-specific part.

```mermaid
classDiagram
    class CoreComponentBase["CoreComponentBase~TCore~"] { <<abstract>> }
    class DamageReceiverBase["DamageReceiverBase~TCore~"] { <<abstract>> +TakeDamage() #Mitigate() #OnDamaged() }
    class CharacterInputBase["CharacterInputBase~TCore~"] { <<abstract>> +AimDirection +AimPoint +OnTakeDamage() }
    class MovementBase["MovementBase~TCore~"] { <<abstract>> +SetVelocity() +ApplyKnockback() +AddSpeedMultiplier() +Lock() }
    class WeaponHolderBase["WeaponHolderBase~TCore~"] { <<abstract>> +Attack() +MakeDamage() +EndDamage() }
    class AbilityHolderBase["AbilityHolderBase~TCore~"] { <<abstract>> +TryDoAbility() +TryDoAnyAbility() +HandleInput() #ResolveBindings() }
    class INegativeReceiver { <<interface>> }
    class ICharacterInput { <<interface>> }
    class IMovement { <<interface>> }
    class IWeaponHolder { <<interface>> }
    class IAbilityOwner { <<interface>> }
    class Weapon { <<abstract>> +OnAttackEnter(IWeaponHolder) }

    CoreComponentBase <|-- DamageReceiverBase
    CoreComponentBase <|-- CharacterInputBase
    CoreComponentBase <|-- MovementBase
    CoreComponentBase <|-- WeaponHolderBase
    CoreComponentBase <|-- AbilityHolderBase
    INegativeReceiver <|.. DamageReceiverBase
    ICharacterInput <|.. CharacterInputBase
    IMovement <|.. MovementBase
    IWeaponHolder <|.. WeaponHolderBase
    IAbilityOwner <|.. AbilityHolderBase
    DamageReceiverBase <|-- NegativeReciver
    DamageReceiverBase <|-- EntityNegativeReciver
    CharacterInputBase <|-- PlayerInputHandler
    CharacterInputBase <|-- EntityInput
    MovementBase <|-- PlayerMovement
    MovementBase <|-- EntityMovement
    WeaponHolderBase <|-- WeaponHolder
    WeaponHolderBase <|-- EntityWeaponHolder
    AbilityHolderBase <|-- AbilityHolder
    AbilityHolderBase <|-- EntityAbilityHolder
    Weapon --> IWeaponHolder : equipped to
```

## 10. Flow — enemy casts an ability

```mermaid
sequenceDiagram
    participant BS as EntityBasicState
    participant AH as EntityAbilityHolder
    participant AS as EntityAbilityState
    participant AI as AbilityInstance
    BS->>AH: Processing() (cooldowns)
    BS->>AH: target locked && in attack range → TryDoAnyAbility()
    AH-->>BS: true (first ready slot; animator override applied)
    BS->>AS: ChangeState(AbilityState)
    AS->>AH: StartHold()
    loop LogicUpdate
        AS->>AH: HandleInput() — Start→Cast (pay cost)
        AS->>AH: Cast: dispatch once, hold MaxHoldTime, CancelHold → Do
        AS->>AI: Do: Execute effects, start cooldown → Exit
    end
    AS->>AS: Exit → IdleState, restore EntityData.Aima
```
