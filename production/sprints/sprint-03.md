# Sprint 3 — 2026-06-23 to 2026-06-27

## Sprint Goal

Clear the P1 debt that is actively blocking combat verification, then begin the stats system promotion: fix the build-breaking `UnityEditor` imports (BUG-AH-1), close Bug #9 (animation stuck), complete the `Core.GetCoreComponent<T>()` zero-alloc requirement (S2-03), and land the combat-testability gate (Bug #4 / WeaponMelee damage) — so that by mid-week the equipped-weapon → attack → damage loop is playable. If capacity allows, start promoting `TalentManager` from prototype to production-grade SO-driven stats system.

> **Context**: Sprint 2 closed with 14% velocity on the sprint branch (36% including the unmerged parallel branch). Three P1 bugs still open and one task partially done. Sprint 3 is a recovery sprint: resolve the backlog P1 items first, then advance the stats system theme planned since Sprint 2. The deferred combat-loop blockers (player death, enemy death, room-clear) remain parked until after stats stabilize (Sprint 4+).

---

## Capacity

- Total days: 5
- Buffer (20%): 1 day
- Available: 4 days
- *Same capacity assumption as previous sprints. Adjust if availability changes.*

---

## Carry-Over From Sprint 2

| Item | Type | Priority | Source |
|------|------|----------|--------|
| Fix Bug #9 — `AnimationPlayerController` double-registration (lines 21 + 29) | Bug | P1 | S2-04 |
| Fix BUG-AH-1 — `AbilityHolder` `UnityEditor` imports (build-breaking) | Bug | P1 | Wrapup 2026-06-20 |
| Complete S2-03 — `Core.GetCoreComponent<T>()` replace LINQ → `foreach` | Task | P1 | S2-03 partial |
| Fix BUG-PIH-1 — `CancelInvoke` missing in `PlayerInputHandle` | Bug | P2 | Wrapup 2026-06-20 |
| S2-02 — Decouple `Weapon` ↔ `WeaponHolder`/`AbilityHolder` via push-on-equip | Task | P2 | S2-02 cut |
| Merge commits e314b88 / 5fa5e27 / 81c95de (parallel branch fixes) | Action | Pre-sprint | Sprint Close 2026-06-20 |

---

## Tasks

### Must Have (Critical Path)

| ID | Task | Agent/Owner | Est. Days | Dependencies | Acceptance Criteria |
|----|------|-------------|-----------|--------------|---------------------|
| S3-01 | Fix BUG-AH-1 — remove `UnityEditor` imports from `AbilityHolder.cs` (and any other runtime script with editor-only using directives); wrap any editor-only code in `#if UNITY_EDITOR` | gameplay-programmer | 0.25 | None | Editor compiles clean; Player build succeeds (no `UnityEditor.*` in runtime code); `AbilityHolder` functional in Play Mode |
| S3-02 | Fix Bug #9 — `AnimationPlayerController.cs`: change line 21 second registration from `StartAnimation` → `EndAnimation`; mirror fix in `OnDisable` line 29; verify `PlayerUseWeaponState` exits cleanly | gameplay-programmer | 0.25 | S3-01 | `OnEnable`/`OnDisable` register/unregister `StartAnimation` AND `EndAnimation` distinctly; `PlayerUseWeaponState` exits on `animFinish` confirmed; no ability stuck state |
| S3-03 | Complete S2-03 — replace LINQ `OfType<T>().FirstOrDefault()` in `Core.GetCoreComponent<T>()` with a `foreach` loop; no per-frame allocation; verify no `"<T> not found"` Console warnings | lead-programmer | 0.25 | S3-01 | Zero LINQ in `Core.cs`; `foreach` loop; Play Mode shows no allocation warnings; grep `\.Core\.` / `core\.` confirms all call sites use lazy-cached accessor |
| S3-04 | Fix Bug #4 — `WeaponMelee.Attack()`: add `INegativeReceiver.TakeDamage(currrentSA.attackDamege, transform.position)` inside `foreach`; mirror `EntityWeaponMelee.Attack()` exactly (keep typo `attackDamege`) | gameplay-programmer | 0.25 | S3-02 | Foreach calls `TakeDamage`; hitting a live enemy reduces `EntityStatsSO.Health` in Play Mode; no direct health mutation bypassing the interface |

### Should Have

