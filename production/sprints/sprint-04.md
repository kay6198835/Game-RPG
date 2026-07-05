# Sprint 4 — 2026-07-07 to 2026-07-11

## Sprint Goal

**Close the P1 backlog. No exceptions, no off-plan work until done.**

Four bugs (S4-01 → S4-04) have been carried for 8 consecutive sprints. Each is ≤0.25d. The entire Must-Have block can be completed in a single focused session (~1d total). Sprint 4's only success condition is landing all four Must-Haves and verifying in Play Mode that melee damage works end-to-end.

If Must-Haves are done early (by Tue), Should-Have S4-05 (CancelInvoke) and S4-06 (TalentManager SO) follow. Combat-loop blockers (player death, enemy death, room-clear) remain deferred to Sprint 5 as originally planned.

> **Context**: Sprint 3 closed with 0% velocity (8th consecutive failure on the same P1 items). The two commits that landed were combo-attack polish (off-plan), not the P1 fixes. The carry items are all tiny — the pattern suggests a prioritization problem, not a capacity problem.

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

### Should Have

| ID | Task | Agent/Owner | Est. Days | Dependencies | Acceptance Criteria |
|----|------|-------------|-----------|--------------|---------------------|
| S4-05 | Fix BUG-PIH-1 — audit all `Invoke`/`InvokeRepeating` calls in `PlayerInputHandle.cs`; add `CancelInvoke` in `OnDisable`; verify no leaked invocations on state transitions | gameplay-programmer | 0.25 | S4-04 | Every `Invoke`/`InvokeRepeating` has a matching `CancelInvoke(name)` in `OnDisable`; no leaked invocations in Play Mode transition log |
| S4-06 | Stats system — promote `TalentManager.cs` from prototype: remove hardcoded `Awake` literal assignments for `strength`/`dex`/`int`/`cha`/`skillPoint`; wire to a `StatsCharacter` SO instance; Inspector-assignable; no `Find()` | gameplay-programmer / lead-programmer | 1.0 | S4-03 | `TalentManager` reads all stat fields from an Inspector-assigned SO; no `Awake` literal assignments remain; SO uses `[SerializeField]` + `[Range]` on numeric fields; no singletons |

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

## Definition of Done for This Sprint

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
