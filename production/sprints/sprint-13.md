# Sprint 13 — 2026-09-08 to 2026-09-12

**Opened:** 2026-09-07 (Sunday 22:00 `pm-weekly-kickoff`, on-slot autonomous run — no owner present).
Branch `sprint-13`, created from `sprint-12` tip (`3f6abeb`, "chore(wrapup): weekly wrap-up 2026-09-06").
A stale local `sprint-13` branch already existed (pointing at an older ancestor, `1835cbe`, with zero
unique commits of its own — confirmed via `git log sprint-12..sprint-13` returning empty before reset);
it was reset (`git branch -f sprint-13 sprint-12`) to the current `sprint-12` tip rather than left
diverged, since it carried no work that would have been lost. `gh` CLI unavailable in this environment
(`gh: command not found`) — draft PR (`--base sprint-12 --head sprint-13`, title `Sprint 13`) was **not**
auto-created; run manually if desired:
`gh pr create --draft --base sprint-12 --head sprint-13 --title "Sprint 13"`

**Review mode:** lean (from `production/review-mode.txt`) — producer feasibility gate (PR-SPRINT)
skipped per lean-mode rule.

---

## Sprint Goal

**Close BUG-064 for real** — land the one remaining sub-item (`RangeWeapon.cs` DI wiring) as the literal
first commit, then get the sprint's first-ever owner-in-Editor Play Mode confirmation (Console-clean,
kill one enemy, fire the ranged weapon once). Every other verification in this backlog — BUG-053,
BUG-042/043/044/046/033/NEW-1-4 — has been "fixed pending Play Mode confirmation" for 6+ sprints running;
this sprint's second priority is finally producing that confirmation, not another source-read pass.

---

## Capacity

- Total days: 5
- Buffer (20%): 1 day
- Available: 4 days

Must-Have load ≈ 1.15d.

---

## Carryover from Previous Sprint

Full detail: `production/sprints/sprint-12.md` (closure block at top),
`production/retros/retro-sprint-12-2026-09-06.md`, `production/qa/bug-triage-2026-09-06.md`.

Sprint 12 closed **CLOSED — FAIL**: 0 of 5 Must-Have tasks fully met (2 partial), 0 of 6 Should-Have
landed. Verified directly against file contents at this kickoff (not commit messages) — every item below
is carried exactly as the Sprint 12 retro and bug-triage described it:

| Task | Reason | New Estimate |
|------|--------|-------------|
| **BUG-064 item 7** (`RangeWeapon.cs:7` `IObjecPoolService poolManager` has no `[Inject]` wiring) | Sole remaining sub-item of the sprint's stated goal; fix pattern already proven same-repo (`ItemSpawner.cs:8-10`) | 0.1d |
| S12-N1 → S13-02 (owner-in-Editor smoke session) | Never once occurred across Sprint 11 or Sprint 12 — every "fixed" claim in the backlog is a source read, not a confirmed running build, for 6+ sprints running | 0.2d |
| S12-02 → S13-03 (BUG-063, `Stat.cs:63-65` `[SerializeField]` regression) | 24th+ consecutive carry on a one-line documented fix. Triage explicitly recommends landing it as a fully isolated commit, decoupled from any other task | 0.05d |
| S12-05 → S13-04 (pre-push hook placeholder) | 19th+ carry, requires owner/producer action not agent time | 0.15d |
| S12-10 → S13-05 (`/doc-sync`) | Escalated to blocking by this cycle's triage — CLAUDE.md's Repository Layout is now stale against roughly half the codebase after the 298-file restructure | 0.4d |
| BUG-053 formal close | Fixed in source per triage, but "confirmed against the compiling build" never done — bundle with S13-02's smoke session | — (covered by S13-02) |
| BUG-065 (new, `PlayerDeathState` no longer stops `PlayerMovement`) | Found in Sprint 12's wrap-up code review | 0.1d |
| BUG-066 (new, `EntityVitalStats` unguarded dictionary indexer) | Found in Sprint 12's wrap-up code review; natural first EditMode-test assertion | 0.15d |
| S12-06 → S13-09 (S4-05/S4-06 forced decision) | 17th+ carry, oldest unresolved item in the project, zero movement any cycle | 0.1d |
| S12-07 → S13-10 (ADR-0002 → Accepted) | 13th+ carry, trivial sign-off-only change | 0.1d |
| S12-08 → S13-11 (VContainer/Item-system ADR for `LifetimeScope/`) | 3rd carry — duplication resolved (single `ObjectPoolManager` now), but the pattern itself remains undocumented | 0.3d |
| S12-09 → S13-12 (batch-generate remaining `BUG-NNN.md` files) | 7th+ cycle, only 6 of 9+ open P1 items have files (BUG-052/053/063/064/065/066 exist; earlier P1 table entries do not) | 0.2d |
| S12-11 → S13-13 (first EditMode/PlayMode tests, Entity damage chain) | TD-014, `.gitkeep`-only for the entire project history — BUG-066 gives this a concrete first assertion | 0.3d |
| QA plan | 25th+ consecutive cycle missing | — see QA Plan Gate below |
| First playtest | Unreached, last log 2026-06-12 | — blocked on S13-02 |

