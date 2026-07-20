# Sprint 5 — Daily Plan & Progress Tracker

> **Sprint**: 2026-07-13 (Mon) → 2026-07-17 (Fri) — **reopened**, window extended by 1 day
> **Companion to**: `sprint-05.md` (formal plan) — this file is the day-by-day breakdown + live tracker
> **Maintained by**: PM assistant — updated each session/standup
> **Routines (Claude Code)**:
>   - **Mon–Fri 10:00** → `/daily-standup`
>   - **Sat 22:00** → `/weekly-wrapup`
>   - **Sun 22:00** → `/weekly-kickoff`
> **Last updated**: 2026-07-16 (Thu, day 4 standup) — Wed landed S5-B2 (BUG-06 player death, finally
> fixed) as a side effect of a 4th consecutive day of off-plan pooling-system work; B3/B4/B5/C1/C3/C4
> still code-verified broken. See 2026-07-16 log entry for full detail.

---

## Status Verdict: 🔴 CONCERNS — window 07/13→07/17, day 4 of 4 (Thu 07/16) starting at 1.35d
Must-Have burned. Wed (07/15) did not execute the affirmed Wed plan (B2→B3→B4→C1→C4) as sequenced —
instead a 4th consecutive day of off-plan work landed (`ObjectPoolManager`/`Pool`/`IPoolable` pooling
infra + `EnemySpawner`/`RoomModel` wiring, commits `314f19b`/`2552485`/`2c20b0f`/`cc543ba`). One
genuine Must-Have win came out of it anyway: `314f19b` implements `NegativeReciver.TakeDamage()`
(BUG-06/S5-B2) — decrements HP, emits `ON_PLAYER_DEATH`. Confirmed by direct read of
`NegativeReciver.cs` (no more `throw NotImplementedException`). B3/B4/B5 (entity death chain) and
C1/C3/C4 (spawn stabilization) remain exactly as broken as sprint start — verified by direct code
read below, not by log inference.

---

## Burn Summary

| Metric | Value |
|--------|-------|
| Total work estimated | Must-Have ≈ 3.95d (Track A ~1.75 + B ~1.6 + C ~1.1 incl. BUG-ES-4, overlapping) + Should ~0.6d + Nice ~1.75d |
| Capacity (sprint) | 4 days (5 − 20% buffer) |
| Days elapsed | 3 |
| Days remaining | 1 (Thu 07/16 — last non-wrapup day; Fri 07/17 is wrap-up day) |
| Work committed/done | 1.35d Must-Have (S5-A1 0.5 + S5-A2 0.25 + S5-B1 0.1 + S5-B2 0.5 of ~3.95d) — S5-B2 landed Wed (side effect of off-plan pooling work) |
| Velocity | 1.35d / 3 days burned — still badly off pace; ~2.6d of Must-Have work remains against 1 day of capacity |

---

## Task Estimates

| ID | Task | Est (d) | Track/Pri | Status |
|----|------|---------|-----------|--------|
| S5-A1 | Option C full spec into GDD + resolve 5 open-Qs | 0.5 | A / Must | ✅ Done |
| S5-A2 | ADR-0003 ratify Option C | 0.25 | A / Must | ✅ Done |
| S5-A3 | `EnemyModal` refactor (`weight`→`cost` clamp, +`spawnChance`/`tier`) + migrate 6 assets | 0.5 | A / Must | 🟡 In progress — Candidate-Pool `GetSpawnSet()` now **committed** (`RoomModel.cs`, verified 2026-07-15) with `rarityTier` field added, but `EnemyModal` is a plain `[System.Serializable]` class again, not an SO asset — owner decision from Mon log still unresolved, no assets migrated |
| S5-A4 | `RoomModel` refactor (+`roomType`/`budgetTolerance`, −dead fields) + migrate assets | 0.5 | A / Must | 🟡 In progress — `GetSpawnSet()` Candidate-Pool loop committed but **2 known logic gaps still unfixed** (verified 2026-07-15): (1) retry>4 fallback `AddRange(enemiesOfRoom)` ignores `weightBudget` filter — can overshoot budget unbounded; (2) loop stop condition only checks lower-tolerance bound, not eligibleSet-empty. `roomType`/`budgetTolerance` fields not present |
| S5-B1 | Add `ON_ENEMY_DEATH`/`ON_ROOM_CLEAR`/`ON_PLAYER_DEATH` to `EventID` | 0.1 | B / Must | ✅ Done |
| S5-B2 | `NegativeReciver.TakeDamage()` + `ON_PLAYER_DEATH` (BUG-06) | 0.5 | B / Must | ✅ Done — landed `314f19b` 2026-07-15, verified fixed by direct code read 2026-07-16 |
| S5-B3 | `EntityMoveState` null-guard (BUG-05) | 0.25 | B / Must | ⬜ Not started |
| S5-B4 | `EntityDeathState : EntityState` (BUG-07) | 0.5 | B / Must | ⬜ Not started |
| S5-B5 | `EntityBasicState` death transition + `ON_ENEMY_DEATH` (BUG-08) | 0.25 | B / Must | ⬜ Not started |
| S5-C1 | `GetSpawnSet()` empty-list not null (BUG-ES-1) | 0.25 | C / Must | ⬜ Not started |
| S5-C2 | Markerless-room fallback | 0.25 | C / Must | ⬜ Not started |
| S5-C3 | Dedupe spawn driver (BUG-ES-2) | 0.5 | C / Must | ⬜ Not started |
| S5-C4 | Guard `EnemySpawner.cs:60` empty `spawnPosition` list (BUG-ES-4) | 0.1 | C / Must | ⬜ Not started |
| ~~S5-D1~~ | ~~Reapply carried WIP (`cb099ee` + stash)~~ | 0.25 | Should | ✅ Done |
| S5-D2 | ADR-0002 Proposed→Accepted | 0.1 | Should | ⬜ Not started |
| S5-D3 | S4-05 `CancelInvoke` pairing | 0.25 | Should | ⬜ Not started |
| S5-D4 | Quick cleanup batch (AH-2/EM-2/WM-2/LM-1) | 0.25 | Should | ⬜ Not started |
| S5-N1 | Start Candidate-Pool `GetSpawnSet()` rewrite (stretch) | 0.75 | Nice | ⬜ Not started |
| S5-N2 | S4-06 `TalentManager` → SO | 1.0 | Nice | ⬜ Not started |

