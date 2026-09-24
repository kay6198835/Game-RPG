# ADR-0005: Player and Entity share one character contract

## Status
Proposed — implemented on branch `demo-architeture-1` for review; not yet verified in the Unity Editor.

## Date
2026-09-23 · **Amendment 1: 2026-09-24** (owner review — see the end of this document)

> **Amendment 1 summary.** `ICharacter` is an identity marker only and exposes **no component
> interfaces**. Any system that needs an interface gets it with `TryGetComponent` / `GetComponent` /
> `GetComponentInParent` / `GetComponentInChildren`, whichever fits its context. `ICharacter<TCore>`,
> `ICore.Character` and `CharacterLookup` are removed. Sections 1, 5 and 6 below are written in their
> amended form.

## Engine Compatibility

| Field | Value |
|-------|-------|
| Engine | Unity 2022.3.62f3 LTS |
| Domain | Core / Architecture / Scripting |
| Knowledge Risk | LOW (pinned engine version is within training data) |
| References Consulted | `docs/engine-reference/unity/VERSION.md`; `.claude/rules/engine-code.md`, `gameplay-code.md`, `weapon-skill-code.md`, `scriptableobject-data.md` |
| Post-Cutoff APIs Used | None |
| Verification Required | Generic abstract `MonoBehaviour` bases with concrete non-generic subclasses (supported since Unity 4); `GetComponentInParent<T>()` with an interface `T` (supported); `ScriptableObject` cloning with `Object.Instantiate` |

## ADR Dependencies

- **Builds on ADR-0004** (VContainer): keeps its rule that DI supplies cross-system services only and
  that sibling components use the Core hub. This ADR adds a third category — *character-level
  services* — and states they are resolved through the hub, never through the container.
- **Interacts with ADR-0001** (StatSystem data structure): `StatModifierGroup.Apply/Remmove` becomes
  the one way to attach a modifier bundle to a character.
- **Consistent with ADR-0002**: no new singleton is introduced.
- **Precedent**: the Map Grid/Cell system (`IGrid`/`IGrid<T>`/`BaseGrid<T>`/`BaseCell`/`Cell`/`MazeController`).
  See `character-architecture-analysis.md` §1-2.

## GDD Requirements Addressed
None directly — there is no GDD for Abilities v2 or items yet (BUG-052). The requirement comes from the
owner brief: any system that changes stats must reach a player or an enemy through interfaces only.

---

## Context

Player and Entity already share `CoreBase`/`CoreComponentBase<T>`, but everything above that layer
diverged (analysis §3): duplicated stat and vital components with different interfaces and method
names, two damage receivers on the player, an empty `ICharacter`, and every stat-changing system
hard-wired to the player — Abilities v2 can only affect the caster, items cast to `ResourceReceiver`,
and weapon equip reaches `Core.Player.Data.Stats`. A stat effect therefore cannot be applied to an enemy
at all, let alone through the same code path as to the player.

## Decision

### 1. One character identity, one set of component interfaces

```csharp
public interface ICharacter          // identity of the character root — the IGrid role
{
    Transform Transform { get; }
}
```
Filled in place in `Character/Base/Interface/ICharacter.cs` (existing file, not serialized).

The shared contract is the **set of component interfaces** both sides implement (§2), not properties
on `ICharacter`. `ICharacter` exists so a system can find a character's root
(`GetComponentInParent<ICharacter>()`); from there it takes whatever component interface it needs.

**Rule — getting a component interface.** Inside one character, siblings use the hub
(`Core.GetCoreComponent<T>` / `Core.TryGetCapability<T>`). From outside, a system uses the Unity
lookup that fits its context, and never a property on `ICharacter`:

| Context | Lookup |
|---|---|
| The collider hit carries the capability (hurtbox → `INegativeReceiver`) | `hit.TryGetComponent(out T)` |
| Starting from a child of the character (holder, interactor, hurtbox) | `GetComponentInParent<ICharacter>()` |
| From the character root to a capability that has no collider (`IVitalComponent`, `IStatService`) | `character.Transform.GetComponentInChildren<T>()` |

By design each externally-targeted component owns its own collider (hurtbox, hitbox, range check), so
`TryGetComponent` on the collider returns the component of the right prefab. Internal state
(Vital, Stats) has no collider and is reached from the root.

