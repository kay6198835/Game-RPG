# Ability System — Diagrams

> Source: `Assets/Skill Enhance/Scripts/`
> Branch: `claude/review-skill-architecture-2df7z`
> Date: 2026-05-20

---

## 1. Kiến trúc tổng thể (Architecture Overview)

```mermaid
flowchart TD
    subgraph DESIGN["Design Time"]
        SO["AbilityDefinition (SO)\nActivationType: Active\nDefaultKey: E\nManaCost: 20 / Cooldown: 8s"]
        EFF["ShootSpiritOrbEffect (SO)\nSpeed: 10 / DmgPerTick: 25\nDuration: 5s / SummonPrefab"]
        COND["HasEnoughManaCondition"]
        SO --> COND & EFF
    end

    subgraph RUNTIME["Runtime"]
        AS["AbilitySystem\nUpdate()"]
        AI["AbilityInstance\nCooldownRemaining / Mana check"]
        CTX["AbilityContext\nOrigin / Forward / HoldRatio"]
    end

    subgraph PROJECTILE["Projectile"]
        ORB["SpiritOrbProjectile\nRigidbody2D.velocity = dir × 10\nDestroy after 8s nếu miss"]
    end

    subgraph ENEMY_DOT["DoT on Enemy"]
        DOT["SpiritDoTBehaviour\n-25 HP mỗi giây × 5 lần"]
        CHECK{"IsDead trong 5s?"}
        SUMMON["Instantiate SummonPrefab\ntại vị trí enemy"]
        EXPIRE["Destroy component\nkhông triệu hồi"]
    end

    SO -->|Equip| AI
    AS -->|Tick + GetKey E| AI
    AI -->|CanStart ✓| CTX
    CTX -->|Validate| COND -->|PASS| EFF
    EFF -->|Instantiate + Launch| ORB
    ORB -->|OnTriggerEnter2D| DOT
    DOT --> CHECK
    CHECK -->|YES| SUMMON
    CHECK -->|NO - hết 5s| EXPIRE
```

---

## 2. Luồng kích hoạt Spirit Orb (Sequence Diagram)

```mermaid
sequenceDiagram
    actor P as Player
    participant AS as AbilitySystem
    participant AI as AbilityInstance
    participant EFF as ShootSpiritOrbEffect
    participant ORB as SpiritOrbProjectile
    participant DOT as SpiritDoTBehaviour
    participant E as Enemy

    P->>AS: Nhấn E
    AS->>AI: CanStart()?
    AI-->>AS: ✓
    AS->>AI: TryActivateInstant()
    AI->>AI: BuildContext() / SpendMana(20)
    AI->>EFF: Apply(context)
    EFF->>ORB: Instantiate + Launch(dir, 10)
    AI->>AI: StartCooldown(8s)

    ORB->>E: OnTriggerEnter2D
    ORB->>DOT: AddComponent.Initialize(25, 5s)
    ORB-->>ORB: Destroy()

    loop mỗi 1 giây tối đa 5 lần
        DOT->>E: TakeDamage(25)
        alt IsDead == true
            DOT->>DOT: TrySummon()
            DOT-->>DOT: Destroy(this)
        end
    end
    Note over DOT: Hết 5s vẫn sống → Destroy, không summon
```

---

## 3. Vòng đời Ability (State Diagram)

```mermaid
stateDiagram-v2
    [*] --> Ready: Equip

    Ready --> Activating: Nhấn E\n[cooldown=0, mana≥20]
    Ready --> Ready: [cooldown>0 hoặc mana<20]
    Activating --> OnCooldown: SpendMana + StartCooldown(8s)
    OnCooldown --> Ready: Hết 8s

    Activating --> Flying: Spawn Orb
    Flying --> [*]: Miss — hết 8s
    Flying --> DoT_Active: Trúng enemy

    state DoT_Active {
        [*] --> s1: tick 1 → -25HP
        s1 --> s2: tick 2 → -25HP
        s2 --> s3: tick 3 → -25HP
        s3 --> s4: tick 4 → -25HP
        s4 --> s5: tick 5 → -25HP
        s5 --> [*]
    }

    DoT_Active --> Summon: IsDead trong 5s
    DoT_Active --> End: Hết 5s, sống
    Summon --> [*]: Instantiate entity
    End --> [*]
```

---

## 4. Class Diagram — Full System (Draw.io Compatible)