Status legend: ⬜ Not started · 🟡 In progress · ✅ Done · ⏸️ Blocked · ✂️ Cut

---

## Day-by-Day Breakdown

### Mon 07/13 — PLAN
**Goal: Land the events enum, the Option C design-lock (spec + ADR), and player death.**

| # | Task | Est | Priority |
|---|------|-----|----------|
| 1 | **S5-B1** — add `ON_ENEMY_DEATH`/`ON_ROOM_CLEAR`/`ON_PLAYER_DEATH` to `EventID` (do first — unblocks B2/B5) | 0.1d | 🔴 Must |
| 2 | **S5-A1** — write full Option C spec into GDD; resolve the 5 open-Qs (band under-spend, room→preset, pick algorithm, `roomType` role, RNG) | 0.5d | 🔴 Must |
| 3 | **S5-A2** — `/architecture-decision` → ADR-0003 ratifying Option C | 0.25d | 🔴 Must |
| 4 | **S5-B2** — `NegativeReciver.TakeDamage()`: decrement HP, emit `ON_PLAYER_DEATH` at 0 (BUG-06) | 0.5d | 🔴 Must |

### Tue 07/14 — PLAN
**Goal: Data-model refactor for Option C + start the enemy death chain.**

| # | Task | Est | Priority |
|---|------|-----|----------|
| 1 | **S5-A3** — `EnemyModal`: `weight`→`cost` (`[Range(1,99)]`+clamp, `[FormerlySerializedAs("weight")]`), add `spawnChance`/`tier`; migrate 6 assets | 0.5d | 🔴 Must |
| 2 | **S5-A4** — `RoomModel`: add `roomType`/`budgetTolerance`, remove dead fields; migrate assets | 0.5d | 🔴 Must |
| 3 | **S5-B3** — `EntityMoveState` null-guard to top (BUG-05) | 0.25d | 🔴 Must |

### Wed 07/15 — PLAN
**Goal: Finish the enemy death chain + spawn safety guards.**

| # | Task | Est | Priority |
|---|------|-----|----------|
| 1 | **S5-B4** — `EntityDeathState : EntityState` rewrite + wire (BUG-07) | 0.5d | 🔴 Must |
| 2 | **S5-B5** — `EntityBasicState` death transition + emit `ON_ENEMY_DEATH` (BUG-08) | 0.25d | 🔴 Must |
| 3 | **S5-C1** — `GetSpawnSet()` empty-list not null (BUG-ES-1) | 0.25d | 🔴 Must |
| 4 | **S5-C2** — markerless-room fallback (room-centre + warning) | 0.25d | 🔴 Must |
| 5 | **S5-C4** — guard `EnemySpawner.cs:60` empty `spawnPosition` read (BUG-ES-4) | 0.1d | 🔴 Must |
| 6 | **Smoke** — Play Mode: melee enemy to 0 HP → `EntityDeathState` reached, `ON_ENEMY_DEATH` once | — | Advisory |

### Thu 07/16 — PLAN (re-sequenced at day-4 standup — original table superseded)
**Goal: last non-wrapup day. S5-B2 landed Wed; B3/B4/B5/C1/C3/C4 all confirmed still broken by
direct code read. Only 1 day of capacity left — prioritize smallest, dependency-free Must-Haves
first so at least some of the death/spawn pillar closes before Friday wrap-up.**

