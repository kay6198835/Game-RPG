# Core Component Access — Public vs Internal Interfaces (Plan)

> 📜 Change log: [changelog/core-component-access-plan.CHANGELOG.md](changelog/core-component-access-plan.CHANGELOG.md)

> **Status:** PLAN — approved by the owner on 2026-10-27, **not implemented**. No `.cs` file has been
> changed. Code facts below were read at HEAD `05dcf63` (branch `origin/feature/ui-flow-maingameplay`).
> Extends [ADR-0005](adr-0005-unified-character-contract.md); an Amendment is part of this plan.

## 1. Context

The core components of one character call each other through public interfaces (`IVitalComponent`,
`ICharacterInput`, `IMovement`…), and systems outside the character (items, abilities, weapons, UI)
use the very same interfaces. Two consequences:

- A member meant only for the character's own components sits on the same surface as the members
  meant for the outside world.
- Nothing stops an Enemy component from depending on a Player-internal member by mistake.

## 2. Decision — the `ICharacter` / `ICharacter<TCore>` pattern, applied per component interface

| Interface | Role | Existing example |
|---|---|---|
| Non-generic | **external access** — anything outside the hub | `ICharacter` |
| Generic `<out TCore> where TCore : ICore`, derives from the non-generic one | **internal access** — siblings of the same core | `ICharacter<TCore>` |

The component marker pair already follows this shape: `ICoreComponent` (non-generic) and
`ICoreComponent<out TCore>` (generic). No new marker is introduced.

**Applied selectively — not every interface gets a pair:**

| Kind of interface | Shape |
|---|---|
| Needs both external and internal access | `IX` (unchanged, no marker added) + `IX<out TCore> : IX, ICoreComponent<TCore>` |
| External only (`IStatService`, `IWallet`, `INegativeReceiver`, `IWeaponHolder`, `IAbilityOwner`…) | Unchanged |
| Internal only (`IRegenResource`) | Generic only: `IX<out TCore> : ICoreComponent<TCore>` |

**Principles agreed with the owner**

- Internal interfaces stay `public`. A component is an outside class and C# interface members are
  public, so the goal is **isolation by core type** plus **a smaller external surface**, not hiding.
- No runtime guard in the hub, no `.asmdef` (BUG-084 stays a separate item).
- `out TCore` matches `ICoreComponent<out T>`, so `CoreBase.Setup()` can still cast every component to
  `ICoreComponent<ICore>`. Caveat: with `out`, `TCore` may appear only in output positions; an internal
  member that must take a `TCore` parameter needs its interface declared without `out`.

## 3. Evidence (HEAD `05dcf63`)

Members whose only callers are inside the character — the pilot:

| Member | Only caller(s) |
|---|---|
| `IVitalComponent.Reborn()` | the class itself (`VitalComponent.cs:6`, `EntityVitalStats.cs:22`) |
| `ICharacterInput.OnTakeDamage()` | `NegativeReceiverBase.cs:26` (sibling) |
| `IMovement.ClearImpacts()` | `EntityVitalStats.cs:21` (sibling) |
| `IRegenResource` (whole interface) | `RegenComponent` (dormant — on no prefab or scene) |

Stay public because outside code uses them today: `Recovery` / `Reduction` (ability effects),
`ApplyBuffDebuff` (items), `BuffDebuffForDuration` (abilities), `AddModifiersFromSource` (`Weapon`),
`IWallet.AddCoins` (items), `GetCurrentStatValue` (UI). `IMovement.SetVelocity` / `Stop` stay public by
owner decision.

`ICharacter.Core` (non-generic) has exactly one external caller: `UpgradeController.cs:10`.

## 4. Worked example — the Player's `VitalStatsComponent`

```csharp
// External access — unchanged except that Reborn moves out
public interface IVitalComponent
{
    event System.Action CurrentStatsChanged;
    float GetCurrentStatValue(StatType statType);
    void Recovery(StatType statType, float amount);
    void Reduction(StatType statType, float amount);
    // ApplyBuffDebuff, BuffDebuffForDuration, *PerTimeForDuration …
}

// Internal access — siblings of the same core
public interface IVitalComponent<out TCore> : IVitalComponent, ICoreComponent<TCore> where TCore : ICore
{
    void Reborn();
}

public abstract class VitalStatsBase<TCore> : CoreComponentBase<TCore>, IVitalComponent<TCore> where TCore : CoreBase
// VitalStatsComponent (Player) and EntityVitalStats (Enemy) need no change.
```

| # | Caller | Code | Result |
|---|---|---|---|
| 1 | Item (outside the hub) | `target.Transform.GetComponentInChildren<IVitalComponent>()` | ✅ Found; sees `Recovery`/`Reduction`, not `Reborn` |
| 2 | Player sibling (`CoreComponentBase<Core>`) | `TryGetInternal(out IVitalComponent<Core> v)` | ✅ `VitalStatsComponent`, with `Reborn` |
| 3 | Enemy sibling (`T = EntityCore`) | `TryGetInternal(out IVitalComponent<Core> v)` | ❌ **CS0311 at compile time** |
| 4 | Enemy sibling | `TryGetInternal(out IVitalComponent<EntityCore> v)` | ✅ Its own `EntityVitalStats` only |
| 5 | Shared base (`NegativeReceiverBase<TCore>`) | `TryGetInternal(out IVitalComponent<TCore> v)` | ✅ Written once, works on both sides |
| 6 | Player sibling widening the type | `TryGetInternal(out IVitalComponent<ICore> v)` | ❌ Compile error — `ICoreComponent<ICore>` is not an `ICoreComponent<Core>` |

