# Sprint 5 — 2026-07-14 to 2026-07-18

> **Renewed 2026-07-11** — this replaces the original kickoff draft (opened Sunday, before this
> week's enemy-spawn doc-sync). The kickoff draft assumed the spawn data+algorithm was still
> to-be-built and made `EnemyManager` lifecycle a Must-Have with no blocking design decision. The
> doc-sync showed the data+algorithm layer is already built, the `Tile_Spawn_Enemy` parser exists,
> `EnemySpawner` is wired, and the owner committed to the **Room Budget + Candidate Pool + Spawn
> Chance** design (Option C in `design/gdd/enemy-spawn-system.md` → Future Architecture Direction).
> Sprint 5 is re-scoped accordingly.

## Sprint Goal

Lock the enemy-spawn design (adopt the **Room Budget + Candidate Pool + Spawn Chance** model:
full GDD spec + ADR + data-model refactor) **and** land the combat death loop (player death +
enemy death chain + spawn-bug stabilization), so Sprint 6 can wire room-clear on a stable, decided
foundation.

Two pillars:
1. **Adopt Option C** — formalize the reviewed spec into the GDD, ratify it with an ADR, and do the
   *data-model refactor* it needs. The heavier *algorithm rewrite + `EnemyManager` runtime wiring*
   is deliberately scoped to Sprint 6 on this locked foundation — building all of it in one 4-day
   sprint would overrun.
2. **Land the combat death loop** — player death (BUG-06) + enemy death chain (Bugs #5/#7/#8) + the
   small spawn-bug fixes (ES-1/2/3). Genuinely unblocked, carried 5 sprints, the real demo spine.

---

## Capacity

- Total days: 5
- Buffer (20%): 1 day
- Available: 4 days

Must-Have load ≈ 3.85d (Track A ≈ 1.75d + Track B ≈ 1.6d + Track C small ≈ 1.0d, with overlap) —
fits 4 days; Should/Nice are stretch.

---

## Carry-Over From Sprint 4

| Item | Type | Priority | Origin |
|------|------|----------|--------|
| `cb099ee` "random padding position" (not on `sprint-05` branch) + stash `wip-before-sprint-kickoff-2026-07-13` | Carry | P1 | S4 post-wrapup |
| BUG-06 — player death chain (`NegativeReciver.TakeDamage()` throws) | Bug (S1) | P1 | sweep→S5 (5th carry) |
| BUG-05/07/08 — enemy death chain (`EntityMoveState` NRE, `EntityDeathState` base class, `EntityBasicState` empty death block) | Bug (S1) | P1 | sweep→S5 (5th carry) |
| BUG-ES-1/2/3 — spawn null-return NRE / duplicate drivers / missing `EventID` values | Bug | P1/P2 | S4 |
| S4-05 — `CancelInvoke` pairing; S4-06 — `TalentManager` SO | Bug/Task | P2 | S2/S3→S5 |
| ADR-0002 (`EnemyManager` singleton) Proposed→Accepted | Decision | P2 | S4 |

---

## Tasks

### Must Have (P1)

#### Track A — Adopt Option C (design + data foundation)

| ID | Task | Agent/Owner | Est. Days | Dependencies | Acceptance Criteria |
|----|------|-------------|-----------|--------------|---------------------|
| S5-A1 | Write the full Option C spec into `design/gdd/enemy-spawn-system.md` (8-step Candidate-Pool flow, Formulas, Edge Cases, Acceptance Criteria) from the reviewed draft. Resolve the 5 spec open-questions inline: (1) band under-spend handling, (2) room→preset mapping, (3) pick algorithm at step 6, (4) `roomType` role in the flow, (5) RNG source. | game-designer / systems-designer | 0.5 | None | GDD has a complete Candidate-Pool spec section; each of the 5 open-Qs has a locked decision written in the relevant section |
| S5-A2 | `/architecture-decision` → ADR-0003 ratifying Option C as the chosen selection direction (supersedes the Q#8 three-option evaluation). | systems-designer / lead-programmer | 0.25 | S5-A1 | ADR-0003 written; Status Accepted, or Proposed with an explicit owner sign-off path |
| S5-A3 | Refactor `EnemyModal`: rename `weight`→`cost` with `[Range(1,99)]` + `OnValidate` clamp (fixes the `weight ≤ 0` hang, TD/BUG); add `spawnChance` `[Range(0,1)]` and `tier` enum. Migrate the 6 existing enemy SO assets so serialized values survive the rename. | gameplay-programmer | 0.5 | S5-A1 | Fields present + clamped; `cost ≤ 0` no longer authorable; all 6 assets open in Inspector with values intact |
| S5-A4 | Refactor `RoomModel`: add `roomType` and `budgetTolerance` `[Range(0,0.5)]`; remove the dead `randomRatio`/`overflowPercent`/`selectionWeight` fields. Migrate `RoomData`/`RoomModel` assets. | gameplay-programmer | 0.5 | S5-A1 | New fields present; dead fields gone; assets migrate clean, no reset |

> **Scope note:** the `GetSpawnSet()` algorithm rewrite to the Candidate-Pool loop and the
> `EnemyManager` runtime wiring are **Sprint 6** — this sprint locks the spec + data shape they build
> on. Attempting the rewrite this sprint (on top of Track B) overruns 4 days.

#### Track B — Combat death loop

| ID | Task | Bug | Agent/Owner | Est. Days | Dependencies | Acceptance Criteria |
|----|------|-----|-------------|-----------|--------------|---------------------|
| S5-B1 | Add `ON_ENEMY_DEATH`, `ON_ROOM_CLEAR`, `ON_PLAYER_DEATH` to `EventID` (do first — B2/B5 depend) | BUG-ES-3 | gameplay-programmer | 0.1 | None | All 3 values present; pure enum addition, nothing else changed |
| S5-B2 | `NegativeReciver.TakeDamage()` — decrement `PlayerData.currentHealth`; `if (currentHealth <= 0) Emit(ON_PLAYER_DEATH)` | BUG-06 | gameplay-programmer | 0.5 | S5-B1 | No longer throws; hitting player in Play Mode drops HP; event fires at 0; no direct health mutation bypassing the interface |
| S5-B3 | `EntityMoveState.LogicUpdate()` — move `if (entity.Input.Target == null)` guard to the top before the line-30 dereference | BUG-05 | ai-programmer | 0.25 | None | Guard is first statement; losing target mid-chase → Idle, no NRE |
| S5-B4 | Rewrite `EntityDeathState` to extend `EntityState` (not `MonoBehaviour`); wire into `EntityStateMachine` | BUG-07 | ai-programmer | 0.5 | S5-B3 | `EntityDeathState : EntityState`; compiles; reachable via transition |
| S5-B5 | Fill `EntityBasicState` empty `Health <= 0` block → transition to `EntityDeathState`; emit `ON_ENEMY_DEATH` once on entry | BUG-08 | ai-programmer | 0.25 | S5-B4, S5-B1 | Enemy at 0 HP → death state in Play Mode; `ON_ENEMY_DEATH` fires exactly once per death |

#### Track C — Spawn stabilization (small, no algorithm change)

| ID | Task | Bug | Agent/Owner | Est. Days | Dependencies | Acceptance Criteria |
|----|------|-----|-------------|-----------|--------------|---------------------|
| S5-C1 | `RoomModel.GetSpawnSet()` return an empty list, not `null`, on an empty pool; guard both driver call sites | BUG-ES-1 | gameplay-programmer | 0.25 | None | Empty enemy pool → no NullReferenceException in either driver |
| S5-C2 | Markerless-room fallback — empty `spawnPositions` list → room-centre position + warning (12/13 rooms today) | new | gameplay-programmer | 0.25 | None | All 13 rooms load without throwing at spawn time |
| S5-C3 | Pick the canonical spawn driver (`EnemySpawner` event-driven vs `LevelManager.SpawnRoomEnemies()` Editor button); delete the losing path | BUG-ES-2 | lead-programmer | 0.5 | S5-C1 | Exactly one spawn driver remains in `Assets/Script/`; the duplicate is deleted, not left dead |

### Should Have

| ID | Task | Agent/Owner | Est. Days | Dependencies | Acceptance Criteria |
|----|------|-------------|-----------|--------------|---------------------|
| S5-D1 | Reapply carried WIP — cherry-pick `cb099ee` + reapply stash `wip-before-sprint-kickoff-2026-07-13` onto `sprint-05`, reconciled against S5-C1/C3 changes to the same files | gameplay-programmer | 0.25 | S5-C1, S5-C3 | `EnemySpawner.cs` + scene reflect the carried padding fix without reintroducing BUG-ES-1; stash dropped |
| S5-D2 | Flip ADR-0002 `Proposed → Accepted` (review against the current stub state + this sprint's decisions) | producer | 0.1 | None | ADR-0002 Status reads Accepted; sign-off note recorded |
| S5-D3 | S4-05 — `PlayerInputHandle.cs:264` `Invoke(nameof(ChangeIsTakeDamage))` paired with `CancelInvoke` in `OnDisable` | gameplay-programmer | 0.25 | None | Every `Invoke`/`InvokeRepeating` has a matching `CancelInvoke` in `OnDisable` |
| S5-D4 | Quick cleanup batch — BUG-AH-2 (dead `using UnityEngine.UIElements;`), BUG-EM-2 (dead `using System.Numerics;`), BUG-WM-2 (dup `lastClickTime` assign), BUG-LM-1 (dead `index == null` check on non-nullable int) | gameplay-programmer | 0.25 | None | All 4 grep-confirmed clean; no behavior change |

### Nice To Have

| ID | Task | Agent/Owner | Est. Days | Dependencies | Acceptance Criteria |
|----|------|-------------|-----------|--------------|---------------------|
| S5-N1 | Start the `GetSpawnSet()` Candidate-Pool rewrite (only if S5-A1→A4 land early; otherwise S6) | gameplay-programmer | 0.75 | S5-A3, S5-A4 | Candidate-Pool loop present, matching the S5-A1 spec; existing tests/smoke still pass |
| S5-N2 | S4-06 — `TalentManager` prototype → SO-driven | gameplay-programmer / lead-programmer | 1.0 | None | No `Awake` literal assignments; SO uses `[SerializeField]` + `[Range]`; no singletons |

---

## Deferred (Explicit) → Sprint 6

| Task | Reason | Target |
|------|--------|--------|
| `GetSpawnSet()` full Candidate-Pool algorithm rewrite | Locked spec + data shape land this sprint (Track A); rewrite builds on them | Sprint 6 (or S5-N1 if early) |
| `EnemyManager` lifecycle body — alive-count, lock/unlock, room-clear emit | Needs the death chain (Track B) landed + the locked data shape (Track A) | Sprint 6 |
| Placement polish — round-robin, entry-safety, jitter; marker authoring into the other 12 room JSONs | Not on the critical path for design-lock or death loop | Sprint 6 |
| HUD health bar; between-room upgrade cards | Consume `ON_PLAYER_DEATH`/`ON_ROOM_CLEAR` which land this sprint | Sprint 6 |

---

## Sequencing (daily)

- **Mon 07/14**: S5-B1 (events) → S5-A1/A2 (Option C spec + ADR-0003) → S5-B2 (player death)
- **Tue 07/15**: S5-A3/A4 (data refactor + asset migration) → S5-B3 (`EntityMoveState` guard)
- **Wed 07/16**: S5-B4/B5 (enemy death chain) → S5-C1/C2 (spawn null-guard + markerless fallback)
- **Thu 07/17**: S5-C3 (dedupe driver) → S5-D1 (WIP reapply) → S5-D2 (ADR-0002 accept)
- **Fri 07/18**: full-loop smoke-check + `/weekly-wrapup`. S5-D3/D4/N1 if time.

---

## Definition of Done for This Sprint

- [ ] Track A: Option C spec in the GDD, ADR-0003 written, `EnemyModal`/`RoomModel` refactored + all assets migrated with values intact
- [ ] Track B: Play Mode — player takes damage → dies → `ON_PLAYER_DEATH`; enemy takes damage → dies → `EntityDeathState` reached → `ON_ENEMY_DEATH` fires once
- [ ] Track C: empty-pool room and markerless room both load without throwing; exactly one canonical spawn driver remains
- [ ] `EventID` contains `ON_ENEMY_DEATH`, `ON_ROOM_CLEAR`, `ON_PLAYER_DEATH`
- [ ] Carried WIP (`cb099ee` + stash) reconciled; working tree clean before end-of-sprint commit
- [ ] Carry-over decision recorded for Sprint 6
- [ ] `/qa-plan sprint` run before Track B lands (see QA Plan gate)

---

## QA Plan

⚠️ **No QA Plan yet** (`production/qa/qa-plan-sprint-05.md` does not exist). Run `/qa-plan sprint`
before Track B is implemented — Track A's data refactor and Track B's death chain are exactly the
Logic/Integration surface that needs explicit test classification per `test-standards.md`. The
Production → Polish gate requires a QA sign-off, which requires this plan.

---

## Risks

| Risk | Probability | Impact | Mitigation |
|------|------------|--------|------------|
| Track A data refactor breaks existing SO assets / serialization (the `weight`→`cost` rename especially) | Medium | High | Migrate assets in the same task; a field rename drops the old serialized value unless handled — use `[FormerlySerializedAs("weight")]` on `cost` so the 6 assets keep their numbers; verify each asset opens clean before moving on |
| Scope too heavy (design + refactor + death chain in 4d) | Medium | Medium | Algorithm rewrite + `EnemyManager` explicitly deferred to S6; S5-N1/N2 are stretch only, not committed |
| Off-plan work recurs (pattern flagged 9 sprints pre-S4) | Medium | High | S5-A + S5-B are the critical path; hold new spawn-feature work until they land |
| No QA plan (lean mode) | Confirmed | Medium | Run `/qa-plan sprint` before Track B — flagged, not silently skipped |
| No Unity CLI in this environment | Known constraint | Low | All Play Mode smoke checks are manual in-Editor by the owner |

---

## Dependencies on External Factors

- No Unity CLI — Play Mode smoke checks require manual in-Editor confirmation (same as Sprint 4).
- `gh` CLI unavailable — the draft PR for `sprint-05` (base `sprint-04`) was not auto-created; run
  manually if desired: `gh pr create --draft --base sprint-04 --head sprint-05 --title "Sprint 5"`.

---

## Next Sprint Outlook (Sprint 6)

- Implement the Candidate-Pool `GetSpawnSet()` on the locked Option C data shape (S5-N1 may start it).
- Build `EnemyManager` lifecycle → room-clear: doors lock on entry, alive-count via `ON_ENEMY_DEATH`,
  unlock + `ON_CLEAR_ENEMY`/`ON_ROOM_CLEAR` at zero alive (per ADR-0002).
- Placement polish (round-robin/entry-safety/jitter) + author `Tile_Spawn_Enemy` markers into the
  other 12 room JSONs.
- HUD health bar + between-room upgrade cards, consuming this sprint's new events.
- **First full playtest**: once player death, enemy death, and room-clear are all live.
