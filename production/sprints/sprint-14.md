# Sprint 14 — 2026-09-14 to 2026-09-18

**Status: COMPLETE (closed) — FAIL. Finalized 2026-09-20 at Sunday `pm-weekly-kickoff`; carry-over below handed to `sprint-15.md`. Closed by (2026-09-19 Saturday `pm-weekly-wrapup`, autonomous).** Full detail:
`production/retros/retro-sprint-14-2026-09-19.md`, `production/qa/bug-triage-2026-09-19.md`.
Verified against source at HEAD `17ac9af`.

| Item | Final status |
|------|--------------|
| S14-01 BUG-067 | APPARENTLY FIXED (incidental via interface rename; unverified, no Play Mode) |
| S14-02 BUG-068 | NOT MET; downgraded S3 — `GetAbility()` now has zero callers |
| S14-03 BUG-063 | NOT MET (29th+ carry) |
| S14-04 BUG-064 item 7 | NOT MET |
| S14-05 BUG-065 | NOT MET |
| S14-06 BUG-066 + BUG-070 | NOT MET |
| S14-07 Play Mode smoke | NOT MET (8th sprint) — record as accepted risk |
| S14-08 review-gate decision | NOT MET |
| S14-09 pre-push hook | NOT MET (25th+) |
| S14-10..S14-13 | NOT MET |
| S14-14 BUG-069 | APPARENTLY FIXED (incidental; `TryDoAbility` calls `CanStart()`), unverified |
| QA plan | none (31st+ cycle) |

**Velocity:** Must-Have 0 of 9 delivered as planned (~0.05d incidental of 0.9d). Off-plan: Paladin kit +
spawn-effect refactor (46 `.cs` files, +570/-438). 4 new bugs (BUG-071..074).

**Carry-over to Sprint 15:** BUG-063, BUG-064 item 7, BUG-065, BUG-066+070, BUG-073, BUG-071, BUG-072,
S14-07 (accepted-risk decision), S14-08, S14-09, S14-10/11, S14-12, S14-13, verify BUG-067/069, QA plan.

---