### 2. Interfaces and who implements them

| Interface | Decision | Player | Entity |
|---|---|---|---|
| `IStatService` (new, `Assets/Script/Interface/`) | character-level stat members | StatHandler | EntityStatsHandler |
| `IPlayerStatService` | **kept**, now `: IStatService`, only `AddPrimaryPoint` + `GetLevelUpStatsBonus`; still the DI key | StatHandler | — |
| `IVitalComponent` | kept; gains `Reborn()` | VitalStatsComponent | **EntityVitalStats** (new) |
| `INegativeReceiver` | kept, signature `TakeDamage(float, Vector2)` unchanged | NegativeReciver (only) | EntityNegativeReciver |
| `IResourceReceiver` | **removed** — a subset of `IVitalComponent`, and its only implementer NRE'd (BUG-080) | — | — |
| `ICharacter` | identity only (Amendment 1) | Player | Entity |

`IPlayerStatService` is deliberately **not** renamed: the character-level part moved out, and what
remains is exactly the player-scoped DI service, so no UI file changes.

### 3. Shared bases (P2 + P3)

- `StatHandlerBase<TCore> : CoreComponentBase<TCore>, IStatService` — template method
  `ResolveProfile()`; `[SerializeField] statsSO` keeps its name.
- `VitalStatsBase<TCore> : CoreComponentBase<TCore>, IVitalComponent` — current values, `Reborn()`.
- `CharacterBase<TCore> : BaseEntity, ICharacter` — `[SerializeField] protected TCore core`
  (same name as both old fields) and the typed `Core` property the state classes use. It hands out no
  component (Amendment 1).

Every existing MonoBehaviour keeps its class and file name, so script GUIDs and prefab references are
untouched.

### 4. Core hubs stay separate

`Core` and `EntityCore` stay as two classes on `CoreBase`; no `CharacterCore<TOwner>`. They differ only
in the typed owner property, `Core.Player`/`Core.Entity` is used by every state, and merging would gain
nothing. `CoreComponent<T>`/`EntityCoreComponent<T>` stay as the typed shims. `ICore` gains one
additive member, for siblings inside a character only: `bool TryGetCapability<T>(out T) where T : class`
— `GetCoreComponent<T>` requires `T : ICoreComponent<ICore>`, which service interfaces cannot satisfy
(the shared `VitalStatsBase` needs `IStatService` without knowing the concrete handler).

### 5. Lookup paths

Per the rule in §1: Unity's `GetComponent` family, chosen by context. No helper class, no
`FindObjectOfType`, no singleton, no DI lookup. A hit counts only if the collider's own GameObject
carries an `INegativeReceiver` (the hurtbox), so a weapon hitbox or the root body never counts and one
hit cannot land twice on the same character.

### 6. Abilities independent of character type

- `AbilityContext` gains `Collider2D Target` — the hurtbox hit. **Set-then-invoke is kept**: the
  spawned object assigns `Target` (null for a non-hurtbox) and then invokes the callback. Damage effects
  do `Target.TryGetComponent(out INegativeReceiver)`.
- `AbilityContext.TryGetRecipientComponent<T>(recipient, out T)` goes from the caster or the hit
  collider up to its `ICharacter` root, then `GetComponentInChildren<T>()`.
- `IAbilityServices` keeps only `Pool`; caster values come from the existing
  `IAbilityOwner.GetCurrentStatValue` / `PayCost` (AbilityHolder resolves its sibling Vital through the hub).
- `StatsEffectBase` and `BuffDebuffStatsForDuration` gain `EffectRecipient { Caster = 0, Target }`.
  Default `Caster` keeps every existing Paladin asset unchanged.
- Spawn effects run their (so far unused, all empty) `SubEffects` on hit, so a Target-recipient stat
  effect acts on whoever was hit.

### 7. VContainer

`GameLifetimeScope.Configure()` is unchanged. `ICharacter`, `IStatService`, `IVitalComponent`,
`INegativeReceiver` are **never registered** — they are per-instance. Pooled enemies keep receiving
only cross-system services from `Pool.Spawn()`; their character services come from their own core,
exactly like the player's.

