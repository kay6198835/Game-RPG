# ADR-0003: Enemy Spawn Selection Algorithm — Room Budget + Candidate Pool + RarityTier (Option C)

## Status
Accepted (2026-07-23 — see Amendment section below; Data Model amended to match shipped code, algorithm/selection-flow unchanged from original)

## Date
2026-07-13

## Engine Compatibility

| Field | Value |
|-------|-------|
| **Engine** | Unity 2022.3.62f3 LTS |
| **Domain** | Core / Scripting (ScriptableObject-based selection algorithm — pure C#, no engine subsystem) |
| **Knowledge Risk** | LOW — pinned version is within LLM training data |
| **References Consulted** | `docs/engine-reference/unity/VERSION.md`; `.claude/rules/engine-code.md` (Zero-Alloc Hot Paths); `.claude/rules/gameplay-code.md`; `.claude/rules/scriptableobject-data.md` |
| **Post-Cutoff APIs Used** | None — `UnityEngine.Random`, `[Range]`, `OnValidate`, and enum-with-explicit-int-values are all stable long-standing APIs |
| **Verification Required** | At implementation time (S5-A3 / Sprint 6): (1) confirm `UnityEngine.Random.value`'s inclusive `[0,1]` range is what the `r ≤ chance(tier)` comparison assumes (it is); (2) confirm the per-pick `chance` roll and eligible-set build allocate nothing per candidate (reuse `RoomModel.GetSpawnSet()`'s existing `_fitBuf`/`_entries` scratch-buffer pattern), per the Zero-Alloc Hot Path rule. |

## ADR Dependencies

| Field | Value |
|-------|-------|
| **Depends On** | None — the selection method is a pure method on `RoomModel`/`EnemyModal` and needs no other ADR to be Accepted before it can be implemented. |
| **Enables** | Sprint 5 Track A data refactor **S5-A3** (add the `RarityTier` enum + `rarityTier` field to `EnemyModal`); the Sprint 6 `RoomModel.GetSpawnSet()` rewrite to the eight-step Candidate-Pool flow. |
| **Blocks** | The S5-A3 and Sprint 6 `GetSpawnSet()` rewrite stories should not be finalized/implemented against a different algorithm until this ADR is Accepted. |
| **Ordering Note** | Sibling to ADR-0002. ADR-0002 governs the `EnemyManager` singleton that *drives the room lifecycle and calls* this selection method; this ADR governs *which enemies the method returns*. The two are complementary and independently implementable — ADR-0002 explicitly keeps the selection algorithm off the singleton so it stays unit-testable. Neither must precede the other. |

## Context

### Problem Statement
The enemy-spawn system must decide **which enemies appear in each room** against a per-room difficulty budget, producing run-to-run variety (the "each run is a fresh challenge" pillar). This selection algorithm has churned through three incompatible shapes and the divergence needed to be closed by an explicit, recorded decision:

- **What shipped (Option A):** `RoomModel.GetSpawnSet()` — a two-phase (random sub-budget + overflow fill) loop that picks **uniformly at random** among cost-fitting candidates using static `UnityEngine.Random`. Non-deterministic, composition variety is only a side-effect of budget arithmetic, and it can legitimately under-spend the budget while candidates still technically fit.
- **What was designed on 2026-07-08 but never built (Option B):** an `EnemyDatabase` id-lookup + injected `System.Random` + deterministic `argmin |weight − remaining|` fill. Fully testable, but the most implementation work of the three, and its `argmin` fill was already flagged as biasing toward *fewer, heavier* enemies.
- **What the owner proposed and locked (Option C):** Room Budget + a per-pick **Candidate Pool** built by rolling each eligible enemy's `RarityTier` chance, with a bounded retry-then-fallback guarantee.

`design/gdd/enemy-spawn-system.md` Open Question #8 tracked exactly this "which direction do we commit to" decision. It was resolved on 2026-07-13 (Option C), and the GDD now carries the full formal spec ("Option C — Formal Specification (CHOSEN 2026-07-13)"). This ADR records that architectural commitment and its termination/fallback guarantees so downstream stories (S5-A3, the Sprint 6 rewrite) implement against a defined, reviewed target rather than an in-flux GDD section.