**Opened:** 2026-09-13 (Sunday 22:00 `pm-weekly-kickoff`, on-slot autonomous run — no owner present).
Branch `sprint-14`, created from `sprint-13` tip (`c4be3c7`, "Merge branch 'origin/feature/fix-player-control'
into sprint-13"). `gh` CLI unavailable in this environment (`gh: command not found`) — draft PR
(`--base sprint-13 --head sprint-14`, title `Sprint 14`) was **not** auto-created; run manually if desired:

```bash
gh pr create --draft --base sprint-13 --head sprint-14 --title "Sprint 14"
```

**Review mode:** lean (from `production/review-mode.txt`) — producer feasibility gate (PR-SPRINT)
skipped per lean-mode rule.

---

## Sprint Goal

**Break the 3-sprint pattern of trivial fixes losing every session to unplanned work.** Land the six
carried, fully-diagnosed, zero-dependency bug fixes — BUG-067 and BUG-068 first (both live,
player-reachable, and BUG-068 will likely crash the very Play Mode session this sprint needs), then
BUG-063, BUG-064 item 7, BUG-065, and BUG-066+BUG-070 — as the literal first commits of the sprint,
before any other file is opened. Then finally produce the sprint's first-ever owner-in-Editor Play
Mode confirmation — 6 consecutive sprints overdue (tracked since Sprint 8/S11-07).

---

## Capacity

- Total days: 5
- Buffer (20%): 1 day
- Available: 4 days

Must-Have load ≈ 0.9d.

---

## Carryover from Previous Sprint

Full detail: `production/sprints/sprint-13.md` (closure block at top),
`production/retros/retro-sprint-13-2026-09-13.md`, `production/qa/bug-triage-2026-09-13.md`.

Sprint 13 closed **CLOSED — FAIL**: 0 of 6 Must-Have tasks met, 2 of 6 Should-Have landed (the two
largest-effort ones — `/doc-sync` and the VContainer ADR — while the four smallest, cheapest,
most-isolated Must-Have fixes landed zero sessions for the third sprint running). Off-plan work
(Abilities v2 completion) displaced the entire sprint plan and shipped 4 new S1/S2 bugs of its own,
caught only by the autonomous wrap-up's code-review pass 5 days after the work landed. Verified
directly against file contents at this kickoff (not commit messages):

| Task | Reason | New Estimate |
|------|--------|-------------|
| **BUG-067** (new, `ResourceReceiver` heal/damage swap) | Live, reachable regression — a healing item pickup currently damages the player. Threatens the Play Mode smoke session directly. Fix first | 0.05d |
| **BUG-068** (new, `AbilityHolder.GetAbility()` unguarded null deref) | NRE on first skill press if any `AbilitySlot` is unbound. Threatens the Play Mode smoke session directly. Fix second | 0.05d |
| BUG-063 (`Stat.cs:63-66` `[SerializeField]` regression) | 25th+ consecutive carry (since Sprint 10/NEW-4), one-line documented fix, zero technical blocker | 0.05d |
| BUG-064 item 7 (`RangeWeapon.cs:7` DI wiring) | 2nd carry from Sprint 13, sole remaining sub-item, fix pattern proven in-repo (`ItemSpawner.cs:8-10`) | 0.1d |
| BUG-065 (`PlayerDeathState.Enter()` doesn't stop `PlayerMovement`) | 2nd carry, one `Core.GetCoreComponent` call + `.Stop()`, no deps | 0.1d |
| BUG-066 + **BUG-070** (new) (`EntityVitalStats` / `VitalStatsComponent` unguarded dictionary indexer — identical shape, enemy and player sides) | Bundle both in one pass per triage recommendation; natural shared first EditMode-test assertion | 0.2d |
| S13-06 → pre-push hook placeholder | 21st+ carry, requires owner/producer action not agent time | 0.15d |
| S13-02 → owner-in-Editor Play Mode smoke session | 6th consecutive sprint asking for this exact gate (tracked since Sprint 8/S11-07) — if it cannot happen again, the retro recommends recording it as an explicit accepted-risk decision rather than re-asking as a task a 7th time | 0.2d |
| **NEW** — producer decision: does off-plan feature work need a same-day review gate before merging to a shared branch? | Retro action item #4 — 4 new S1/S2 bugs shipped unplanned this cycle with zero review for 5 days | 0.1d |
| S13-08 → S4-05/S4-06 forced decision | 18th+ carry, oldest unresolved item in the project, zero movement any cycle | 0.1d |
| S13-09 → ADR-0002 (`EnemyManager` singleton) → Accepted | 14th+ carry, trivial sign-off-only change | 0.1d |
| TD-040 (Abilities v1 vs v2 convergence — no ADR) | Widened scope of BUG-052 this cycle; a 4th sprint of v2 work is about to ship with this decision still open | 0.3d |
| S13-12 → first EditMode test (TD-014) | `tests/EditMode/` still `.gitkeep`-only for the entire project history — BUG-066/BUG-070's identical shape is the natural shared first assertion | 0.3d |
| S13-11 → batch-generate remaining `BUG-NNN.md` files | **Resolved at this kickoff** — all 9 open bugs (BUG-052, 063-070) now have individual files (10 total on disk incl. closed BUG-053); the 4 files this cycle came from the autonomous code-review pass, not the originally-scoped task, but the acceptance criteria is met. No longer carried | — (done) |
| QA plan | 26th+ consecutive cycle missing | — see QA Plan Gate below |
| First playtest | Unreached, last log 2026-06-12 | — blocked on the smoke session |

---

## Tasks

### Must Have (Critical Path)

| ID | Task | Agent/Owner | Est. Days | Dependencies | Acceptance Criteria |
|----|------|-------------|-----------|--------------|---------------------|
| S14-01 | Fix BUG-067 — correct the swapped calls in `ResourceReceiver.ReceverRecovery`/`ReceiveReduction` so recovery effects call the recovery path on `VitalStatsComponent`, not the reduction path | gameplay-programmer | 0.05 | None — literal first commit | A healing item pickup increases current HP/Mana; it no longer decreases it |
| S14-02 | Fix BUG-068 — null-guard `currentAbility.Definition` in `AbilityHolder.GetAbility()` after a `TryGetValue` miss; no-op or log instead of dereferencing null | gameplay-programmer | 0.05 | None — literal second commit | Pressing a skill key bound to an unbound `AbilitySlot` does not throw an NRE |
| S14-03 | Fix BUG-063 — remove the `#if UNITY_EDITOR` / `[SerializeField]` block on `Stat.cs:63-66` above `private List<StatModifier> modifiers`, as a fully isolated single-purpose commit | gameplay-programmer | 0.05 | None | `Stat.modifiers` carries no `[SerializeField]` under any build symbol; committed separately from any other change |
| S14-04 | Fix BUG-064 item 7 — wire `RangeWeapon.cs`'s `poolManager` via `[Inject] Construct(IObjecPoolService)`, mirroring `ItemSpawner.cs:8-10` exactly | gameplay-programmer | 0.1 | None | `RangeWeapon.cs` no longer relies on Inspector serialization of an interface field; `poolManager` is non-null at runtime via DI |
| S14-05 | Fix BUG-065 — resolve `PlayerMovement` via `Core.GetCoreComponent` in `PlayerDeathState.Enter()` and stop it | gameplay-programmer | 0.1 | None | Player no longer slides/moves during the death state |
| S14-06 | Fix BUG-066 + BUG-070 together — guard `EntityVitalStats` and `VitalStatsComponent`'s `GetCurrentStatValue`/`ReceiverRecovery`/`ReceiveReduction` with `TryGetValue` instead of a raw indexer; consider a shared guarded-lookup helper to prevent a third recurrence | ai-programmer | 0.2 | None | No raw `KeyNotFoundException` possible from either component's public API |
| S14-07 | **Owner-in-Editor Play Mode smoke session** — open `LoadRandomMap`, confirm Console clean, kill one enemy, fire the ranged weapon once, press a skill once | owner (Kay) | 0.2 | S14-01, S14-02, S14-04 | Console clean; enemy dies with no NRE; ranged weapon fires and deals damage; skill press does not throw — first Play Mode evidence in 6+ sprints |
| S14-08 | **Forced decision** — off-plan feature work review gate: does any multi-file change touching a new/actively-promoted system get a same-day `/code-review` pass regardless of sprint-plan status? | producer | 0.1 | None | Written decision recorded in this file or a linked doc |
| S14-09 | Land the process gate as an enforced pre-push hook, even a bare placeholder (`exit 0` + TODO) — 21st carry | producer / owner (Kay) | 0.15 | None | A working `.git/hooks/pre-push` (or committed equivalent, documented as required) exists |

Must-Have total ≈ **0.9d** (0.05+0.05+0.05+0.1+0.1+0.2+0.2+0.1+0.15).

### Should Have

| ID | Task | Agent/Owner | Est. Days | Dependencies | Acceptance Criteria |
|----|------|-------------|-----------|--------------|---------------------|
| S14-10 | **Forced decision** — S4-05/S4-06 keep-or-cut, 18th+ carry, zero movement across every prior cycle | owner (Kay) | 0.1 | None | Written decision recorded in this file or a linked doc |
| S14-11 | Flip ADR-0002 (`EnemyManager` singleton) `Proposed → Accepted`, 14th+ carry | producer | 0.1 | None | ADR-0002 Status reads Accepted; sign-off note recorded |
| S14-12 | File an ADR (or explicit backlog decision) resolving TD-040 — Abilities v1 (`ActivateSkill`) vs v2 (`AbilityDefinition`) convergence, before a 4th sprint of v2 work ships with the decision still open | technical-director | 0.3 | None | ADR filed under `docs/architecture/` or an explicit "stays dual-path" decision recorded, resolving BUG-052's widened scope |
| S14-13 | Write first EditMode tests for the Entity/Player vitals guard — `tests/EditMode/` has been `.gitkeep`-only for the entire project history (TD-014). Use BUG-066/BUG-070 as the first assertion | ai-programmer / qa-lead | 0.3 | S14-06 | At least one passing EditMode test asserting `GetCurrentStatValue`/`ReceiveReduction` on a `StatType` absent from the profile does not throw an unlabelled framework exception, on both `EntityVitalStats` and `VitalStatsComponent` |
| S14-14 | Fix BUG-069 — wire `AbilityInstance.CanStart()`'s cooldown gate into the actual activation call path; currently has zero callers project-wide | gameplay-programmer | 0.15 | None | An ability cannot be re-triggered before its `Cooldown` elapses |

### Nice to Have

| ID | Task | Agent/Owner | Est. Days | Dependencies | Acceptance Criteria |
|----|------|-------------|-----------|--------------|---------------------|
| S14-N1 | First full playtest session — once S14-07 confirms a stable build | producer / owner | — | S14-07 | `/playtest-report` filed — last log 2026-06-12 |
| S14-N2 | Re-verify BUG-042/043/044/046/033/NEW-1-4 once more against the S14-07 smoke session, not just source | qa-lead | 0.2 | S14-07 | Each item's acceptance criteria checked against a confirmed-compiling, confirmed-running build |

---

## Risks

| Risk | Probability | Impact | Mitigation |
|------|------------|--------|------------|
| The six trivial Must-Have fixes lose every session to unplanned work a 4th time — same pattern as Sprint 11/12/13 | High — identical framing failed 3 sprints running despite a schedule note each time | High | **Hard gate this cycle, not another schedule note**: the daily plan (`sprint-14-daily-plan.md`) names S14-01 through S14-06 as the literal first six commits of the sprint, checked explicitly at each standup before any other diff is reviewed. Per the retro's own recommendation, a 4th schedule-note failure should be escalated to "accepted risk" in the next retro rather than carried a 7th/8th time |
| S14-07 (owner-in-Editor session) does not happen — 7th consecutive sprint attempt at the identical gate | High — identical framing failed in Sprint 8 through Sprint 13 without exception | High | Retro explicitly recommends: if this does not happen this cycle, stop scheduling it as a task in Sprint 15 and instead record it as a formal accepted-risk decision requiring explicit owner sign-off, rather than a 7th silent re-ask |
| Off-plan, unreviewed feature work displaces the sprint plan again | Medium-High — happened 2 cycles running, the second time shipping 4 new S1/S2 bugs | High | S14-08 forces an explicit producer decision this cycle rather than deferring a 3rd time |
| No QA plan — 26th+ consecutive cycle | Confirmed | Medium | Flagged explicitly below, deferred to owner per every prior cycle's handling |
| No Unity CLI / no `gh` CLI in this environment | Known constraint | Low | Play Mode smoke check (S14-07) is manual in-Editor by the owner; PR creation noted as a manual follow-up |

---

## Dependencies on External Factors

- No Unity CLI — the Play Mode smoke gate on S14-07 (and everything sequenced after it, including
  formal closure of BUG-067/068) requires manual in-Editor confirmation by the owner.
- `gh` CLI unavailable — draft PR for `sprint-14` (base `sprint-13`) not auto-created; run manually:
  `gh pr create --draft --base sprint-13 --head sprint-14 --title "Sprint 14"`.

---

## Definition of Done for This Sprint

- [ ] S14-01: BUG-067 fixed — healing items heal, not damage
- [ ] S14-02: BUG-068 fixed — unbound ability slot does not NRE
- [ ] S14-03: BUG-063 fixed — `Stat.modifiers` has no `[SerializeField]` under any build symbol
- [ ] S14-04: BUG-064 item 7 fixed — `RangeWeapon.cs` DI-wired, no Inspector-serialized interface field
- [ ] S14-05: BUG-065 fixed — `PlayerDeathState` stops `PlayerMovement` on Enter()
- [ ] S14-06: BUG-066 + BUG-070 fixed — no raw dictionary indexer on either vitals component's public API
- [ ] S14-07: Owner-in-Editor Play Mode smoke session confirmed
- [ ] S14-08: Off-plan review gate decision recorded
- [ ] S14-09: Process gate landed as at least a placeholder enforced hook
- [ ] QA Plan gate resolved (see below)

Everything else (S14-10 through S14-14, all Nice-to-Have) is Should-Have — carried but not blocking
sprint close, consistent with prior sprints' Definition of Done scoping.

---

## QA Plan

⚠️ **No QA plan exists** for Sprint 14 (`production/qa/qa-plan-sprint-14.md` not found) — **26th+
consecutive sprint cycle** without one. This kickoff ran autonomously (no owner present); consistent
with every prior cycle's handling of this gate, the choice requiring judgment (full plan now vs. defer)
is deferred to the owner rather than decided unattended.

> ⚠️ This sprint was started without a QA plan. Run `/qa-plan sprint` before the last story is
> implemented. The Production → Polish gate requires a QA sign-off report, which requires a QA plan.
> Given S14-07 is this sprint's first real Play Mode verification opportunity in 6+ sprints, a QA plan
> run early would meaningfully de-risk sign-off later — this recommendation has now repeated 26+ times.

---

## Next Sprint Outlook (Sprint 15)

- If Must-Have closes clean: first playtest (S14-N1, if not run here), re-verification of the older
  bug set against a confirmed build (S14-N2, if not run here), HUD health bar work (blocked on the death
  chain being confirmed stable).
- S14-10/S14-11 (owner sign-off pass) and S14-12 (Abilities v1/v2 convergence ADR), if not completed
  here — architecture debt should not compound further without them.
- BUG-069 (cooldown gate), if not completed here — every player ability is currently spammable.
- QA-plan process item, if not completed as Should-Have here — 27th+ cycle would be the next flag point.
- If S14-07 (the owner smoke session) does not occur this cycle either: Sprint 15 should not re-ask it
  as a task a 7th time — record it as an explicit accepted-risk decision instead, per this retro's
  recommendation.
- Between-room upgrade cards (`BaseStatsSO.AddModifiersFromSource()`), once combat is confirmed stable
  in Play Mode for a full sprint running.
