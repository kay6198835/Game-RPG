# ADR-0005: Player and Entity share one character contract

## Status
Proposed — implemented on branch `demo-architeture-1` for review; not yet verified in the Unity Editor.

## Date
2026-09-23

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

### 1. One character contract

```csharp
public interface ICharacter                       // coordination — the IGrid role
{
    Transform Transform { get; }
    ICore Core { get; }
    IStatService Stats { get; }                   // max values
    IVitalComponent Vital { get; }                // current values
    INegativeReceiver DamageReceiver { get; }     // exactly one per character
}
public interface ICharacter<out TCore> : ICharacter where TCore : ICore   // typed — the IGrid<T> role
{
    new TCore Core { get; }
}
```
Filled in place in `Character/Base/Interface/ICharacter.cs` (existing file, not serialized).

### 2. Interfaces and who implements them

| Interface | Decision | Player | Entity |
|---|---|---|---|
| `IStatService` (new, `Assets/Script/Interface/`) | character-level stat members | StatHandler | EntityStatsHandler |
| `IPlayerStatService` | **kept**, now `: IStatService`, only `AddPrimaryPoint` + `GetLevelUpStatsBonus`; still the DI key | StatHandler | — |
| `IVitalComponent` | kept; gains `Reborn()` | VitalStatsComponent | **EntityVitalStats** (new) |
| `INegativeReceiver` | kept, signature `TakeDamage(float, Vector2)` unchanged | NegativeReciver (only) | EntityNegativeReciver |
| `IResourceReceiver` | **removed** — a subset of `IVitalComponent`, and its only implementer NRE'd (BUG-080) | — | — |
| `ICharacter<TCore>` | filled | Player | Entity |

`IPlayerStatService` is deliberately **not** renamed: the character-level part moved out, and what
remains is exactly the player-scoped DI service, so no UI file changes.

### 3. Shared bases (P2 + P3)

- `StatHandlerBase<TCore> : CoreComponentBase<TCore>, IStatService` — template method
  `ResolveProfile()`; `[SerializeField] statsSO` keeps its name.
- `VitalStatsBase<TCore> : CoreComponentBase<TCore>, IVitalComponent` — current values, `Reborn()`.
- `CharacterBase<TCore> : BaseEntity, ICharacter<TCore>` — `[SerializeField] protected TCore core`
  (same name as both old fields), resolves `Stats`/`Vital`/`DamageReceiver` lazily from its core and
  logs an error instead of returning a silent null.

Every existing MonoBehaviour keeps its class and file name, so script GUIDs and prefab references are
untouched.

### 4. Core hubs stay separate

`Core` and `EntityCore` stay as two classes on `CoreBase`; no `CharacterCore<TOwner>`. They differ only
in the typed owner property, `Core.Player`/`Core.Entity` is used by every state, and merging would gain
nothing. `CoreComponent<T>`/`EntityCoreComponent<T>` stay as the typed shims. `ICore` gains two
additive members:
- `bool TryGetCapability<T>(out T) where T : class` — `GetCoreComponent<T>` requires
  `T : ICoreComponent<ICore>`, which service interfaces cannot satisfy;
- `ICharacter Character { get; }`.

### 5. One lookup path

- **Hit path** — `CharacterLookup.TryGetCharacter(this Component hit, out ICharacter)`: a collider
  resolves only if it is a hurtbox (its GameObject carries an `INegativeReceiver`), then
  `GetComponentInParent<ICharacter>()`. Non-allocating; a weapon hitbox or the root body never
  resolves, so one hit cannot land twice on the same character.
- **Owner path** — a core component reaches its own character with `Core.Character`.

No `FindObjectOfType`, no singleton, no DI lookup.

### 6. Abilities independent of character type

- `IAbilityOwner` gains `ICharacter Character`.
- `AbilityContext` gains `ICharacter Target` and `CasterCharacter`. **Set-then-invoke is kept**: the
  spawned object assigns `Target` (null for a non-hurtbox) and then invokes the callback.
- `IAbilityServices` keeps only `Pool`; caster data is read from `ctx.CasterCharacter`.
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

## Residuals (out of scope)

- `EntityEffectStats` (Abilities v1) still writes `Core.Entity.Data.StatsSO`, the shared asset.
- `bullet.cs`, `Projectile.cs`, `Spell.cs` still look receivers up with `GetComponentInChildren`.