| # | Task | Est | Priority |
|---|------|-----|----------|
| 1 | **S5-B3** — `EntityMoveState` null-guard to top of `LogicUpdate()` (BUG-05) | 0.25d | 🔴 Must — no dependency, confirmed still broken (dereferences `entity.Input.Target.transform.position` before the null check below it) |
| 2 | **S5-C1** — `RoomModel.GetSpawnSet()` return `new List<>()` not `null` on empty pool; add the missing null-guard in `EnemySpawner.GetRoomSpawnSet()` around the return value (BUG-ES-1) | 0.25d | 🔴 Must — no dependency, confirmed still broken |
| 3 | **S5-C4** — guard `EnemySpawner.SpawnRoomEnemies()` empty `spawnPosition` read before `Random.Range` (BUG-ES-4) | 0.1d | 🔴 Must — no dependency, confirmed still broken, pairs with #2 (same file) |
| 4 | If time: **S5-B4** — `EntityDeathState : EntityState` rewrite + wire into `EntityStateMachine` (BUG-07) | 0.5d | 🔴 Must — depends on B3, confirmed still `: MonoBehaviour` stub |
| 5 | If time: **S5-B5** — `EntityBasicState` death transition + `ON_ENEMY_DEATH` (BUG-08) | 0.25d | 🔴 Must — depends on B4, confirmed still empty block |
| — | **Cut from today**: S5-C3 (dedupe driver), S5-D2 (ADR-0002 accept), S5-D3/D4 (cleanup) — recommend carry to Sprint 6 if items 1-3 (0.6d) plus any of 4-5 don't leave room |

### Fri 07/17 — WRAPUP DAY
**Goal: Full-loop smoke-check + `/weekly-wrapup`.**

| # | Task | Est |
|---|------|-----|
| 1 | Full Play Mode pass: take damage → player dies → `ON_PLAYER_DEATH`; kill enemy → `ON_ENEMY_DEATH` → `EntityDeathState`; empty-pool + markerless + empty-spawn-position rooms all load without throw | — |
| 2 | `/weekly-wrapup` — review week's `.cs`, playtest log if any, bug triage, light retro | — |
| 3 | Record carry-over + velocity in `sprint-05.md` if anything slips | — |
| 4 | Confirm `/qa-plan sprint` was run (blocking for next gate); flag if not | — |
| 5 | If Must-Haves done early: **S5-N1** (start Candidate-Pool rewrite) | — |

---

## Risks (live — updated each standup)