---

## Tasks

### Must Have (Critical Path)

| ID | Task | Agent/Owner | Est. Days | Dependencies | Acceptance Criteria |
|----|------|-------------|-----------|--------------|---------------------|
| S13-01 | Fix BUG-064 item 7 — wire `RangeWeapon.cs`'s `poolManager` via `[Inject] Construct(IObjecPoolService)`, mirroring `ItemSpawner.cs:8-10` exactly | gameplay-programmer | 0.1 | None — literal first commit | `RangeWeapon.cs` no longer relies on Inspector serialization of an interface field; `poolManager` is non-null at runtime via DI |
| S13-02 | **Owner-in-Editor Play Mode smoke session** — open `LoadRandomMap`, confirm Unity Console shows zero errors, kill one enemy, fire the ranged weapon once | owner (Kay) | 0.2 | S13-01 | Console clean; enemy dies with no NRE (closes BUG-053 formally); ranged weapon fires and deals damage (closes BUG-064) — first Play Mode evidence in 6+ sprints |
| S13-03 | Fix BUG-063 — remove the `#if UNITY_EDITOR` / `[SerializeField]` block on `Stat.cs:63-65` above `private List<StatModifier> modifiers`, as a fully isolated single-purpose commit | gameplay-programmer | 0.05 | None | `Stat.modifiers` carries no `[SerializeField]` under any build symbol; committed separately from any other change |
| S13-04 | Fix BUG-065 — resolve `PlayerMovement` via `Core.GetCoreComponent` in `PlayerDeathState.Enter()` and stop it, matching CLAUDE.md's original BUG-044 intent | gameplay-programmer | 0.1 | None | Player no longer slides/moves during the death state; CLAUDE.md's BUG-044 claim is either true again or corrected |
| S13-05 | Fix BUG-066 — guard `EntityVitalStats.GetCurrentStatValue` / `ReceiverRecovery` / `ReceiveReduction` with `TryGetValue` instead of a raw indexer; log + no-op or throw a named exception on a missing `StatType` | ai-programmer | 0.15 | None | No raw `KeyNotFoundException` possible from any of the three methods; pairs with S13-13's first EditMode test |
| S13-06 | Land the process gate as an enforced pre-push hook, even a bare placeholder (`exit 0` + TODO) — 19th carry | producer / owner (Kay) | 0.15 | None | A working `.git/hooks/pre-push` (or committed equivalent, documented as required) exists |

Must-Have total ≈ **0.75d** (0.1+0.2+0.05+0.1+0.15+0.15).

### Should Have

| ID | Task | Agent/Owner | Est. Days | Dependencies | Acceptance Criteria |
|----|------|-------------|-----------|--------------|---------------------|
| S13-07 | Run `/doc-sync` — reconcile CLAUDE.md's Repository Layout, Known Bugs, and Event System sections against the 298-file restructure (`Assets/Script/System/`), the new VContainer DI layer, and the new Item/loot system | lead-programmer | 0.4 | S13-02's findings ideally, but should not wait indefinitely | CLAUDE.md matches verified current source paths and state |
| S13-08 | **Forced decision** — S4-05/S4-06 keep-or-cut, 17th+ carry, zero movement across every prior cycle | owner (Kay) | 0.1 | None | Written decision recorded in this file or a linked doc |
| S13-09 | Flip ADR-0002 (`EnemyManager` singleton) `Proposed → Accepted`, 13th+ carry | producer | 0.1 | None | ADR-0002 Status reads Accepted; sign-off note recorded |
| S13-10 | File an ADR for the VContainer/DI layer (`Assets/Script/System/LifetimeScope/`) — the duplicate-`ObjectPoolManager` symptom is resolved, but the DI pattern itself is still undocumented | technical-director | 0.3 | None | ADR filed under `docs/architecture/`; documents the `LifetimeScope`/`[Inject]` pattern as the project's standard for new services |
| S13-11 | Batch-generate remaining individual `production/qa/bugs/BUG-NNN.md` reports for any open P1 items still missing one | qa-lead | 0.2 | None | All open P1 items have individual files |
| S13-12 | Write first EditMode tests for the Entity damage chain — `tests/EditMode/` has been `.gitkeep`-only for the entire project history (TD-014). Use BUG-066 as the first assertion | ai-programmer / qa-lead | 0.3 | S13-05 | At least one passing EditMode test asserting `EntityVitalStats.ReceiveReduction`/`GetCurrentStatValue` on a `StatType` absent from the profile does not throw an unlabelled framework exception |

### Nice to Have

