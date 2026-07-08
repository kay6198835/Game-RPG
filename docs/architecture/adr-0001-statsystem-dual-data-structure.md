# ADR-0001: StatSystem keeps List<Stat> and Dictionary<StatType, float> in parallel

## Status
Proposed

## Date
2026-07-06

## Engine Compatibility

| Field | Value |
|-------|-------|
| Engine | Unity 2022.3.62f1 LTS |
| Domain | Core / Serialization |
| Knowledge Risk | LOW (pinned version is within training data) |
| References Consulted | docs/engine-reference/unity/VERSION.md |
| Post-Cutoff APIs Used | None on the pinned version. The removal condition references Unity 6.0.0.5 (post-cutoff). |
| Verification Required | Before removing `List`, confirm directly in Unity 6.0.0.5 that the Inspector can edit Dictionary values. |

## ADR Dependencies

| Field | Value |
|-------|-------|
| Depends On | None |
| Enables | None |
| Blocks | None |
| Ordering Note | None |

## Context

### Problem Statement
`StatsSO` must support two things at once: (a) editing each stat's value in the
Inspector at author time, and (b) reading values by `StatType` at O(1) cost at
runtime. The Unity Inspector does NOT support editing `Dictionary` values (through
at least Unity 2022.3 LTS), so a `Dictionary` cannot be used as the author-time
data source.

### Constraints
- The Unity Inspector cannot serialize/edit `Dictionary<K,V>` on the current engine version.
- `StatModifier` is runtime-only and not Unity-serializable (see `Stat.modifiers`).
- Gameplay reads stats through `StatsSO.Get(StatType)` — a hot path that requires O(1).

### Requirements
- Authors can edit each stat's base value in the Inspector.
- `Get(StatType)` is O(1) and allocation-free per read.
- A 1:1 invariant between `List` and `Dictionary` — no drift, no duplicate keys.

## Decision
Keep `List<Stat> stats` as the serialized source (edited in the Inspector) and build
a runtime-only `Dictionary<StatType, float> lookup` as an O(1) lookup index.
`EnsureInitialized()` builds `lookup` from `stats` once when the SO loads, dropping
null/duplicate keys and backfilling any missing `StatType`, keeping the
List ↔ Dictionary invariant.

### Key Interfaces
- `float StatsSO.Get(StatType type)` — O(1) read via `lookup`.
- `void StatsSO.AddModifier / RemoveModifiersFromSource / AddPrimaryPoint` — write through `List`, then resync.

## Alternatives Considered

### Alternative A: Serialized List<Stat> + runtime Dictionary index (CHOSEN)
- Description: List is the author-time source; Dictionary is built at runtime for lookup.
- Pros: editable in the Inspector; Get() is O(1); reuses List's built-in serialization.
- Cons: the two structures must be kept in sync; extra memory for the index; more code.
- Rejection Reason: N/A — chosen.

### Alternative B: Dictionary-only with custom serialization
- Description: Use `Dictionary` as the source, serialized via `ISerializationCallbackReceiver`
  or a hand-written SerializedDictionary type so it shows/edits in the Inspector.
- Pros: a single data source; no syncing needed.
- Cons: must author/maintain a serialization layer + property drawer; error-prone;
  still not the native Inspector experience.
- Rejection Reason: maintenance cost and risk outweigh the benefit versus Alternative A.

### Alternative C: List-only with linear lookup
- Description: Drop the Dictionary; `Get()` scans the `List` linearly.
- Pros: a single structure; simplest.
- Cons: `Get()` is O(n) on a hot path; degrades as the number of StatTypes grows.
- Rejection Reason: violates the O(1) hot-path requirement.

## Consequences

### Positive
- Authors edit stats directly in the Inspector.
- Stat reads are O(1) with no per-frame allocation.

### Negative
- State duplication: `List` and `Dictionary` must be kept in sync (accepted deliberately).
- Extra memory for the runtime index.

### Risks
- `List` and `Dictionary` could drift if a write path bypasses `EnsureInitialized()`.
  Mitigation: all writes go through the `StatsSO` API; the index is rebuilt when the SO reloads.

## Removal Condition (Unity 6)
When the project upgrades to **Unity 6.0.0.5** and it is verified that the Inspector
can edit Dictionary values, consider moving to a single `Dictionary` source (dropping
`List`), removing the state duplication. At that point, revisit this ADR and update its
Status. This is the reason the dual structure exists — any issue or warning asking
"why are there two structures?" should point back to this ADR to weigh the trade-off.

## GDD Requirements Addressed

| GDD System | Requirement | How This ADR Addresses It |
|------------|-------------|--------------------------|
| design/gdd/stat-system.md | Runtime stat lookup + author-time editing | Records the storage decision behind `StatsSO`'s List + Dictionary pair |

## Performance Implications
- CPU: Get() is O(1) instead of O(n).
- Memory: +1 runtime Dictionary index (~one entry per StatType).
- Load Time: one index build when the SO loads (EnsureInitialized).

## Validation Criteria
- `Get(StatType)` returns the value set in the Inspector for every StatType.
- No duplicate/null keys remain after `EnsureInitialized()`.

## Related Decisions
- Assets/Script/StatSystem/StatsSO.cs, Stat.cs (current implementation)