| Risk | Status | Mitigation |
|------|--------|------------|
| Scope fails to execute a 2nd time (already slipped once, 0 days) | 🔴 WATCH | If this window also closes near 0%, investigate the execution gap before re-carrying a 3rd time |
| `weight`→`cost` rename drops serialized values on 6 assets | 🔴 WATCH | Use `[FormerlySerializedAs("weight")]` on `cost`; verify each asset opens with value intact before moving on |
| Scope heavy (design + refactor + death chain + BUG-ES-4 in 4d) | 🟡 WATCH | Algorithm rewrite + `EnemyManager` deferred to S6; N1/N2 stretch only |
| Off-plan work recurs | 🟡 WATCH | Track A + B are critical path; hold new spawn-feature work until they land |
| No QA plan (lean) | 🔴 OPEN | Run `/qa-plan sprint` before Track B |
| No Unity CLI | 🟢 KNOWN | Play Mode smoke = manual in-Editor |
| `EnemyModal` deleted as an SO, folded into `RoomModel.cs` as a plain serializable class (2026-07-14 finding) | 🔴 OPEN — still unresolved 2026-07-15 | Confirm with owner whether losing shared-asset reuse across rooms is intentional; reconcile against ADR-0003 before authoring room enemy lists |
| `GetSpawnSet()` Candidate-Pool rewrite now committed but still has 2 logic gaps (fallback ignores budget filter; stop condition doesn't check eligibleSet-empty) | 🔴 OPEN — confirmed still present in code 2026-07-15, not fixed Tue as planned | Fix before this is treated as done — see 2026-07-14 log entry for detail; re-verify against the GDD's worked example |
| Off-plan work recurring — now 3 days running (architecture review + Candidate-Pool rewrite Mon; Player/Entity `Base/` framework unification + `StatusAnimation` enum adoption Tue — neither was on the Mon or Tue task list) | 🔴 ESCALATED from WATCH | Did not stabilize by Wed standup as flagged. Zero Track B (death loop) or Track C (spawn stabilization) Must-Have items have moved in 2 full days. Recommend owner explicitly re-plan Wed/Thu around only S5-B2→B5 and S5-C1→C4 — the sprint goal's death-loop pillar is now the at-risk item, not the design-lock pillar |
| ~~Sprint work scattered across 3 branches~~ | 🟢 RESOLVED 2026-07-16 | `sprint-05` tip now equals `origin/feature/spawn-enemy` tip (`cc543ba`) — verified `git merge-base --is-ancestor`. Branches converged; no reconciliation needed before Friday wrap-up |
| **NEW** — 4th consecutive day of off-plan work (pooling infra: `ObjectPoolManager`/`Pool`/`IPoolable`) — only 1 day of Must-Have capacity (Thu) remains before Fri wrap-up, with B3/B4/B5/C1/C3/C4 all still open | 🔴 ESCALATED | No further off-plan/architecture work should be picked up Thu — see re-sequenced Thu plan above; if B4/B5/C3 don't land, explicitly carry to Sprint 6 rather than silently rolling |

---

## Daily Log
> Owner reports what was done; PM updates estimates/status here each standup.

- **2026-07-11 (Fri) — plan renewed**: Original kickoff sprint-05 draft (combat-loop-only, EnemyManager-body-as-Must-Have) replaced after this week's enemy-spawn doc-sync. Doc-sync established: data+algorithm layer already built, `Tile_Spawn_Enemy` parser exists, `EnemySpawner` wired, and owner committed to Option C (Room Budget + Candidate Pool + Spawn Chance). Renewed plan approved via plan mode. Re-scoped into Track A (adopt Option C — spec + ADR-0003 + data refactor), Track B (combat death loop), Track C (spawn stabilization); deferred the `GetSpawnSet()` rewrite + `EnemyManager` runtime to Sprint 6. No QA plan yet — flagged. `gh` unavailable — draft PR not created.
- **2026-07-11 (Sat 22:00) — automated interim wrap-up**: Ran `/weekly-wrapup` one day after the plan renewal, still 0 days into the 07-14→07-18 execution window. Code review found the carried WIP (`EnemySpawner.cs` padding fields, S5-D1) staged but uncommitted, and surfaced 3 new bugs on the same file: BUG-ES-4 (P1 — unguarded `spawnPosition` read, same shape as BUG-ES-1), BUG-ES-6 (P2 — padding has no room-bounds clamp), BUG-ES-5 (P3 — scratch values needlessly `[SerializeField]`). Full findings: `production/qa/bug-triage-2026-07-11.md`. Retro (light, interim): `production/retros/retro-interim-2026-07-11.md`. No playtest this window (none logged since 2026-06-12).
- **2026-07-13 (Mon) — reopened, not rolled to Sprint 6**: The 07/14→07/18 window closed at 0% Must-Have — only S5-D1 (padding fields) landed. Because nothing else executed, this stays Sprint 5 with the window extended to 07/13→07/17 rather than incrementing to a new sprint number. S5-D1 marked done; BUG-ES-4 (unguarded `spawnPosition[Random.Range(...)]` read at `EnemySpawner.cs:60`, confirmed still present) added as S5-C4. All other Must-Have/Should-Have/Nice-to-Have tasks unchanged. QA plan for this scope still doesn't exist — 2nd cycle unresolved, flagged again.
- **2026-07-13 (Mon 10:00) — day 1 standup**: `git log --since="yesterday"` shows only the reopen commit (`bc8d99e`, tracker/plan edits) — no `Assets/` changes since the reopen. Day 1 of the extended window begins now; 0 of 4 Must-Have days burned yet. No status changes to today's task table. Today's plan unchanged from the Mon 07/13 breakdown: S5-B1 → S5-A1 → S5-A2 → S5-B2.
- **2026-07-13 (Mon, post-standup chat) — Option C spec locked with owner, not yet written to GDD**: Owner worked through the Option C ("Room Budget + Candidate Pool + Spawn Chance") open questions interactively and locked a concrete design, superseding the `SpawnChance` float sketch in `design/gdd/enemy-spawn-system.md:628-636`. This session could not write to `design/gdd/` (standup hard rule: only `production/**/*.md`), so the locked decisions are captured here for the session that runs S5-A1/S5-A2/S5-A3.

  **Data model (replaces the `SpawnChance` float idea):**
  ```csharp
  public enum RarityTier { Common = 50, Rare = 30, Epic = 15, Legendary = 5 }  // % roll chance, NOT a weighted-pick pool

  public class EnemyModal : EntityModel
  {
      public GameObject prefab;
      public int weight;          // owner confirmed: weight IS cost, keep the field name — DROP the weight→cost rename planned in S5-A3
      public RarityTier rarityTier;
  }

  public class RoomModel : EntityModel
  {
      [SerializeField] private List<EnemyModal> enemiesOfRoom = new List<EnemyModal>();
      // no wrapper class — per-room variety comes from authoring separate EnemyModal asset
      // variants (e.g. Bat_Common.asset vs Bat_Rare.asset, same prefab, different weight/tier)
      // and choosing which variants go in which room's list
  }
  ```
  `weight`/cost and `rarityTier` are fully independent — owner explicitly rejected any inverse-correlation enforcement (no `OnValidate` cross-check needed).

  **8-step Candidate-Pool flow (resolves Open Q#8's Option C evaluation + the step-6/step-8 gaps flagged in the GDD's own weaknesses list):**
  ```
  1. Per enemy to spawn — start a pick round, retryCount = 0
  2. eligibleSet = filter enemiesOfRoom where weight ≤ remaining budget (empty → stop, step 8)
  3. For each entry in eligibleSet — roll r = Random.value; PASS if r ≤ tier chance
     (Common 0.50 / Rare 0.30 / Epic 0.15 / Legendary 0.05)
  4. PASS entries → Candidate Pool
  5. Candidate Pool empty → retryCount++
       retryCount ≤ 4 → back to step 3 (re-roll the same eligibleSet)
       retryCount > 4 → Candidate Pool = eligibleSet (fallback, ignore chance — guarantees
       termination; this bounded-retry fallback is the piece the GDD flagged as unformalized)
  6. Pick 1 from Candidate Pool — uniform random (owner confirmed, no weighted/argmin pick)
  7. remaining -= picked.weight; add to spawn result; reset retryCount = 0
  8. Repeat 1→7 until remaining ∈ ToleranceBand [B×0.9, B×1.1] or eligibleSet is empty
  ```

  **Plan deltas this creates:**
  - S5-A1 (GDD spec): write this flow into Detailed Design + Formulas (band math + worked example)
    + Edge Cases (empty eligibleSet, weight ≤ 0 hang risk carried over from Option A, retry-cap
    exhaustion) + Acceptance Criteria (budget bound, retry-cap termination, band-tolerance) +
    resolve Open Q#8 as Option C, chosen.
  - S5-A3 (data refactor): scope changes from "rename weight→cost + clamp" to "add `RarityTier`
    enum + `rarityTier` field to `EnemyModal`, no rename." Re-estimate if needed at next standup.
  - S5-A2 (ADR-0003): ratifies this exact flow, not the original float-`SpawnChance` sketch.
- **2026-07-13 (Mon, continued) — S5-A1 done**: All 5 open-Qs resolved (band under-spend, pick
  algorithm = uniform random, Cost/Tier independence, room→preset = kept `MapModel.GetRandomRoom()`
  shuffle-bag / no per-room mapping built (Q#4), RNG = `UnityEngine.Random` unseeded (Q#2), `RoomType`
  = design-time classification only, not consumed by the flow). Written to
  `design/gdd/enemy-spawn-system.md` via `/design-system` retrofit — commits `0d7a5b7`, `4fc04c5`.
  S5-A1 marked ✅ Done. Next up per Mon sequencing: S5-A2 (ADR-0003 ratifying Option C), or S5-B1
  (`EventID` additions, 0.1d, no dependency, fastest win).
- **2026-07-13 (Mon, rest of day) — S5-A2 + S5-B1 landed, plus an off-plan architecture pivot**:
  `git log` for Mon shows, in order: `0c1a7b4` ADR-0003 ratifying Option C (S5-A2 ✅), `c355d08`
  full `/architecture-review` (not on the Mon list — bonus/off-plan, CONCERNS verdict, flags Event
  Bus + `INegativeReceiver` as HIGH-priority Foundation gaps; ran at 08:59 UTC, **before** the
  commits below), then `2151ec1`/`8f19ae6`/`39ddc21` ("update logic spawn enemy" / "polish code" ×2)
  at 16:38+ local. `EventID.ON_ENEMY_DEATH`/`ON_ROOM_CLEAR`/`ON_PLAYER_DEATH` added in `2151ec1`
  (S5-B1 ✅). S5-B2 (`NegativeReciver.TakeDamage()`) did **not** land — still `throw new
  NotImplementedException()`, confirmed via grep. Instead, the owner jumped straight to implementing
  the actual Candidate-Pool `GetSpawnSet()` algorithm in `RoomModel.cs` (this is S5-N1 stretch
  scope, and it folds in what S5-A3/S5-A4 were meant to do incrementally) — done directly rather
  than through the planned refactor-then-implement sequence.
  **Unplanned architecture change, not yet reviewed**: `EnemyModal.cs` (an `EntityModel`-derived
  ScriptableObject) was deleted outright in `2151ec1`; its fields (`prefab`→`Prefab`, `weight`) plus
  the new `rarityTier` were folded into a plain `[System.Serializable] class EnemyModal` declared
  inline in `RoomModel.cs`. This is a bigger change than "add a field" — `EnemyModal` is no longer an
  asset type, so it can't be authored as a shared, drag-drop-reusable SO across rooms the way the
  Mon-locked data model assumed. No `.asset` files reference it yet (S5-A3's "migrate 6 assets" step
  never ran), so nothing is broken today, but this deviates from the data model locked in the Mon log
  entry and from ADR-0003 as ratified — needs an owner decision before more rooms are authored around
  it. Flagging, not blocking.
  **Uncommitted working-tree state at standup time** (`git diff --stat`, 5 files, 8+/8−): the
  `GetSpawnSet()` rewrite above, `entry.enemy.prefab`→`.Prefab` casing fixups in `EnemySpawner.cs` /
  `LevelManager.cs`, a dead `_tileSetDelay` field removed from `RoomGeneraterController.cs`, and an
  `AbilityHolder.Start()` fix (`private`→`protected override` + `base.Start()` call — was silently
  skipping `CoreComponent.Start()`). None of this is committed. Two logic notes on the uncommitted
  `GetSpawnSet()` for whoever picks this back up: (1) `SetListCandidate`'s retry>4 fallback does
  `candidateEnemies.AddRange(enemiesOfRoom)` — the *whole* room list, not just entries that still fit
  `weightBudget` — so the guaranteed-termination fallback can push the spend over budget by an
  unbounded amount instead of the design's "eligibleSet, budget-respecting" fallback; (2) the outer
  loop's stop condition (`weightBudget > this.weightBudget * 0.1f`) only encodes the lower-tolerance
  bound from the design's step 8, not "eligibleSet is empty" — currently masked by (1) always
  returning a non-empty candidate pool, so it doesn't hang, but the band/overshoot math should be
  double-checked against the GDD's worked example before this ships.
  **Branch note**: standup did not `git checkout sprint-05` — the actual working branch
  (`origin/feature/spawn-enemy`) is 3 commits ahead of `sprint-05` (`2151ec1`/`8f19ae6`/`39ddc21`)
  plus the uncommitted diff above; checking out the older `sprint-05` tip would have collided with
  those uncommitted changes. Recommend committing the WIP first, then merging/rebasing this feature
  branch onto `sprint-05` so the tracker and the code branch stay in sync.
- **2026-07-14 (Tue 02:00) — day 2 standup**: Day 1 closed at 0.85d Must-Have (S5-A1/A2/B1 done),
  below the ~1.35d planned for Mon (S5-B2 slipped), but with unplanned extra progress on S5-A3/A4/N1
  merged into one uncommitted `GetSpawnSet()` rewrite. Today's plan, re-sequenced around that reality
  instead of the original Tue table:
  1. **Commit + harden the uncommitted `GetSpawnSet()` WIP** — fix the retry-fallback budget filter
     and the eligibleSet-empty stop condition noted above — 0.3d (🔴 Must, no new scope, closes out
     what's already 90% done)
  2. **Resolve the `EnemyModal` SO-vs-plain-class question with the owner** and note the decision
     against ADR-0003 — 0.15d (🔴 Must, blocks safely authoring room enemy lists)
  3. **S5-B2** — `NegativeReciver.TakeDamage()` + emit `ON_PLAYER_DEATH` (BUG-06) — 0.5d (🔴 Must,
     carried from Mon, `EventID` values now exist so this is unblocked)
  4. **S5-B3** — `EntityMoveState` null-guard to top of `LogicUpdate()` (BUG-05) — 0.25d (🔴 Must,
     no dependency)
  Total ≈ 1.2d planned for today. S5-A3/S5-A4 as originally scoped (separate incremental refactors)
  are superseded by item 1 above — table status reflects "in progress" rather than "not started."
- **2026-07-15 (Wed 02:00) — day 3 standup**: Checked out `sprint-05`, fast-forwarded to
  `origin/sprint-05` (974cf8b). None of Tue's 4 planned items landed. `git log --since=2026-07-14`
  on the actual working branch (`origin/feature/spawn-enemy`, not `sprint-05` — see branch-scatter
  risk below) shows Tue's real commits: `aa8720f` "unify Player and Entity onto a shared framework"
  (03:08, Claude session) — new `Assets/Script/Character/Base/` with `IState`/`StateMachine<TState>`/
  `CoreBase`/`CoreComponentBase`/`DirectionResolver`, 18 files touched; `4a73d16` "adopt
  StatusAnimation enum in EntityState" (08:01, Claude session) — mirrors Player's animation-status
  pattern onto Entity, 8 files; `a8089c5` "coding" (10:18, owner) — wiring/meta files for the above
  plus stray casing fixups; `aa247e6` "remove file trash" (11:42, owner) — deleted the old
  `Assets/Backup/` tree and dead `Assets/Script/Enemy/NewEnemy*.cs` stub files. None of this is on
  the Sprint 5 task list — it's a bigger architectural move (framework unification) than anything
  scoped for this sprint, executed via Claude Code agent sessions outside the tracked plan.

  **Code-verified bug status** (read `origin/feature/spawn-enemy` tip `4a73d16` directly, no edits
  made — read-only per standup hard rule):
  - S5-B2 (BUG-06): `NegativeReciver.TakeDamage()` still `throw new System.NotImplementedException()`
    — confirmed NOT fixed.
  - S5-B3 (BUG-05): `EntityMoveState.LogicUpdate()` still dereferences
    `entity.Input.Target.transform.position` before the `if (entity.Input.Target == null)` guard
    below it — confirmed NOT fixed.
  - S5-B4 (BUG-07): `EntityDeathState` still `: MonoBehaviour`, empty `Start`/`Update` stubs —
    confirmed NOT fixed.
  - S5-B5 (BUG-08): `EntityBasicState`'s `Health <= 0` block is still empty (`{ }`, no transition) —
    confirmed NOT fixed.
  - S5-C1 (BUG-ES-1): `RoomModel.GetSpawnSet()` still `return null` when `enemiesOfRoom.Count == 0`.
    `EnemySpawner.GetRoomSpawnSet()` only null-guards `roomModel` itself, NOT the `GetSpawnSet()`
    return value — an empty-pool room will still NRE at `SpawnRoomEnemies()`'s `set.Count` check.
    Confirmed NOT fixed.
  - S5-C4 (BUG-ES-4): `EnemySpawner.SpawnRoomEnemies()` — `spawnPosition[Random.Range(0,
    spawnPosition.Count)]` — still unguarded against an empty `spawnPosition` list. Confirmed NOT
    fixed.
  - S5-D2: ADR-0002 (`docs/architecture/adr-0002-enemymanager-singleton-exception.md`) — `Status:
    Proposed`, unchanged. Confirmed NOT fixed.
  - No QA plan file exists anywhere in `production/qa/` — gate still open, 3rd cycle unresolved.

  **Net effect**: the sprint goal's 2nd pillar (combat death loop) has had zero code movement across
  2 full days (Tue + the Mon afternoon that also went off-plan). All 5 death/spawn bugs carried from
  Sprint 4 are still exactly as they were at sprint start. The only forward motion this window is
  Track A's design-lock (done Mon) plus an unplanned framework refactor that, while plausibly good
  engineering, does not close any Sprint 5 acceptance criterion.

  **Today's plan (Wed 07/15)** — re-affirming the original Wed table since nothing from Tue
  substitutes for it; treat as the sprint's last full day before Thu's dedupe-driver work and Fri's
  wrap-up:
  1. **S5-B2** — `NegativeReciver.TakeDamage()`: decrement HP, emit `ON_PLAYER_DEATH` at 0 (BUG-06) —
     0.5d, 🔴 Must, carried 2 days, `EventID` values already exist so fully unblocked
  2. **S5-B3** — `EntityMoveState` null-guard moved to top of `LogicUpdate()` (BUG-05) — 0.25d, 🔴
     Must, no dependency, smallest remaining Must-Have
  3. **S5-B4** — `EntityDeathState : EntityState` rewrite + wire into `EntityStateMachine` (BUG-07) —
     0.5d, 🔴 Must, depends on S5-B3 landing first per original sequencing
  4. **S5-C1** — `RoomModel.GetSpawnSet()` return `[]` not `null` on empty pool; guard both driver
     call sites (BUG-ES-1) — 0.25d, 🔴 Must, small, no dependency
  5. **S5-C4** — guard `EnemySpawner.cs` empty `spawnPosition` read (BUG-ES-4) — 0.1d, 🔴 Must, small,
     no dependency, pairs naturally with S5-C1 in the same file area
  6. If time: **S5-B5** — `EntityBasicState` death transition + `ON_ENEMY_DEATH` (BUG-08), 0.25d, 🔴
     Must, depends on S5-B4
  Total ≈ 1.6d against 1 remaining full day (Thu is the last non-wrapup day) — **not achievable in
  full**; recommend dropping S5-B5 to Thu and treating items 1–5 as today's ceiling. Realistic today:
  items 1–2 (0.75d) at minimum, items 3–5 stretch.
  **Blockers**: none technical — every remaining Must-Have item is unblocked. The real blocker is
  process: 2 of 4 sprint days spent on unplanned work. No further design/architecture work should be
  picked up before S5-B2→B5 and S5-C1→C4 land.
  **Risks carried forward**: branch scatter (new, see Risks table); QA plan still missing (3rd
  cycle); `EnemyModal` SO-vs-plain-class decision still unresolved.

- **2026-07-16 (Thu 02:00) — day 4 standup**: Checked out `sprint-05` (already up to date with
  `origin/sprint-05`, tip `cc543ba`). `git log --since=2026-07-15` shows 4 commits, all same-day
  (13:52–22:24 local): `314f19b` "add pooling manager and pooling object", `2552485`/`2c20b0f`
  "polish" ×2, `cc543ba` "done prototy spawn enemy". None of these are on the Wed task list — a 4th
  consecutive day of off-plan work, this time building a generic object-pool system
  (`Assets/Script/Poolable/ObjectPoolManager.cs`, `Pool.cs`, `IPoolable.cs`) and wiring it into
  `EnemySpawner`/`RoomModel`/`MapModel`.

  **Good news buried in the off-plan work**: `314f19b` also implements
  `NegativeReciver.TakeDamage()` (BUG-06/S5-B2) — decrements `currentHealth`, emits
  `ON_PLAYER_DEATH` at 0. Verified by direct read of `NegativeReciver.cs` — no more
  `throw NotImplementedException`. This closes a Must-Have that had carried since Mon. Marked ✅ Done.

  **Code-verified bug status** (direct read of `sprint-05` tip `cc543ba`, no edits made — read-only
  per standup hard rule):
  - S5-B2 (BUG-06): ✅ **Fixed** — see above.
  - S5-B3 (BUG-05): `EntityMoveState.LogicUpdate()` still dereferences
    `entity.Input.Target.transform.position` (the `Vector2.Distance(...)` call) before the
    `if (entity.Input.Target == null)` guard several lines below it — confirmed NOT fixed.
  - S5-B4 (BUG-07): `EntityDeathState` still `: MonoBehaviour` with empty `Start`/`Update` stubs —
    confirmed NOT fixed.
  - S5-B5 (BUG-08): `EntityBasicState`'s `Health <= 0` block is still an empty `{ }` (no transition
    to `EntityDeathState`) — confirmed NOT fixed.
  - S5-C1 (BUG-ES-1): `RoomModel.GetSpawnSet()` still `return null` when `enemiesOfRoom.Count == 0`
    (line 16). `EnemySpawner.GetRoomSpawnSet()` only null-guards `roomModel` itself, not the
    `GetSpawnSet()` return value — an empty-pool room still risks NRE at
    `SpawnRoomEnemies()`'s `set.Count` check. Confirmed NOT fixed.
  - S5-C4 (BUG-ES-4): `EnemySpawner.SpawnRoomEnemies()` — `spawnPosition[Random.Range(0,
    spawnPosition.Count)]` — still unguarded against an empty `spawnPosition` list. Confirmed NOT
    fixed.
  - S5-D2: ADR-0002 status unchanged (not re-checked this standup — no evidence of edits to
    `docs/architecture/` in yesterday's commits).
  - **Branch scatter risk resolved**: `sprint-05` and `origin/feature/spawn-enemy` have converged —
    both point at `cc543ba`. No merge/rebase needed before Friday wrap-up.
  - No QA plan file exists anywhere in `production/qa/` — gate still open, 3rd cycle unresolved.

  **Net effect**: 1.35d of ~3.95d Must-Have work done after 3 of 4 sprint days (S5-A1/A2/B1/B2).
  ~2.6d of Must-Have work remains against exactly 1 day of capacity (today) before Friday's
  wrap-up-only day — not achievable in full. Today's plan re-sequenced above around the 3 smallest,
  dependency-free confirmed-still-broken bugs (S5-B3, S5-C1, S5-C4 — 0.6d combined) so at least
  those close today regardless of what else happens; S5-B4/B5/C3 are explicit stretch/carry
  candidates for the Friday retro to decide on.
  **Blockers**: none technical. Process blocker unchanged: 3 of 4 days spent on unplanned pooling/
  framework work instead of the tracked Must-Have list. Recommend the Friday `/weekly-wrapup`
  retro explicitly address why off-plan work has now recurred 4 days running.

### Sprint Close-Out — 2026-07-20 (delayed Sat 22:00 wrap-up)

> Fri 07/17's planned wrap-up did not run on schedule; this is the catch-up run, executed 2026-07-20.
> Full detail: `production/retros/retro-sprint-05-2026-07-20.md`, `production/qa/bug-triage-2026-07-20.md`.

**Final Verdict: 🔴 CONCERNS (bordering FAIL vs sprint goal)**

**Final Burn**: 1.35d / 3.95d Must-Have committed (34%). S5-A1/A2/B1/B2 done; S5-B3/B4/B5, S5-C1/C2/
C3/C4, S5-D2/D3/D4 (partial), S5-N1/N2 not started. Root cause unchanged from every daily checkpoint:
4 consecutive days of off-plan architecture/tooling work (Mon pivot, Tue/Wed `Base/` framework
unification, Thu pooling system) displaced the tracked Track B/C death-loop and spawn-stabilization
tasks.

**Velocity**: 1.35d Must-Have / 7 elapsed days (extended window) — for comparison, Sprint 4 closed at
100% Must-Have completion (`retro-sprint-04-2026-07-10.md`).

**New risks surfaced by this wrap-up's code review** (3 parallel agents: lead-programmer,
ai-programmer, unity-specialist, reviewing `7ca465f..HEAD`, 39 files):
- Possible build break — `PoolMember.cs:9` `[SerializeField]` on an auto-property (CS0592 pattern),
  unverified against an actual Unity compile — **verify first**, Sprint 6 priority 0
- New S1 hang risk — `RoomModel.GetSpawnSet()` infinite loop if any `EnemyModal.weight == 0`
- BUG-06 marked Done is only a partial fix — `NegativeReciver` doesn't write through to
  `PlayerData.currentHealth`, breaking the `Reborn()` reset contract
- `EnemyModal` regressed from a reusable SO asset to a plain per-room class — undoes part of the
  ADR-0003 data model; ADR-0003 itself still reads `Status: Proposed` despite S5-A2 marked Done

**Carry-Over to Sprint 6** (full detail + priority in the bug-triage report):
- BUG-05/07/08 (entity death chain) — 7th carry cycle, make this Sprint 6's literal first task
- BUG-ES-1/ES-4 (spawn null/index guards) — 2nd-3rd carry
- NEW-1 (`GetSpawnSet` weight==0 hang) — new, high priority
- Possible `PoolMember.cs` build break — new, verify immediately
- BUG-06 partial-fix (dual HP source of truth) — re-open, don't treat as closed
- `EnemyModal` SO-vs-plain-class decision — needs explicit owner call
- ADR-0002 Proposed→Accepted — 3rd carry, 0.1d task
- S4-05/S4-06, Skill Enhance ADR — 4th/3rd carry, needs keep-or-cut decision
- First playtest — 6th retro with zero movement, tied to death-chain landing
- `origin/feature/spawn-enemy` 2 commits ahead of `sprint-05` (`dce9be1`/`d653654`, 07-16,
  `RoomGeneraterController.cs`/`RoomGridController.cs`) — merge before Sunday kickoff

**Playtest**: skipped this cycle — no playtest log filed since `playtest-2026-06-12-weekly-wrapup.md`.
Run `/playtest-report` manually if an ad-hoc session happens; otherwise tie the first real session to
Sprint 6's death-chain work landing.

---

### Interim Wrap-Up — 2026-07-11 (Sat 22:00)

- **Verdict**: 🟡 CONCERNS — not a sprint-execution verdict (0 days elapsed), but flagging: (1) carried WIP still uncommitted going into the sprint week, (2) that WIP hides a new P1 (BUG-ES-4) not yet scoped into any task, (3) QA plan for Sprint 5 still doesn't exist (blocking gate before Track B).
- **Carry-over**: unchanged from `sprint-05.md`'s Carry-Over From Sprint 4 table, plus 3 new items found this triage — BUG-ES-4 (fold into S5-C1), BUG-ES-6 (resolve or document in S5-D1), BUG-ES-5 (fold into S5-D1 cleanup). See bug-triage report for detail.
- **Velocity**: N/A — Sprint 5 execution window has not started (0 of 5 days elapsed). Reference: Sprint 4 closed at 100% Must-Have completion (see `retro-sprint-04-2026-07-10.md`).