| ID | Task | Agent/Owner | Est. Days | Dependencies | Acceptance Criteria |
|----|------|-------------|-----------|--------------|---------------------|
| S3-05 | Fix BUG-PIH-1 — add `CancelInvoke` pairing in `PlayerInputHandle`; verify no coroutine/invoke leak on state transitions | gameplay-programmer | 0.25 | S3-02 | Every `Invoke()`/`InvokeRepeating()` call has a corresponding `CancelInvoke()` in `OnDisable` or state exit; no leaked invocations in Play Mode |
| S3-06 | Begin stats system — promote `TalentManager.cs` from prototype to production: remove hardcoded `Awake` values, wire `strength`/`dex`/`int`/`cha`/`skillPoint` fields to a `StatsCharacter` SO instance; Inspector-assignable (no `Find()`); obey `scriptableobject-data.md` | gameplay-programmer / lead-programmer | 1.0 | S3-03 | `TalentManager` reads stats from an SO (not hardcoded values); SO instance is Inspector-assigned; SO fields use `[SerializeField]` with `[Range]` validation; `TalentManager` contains no `Awake` literal assignments for stat values; no singletons |

### Nice To Have

| ID | Task | Agent/Owner | Est. Days | Dependencies | Acceptance Criteria |
|----|------|-------------|-----------|--------------|---------------------|
| S3-07 | S2-02 carry-over — decouple `Weapon` ↔ `WeaponHolder`/`AbilityHolder` via push-on-equip: `Weapon.Interact()` calls `((WeaponHolder)interactor).Equip(this)`; `WeaponHolder.Equip()` pushes abilities into `AbilityHolder`; remove `using UnityEditor.*` from `AbilityHolder` | lead-programmer | 1.0 | S3-03 | `Weapon` has no direct `holder`/`abilityHolder` references; equip → **E** fires Special, **RMB** fires Block; unequip clears both slots (per `weapon-skill-code.md`) |
| S3-08 | One EditMode test for `Core.GetCoreComponent<T>()` (covers the S2-03 + S3-03 regression surface) | qa-tester | 0.5 | S3-03 | Test creates `Core` + two `CoreComponent` instances; `GetCoreComponent<T>()` resolves to the correct type; second call returns the cached instance; no LINQ warning; test in `tests/EditMode/`; teardown destroys all GameObjects |

---

## Deferred (Explicit)

| Task | Reason | Target |
|------|--------|--------|
| S1-03 (Bug #6) Player death + restart | Behind system work | Sprint 4+ (after stats) |
| S1-04 (Bugs #5/#7/#8) Enemy AI death chain | Deferred | Sprint 4+ |
| S1-05 Room-clear lock/unlock | Blocked on S1-04 | Sprint 4+ |
| Map/Room bugs | Out of current scope | Sprint 4+ / when map work resumes |

> These combat-loop blockers have been deferred 6 consecutive weeks. Deferral remains intentional: finish the P1 architecture debt (this sprint) and stats system (Sprint 3–4) before combat closure. Record and revisit at Sprint 4.

---

## Definition of Done for This Sprint

- [ ] All Must Have tasks (S3-01 → S3-04) completed and pass acceptance criteria
- [ ] Editor compiles clean — no `UnityEditor.*` in runtime code, no `"<T> not found"` Console warnings
- [ ] Play Mode smoke: equip weapon → melee hit deals damage (S3-04) → ability runs and exits cleanly (S3-02)
- [ ] Working tree clean (no uncommitted must-have code) before end-of-sprint commit
- [ ] Carry-over decision recorded for Sprint 4
- [ ] Retrospective note if velocity < 50%

---

## Risks

| Risk | Probability | Impact | Mitigation |
|------|------------|--------|------------|
| Recurring low velocity pattern (6th week) — P1 items still open | High | High | S3-01 → S3-04 are all ≤0.25d each — even 1 productive day should close them; time-box strictly |
| Dirty working tree (PlayerInputHandle, PlayerState, PlayerAttackState, WeaponMelee uncommitted) | High | Medium | Commit dirty tree as first action Mon 23/06 before any Sprint 3 work begins |
| BUG-AH-1 + Bug #9 + Bug #4 interact — fixing one may expose another | Medium | Medium | Fix in order: BUG-AH-1 (S3-01) → Bug #9 (S3-02) → S2-03 (S3-03) → Bug #4 (S3-04); verify after each |
| Stats system (S3-06) underscoped — `TalentManager` prototype may be more complex to promote | Medium | Low | S3-06 is Should Have; cut if P1 backlog absorbs capacity |
| Zero automated tests persist (TD-014) | High | Medium | S3-08 adds first `Core` EditMode test; Nice to Have but important for regression safety |

---

## Next Sprint Outlook (Sprint 4 — tentative)

- **If stats promotion (S3-06) lands:** continue stats system — connect SO stats to player damage/speed formulas; validate with playtest.
- **If S3-07 (Weapon decoupling) not done:** pull forward to Sprint 4 Must Have.
- **Combat-loop blockers:** Player death (Bug #6), enemy death chain (Bugs #5/#7/#8), room-clear — target Sprint 4 if stats land early, Sprint 5 otherwise.
- **First playtest session** should happen once S3-04 (melee damage) and S3-02 (ability exit) both land — book one this sprint if possible.
