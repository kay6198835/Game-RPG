# Character Architecture — Diagrams

Companion to [ADR-0005](../architecture/adr-0005-unified-character-contract.md). Before = `85bd612`;
after = branch `demo-architeture-1`.

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

## 2. Class diagram — after

```mermaid
classDiagram
    class ICharacter { <<interface>> +Transform +Core +Stats +Vital +DamageReceiver }
    class ICharacterT["ICharacter~TCore~"] { <<interface>> +Core: TCore }
    class ICore { <<interface>> +GetCoreComponent~T~() +TryGetCapability~T~() +Character }
    class BaseEntity { <<abstract>> }
    class CharacterBase["CharacterBase~TCore~"] { <<abstract>> #core: TCore }
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
    class NegativeReciver
    class EntityNegativeReciver
    class CharacterLookup { <<static>> +TryGetCharacter(Component) }

    ICharacter <|-- ICharacterT
    BaseEntity <|-- CharacterBase
    ICharacterT <|.. CharacterBase
    CharacterBase <|-- Player
    CharacterBase <|-- Entity
    ICore <|.. CoreBase
    CoreBase <|-- Core
    CoreBase <|-- EntityCore
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
    CharacterLookup ..> ICharacter
```

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
      IC[ICharacter] --> ICT["ICharacter&lt;TCore&gt;"] --> CB["CharacterBase&lt;TCore&gt;"] --> PE[Player / Entity]
      SB["StatHandlerBase.ResolveProfile()"] --> SH[StatHandler / EntityStatsHandler]
      SO[BaseStatsSO + current values] -.-> SB
      SYS[StatsEffect / Item / Weapon] -->|ICharacter only| IC
    end
```

## 4. Flow — stat effect on a target (player or enemy, same path)

```mermaid
sequenceDiagram
    participant AH as AbilityHolder (IAbilityOwner)
    participant AI as AbilityInstance
    participant FX as SpawnProjectileEffect
    participant SM as SpawnProjectileBase
    participant LK as CharacterLookup
    participant SE as SubEffect: StatsEffectBase (recipient=Target)
    participant T as ICharacter
    AH->>AI: TryDoAbility → CanStart → TryStart (CasterCharacter.Vital)
    AI->>FX: Apply(ctx) [Do]
    FX->>SM: Pool.Spawn + Launch(lifetime, ctx, Execute)
    SM->>LK: OnTriggerEnter2D(other) → TryGetCharacter
    LK-->>SM: target or null
    SM->>SM: ctx.Target = target (set)
    SM->>FX: Execute(ctx) (then invoke)
    FX->>T: Target.DamageReceiver.TakeDamage(baseDamage, Origin)
    FX->>SE: ApplyOnHitEffects → Apply(ctx)
    SE->>T: ResolveRecipient(Target).Vital.Reduction / Recovery
    Note over SE,T: no "is Player" / "is Entity" — IVitalComponent only
```

## 5. Flow — damage (signature unchanged)

```mermaid
sequenceDiagram
    participant W as Projectile / Lightning
    participant LK as CharacterLookup
    participant C as ICharacter
    participant R as INegativeReceiver (one per character)
    participant V as IVitalComponent
    W->>LK: hit.TryGetCharacter(out c)
    Note right of LK: hurtbox only (carries INegativeReceiver)
    LK-->>W: c
    W->>C: c.DamageReceiver
    W->>R: TakeDamage(amount, position)
    R->>V: Reduction(HP, final)  (enemy subtracts Defense first)
```

## 6. Flow — item pickup

```mermaid
sequenceDiagram
    participant PS as PlayerResourceReceiverState
    participant RR as ResourceReceiver (Interact)
    participant IC as ItemController
    participant E as ItemEffectDefinition
    participant C as ICharacter
    PS->>RR: Intertion()
    RR->>IC: Interact(this)
    IC->>IC: target = interactor.Core.Character
    IC->>E: Apply(target)
    E->>C: Vital.Recovery(HP) / Vital.ApplyBuffDebuff(group)
    IC->>IC: Emit(ON_COLLECT_ITEM)
```

## 7. Flow — weapon equip

```mermaid
sequenceDiagram
    participant WH as WeaponHolder
    participant W as Weapon
    participant C as ICharacter
    participant S as IStatService
    WH->>W: Equid(holder)
    W->>C: holder.Core.Character
    W->>S: StatModifiers.Apply(Stats.AddModifiersFromSource, this)
    Note over W,S: UnEquid → StatModifiers.Remmove(Stats.RemoveModifiersFromSource, this)
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
    Note over EN,SH: Stats / Vital / DamageReceiver resolved through core.TryGetCapability — never DI
```
