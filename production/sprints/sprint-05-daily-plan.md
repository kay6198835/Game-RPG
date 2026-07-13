# Sprint 5 — Daily Plan & Progress Tracker

> **Sprint**: 2026-07-13 (Mon) → 2026-07-17 (Fri) — **reopened**, window extended by 1 day
> **Companion to**: `sprint-05.md` (formal plan) — this file is the day-by-day breakdown + live tracker
> **Maintained by**: PM assistant — updated each session/standup
> **Routines (Claude Code)**:
>   - **Mon–Fri 10:00** → `/daily-standup`
>   - **Sat 22:00** → `/weekly-wrapup`
>   - **Sun 22:00** → `/weekly-kickoff`
> **Last updated**: 2026-07-13 (Mon, reopened) — original 07/14→07/18 window never executed (0 days
> elapsed, only S5-D1 padding fields landed). Stays Sprint 5 rather than rolling to Sprint 6 since
> zero Must-Have work landed; window extended to 07/13→07/17. Adds S5-C4 (BUG-ES-4, found while
> landing S5-D1). S5-D1 marked done and dropped from the active task list.

---

## Status Verdict: ⬜ NOT STARTED (reopened) — window 07/13→07/17. Only carried WIP (S5-D1) is done;
all Must-Have tracks remain untouched.

---

## Burn Summary

| Metric | Value |
|--------|-------|
| Total work estimated | Must-Have ≈ 3.95d (Track A ~1.75 + B ~1.6 + C ~1.1 incl. BUG-ES-4, overlapping) + Should ~0.6d + Nice ~1.75d |
| Capacity (sprint) | 4 days (5 − 20% buffer) |
| Days elapsed | 0 |
| Days remaining | 5 |
| Work committed/done | 0 Must-Have (S5-D1 carried WIP is done, not counted against Must-Have capacity) |
| Velocity | N/A — first real execution attempt for this scope |

---

## Task Estimates

| ID | Task | Est (d) | Track/Pri | Status |
|----|------|---------|-----------|--------|
| S5-A1 | Option C full spec into GDD + resolve 5 open-Qs | 0.5 | A / Must | ⬜ Not started |
| S5-A2 | ADR-0003 ratify Option C | 0.25 | A / Must | ⬜ Not started |
| S5-A3 | `EnemyModal` refactor (`weight`→`cost` clamp, +`spawnChance`/`tier`) + migrate 6 assets | 0.5 | A / Must | ⬜ Not started |
| S5-A4 | `RoomModel` refactor (+`roomType`/`budgetTolerance`, −dead fields) + migrate assets | 0.5 | A / Must | ⬜ Not started |
| S5-B1 | Add `ON_ENEMY_DEATH`/`ON_ROOM_CLEAR`/`ON_PLAYER_DEATH` to `EventID` | 0.1 | B / Must | ⬜ Not started |
| S5-B2 | `NegativeReciver.TakeDamage()` + `ON_PLAYER_DEATH` (BUG-06) | 0.5 | B / Must | ⬜ Not started |
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

### Thu 07/16 — PLAN
**Goal: Dedupe spawn driver + ADR-0002 accept + cleanup if time.**

| # | Task | Est | Priority |
|---|------|-----|----------|
| 1 | **S5-C3** — pick canonical spawn driver, delete the loser (BUG-ES-2) | 0.5d | 🔴 Must |
| 2 | **S5-D2** — ADR-0002 Proposed→Accepted | 0.1d | 🟡 Should |
| 3 | If time: **S5-D3** (`CancelInvoke`) / **S5-D4** (cleanup batch) | 0.5d | 🟡 Should |

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

### Interim Wrap-Up — 2026-07-11 (Sat 22:00)

- **Verdict**: 🟡 CONCERNS — not a sprint-execution verdict (0 days elapsed), but flagging: (1) carried WIP still uncommitted going into the sprint week, (2) that WIP hides a new P1 (BUG-ES-4) not yet scoped into any task, (3) QA plan for Sprint 5 still doesn't exist (blocking gate before Track B).
- **Carry-over**: unchanged from `sprint-05.md`'s Carry-Over From Sprint 4 table, plus 3 new items found this triage — BUG-ES-4 (fold into S5-C1), BUG-ES-6 (resolve or document in S5-D1), BUG-ES-5 (fold into S5-D1 cleanup). See bug-triage report for detail.
- **Velocity**: N/A — Sprint 5 execution window has not started (0 of 5 days elapsed). Reference: Sprint 4 closed at 100% Must-Have completion (see `retro-sprint-04-2026-07-10.md`).