| ID | Task | Agent/Owner | Est. Days | Dependencies | Acceptance Criteria |
|----|------|-------------|-----------|--------------|---------------------|
| S13-N1 | First full playtest session — once S13-02 confirms a stable build | producer / owner | — | S13-02 | `/playtest-report` filed — last log 2026-06-12 |
| S13-N2 | Re-verify BUG-042/043/044/046/033/NEW-1-4 once more against the S13-02 smoke session, not just source | qa-lead | 0.2 | S13-02 | Each item's acceptance criteria checked against a confirmed-compiling, confirmed-running build |

---

## Risks

| Risk | Probability | Impact | Mitigation |
|------|------------|--------|------------|
| S13-02 (owner-in-Editor session) does not happen — same pattern that stalled this exact gate for 6+ consecutive sprints | High — identical framing failed in Sprint 10 through Sprint 12 without exception | High | Named as the sprint's #2 priority immediately after the 0.1d DI fix; kept as its own line item rather than folded into "S13-01 acceptance criteria" so its absence is visible on its own |
| Trivial/decision-avoidance items (S13-03, S13-04, S13-06, S13-08, S13-09) lose every session to larger work again, repeating the pattern named in the last four triage reports | High — explicitly the most consistent finding across recent cycles | Medium | Daily plan schedules the cheapest items as a dedicated block at the very start of Day 1, before any other file is opened |
| `/doc-sync` (S13-07) again loses to code work, letting drift compound a third consecutive cycle | Medium-High | Medium | Escalated to Should-Have's first line item this cycle per triage's explicit recommendation |
| No QA plan — 25th+ consecutive cycle | Confirmed | Medium | Flagged explicitly below, deferred to owner per every prior cycle's handling |
| No Unity CLI / no `gh` CLI in this environment | Known constraint | Low | Play Mode smoke check (S13-02) is manual in-Editor by the owner; PR creation noted as a manual follow-up |

---

## Dependencies on External Factors

- No Unity CLI — the Play Mode smoke gate on S13-02 (and everything sequenced after it, including formal
  closure of BUG-053 and BUG-064) requires manual in-Editor confirmation by the owner.
- `gh` CLI unavailable — draft PR for `sprint-13` (base `sprint-12`) not auto-created; run manually:
  `gh pr create --draft --base sprint-12 --head sprint-13 --title "Sprint 13"`.

---

## Definition of Done for This Sprint

- [ ] S13-01: BUG-064 item 7 fixed — `RangeWeapon.cs` DI-wired, no Inspector-serialized interface field
- [ ] S13-02: Owner-in-Editor Play Mode smoke session confirmed — Console clean, enemy killed, ranged weapon fired
- [ ] S13-03: BUG-063 fixed — `Stat.modifiers` has no `[SerializeField]` under any build symbol
- [ ] S13-04: BUG-065 fixed — `PlayerDeathState` stops `PlayerMovement` on Enter()
- [ ] S13-05: BUG-066 fixed — no raw dictionary indexer on `EntityVitalStats`'s public API
- [ ] S13-06: Process gate landed as at least a placeholder enforced hook
- [ ] QA Plan gate resolved (see below)

Everything else (S13-07 through S13-12, all Nice-to-Have) is Should-Have — carried but not blocking
sprint close, consistent with prior sprints' Definition of Done scoping.

---

## QA Plan

⚠️ **No QA plan exists** for Sprint 13 (`production/qa/qa-plan-sprint-13.md` not found) — **25th+
consecutive sprint cycle** without one. This kickoff ran autonomously (no owner present); consistent
with every prior cycle's handling of this gate, the choice requiring judgment (full plan now vs. defer)
is deferred to the owner rather than decided unattended.

> ⚠️ This sprint was started without a QA plan. Run `/qa-plan sprint` before the last story is
> implemented. The Production → Polish gate requires a QA sign-off report, which requires a QA plan.
> Given S13-02 is this sprint's first real Play Mode verification opportunity in 6+ sprints, a QA plan
> run early would meaningfully de-risk sign-off later — this recommendation has now repeated 25+ times.

---

## Next Sprint Outlook (Sprint 14)

- If Must-Have closes clean: first playtest (S13-N1, if not run here), re-verification of the older
  bug set against a confirmed build (S13-N2, if not run here), HUD health bar work (blocked on the death
  chain being confirmed stable — now finally possible once S13-02 lands).
- S13-08/S13-09 (owner sign-off pass) and S13-10 (DI ADR), if not completed here — architecture debt
  should not compound further without them.
- `/doc-sync` (S13-07), if not completed here — CLAUDE.md drift should not compound a fourth cycle.
- QA-plan process item, if not completed as Should-Have here — 26th+ cycle would be the next flag point.
- Between-room upgrade cards (`StatsSO.AddModifiersFromSource()`), once combat is confirmed stable in
  Play Mode for a full sprint running.