### 8. Per-instance enemy stats

`EntityStatsHandler.ResolveProfile()` returns `Instantiate(EntityData.StatsSO)`; the clone is destroyed
with the enemy and `EntityVitalStats.Reborn()` clears its runtime modifiers on every pool re-spawn.
The player keeps the shared asset (BUG-063 stays accepted/deferred).

## Alternatives Considered

1. **Implement `ICharacter` directly on Player and Entity, no base.** Less code, but duplicates the
   resolution logic and breaks P2; rejected.
2. **Merge `Core`/`EntityCore` into `CharacterCore<TOwner>`.** Touches every state for no behaviour
   gain; rejected (§4).
3. **Rename `IPlayerStatService` → `IStatService`.** Loses the player-scoped DI key and forces UI edits;
   rejected in favour of the split.
4. **Register character services in VContainer.** They are per-instance; a container registration
   would be a global answer to a per-character question; rejected.
5. **Replace set-then-invoke with a callback parameter.** Owner confirmed set-then-invoke as
   deliberate; kept, only the slot's type changes.

## Consequences

**Positive**
- A stat effect reaches a player or an enemy through one code path with no `is Player`/`is Entity`.
- Player and Entity implement the same character-level interface set.
- BUG-080 and BUG-081 (player side) are resolved; BUG-066/070 collapse to one site.
- Enemy stat changes are per instance and cannot leak into assets.

**Negative / risks**
- BUG-071 now lives in the shared base, so per-time effects are also broken on enemies.
- Lazy resolution moves some failures from `Awake` to first use (mitigated by explicit error logs).
- One `ScriptableObject` clone per enemy instance (reused by the pool).
- Not yet compiled in the Editor at the time of writing.

## Bugs Touched

| Bug | Outcome |
|---|---|
| BUG-066 / BUG-070 | Absorbed — one indexer in `VitalStatsBase`; behaviour preserved, still open |
| BUG-071 | Absorbed — moved unchanged into the base; still open, now reachable on enemies |
| BUG-080 | Resolved — the lazily-assigned field and its forwarding methods are gone |
| BUG-081 | Resolved on the player; stray component removed from `SO/Database/EnemyPrefab.prefab` |
| BUG-087 | Partially absorbed — player gains `Reborn()`; GameManager/subscriber still missing |
| BUG-063 | Unchanged for the player (accepted/deferred) |
| New (not filed) | Shared enemy stat SO — fixed by §8 |

## Amendment 1 — 2026-09-24 (owner review)

**Decision.** Keep `ICharacter`, but it must not carry component interfaces. Systems get the interface
they need with `TryGetComponent` / `GetComponent` / `GetComponentInParent` / `GetComponentInChildren`
according to their own context.

**Why.** Exposing `Stats`/`Vital`/`DamageReceiver`/`Core` on `ICharacter` made it a service locator that
every system reached through (`target.Vital.Reduction(...)`), grew with every new capability, and leaked
the whole hub through `Core`. The project's design already addresses capabilities by collider — every
externally-targeted component owns its hurtbox, hitbox or range check — so the Unity lookup on the
collider or the character root is the natural address and needs no extra layer.

**Removed:** `ICharacter<TCore>`; `ICharacter.Core/Stats/Vital/DamageReceiver`; `ICore.Character`;
`CharacterLookup`; `IAbilityOwner.Character`; `AbilityContext.CasterCharacter/ResolveRecipient`.
**Changed:** `AbilityContext.Target` is now `Collider2D`; items, weapon equip and stat effects resolve
through the `GetComponent` family.

**Still open (raised in the same review, not decided):** stat effects write `IVitalComponent` directly,
which skips the character's own rules — an enemy's health bar is only refreshed by
`EntityNegativeReciver.TakeDamage()`, so a stat-effect HP reduction does not update it, and no hit
reaction or Defense applies. Options: a gateway interface on the hurtbox component, or change events on
`IVitalComponent` that the UI subscribes to.

## Residuals (out of scope)

- `EntityEffectStats` (Abilities v1) still writes `Core.Entity.Data.StatsSO`, the shared asset.
- `bullet.cs`, `Projectile.cs`, `Spell.cs` still look receivers up with `GetComponentInChildren`.
