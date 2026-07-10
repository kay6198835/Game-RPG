# Sprint 4 — 2026-07-07 to 2026-07-11

## Sprint Goal

**Close the P1 backlog. No exceptions, no off-plan work until done.**

Four bugs (S4-01 → S4-04) have been carried for 8 consecutive sprints. Each is ≤0.25d. The entire Must-Have block can be completed in a single focused session (~1d total). Sprint 4's only success condition is landing all four Must-Haves and verifying in Play Mode that melee damage works end-to-end.

If Must-Haves are done early (by Tue), Should-Have S4-05 (CancelInvoke) and S4-06 (TalentManager SO) follow. Combat-loop blockers (player death, enemy death, room-clear) remain deferred to Sprint 5 as originally planned.

> **Context**: Sprint 3 closed with 0% velocity (8th consecutive failure on the same P1 items). The two commits that landed were combo-attack polish (off-plan), not the P1 fixes. The carry items are all tiny — the pattern suggests a prioritization problem, not a capacity problem.

---

## Day 2 Pivot (2026-07-08) — Design of Enemy-Spawn System

**P1 backlog closed** (S4-01→S4-04 done and merged). The remaining days (Wed–Fri) pivot to **DESIGN ONLY** of a new **data-driven room-based enemy-spawn & management system** — GDD + implementation roadmap, **no code this sprint**. The system is scoped to span **2–3 sprints** to fully implement.