```mermaid
classDiagram
    class AbilityDefinition {
        ScriptableObject
        +string Id
        +string DisplayName
        +ActivationType ActivationType
        +KeyCode DefaultKey
        +float Cooldown
        +float ManaCost
        +float MaxHoldTime
    }

    class AbilitySystem {
        MonoBehaviour
        +Equip(slot, definition)
        +Unequip(slot)
        +GetAbility(slot)
        -HandleInput()
    }

    class AbilityInstance {
        +float CooldownRemaining
        +bool IsHolding
        +float CurrentHoldTime
        +Tick(dt)
        +CanStart()
        +TryRelease()
        +TryActivateInstant()
    }

    class AbilityContext {
        +IAbilityOwner Caster
        +Vector3 Origin
        +Vector3 Forward
        +float HoldTime
        +float HoldRatio
    }

    class AbilityConditionDefinition {
        ScriptableObject
        +IsMet(context)
    }

    class AbilityEffectDefinition {
        ScriptableObject
        +Apply(context)
    }

    class IAbilityOwner {
        interface
        +Transform Transform
        +CharacterStats Stats
        +Health Health
        +SimpleCharacterMotor Motor
    }

    class HasEnoughManaCondition {
        ScriptableObject
        +IsMet(context)
    }

    class NotDeadCondition {
        ScriptableObject
        +IsMet(context)
    }

    class ShootSpiritOrbEffect {
        ScriptableObject
        +GameObject OrbPrefab
        +float Speed
        +float DamagePerTick
        +float Duration
        +GameObject SummonPrefab
        +Apply(context)
    }

    class DamageInFrontEffect {
        ScriptableObject
        +float BaseDamage
        +float BonusDamageAtMaxHold
        +float Radius
        +float Angle
        +Apply(context)
    }

    class LungeForwardEffect {
        ScriptableObject
        +float BaseDistance
        +float BonusDistanceAtMaxHold
        +Apply(context)
    }

    class SpiritOrbProjectile {
        MonoBehaviour
        +Launch(dir, speed, lifetime, dmg, dur, summon)
        -OnTriggerEnter2D(other)
    }

    class SpiritDoTBehaviour {
        MonoBehaviour
        +Initialize(dmgPerTick, duration, summonPrefab)
        -DoTRoutine()
        -TrySummon()
    }

    class PlayerAbilityOwner {
        MonoBehaviour
        +CharacterStats Stats
        +Health Health
        +SimpleCharacterMotor Motor
    }

    class SimpleCharacterMotor {
        MonoBehaviour
        +Lunge(direction, distance, duration)
    }

    class Health {
        MonoBehaviour
        +float CurrentHealth
        +bool IsDead
        +TakeDamage(float damage)
    }

    class SimpleDamageReceiver {
        MonoBehaviour
        +ReceiveDamage(float damage)
    }

    class Damageable {
        interface
        +ReceiveDamage(float damage)
    }

    class CharacterStats {
        MonoBehaviour
        +RuntimeStat Attack
        +RuntimeStat MoveSpeed
        +RuntimeStat MaxMana
        +float CurrentMana
        +SpendMana(float)
        +RecoverMana(float)
    }

    class RuntimeStat {
        +float BaseValue
        +float Value
        +AddModifier(modifier)
        +RemoveModifiersBySource(source)
    }

    class StatModifier {
        +StatModifierType Type
        +float Value
        +object Source
    }

    AbilitySystem --> AbilityInstance
    AbilitySystem --> IAbilityOwner
    AbilitySystem --> AbilityDefinition

    AbilityInstance --> AbilityDefinition
    AbilityInstance --> AbilityContext
    AbilityInstance --> IAbilityOwner

    AbilityDefinition --> AbilityConditionDefinition
    AbilityDefinition --> AbilityEffectDefinition

    HasEnoughManaCondition --|> AbilityConditionDefinition
    NotDeadCondition --|> AbilityConditionDefinition

    ShootSpiritOrbEffect --|> AbilityEffectDefinition
    DamageInFrontEffect --|> AbilityEffectDefinition
    LungeForwardEffect --|> AbilityEffectDefinition

    ShootSpiritOrbEffect ..> SpiritOrbProjectile
    SpiritOrbProjectile ..> SpiritDoTBehaviour
    SpiritDoTBehaviour --> Health

    PlayerAbilityOwner ..|> IAbilityOwner
    PlayerAbilityOwner --> CharacterStats
    PlayerAbilityOwner --> Health
    PlayerAbilityOwner --> SimpleCharacterMotor

    SimpleDamageReceiver ..|> Damageable
    SimpleDamageReceiver --> Health

    CharacterStats --> RuntimeStat
    RuntimeStat --> StatModifier
```