### Constraints
- **Zero-alloc hot path** (`.claude/rules/engine-code.md`): the per-pick eligible-set build and per-candidate chance roll must not allocate. The existing `RoomModel.GetSpawnSet()` already proves a reusable scratch-buffer (`_fitBuf`/`_entries`) pattern in this exact code — Option C reuses it, it does not reintroduce per-pick allocation.
- **ScriptableObject-first** (`.claude/rules/scriptableobject-data.md`): all spawn data (`weight`/Cost, `rarityTier`, Room Budget, enemy pool) stays on `EnemyModal`/`RoomModel` SO assets — no hardcoded values.
- **Stat-system boundary** (ADR-0001): `EnemyModal` is spawn metadata only. Combat stats (HP, damage, defense) stay on the prefab's `EntityData`/`StatsSO`. `EnemyModal` must **not** become a fourth stat store; `weight` is a spawn-budget cost, unrelated to the stat formula.
- **RNG source is fixed by owner decision (Open Q#2, resolved 2026-07-13):** stays `UnityEngine.Random`, unseeded. Option C's acceptance criteria are deliberately written **not** to assume determinism.
- **`weight` field name is kept** (Open Q#8 / S5-A3 scope change): the previously-planned `weight`→`cost` rename is **dropped**. `weight` already means "Cost".
- Demo timeline — this is scoped for the S5-A3 data-field add and the Sprint 6 `GetSpawnSet()` rewrite; no new manager class or RNG-injection infrastructure.

### Requirements
- Total selected spend must be **budget-bounded** — never exceed the Room Budget `B` (a change from Option A, which allowed an overflow cap above `B`).
- The algorithm must **terminate** in bounded time for any valid input, with no possible infinite loop.
- Designers get a **second, independent knob** (appearance chance) separate from budget cost, so "a cheap enemy that is rare" or "an expensive enemy that is common" is directly authorable.
- Must integrate with the existing runtime path: `RoomModel.GetSpawnSet()` returning `List<EnemySpawnEntry>` (`{enemy, count}`), called by the room-fill driver (currently `EnemySpawner`/`LevelManager`, eventually `EnemyManager` per ADR-0002).

## Decision

Adopt **Option C — Room Budget + Candidate Pool + `RarityTier`** as the enemy-spawn selection algorithm. This resolves GDD Open Question #8 and supersedes the shipped Option A behavior and the never-built Option B target.

### Data Model

**[AMENDED 2026-07-23 — see Amendment section below]** The SO-extending-`EntityModel` shape originally specified here was not built. `EnemyModal` shipped as a plain `[System.Serializable]` class nested inside `RoomModel.cs` during the Sprint 5→6 Core/CoreComponent refactor. The owner reviewed this drift (sprint-06 task S6-09) on 2026-07-23 and accepted the plain-class shape as final rather than migrating to SO — this section is amended to match the shipped code. The selection algorithm (Candidate-Pool Selection Flow below) is unaffected: it only needs a list of `{weight, rarityTier}` values, independent of whether `EnemyModal` is an SO or a plain class.

`EnemyModal` carries a `RarityTier` enum field. `weight` (Cost) and `rarityTier` are **fully independent by design** — there is no code-enforced correlation between them (no `OnValidate` cross-check flagging "high weight + high roll chance"). This independence is the entire point: it lets a designer make an expensive enemy common or a cheap enemy rare.

**Actual shipped shape** ([RoomModel.cs](../../Assets/Script/Database-SO/Modal/RoomModel.cs)):
```csharp
public enum RarityTier
{
    Common    = 50,  // roll chance, percent
    Rare      = 30,
    Epic      = 15,
    Legendary = 5
}

[System.Serializable]
public class EnemyModal   // plain class, nested in RoomModel.cs — NOT a ScriptableObject, does not
                          // extend EntityModel, no separate asset file, no GUID ID, no cross-room reuse
{
    public GameObject Prefab;
    [Range(1, 100)] public int weight;   // Inspector-slider clamp only, no OnValidate hard clamp
    public RarityTier rarityTier;
}

public class RoomModel : EntityModel
{
    [SerializeField] private List<EnemyModal> enemiesOfRoom = new List<EnemyModal>();
    // Each room authors its own EnemyModal entries inline. There is no
    // Bat_Common.asset/Bat_Rare.asset sharing pattern — if the same enemy config is wanted in two
    // rooms, it is re-entered in both. This is the accepted trade-off of the plain-class shape.
}
```

`weight` MUST be `≥ 1` — enforced via `[Range(1, 100)]` on the field (Inspector-slider level only; a value assigned via code rather than the Inspector is not clamped). The originally-specified `[Range(1,99)]` + `OnValidate` combination was not built; the Inspector-slider `[Range]` alone is sufficient to prevent the non-terminating fit-loop risk as long as all `EnemyModal` values are authored through the Inspector, which is the only authoring path that exists today (no runtime/code construction of `EnemyModal` instances occurs in the current spawn flow).

### Candidate-Pool Selection Flow

Runs once per accepted pick; called repeatedly by the room-fill loop (the same per-candidate loop shape `GetSpawnSet()` already uses). Eight steps:

1. **Start pick round.** `retryCount = 0`.
2. **Build eligible set.** `eligibleSet = { e ∈ enemiesOfRoom | e.weight ≤ remaining }`. If empty, stop the fill loop entirely (see step 8).
3. **Roll each eligible candidate independently.** For every `e ∈ eligibleSet`: `r = Random.value`; `e` passes if `r ≤ chance(e.rarityTier)`, where `chance(Common)=0.50`, `chance(Rare)=0.30`, `chance(Epic)=0.15`, `chance(Legendary)=0.05`.
4. **Collect passers.** `CandidatePool = { e ∈ eligibleSet | e passed step 3 }`.
5. **Empty-pool retry.** If `CandidatePool` is empty: `retryCount += 1`. If `retryCount ≤ 4`, return to step 3 and re-roll the same `eligibleSet`. If `retryCount > 4`, set `CandidatePool = eligibleSet` (fallback — every eligible candidate is accepted regardless of its roll, guaranteeing the pick round cannot stall indefinitely).
6. **Pick.** Choose one entry from `CandidatePool` uniformly at random — equal probability per entry, no weighting by tier or cost at this step (tier already did its job in step 3).
7. **Apply.** `remaining -= picked.weight`; append `picked` to the room's spawn result; `retryCount = 0`.
8. **Loop or stop.** Repeat steps 1–7 while `remaining` is outside the tolerance band (see Formulas) **and** `eligibleSet` (step 2) is non-empty. Stop when either condition fails.

### Formulas

**Budget Tolerance band:** `ToleranceBand = [ B × 0.9, B × 1.1 ]`, where `B` = the `RoomModel` Room Budget (the same role `weightBudget` plays in Option A).

**Overspend is structurally impossible.** Because step 2 requires `weight ≤ remaining` (no overflow cap, unlike Option A's Phase 2), `totalSpend = B − remaining` can **never exceed `B`**. The upper half of the band (100–110%) is therefore unreachable by construction and is kept only for symmetry with the owner's original "90–110%" phrasing. The **only practically meaningful bound is the 90% floor**, and even that is a target, not a guarantee (the loop legitimately stops below 90% if `eligibleSet` empties first). **Confirmed intentional: under-spend is an accepted outcome; overspend cannot happen.**

| Variable | Type | Range | Description |
|----------|------|-------|-------------|
| `B` (Room Budget) | int | mirrors today's `weightBudget` `[Range(0,500)]` | Total spend target for the room |
| `remaining` | int | starts at `B`, decreases each pick, never negative | Budget left this fill loop |
| `totalSpend` | int | `B − remaining`, range `[0, B]` | Running total spent so far |
| `retryCount` | int | 0–4 (hard cap per pick round) | Consecutive empty-`CandidatePool` rolls |
| `chance(tier)` | float | `{0.50, 0.30, 0.15, 0.05}` | Fixed per-tier roll chance, not author-adjustable per instance |
| `weight` (per `EnemyModal`) | int | must be `≥ 1` (enforced `[Range(1, 100)]` on the field, Inspector-slider only — no `OnValidate` clamp, see Amendment) | Cost consumed from `remaining` per pick |

**Termination guarantee.** Every accepted pick (step 7) reduces `remaining` by at least 1 (given `weight ≥ 1`), so the outer loop (step 8) terminates in at most `B` iterations. Each pick round (steps 3–5) terminates in at most 5 roll attempts (4 re-rolls + 1 forced fallback) — never unbounded. The retry cap is precisely what closes the "termination not obviously guaranteed" gap in the original owner sketch.

### Architecture Diagram

```
  room-fill driver (EnemySpawner / LevelManager today; EnemyManager per ADR-0002)
        │  calls once per room load
        ▼
  RoomModel.GetSpawnSet()  ── pure method on the SO, no singleton, unit-testable
        │  fill loop (≤ B iterations, step 8)
        ▼
  ┌─────────────────────────────────────────────────────────────┐
  │ one pick round (steps 1–7):                                  │
  │   eligibleSet = { e | e.weight ≤ remaining }   (step 2)      │
  │        │ empty → stop fill loop                              │
  │        ▼                                                     │
  │   roll RarityTier chance per candidate  (step 3)  ┐          │
  │        │                                          │ ≤4       │
  │   CandidatePool = passers  (step 4)               │ retries  │
  │        │ empty → retry  (step 5) ─────────────────┘          │
  │        │ retry > 4 → CandidatePool = eligibleSet (fallback)  │
  │        ▼                                                     │
  │   pick 1 uniformly  (step 6) → remaining -= weight (step 7)  │
  └─────────────────────────────────────────────────────────────┘
        │  returns
        ▼
  List<EnemySpawnEntry> { enemy, count }
```

### Key Interfaces

- `RarityTier` enum (`Common=50, Rare=30, Epic=15, Legendary=5`) — the integer value **is** the roll-chance percent; a `chance(RarityTier)` mapper divides by 100 to produce the `[0,1]` threshold used in step 3.
- `EnemyModal.weight : int` with `[Range(1, 100)]` on the field (Inspector-slider only, no `OnValidate` clamp — see Amendment) — the `≥ 1` invariant is enforced at the leaf class, not only warned about on the containing `RoomModel`.
- `EnemyModal.rarityTier : RarityTier` — new field added in S5-A3.
- `RoomModel.GetSpawnSet() → List<EnemySpawnEntry>` — signature and return shape unchanged from today; only the internal algorithm changes. The pick-round chance roll and eligible-set build reuse the existing reusable scratch buffers (no per-pick allocation).
- No change to `MapModel.GetRandomRoom()` (the room→preset shuffle-bag, Open Q#4 resolved separately) — Option C runs against whatever `RoomModel` that draw returns.

## Alternatives Considered

### Alternative 1: Option A — harden the current uniform-random, direct-ref code
- **Description**: Keep `RoomModel.GetSpawnSet()`'s two-phase uniform-random shape; add only the missing guards (`weight ≥ 1` clamp, null-safe return, markerless-room fallback).
- **Pros**: Smallest change from what ships today; preserves the zero-alloc scratch-buffer work; fastest path to closing the current bugs.
- **Cons**: Stays non-deterministic; composition control stays coarse (budget + weight only) — no way to express "cheap but rare" / "expensive but common"; the fit-loop can under-spend the budget with candidates still available and no fallback to make forward progress.
- **Rejection Reason**: Delivers the least of the stated Player Fantasy — variety is a side-effect of budget arithmetic, not a directly-authored knob. Cannot express the appearance-chance lever the owner wanted.

### Alternative 2: Option B — revive the 2026-07-08 target (injected RNG, id-database, `argmin`)
- **Description**: Build the `EnemyDatabase` id-lookup, inject `System.Random`, restore a deterministic `argmin |weight − remaining|` Phase-2 fill.
- **Pros**: Fully deterministic and testable; the older AC-A3/AC-A4/AC-D1–D3 acceptance criteria already exist for this shape; id-based refs decouple assets from direct SO references.
- **Cons**: The most implementation work of the three (new database class, id plumbing, migrating every `RoomModel`/`MapModel` asset off direct refs); the `argmin` fill was already flagged as biasing toward fewer, heavier enemies — reviving it without the deferred composition-diversity pass reintroduces that known limitation. Also conflicts with the owner's resolved decisions to keep `UnityEngine.Random` (Open Q#2) and direct refs.
- **Rejection Reason**: Highest cost, reintroduces a known bias, and runs against two already-resolved owner decisions (RNG source, direct refs).

### Alternative 3: Owner's original sketch — `Spawn Chance` as a per-instance float (superseded within Option C)
- **Description**: The first form of Option C used a per-`EnemyModal` `Spawn Chance` float and a `Budget Tolerance` described only as prose ("90–110%").
- **Pros**: Maximum per-enemy tuning granularity (any chance value).
- **Cons**: A free float invites inconsistent authoring across assets; the prose tolerance band left the loop's stop condition and termination underspecified.
- **Rejection Reason**: Replaced (same 2026-07-13 owner session) by a fixed four-value `RarityTier` enum (legible, consistent tiers) and a precise Formulas-section tolerance band + retry-cap termination guarantee. The chosen Option C **is** this alternative made rigorous.

## Consequences

### Positive
- Designers get an appearance-chance lever (`RarityTier`) **independent** of budget cost (`weight`) — the strongest of the three options for the Player Fantasy goal ("sometimes a swarm of cheap enemies, sometimes a couple of heavy ones").
- Budget spend is bounded (`totalSpend ≤ B` always) with a provable termination bound (≤ `B` outer iterations, ≤ 5 attempts per pick round).
- The per-pick fallback (empty pool → accept all eligible) guarantees forward progress except at the true boundary (nothing fits) — fixing a real under-spend gap in Option A.
- Closest of the three to what the code already does (per-candidate gather → pick → shrink loop), so it is evolution, not a rewrite — lowest migration risk after Option A.
- `RarityTier` turns the implicit "weight should track power tier" convention into checkable data — closes part of Open Q#5.

### Negative
- Still non-deterministic (`UnityEngine.Random`, unseeded) — no reproducible rolls; algorithm acceptance criteria (AC-C1…C6) are written to avoid asserting determinism, and no EditMode test can pin an exact composition.
- Under-spend is an accepted outcome — a room whose pool has no cheap-enough candidate left can finish below the 90% floor; there is no forced top-up.
- `weight` and `rarityTier` are never cross-checked, so an asset can be authored with an incoherent cost/rarity pairing (this is intentional, but it means no tooling catches a *mistaken* pairing either).
- The fixed four-tier `chance` table is not per-instance adjustable — a designer wanting a fifth tier or a different curve must change the enum/mapper in code.

### Risks
- **Risk**: A `weight ≤ 0` `EnemyModal` asset makes the eligible-set fill-loop non-terminating (the candidate never leaves `eligibleSet`).
  **Mitigation**: `[Range(1, 100)]` on `EnemyModal.weight` (leaf-level, Inspector-slider enforcement — no `OnValidate` clamp was built, see Amendment); this holds as long as all `EnemyModal` values are authored through the Inspector, the only construction path in the current spawn flow.
- **Risk (minor, engine-specialist note)**: encoding roll-chance percent *in the enum's integer value* (`Common=50`…) couples the enum to its probability and requires the `chance()` mapper to divide by 100; a future editor of the enum could change a value's meaning silently.
  **Mitigation**: keep the `chance()` mapping in one place and cover the tier→chance mapping with AC-C6's directional distribution check; treat the enum values as the single source of the percent.
- **Risk**: per-pick chance rolls + eligible-set rebuild allocate on the hot path.
  **Mitigation**: reuse `GetSpawnSet()`'s existing `_fitBuf`/`_entries` scratch buffers (a `foreach` + `Random.value` compare against a pre-sized buffer) — the same pattern already proven in this file; called out in Verification Required.
- **Risk**: the fallback round (retry > 4) picks uniformly across all eligible, so a `Legendary` (5%) gets the same odds as a `Common` (50%) under bad luck — tier weighting is lost in fallback.
  **Mitigation**: intentional and documented (Edge Cases); the fallback exists only to guarantee forward progress, not to preserve tier weighting. AC-C6 counts non-fallback rounds only.

## GDD Requirements Addressed

| GDD System | Requirement | How This ADR Addresses It |
|------------|-------------|--------------------------|
| `design/gdd/enemy-spawn-system.md` | Open Question #8 — "which selection-algorithm direction do we commit to?" | Records Option C as the committed direction and its termination/fallback guarantees, closing Open Q#8. |
| `design/gdd/enemy-spawn-system.md` | "Option C — Formal Specification (CHOSEN 2026-07-13)": data model, 8-step flow, Formulas, Edge Cases, AC-C1…C6 | Ratifies the locked spec as the S5-A3 + Sprint 6 implementation target so downstream stories build against a reviewed architecture. |
| `design/gdd/enemy-spawn-system.md` | Player Fantasy — "sometimes a swarm of cheap enemies, sometimes a couple of heavy ones" | The `RarityTier`/`weight` split gives the independent appearance-chance vs budget-cost knob the fantasy needs; the tolerance band + fill loop produces the swarm-vs-heavy variety. |
| `design/gdd/enemy-spawn-system.md` | Open Q#5 — "should weight/cost track the enemy's tier?" (partial) | `RarityTier` makes the enemy's tier explicit checkable data rather than a weight-authoring convention. |
| `design/gdd/stat-system.md` (ADR-0001 boundary) | `EnemyModal` must not become a stat store; combat stats stay on `EntityData`/`StatsSO` | `RarityTier`/`weight` are spawn metadata only; the ADR reaffirms the boundary — no combat stat is added to `EnemyModal`. |

## Performance Implications
- **CPU**: Per room load only (not per frame). One fill loop of ≤ `B` picks; each pick rolls up to `|eligibleSet|` `Random.value` comparisons, retried ≤ 5 times. For this project's single-digit enemy-pool sizes this is negligible; the zero-alloc scratch-buffer reuse keeps it GC-free.
- **Memory**: No per-call allocation when the existing `_fitBuf`/`_entries` buffers are reused (resized only when the enemy-type count changes). Adds one `enum` field (4 bytes) per `EnemyModal` asset.
- **Load Time**: None measurable — selection runs once at room populate, not at scene load.
- **Network**: N/A — single-player demo.

## Migration Plan
Option C replaces the internals of `RoomModel.GetSpawnSet()`; the method signature and `List<EnemySpawnEntry>` return shape are unchanged, so the `EnemySpawner`/`LevelManager`/future-`EnemyManager` call sites need no change to consume it.

1. **S5-A3 (data field):** ✅ done — `RarityTier` enum and `EnemyModal.rarityTier` field added; `EnemyModal.weight` carries `[Range(1, 100)]` (not the originally-planned `[Range(1,99)]` + `OnValidate` — see Amendment). The `weight`→`cost` rename was dropped as planned.
2. **Sprint 6 (algorithm rewrite):** ⬜ not done as originally scoped — `RoomModel.GetSpawnSet()` still runs the two-phase uniform-random shape (Option A), not the eight-step Candidate-Pool flow. `enemiesOfRoom.Count == 0` still returns `null` (BUG-ES-1), which the caller (`EnemySpawner.GetRoomSpawnSet()`) treats as an accepted, decided-final behavior (S6-06, 2026-07-23) rather than a bug — see GDD "Current Implementation" note. The Candidate-Pool eight-step rewrite itself remains a backlog item, independent of the S6-09 data-model decision.
3. **Tests:** ⬜ not done — no AC-C1…C6 EditMode tests exist yet.
4. Existing `RoomModel.weightBudget` (`[Range(0,500)]`) plays the `B` (Room Budget) role unchanged — no asset migration for the budget dial. Confirmed unchanged as of 2026-07-23.

## Validation Criteria
The GDD's Option C acceptance criteria are the validation bar (EditMode, BLOCKING):
- **AC-C1** — `totalSpend ≤ B` always (never exceeds budget).
- **AC-C2** — a pick round where every roll is forced to FAIL completes in exactly 5 attempts (4 re-rolls + 1 forced fallback) and returns a non-empty `CandidatePool == eligibleSet`.
- **AC-C3** — with `weight ≥ 1` for all candidates, the fill loop completes in at most `B` iterations (no hang).
- **AC-C4 (amended)** — `EnemyModal.weight` cannot be authored below 1 via the Inspector (`[Range(1, 100)]`); no `OnValidate` clamp exists to test against a code-constructed `weight = 0` instance, since `EnemyModal` is a plain class authored only through the Inspector in the current flow (see Amendment). The bounded-completion guarantee (≤ `B` outer iterations) still holds given `weight ≥ 1` for all Inspector-authored candidates.
- **AC-C5** — high-`weight` + high-chance-tier (or the inverse) raises no validation warning/error — `weight` and `rarityTier` are never cross-checked.
- **AC-C6** — over 1000 pick rounds counting only non-fallback rounds, `Common` is selected into `CandidatePool` markedly more often than `Legendary` (directional check, not an exact-ratio assertion — avoids RNG-variance flaking).

## Amendment (2026-07-23 — Status flipped Proposed → Accepted)

**Trigger**: sprint-06 task **S6-09** (`production/sprints/sprint-06-daily-plan.md`) — the Sprint 5→6
Core/CoreComponent refactor (`771f169 "big update core and corecomponet, add interface"`, 2026-07-22)
shipped `EnemyModal` as a plain `[System.Serializable]` class nested in `RoomModel.cs`, not the
SO-extending-`EntityModel` shape this ADR originally specified. This was refactor collateral, not an
explicit architectural call — nobody had decided to abandon the SO design, it just happened. S6-09
existed to force an explicit decision instead of leaving the drift unrecorded.

**Decision (owner, 2026-07-23)**: accept the plain-class shape as final. Do **not** migrate `EnemyModal`
to a `ScriptableObject`/`EntityModel` subclass. This ADR's Status flips to **Accepted** against the
shipped code, not the original spec — the Data Model, Formulas, Migration Plan, and Validation Criteria
sections above are amended in place to describe the plain-class reality (see inline `[AMENDED]`/
`(amended)` markers).

**What is explicitly NOT accepted by this amendment**: the Candidate-Pool eight-step selection algorithm
itself (the core subject of this ADR) is **still not implemented** — `RoomModel.GetSpawnSet()` still
runs the original Option A two-phase uniform-random shape as of 2026-07-23. Accepting the plain-class
data shape does not imply the algorithm rewrite is done or deprioritized; that remains open, tracked
separately from S6-09.

**Consequences of accepting plain-class (superseding the original Consequences section for this one
axis)**:
- **Lost**: cross-room reuse of a single `EnemyModal` definition (no `Bat_Common.asset` shared by 3
  rooms — each room re-enters its own copy); `EntityModel.ID` (GUID) for `EnemyModal` instances; strict
  code-level clamp on `weight` via `OnValidate` (Inspector `[Range]` is enforced only in the Inspector).
- **Kept**: the Candidate-Pool algorithm's termination/fallback guarantees (algorithm-level, independent
  of the data shape); the `weight`/`rarityTier` independence design goal; the zero-alloc scratch-buffer
  reuse pattern (once the algorithm rewrite happens).
- **Follow-up if reuse is needed later**: if design later wants shared enemy definitions across rooms
  (e.g. a bestiary of reusable `EnemyModal` variants), that would require re-opening this decision as a
  new ADR rather than silently re-introducing the SO shape — the plain-class shape and any future SO
  shape are not binary-compatible (existing `RoomModel` assets would need re-authoring either direction).

## Related Decisions
- **ADR-0002** (EnemyManager singleton exception) — sibling. Governs the singleton that drives the room lifecycle and *calls* `GetSpawnSet()`; explicitly keeps the selection algorithm off the singleton so it stays unit-testable. This ADR governs what the method returns.
- **ADR-0001** (StatSystem dual data structure) — boundary contract: `EnemyModal` is spawn metadata, not a fourth stat store. Still holds under the plain-class shape — `weight`/`rarityTier` remain the only fields, no combat stat was added.
- **design/gdd/enemy-spawn-system.md** — Open Question #8 (resolved by this ADR); "Option C — Formal Specification (CHOSEN 2026-07-13)" and its "Current Implementation (as of 2026-07-23)" follow-up note (added alongside this Amendment); Open Q#2 (RNG source, resolved: stays `UnityEngine.Random`); Open Q#4 (room→preset shuffle-bag, resolved separately); Open Q#5 (weight↔tier, partially addressed by `RarityTier`).
- **design/gdd/map-system.md** — the earlier `EncounterSO` + `RoomEnemySpawner` spawn plan this system superseded (2026-07-02).
- **.claude/rules/engine-code.md** (Zero-Alloc Hot Paths), **.claude/rules/scriptableobject-data.md** (this ADR's Amendment is a documented, intentional exception to "all gameplay config in SO assets" — not a silent violation), **.claude/rules/gameplay-code.md** — the standards this decision honors (or explicitly, recordedly, deviates from).