Row 3 is the property this plan exists for: an Enemy component cannot pass the Player's core type to
reach the Player's internal members.

### Why external code must use the non-generic interface

- **The hub honours variance.** `CoreBase.TryGetCapability` matches with the C# `is` operator, which the
  CLR evaluates with variance: `VitalStatsComponent is IVitalComponent<ICore>` is true thanks to `out`,
  and `… is IVitalComponent<EntityCore>` is false.
- **Unity's `GetComponent` does not** (project note in the `ICharacter` doc comment, written for
  ADR-0005; not re-verified in the Editor by this plan). The engine matches the interface a class
  declares, so `GetComponentInChildren<IVitalComponent<ICore>>()` returns null silently.
- Rule: **outside the hub, use only the non-generic interface; inside, use `TryGetInternal`.**
- Still open by choice: outside code *can* write `GetComponentInChildren<IVitalComponent<Core>>()`,
  because the interface is public. Code review catches it; section 5.3 removes the hub route.

## 5. Implementation steps

### 5.1 Infrastructure
- `Assets/Script/Character/Base/CoreComponentBase.cs` — add:
  ```csharp
  protected bool TryGetInternal<TI>(out TI component) where TI : class, ICoreComponent<T>
      => core.TryGetCapability(out component);
  ```
  It reuses `CoreBase.TryGetCapability` and its `_cache`; `CoreBase` and `ICore` are not changed.
- `Assets/Script/Character/Base/Interface/ICoreComponent.cs` — doc comments stating that the
  non-generic marker is the external side and `ICoreComponent<TCore>` the internal side.

### 5.2 Pilot — four interfaces
Interfaces with both sides (the public one keeps its name and its public members):
- `IVitalComponent` → add `IVitalComponent<out TCore>` holding `Reborn()`; `VitalStatsBase<TCore>`
  implements it.
- `ICharacterInput` → add `ICharacterInput<out TCore>` holding `OnTakeDamage()`;
  `CharacterInputBase<TCore>` implements it; `NegativeReceiverBase<TCore>.input` becomes
  `ICharacterInput<TCore>` and is resolved with `TryGetInternal`.
- `IMovement` → add `IMovement<out TCore>` holding `ClearImpacts()`; `MovementBase<TCore>` implements
  it; `EntityVitalStats` resolves `IMovement<EntityCore>` with `TryGetInternal`.

Internal-only:
- `IRegenResource` → only `IRegenResource<out TCore> : ICoreComponent<TCore>`;
  `RegenComponent : … IRegenResource<Core>`. Its wrong base class (`MovementBase`) and null
  `_cacheDatas` are recorded, not fixed here.

Before editing, re-grep that no file outside `Assets/Script/Character/` calls the moved members
through the non-generic interface.

### 5.3 Remove `ICore Core` from the non-generic `ICharacter`
- `Character/Base/Interface/ICharacter.cs` — delete `ICore Core { get; }` from `ICharacter`; in
  `ICharacter<TCore>`, `new TCore Core` becomes `TCore Core`.
- `Character/Base/CharacterBase.cs:12` — delete `ICore ICharacter.Core => core;`.
- `System/RoomGameSystem/UpgradeController.cs:10` — use
  `character.Transform.GetComponentInChildren<IPlayerStatService>()` (implemented by `StatHandler`).
- No other caller at `05dcf63`. **Re-grep after merging `sprint-18` / `sprint-19`.**

### 5.4 Documentation
- `.claude/rules/engine-code.md` — the pair only when both sides are needed; one plain interface for
  external-only; one generic interface for internal-only; outside the hub use the non-generic
  interface; siblings use `TryGetInternal<IX<TCore>>`; drop `out` before adding a `TCore` parameter.
- `CLAUDE.md` › Architecture › Shared base layer — a "public vs internal component interfaces"
  paragraph; correct the `ICharacter.cs` entry that says `CoreBase` exposes `TryGetCapability` to
  callers of `ICharacter`.
- `ICharacter` doc comment — drop the hub reference; state the external / internal roles.
- ADR-0005 — Amendment (core/component access boundary).

## 6. Verification
1. **Compile** — open the Unity Editor; the Console shows no error (there is no CLI build).
2. **Grep** — outside `Assets/Script/Character/`, no `IVitalComponent<` / `ICharacterInput<` /
   `IMovement<` / `IRegenResource<`, no `.Reborn()` / `.OnTakeDamage(` / `.ClearImpacts(` through the
   non-generic interfaces, and no `ICharacter.Core`.
3. **Isolation** — temporarily write `TryGetInternal(out IVitalComponent<Core> v)` in an Enemy
   component: it must fail with CS0311. Delete the line.
4. **Play Mode smoke** on `Assets/Scenes/Main/LoadRandomMap.unity`:
   - player is hit → enters the TakeDamage state (`OnTakeDamage`);
   - pooled enemy respawns with no leftover lock or knockback (`ClearImpacts`);
   - HUD HP refills on Reborn (`Reborn`);
   - interacting with `UpgradeController` still levels up (`ICharacter` without `Core`).