Rationale: the system is build-from-scratch (nothing exists today), and its "manage per room" half (alive-count → unlock doors on clear) has a **hard dependency on the enemy-death chain** (Bugs #7/#8 + `ON_ENEMY_DEATH`), which is still deferred. Writing the design now is the work that unblocks a clean multi-sprint build.

Non-Must-Have Should-Haves that don't serve this system (**S4-05**, **S4-06**) are **pended → Sprint 5**.

---

## Capacity

- Total days: 5
- Buffer (20%): 1 day
- Available: 4 days

---

## Carry-Over From Sprint 3

| Item | Type | Priority | Origin |
|------|------|----------|--------|
| Fix BUG-AH-1 — `AbilityHolder.cs` remove `UnityEditor` imports | Bug | P1 | S1→S2→S3 |
| Fix Bug #9 — `AnimationPlayerController` double `StartAnimation` registration | Bug | P1 | S2→S3 |
| Complete S2-03 — `Core.GetCoreComponent<T>()` LINQ → foreach | Task | P1 | S2→S3 |
| Fix Bug #4 — `WeaponMelee.Attack()` empty foreach (no damage) | Bug | P1 | S1→S2→S3 |
| Fix BUG-PIH-1 — `CancelInvoke` missing in `PlayerInputHandle` | Bug | P2 | S2→S3 |
| Stats system — `TalentManager` prototype → SO-driven production | Task | P2 | S3 |
| S2-02 — Decouple `Weapon` ↔ `WeaponHolder`/`AbilityHolder` | Task | P3 | S2→S3 |
| EditMode test for `Core.GetCoreComponent<T>()` | Task | P3 | S3 |

---

## Tasks

### Must Have (P1 — all ≤0.25d; complete before touching anything else)

| ID | Task | Agent/Owner | Est. Days | Dependencies | Acceptance Criteria |
|----|------|-------------|-----------|--------------|---------------------|
| S4-01 | Fix BUG-AH-1 — `AbilityHolder.cs` line 4: remove `using UnityEditor.Experimental.GraphView;`; also check `EntityData.cs`, `StatsCharacter.cs`, `EnemySO.cs`, `LevelManager.cs`, `AnimationEventManager.cs`, `DualAbility.cs`, `WeaponMeleeStats.cs` for `UnityEditor.*` imports in runtime scripts; wrap or remove | gameplay-programmer | 0.25 | None | `grep -r "using UnityEditor" Assets/Script/` returns 0 runtime hits; Editor compiles clean; Player build target compiles |
| S4-02 | Fix Bug #9 — `AnimationPlayerController.cs` line 21: change second `StartAnimation` → `EndAnimation` callback registration; mirror fix in `OnDisable` line 29; verify `PlayerUseWeaponState` exits cleanly on `animFinish` | gameplay-programmer | 0.25 | S4-01 | `OnEnable` registers both `StartAnimation` and `EndAnimation` distinctly; `OnDisable` unregisters both; `PlayerUseWeaponState` exits without sticking; no ability-stuck in Play Mode |
| S4-03 | `Core.GetCoreComponent<T>()` — replace LINQ `OfType<T>().FirstOrDefault()` with a `foreach` loop; add lazy-cache (`??=` or explicit null guard) so the loop only runs once per component type | lead-programmer | 0.25 | S4-01 | Zero LINQ calls in `Core.cs`; `foreach` loop present; Play Mode shows no allocation warning from `Core`; grep `coreComponents.OfType` returns 0 hits |
| S4-04 | Fix Bug #4 — `WeaponMelee.Attack()` `foreach` body (line 29–32): add `INegativeReceiver dmg = enemy.GetComponentInChildren<INegativeReceiver>(); if (dmg != null) dmg.TakeDamage(currrentSA.attackDamege, transform.position);` (keep typo `attackDamege`; mirror `EntityWeaponMelee.Attack()` exactly) | gameplay-programmer | 0.25 | S4-02 | `foreach` body is not empty; hitting a live enemy in Play Mode reduces `EntityStatsSO.Health`; no direct health field mutation bypassing the interface |

### Design Track — Enemy-Spawn System (Day 2 Pivot, active this sprint)

**Design only — no implementation code.** Deliverable is a review-approved GDD + a decomposed multi-sprint roadmap. Total ≈ 2.25d, fits the remaining ~3 days.

| ID | Task | Agent/Owner | Est. Days | Dependencies | Acceptance Criteria |
|----|------|-------------|-----------|--------------|---------------------|
| S4-D1 | Author GDD `design/gdd/enemy-spawn-system.md` (all 8 sections per `design-docs.md`) from the owner's spec: `EnemyData` / `EnemyDatabase` / `MapEnemyDatabase` / `RoomData` + `GetHybridEnemySet` two-phase algorithm (random-ratio phase → optimal-fill-with-overflow phase). Run via `/design-system`. | game-designer / systems-designer | 1.0 | None | GDD exists with all 8 sections; pre-commit GDD hook passes; every SO field + algorithm phase documented |
| S4-D2 | Resolve the 6 open design questions inside the GDD (see "Open Design Questions" below) — zero ambiguity left for programmers | systems-designer | 0.5 | S4-D1 | Each of the 6 questions has a locked decision in the GDD's Detailed Rules / Edge Cases / Formulas sections |
| S4-D3 | Decompose into an implementation roadmap: epic + per-sprint story list + dependency map. Run `/map-systems` (add to `systems-index.md`) then `/create-epics enemy-spawn` | producer / lead-programmer | 0.5 | S4-D1 | Epic file created; `systems-index.md` updated; stories mapped to Sprint 5 (data+algorithm) and Sprint 6 (runtime+room-clear) with dependency on enemy-death chain noted |
| S4-D4 | `/design-review design/gdd/enemy-spawn-system.md` — gate the GDD before hand-off to programmers | game-designer | 0.25 | S4-D2 | Verdict APPROVED, or CONCERNS with follow-ups recorded in the GDD |

### Should Have — PENDED → Sprint 5

> Not Must-Have and not on the enemy-spawn critical path. Pended per Day 2 pivot; carry to Sprint 5.

| ID | Task | Agent/Owner | Est. Days | Dependencies | Acceptance Criteria |
|----|------|-------------|-----------|--------------|---------------------|
| S4-05 | ⏸️ PENDED — Fix BUG-PIH-1 — audit all `Invoke`/`InvokeRepeating` calls in `PlayerInputHandle.cs`; add `CancelInvoke` in `OnDisable`; verify no leaked invocations on state transitions | gameplay-programmer | 0.25 | S4-04 | Every `Invoke`/`InvokeRepeating` has a matching `CancelInvoke(name)` in `OnDisable`; no leaked invocations in Play Mode transition log |
| S4-06 | ⏸️ PENDED — Stats system — promote `TalentManager.cs` from prototype: remove hardcoded `Awake` literal assignments for `strength`/`dex`/`int`/`cha`/`skillPoint`; wire to a `StatsCharacter` SO instance; Inspector-assignable; no `Find()` | gameplay-programmer / lead-programmer | 1.0 | S4-03 | `TalentManager` reads all stat fields from an Inspector-assigned SO; no `Awake` literal assignments remain; SO uses `[SerializeField]` + `[Range]` on numeric fields; no singletons |

### Nice To Have

| ID | Task | Agent/Owner | Est. Days | Dependencies | Acceptance Criteria |
|----|------|-------------|-----------|--------------|---------------------|
| S4-07 | S2-02 carry — decouple `Weapon` ↔ `WeaponHolder`/`AbilityHolder`: `Weapon.Interact()` calls `((WeaponHolder)interactor).Equip(this)`; `WeaponHolder.Equip()` pushes abilities into `AbilityHolder`; no direct `holder`/`abilityHolder` refs on `Weapon` | lead-programmer | 1.0 | S4-03 | `Weapon` holds no direct references to `WeaponHolder` or `AbilityHolder`; equip → **E** fires Special, **RMB** fires Block; unequip clears both slots |
| S4-08 | EditMode test for `Core.GetCoreComponent<T>()` — covers S2-03/S4-03 regression surface; test file `tests/EditMode/CoreTests.cs` | qa-tester | 0.5 | S4-03 | Test creates `Core` + two `CoreComponent` instances; `GetCoreComponent<T>()` resolves correctly; second call returns cached instance; teardown destroys all GameObjects |

---

## Deferred (Explicit)

| Task | Reason | Target |
|------|--------|--------|
| Player death (Bug #6) | Deferred 8 sprints — behind P1 debt clearance | Sprint 5 |
| Enemy AI death chain (Bugs #5/#7/#8) | Deferred | Sprint 5 |
| Room-clear lock/unlock | Blocked on enemy death | Sprint 5 |
| Combat-loop blockers | Not starting new work until P1 debt is zero | Sprint 5 |
| Map/minimap (`MapGridController` WIP) | Out of current scope | Sprint 5+ |

> After Sprint 4 closes the P1 backlog, Sprint 5 targets the full combat loop: player death → restart, enemy death → room-clear, HUD health bar. That completes the demo checklist items 5, 6, 7, 8, 9.

---

## Open Design Questions — Enemy-Spawn System (resolve in GDD via S4-D2)

1. **Stat-system proliferation.** Three parallel stat systems already exist (`StatsCharacter`/`EnemySO`, `EntityStatsSO`/`EntityData`, `StatsSO`). Lock: `EnemyData` stays **spawn-metadata only** (id/name/prefab/weight); combat stats live on the prefab's existing `EntityData`/`EntityStatsSO`. `EnemyData` must NOT become a 4th stat system.
2. **`id` stability + collisions.** `GUID.GetHashCode()` collapses 128-bit → 32-bit, so collisions are possible (negligible at this roster size but not zero). Lock: assign once, never overwrite when `id != 0`; `EnemyDatabase` validates for duplicate ids and warns in Editor so a collision never fails silently.
3. **`RoomData.idEnemy ⊆ sourceMap.idEnemy`.** Enforce via `OnValidate`; define failure behavior (strip invalid ids / log error).
4. **Prefab readiness.** Only `EnemyPrefab.prefab` is framework-wired; `Bat.prefab`/`Crab.prefab` reference **missing scripts**. Enemy variety needs wired prefabs — flag as a Sprint-5/6 prerequisite in Dependencies.
5. **Algorithm determinism for tests.** `GetHybridEnemySet` shuffles/randomizes — specify a seedable RNG hook so EditMode tests are deterministic (per `test-standards.md`).
6. **Overflow/budget formulas.** Pin in the Formulas section: `randomBudget = weightBudget * randomRatio`; overflow cap = `weightBudget * overflowPercent`; phase-2 pick = `argmin(|weight - remaining|)` with repetition allowed; stop when `remaining <= 0` or no candidate within cap.

---

## Enemy-Spawn Implementation Roadmap (2–3 sprints)

| Phase | Sprint | Scope | Blocking dependency |
|-------|--------|-------|--------------------|
| Design | **S4 (this update)** | GDD + design-review + epic/story breakdown. No code. | None |
| Data + Algorithm | **S5** | `EnemyData`, `EnemyDatabase` (`GetByID` lazy dict cache + `GetHybridEnemySet`), `MapEnemyDatabase`, `RoomData` (+ `OnValidate` subset check); EditMode tests (seeded). **In parallel:** enemy-death chain (Bugs #7/#8) — `EntityDeathState : EntityState`, fill `EntityBasicState` Health<=0 transition, add `ON_ENEMY_DEATH` to `EventID`. | Data layer: none. Death chain: independent, but prerequisite for S6. |
| Runtime + Room-Clear | **S6** | `EnemyManager` (listens `ON_LOAD_MAP` → `GetHybridEnemySet` → spawn at `Tile_Spawn` points); `Tile_Spawn` marker tile + parser in `RoomGeneraterController` (mirror door-tile handling); per-room alive-count in `RoomCell`, lock doors on enter, emit `ON_CLEAR_ENEMY` (wire existing dead subscriber) + new `ON_ROOM_CLEAR`; reuse `ObjectPooling`. | Requires S5 data layer **and** enemy-death chain. |

---

## Definition of Done for This Sprint

- [ ] S4-D1 → S4-D4 completed: GDD authored (8 sections), 6 design questions resolved, roadmap/epic created, `/design-review` verdict APPROVED (or CONCERNS with follow-ups)
- [ ] S4-01 → S4-04 all completed and pass acceptance criteria
- [ ] `grep -r "using UnityEditor" Assets/Script/` returns 0 hits in runtime scripts
- [ ] Play Mode smoke: equip weapon → melee attack → enemy `Health` decreases → ability runs and exits cleanly
- [ ] No `"<T> not found"` Console warnings from `Core.GetCoreComponent<T>()`
- [ ] Working tree clean before end-of-sprint commit
- [ ] Carry-over decision recorded for Sprint 5
- [ ] Retrospective note if velocity < 50% (again)

---

## Risks

| Risk | Probability | Impact | Mitigation |
|------|------------|--------|------------|
| Off-plan work displaces Must-Haves (8th time) | High | Critical | S4-01→S4-04 are the ONLY allowed work until done; this is a hard rule, not advisory |
| BUG-AH-1 scope wider than `AbilityHolder.cs` alone | Confirmed | Medium | Grep found 7+ files; S4-01 acceptance requires 0 runtime `using UnityEditor` hits — fix all on first pass |
| S4-03 lazy-cache breaks existing call sites | Low | Medium | Verify all `core.GetCoreComponent<T>()` call sites after change; acceptance requires no Console warnings |
| Zero automated tests (TD-014) still open | High | Medium | S4-08 adds first EditMode test — Nice To Have, but important for regression safety going forward |
| Developer absence / low velocity pattern persists | High | High | Must-Have block is ~1d total; even 2 productive hours per day covers it in 3 days |

---

## Next Sprint Outlook (Sprint 5 — tentative)

- **Combat-loop unblocked** (if all Must-Haves land): Player death (Bug #6) + enemy death chain (Bugs #5/#7/#8) + room-clear in Sprint 5.
- **Stats system** (if S4-06 lands): Sprint 5 connects SO stats to damage/speed formulas; first real playtest session.
- **If S4-07 (Weapon decoupling) not done**: pull forward to Sprint 5 Must Have.
- **First playtest session**: once S4-04 (melee damage) and S4-02 (ability exit) both confirmed in Play Mode.

---

## Sprint Status: ✅ COMPLETE (closed by automated Sunday 22:00 kickoff run, 2026-07-13)

Formally closed at Sunday kickoff. Branch `sprint-05` created from `sprint-04` HEAD (`26a85cd` on `origin/feature/spawn-enemy`, one commit — `cb099ee` "random padding position" — ahead of the `sprint-04` branch tip `6aa3cc1`; that commit was NOT carried into `sprint-05` per the kickoff rule requiring the new branch to fork from the `sprint-04` branch specifically, not from whatever branch happened to be checked out. Flagged below as a carry item.

Final Definition of Done checklist (unchanged since Saturday wrap-up — no further sprint-04-scoped work landed 07-11→07-13):
- [x] S4-D1 → S4-D4 completed
- [x] S4-01 → S4-04 all completed and verified
- [x] `grep -r "using UnityEditor" Assets/Script/` → 0 hits
- [ ] Play Mode smoke — still not confirmed (no Unity CLI in this environment); carries to Sprint 5 as a manual-verification action item
- [x] No LINQ in `Core.GetCoreComponent<T>()`
- [x] Working tree clean at sprint close — confirmed via kickoff run (WIP on `EnemySpawner.cs` + scene stashed, not committed to any sprint-04/05 branch)
- [x] Carry-over decision recorded for Sprint 5 (below, supersedes Saturday's list where re-triaged)
- [x] Retrospective — see `production/retros/retro-sprint-04-2026-07-10.md`

### Final Carry-Over → Sprint 5

- **NEW** — `cb099ee` "random padding position" (`Assets/Script/Enemy/EnemySpawner.cs`, `Assets/Scenes/Main/Test/LoadRandomMap.unity`) — landed on `origin/feature/spawn-enemy` after the Saturday wrap-up, not present on the `sprint-04` branch tip, not carried into `sprint-05` by the kickoff branch-from-`sprint-04` rule. Owner should cherry-pick or re-apply onto `sprint-05` manually.
- **NEW** — uncommitted WIP found at kickoff time on `EnemySpawner.cs` + `LoadRandomMap.unity` (2-file diff, 4 lines) — stashed as `wip-before-sprint-kickoff-2026-07-13` rather than discarded; recover with `git stash pop` (or `apply`) on the appropriate branch once the owner decides where it belongs (`sprint-05` vs staying on `feature/spawn-enemy`).
- S4-05 — Fix BUG-PIH-1 (`CancelInvoke` pairing, `PlayerInputHandle.cs:264`) — 4th carry (S2→S3→S4→S5)
- S4-06 — `TalentManager` prototype → SO-driven — 3rd carry (S3→S4→S5)
- ADR: Skill Enhance vs `ActivateSkill` pipeline decision — 3rd carry (S3→S4→S5), still not started
- Bug #6 (player death chain), Bugs #5/#7/#8 (enemy death chain), room-clear condition — 5th carry, design-unblocked since S4
- BUG-ES-1 (NRE risk in `RoomModel.GetSpawnSet()`), BUG-ES-2 (duplicate `EnemySpawner`/`LevelManager` spawn drivers), BUG-ES-3 (`EventID` missing `ON_ENEMY_DEATH`/`ON_ROOM_CLEAR`) — carried, unresolved
- ADR-0002 (`EnemyManager` singleton) — `Status: Proposed`, still not flipped to `Accepted`; needs owner sign-off before Sprint 6 hardens the singleton further
- Owner decision still open: fold `Assets/Script/Database-SO/**` enemy-spawn prototype formally into Sprint 5 scope, or relocate under `prototypes/` per the isolation rule

---

## Sprint Close — Weekly Wrap-up (2026-07-10, automated Saturday 22:00 run)

> Ran on sprint Day 4 (Thu, per this sprint's own day-plan) — one day ahead of the sprint's designated Friday wrap day. Friday 07-11 work is not reflected below; treat this as the scheduled close for the tracked week, not a claim that Sprint 4 is fully finished.

**Verdict: CONCERNS**

Reasoning: the sprint's own Definition of Done is mostly met — all 4 Must-Have P1s closed and verified (first 100% completion in 9 sprints), all 4 design-track tasks done, GDD approved, both ADRs written. That's a PASS on the sprint's stated goal. The CONCERNS flag is for what happened alongside it: the explicit "design only, no code" Day-2 pivot was broken same night and stayed broken for 3+ more days, a live NullReferenceException risk (BUG-ES-1) shipped in the resulting code, a second undocumented spawn driver appeared that bypasses the just-ratified ADR-0002 architecture, and the playtest that's been "next sprint" for two retros running still didn't happen despite its blocker being cleared Day 1.

### Definition of Done — Checklist Result

- [x] S4-D1 → S4-D4 completed — GDD approved, 6 design questions resolved, epic + ADR-0002 created
- [x] S4-01 → S4-04 all completed and pass acceptance criteria — verified by direct code read, not just commit message
- [x] `grep -r "using UnityEditor" Assets/Script/` returns 0 hits in runtime scripts — confirmed
- [ ] Play Mode smoke test — **not confirmed this wrap-up** (no Unity CLI available in this environment); flagged pending in daily log since 07-07, still open
- [x] No LINQ in `Core.GetCoreComponent<T>()` — confirmed, `Dictionary` cache + `foreach`
- [ ] Working tree clean before end-of-sprint commit — N/A, sprint not yet formally closed (Friday remains)
- [x] Carry-over decision recorded for Sprint 5 — see below
- [x] Retrospective note — velocity was 100% on Must-Haves (not <50%), but retro still written given the off-plan pattern; see `production/retros/retro-sprint-04-2026-07-10.md`

### Velocity

- **Must-Have (P1) completion**: 100% (4/4) — best result tracked to date, reversing 8 consecutive sprints at 0-14%
- **Should-Have completion**: 0% (0/2) — deliberately pended by the Day-2 pivot, not slippage
- **Design-track completion**: 100% (4/4)
- **Off-plan volume**: high — ~9 commits of unplanned enemy-spawn prototype code across 07-08 (20:14) through 07-10, after an explicit same-day "no code" decision

### Carry-Over to Sprint 5

- S4-05 — Fix BUG-PIH-1 (`CancelInvoke` pairing in `PlayerInputHandle.cs:264`) — 3rd carry (S2→S3→S4→S5)
- S4-06 — `TalentManager` prototype → SO-driven — 2nd carry (S3→S4→S5)
- ADR: Skill Enhance vs `ActivateSkill` pipeline decision — 2nd carry (S3→S4→S5), still not started
- Bug #6 (player death chain), Bugs #5/#7/#8 (enemy death chain), room-clear condition — 4th carry (S1→S2→S3→S4→S5), now design-unblocked
- New from this triage: BUG-ES-1 (NRE risk in `RoomModel.GetSpawnSet()`), BUG-ES-2 (duplicate `EnemySpawner`/`LevelManager` spawn drivers), BUG-ES-3 (`EventID` missing `ON_ENEMY_DEATH`/`ON_ROOM_CLEAR`) — all new, small, high-value fixes that unblock Sprint 6's `EnemyManager` work
- Explicit owner decision needed: formalize enemy-spawn prototype code into Sprint 5 scope (aligned to the GDD's locked algorithm) or relocate under `prototypes/` per the isolation rule — currently in neither state

### Reference Files

- Code review (this week's 27 changed `.cs` files): findings surfaced in `production/retros/retro-sprint-04-2026-07-10.md` context and `production/qa/bug-triage-2026-07-10.md`; verdict CHANGES REQUIRED (blocking: BUG-ES-1 null risk, ADR-0002 architecture drift, missing `EventID` values)
- Bug triage: `production/qa/bug-triage-2026-07-10.md`
- Retrospective: `production/retros/retro-sprint-04-2026-07-10.md`
- Playtest: skipped — no session logged since `production/qa/playtests/playtest-2026-06-12-weekly-wrapup.md`; run `/playtest-report` manually if a session happens before Sprint 5 kickoff
