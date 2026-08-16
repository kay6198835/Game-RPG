# Sprint 9 — 2026-08-10 to 2026-08-14

**Status: CLOSED — CONCERNS (2026-08-15 Saturday `pm-weekly-wrapup`, on-slot autonomous run).** Full
detail: `production/retros/retro-sprint-09-2026-08-15.md` and `production/qa/bug-triage-2026-08-15.md`.

**Final scorecard — 2 of 6 Must-Have tasks code-complete, 0 of 6 Play-Mode-confirmed (S9-12 unreached),
on `sprint-09`:**

| Item | Final status |
|------|--------------|
| S9-00 process gate | ❌ Never drafted — 4th carry, unaddressed since Sprint 4 |
| S9-01 / BUG-041 (player attack unwired) | ✅ CLOSED — `MeleeWeapon.OnActivate()` now correctly runs `OverlapCircleNonAlloc` + `INegativeReceiver.TakeDamage()`, landed via the weapon-architecture refactor merged into `sprint-09` (`bfe7dd4`, 2026-08-13). Static-verified only; Play Mode confirmation still pending. |
| S9-02 / BUG-042 + BUG-053 (enemy TakeDamage + duplicate receiver) | ❌ OPEN, zero movement all sprint — `EntityCore.TakeDamage()` still throws, `EntityNegativeReciver.cs` duplicate still unfixed. Now the sprint's only remaining fully-untouched Must-Have item. |
| S9-06 / BUG-032 (one-line fix) | ⚠️ Code-level fix applied (`EntityWeaponMelee.cs` guarded `GetCoreComponent`) — leaves open pending Play Mode confirmation that the enemy prefab's `holder` reference is actually wired |
| S9-07 / BUG-033 (one-line fix) | ❌ OPEN, untouched — 7th carry |
| S9-12 (Play Mode verify gate) | ❌ Not reached — no Unity Editor session in this automated environment |
| S9-10 (ADR-0002 Accepted) | ❌ Still **Proposed** — untouched |
| S9-11 (S4-05/S4-06 forced decision) | ❌ Zero movement — 9th carry becomes 10th |
| QA Plan | ❌ Still none — 9th consecutive cycle without one |
| First playtest | ❌ Not run — 10th consecutive cycle |
| **Bonus, not originally scoped**: BUG-059 (RangeWeapon.Attack empty) | ✅ CLOSED as a side effect of the weapon-architecture refactor |
| **Bonus, not originally scoped**: BUG-060 (dead `WeaponsController`/`PlayerCombat` references) | ✅ CLOSED — both files deleted this week |

**Root cause of what remains open:** S9-02 (the sprint's largest single item) received zero code
movement across all 5 scheduled days, unlike S9-01/S9-06 which finally landed via a real coding session
(Tue-Thu) merged from `claude/weapon-architecture-stats-dermi4`. S9-00, meant to govern exactly this
kind of feature-branch merge, was never drafted — this week's merge happened to be safe, but the sprint
has no mechanism to have known that in advance, the same gap Sprint 8's BUG-053-introducing merge fell
through.

**Carried forward to Sprint 10:** BUG-042, BUG-053, BUG-054 (bundle, now the sprint's sole remaining
true blocker), BUG-043, BUG-044, Bug #6/S7-11 (10th carry), BUG-033 (7th carry), ADR-0002 flip (6th
carry), S4-05/S4-06 decision (10th carry), QA plan (9th cycle), S9-12 verification gate (now also
covering BUG-032's prefab-wiring confirmation), S9-00/process gate (4th carry).

**Process recommendation for Sprint 10:** open with S9-02 as the literal first task (everything else in
Sprint 9's original Must-Have set is closed or verification-only), and get an owner-in-Editor session
scheduled specifically to run S9-12 — three consecutive sprints (7, 8, 9) have now closed without a
single Play Mode confirmation.

---

**Opened:** 2026-08-10 (Monday, overrun Sunday 22:00 kickoff — the scheduled Sun 2026-08-09 22:00 run
did not fire/complete in time; no `/weekly-wrapup` closed Sprint 8 on Sat 2026-08-08 either). Branch
`sprint-09`, created from `sprint-08` tip (`a29895b`, last commit "daily standup 2026-08-07" — no
further commits existed on `sprint-08` at branch time). `gh` CLI unavailable in this environment
(`gh: command not found`) — draft PR (`--base sprint-08 --head sprint-09`, title `Sprint 9`) was
**not** auto-created; run manually if desired:
`gh pr create --draft --base sprint-08 --head sprint-09 --title "Sprint 9"`

**Review mode:** lean (from `production/review-mode.txt`) — producer feasibility gate (PR-SPRINT)
skipped per lean-mode rule.

---

## Sprint Goal

**Second recovery attempt — land the two P0 combat bugs with nothing else competing for the branch.**
Sprint 8 closed CONCERNS with 0 of 8 Must-Have items landed across all 5 scheduled days, while ~1300
lines of unplanned AI/pathfinding work landed on a separate branch and was later merged in, introducing
a new bug (BUG-053) in the process. The sprint's own countermeasure — a root-cause conversation
requiring the owner in the room — was scheduled 6 times across Sprints 6-8 and held 0 times. This
sprint does not schedule a 7th attempt. Instead: (1) scope is cut to the smallest possible Must-Have
set — just the two P0 bugs plus the four trivial one-liners already sitting idle for 3-6 carries each,
and (2) the process fix is reframed as something that doesn't need a synchronous conversation to exist
(see S9-00 below).

---

## Capacity

- Total days: 5
- Buffer (20%): 1 day
- Available: 4 days

Must-Have load ≈ 1.05d — deliberately cut below Sprint 8's already-light 1.75d. Sprint 8 proved that
even a light Must-Have load doesn't protect against an owner-facilitation dependency (S8-00) or
off-plan branch drift; this sprint narrows scope further and drops the conversation-shaped item
entirely.

---

## Carry-Over From Sprint 8

Full detail: `production/sprints/sprint-08.md` (Status/closure section) and
`production/sprints/sprint-08-daily-plan.md` (Standup Log, Carry-Over Watch List).

| Item | Type | Priority | Origin |
|------|------|----------|--------|
| BUG-041 — `MeleeWeapon.Attack()` empty override; `MakeDamage()` non-override, body commented out — player deals zero damage | Bug (S1, 2nd carry) | P0 | 2nd carry, Sprint 7→8→9 |
| BUG-042 — `EntityCore.TakeDamage()` throws `NotImplementedException` | Bug (S1, 2nd carry) | P0 | 2nd carry, Sprint 7→8→9 |
| BUG-053 — `EntityNegativeReciver.cs` duplicate calls `Core.GetCoreComponent(out PlayerInputHandler input)` (wrong hub — `Core` here is `EntityCore`) and emits `ON_PLAYER_DEATH` on enemy death | Bug (S1, new in S8, 1st carry) | P0 | 1st carry — merge-introduced 2026-08-06, must be resolved together with BUG-042 (delete the duplicate, don't patch both) |
| BUG-043 — `EntityAttack.cs` and `EntityWeaponMelee.cs` still two divergent enemy damage paths (partial: `nextAttackTime` now advances) | Bug (S1, 2nd carry) | P1 | 2nd carry |
| BUG-044 — `PlayerDeathState.LogicUpdate()` body fully commented out | Bug (S1, 2nd carry) | P1 | 2nd carry |
| Bug #6 / S7-11 — `NegativeReciver.currentHealth` still separate from `PlayerData.currentHealth`; no `ON_PLAYER_DEATH` listener; no EditMode test | Bug (S1, 9th carry) | P0 | 9th carry — largest single carried item, regressed twice historically |
| BUG-032 — `EntityWeaponMelee.cs:26` `input` field assignment still commented out | Bug (S1, 3rd carry, one-line fix) | P1 | 3rd carry |
| BUG-033 — `EnemySpawner.cs:62` null-check order still wrong | Bug (S1, 6th carry, one-line fix) | P1 | 6th carry |
| Bug #14 — `MazeController.Awake()` missing `return` after `Destroy(gameObject)` | Bug (S1, carried) | P2 | Carried, untouched |
| Bug #15 — `File.ReadAllText(Application.dataPath...)` breaks Player builds | Bug (S1, carried) | P2 | Carried, untouched |
| ADR-0002 (`EnemyManager` singleton) Proposed→Accepted | Decision | P2 | 5th carry |
| S4-05/S4-06 keep-or-cut call | Decision | P2 | 9th carry — force this cycle, no further silent carry |
| S8-00 root-cause conversation | Process | — | **Retired, not carried as a task.** Replaced by S9-00 (see below) — 6 scheduling attempts, 0 held, across Sprints 6-8. Continuing to re-schedule the same unenforceable item is itself the pattern this sprint is trying to break. |
| S7-D3 — individual `production/qa/bugs/BUG-NNN.md` files | Process | P3 | Not started (2 files exist: BUG-052, BUG-053, ad hoc — not systematic) |
| QA plan | Risk | P1 | 8 consecutive cycles with none |
| First playtest | Milestone | P2 | 10th cycle attempt, gated on BUG-041/042/053 |

---

## Tasks

### Must Have (P0/P1)

| ID | Task | Agent/Owner | Est. Days | Dependencies | Acceptance Criteria |
|----|------|-------------|-----------|--------------|---------------------|
| S9-00 | **Adopt an enforceable process gate in place of the retired root-cause conversation** — recommended: a pre-push/pre-merge compile check on `sprint-09`, and/or a written rule that AI/pathfinding work happens only on its own tracked branch and is reviewed before merge, not merged silently. This is a decision + a config/doc change, not a meeting — can be actioned without synchronous owner presence. | producer / owner | 0.1 | None | Either a working pre-push hook exists, or a written rule is added to a project doc (e.g. `CLAUDE.md` or a new `production/process/` note) stating where AI/pathfinding work is allowed to land. Owner sign-off still required to adopt, but the artifact itself can be drafted without a live conversation. |
| S9-01 | Fix BUG-041 — wire `MeleeWeapon.Attack()`: restore the logic currently stranded in the non-override `MakeDamage()` method (or rewrite `Attack()` directly) so it calls `Physics2D.OverlapCircleNonAlloc` + `INegativeReceiver.TakeDamage()`, mirroring `EntityWeaponMelee.Attack()` | gameplay-programmer | 0.2 | None | Player melee attack in Play Mode registers a hit; no `NotImplementedException`; damage lands on an enemy |
| S9-02 | Fix BUG-042 + BUG-053 together — implement `EntityCore.TakeDamage()` for real (health decrement + death-state hookup); **delete** `EntityNegativeReciver.cs` (the duplicate) rather than patching its wrong-hub `GetCoreComponent` call | ai-programmer | 0.3 | None | Player hit on enemy in Play Mode: no exception; health decrements; exactly one `INegativeReceiver` implementer exists on the enemy prefab; no `ON_PLAYER_DEATH` emitted from enemy code |
| S9-06 | Fix BUG-032, 3rd carry — `EntityWeaponMelee.cs:26` uncomment `Core.GetCoreComponent(out input)` | ai-programmer | 0.1 | None | Enemy skill use in Play Mode: no NullReferenceException |
| S9-07 | Fix BUG-033, 6th carry — `EnemySpawner.cs:62` swap to `set == null \|\| set.Count == 0` | gameplay-programmer | 0.1 | None | Empty-pool room spawns without NullReferenceException |
| S9-12 | **Verify** — Play Mode smoke check: player deals damage to enemy AND enemy deals damage to player, both without exceptions | lead-programmer | 0.2 | S9-01, S9-02 | Owner confirms in-Editor; do not treat any of the above as done without this, per Sprint 7's S7-08 and Sprint 8's S8-12 both never being confirmed |

Must-Have total ≈ **1.0d** (0.1+0.2+0.3+0.1+0.1+0.2) — narrower than Sprint 8's already-light 1.75d.
Nothing else is scheduled as Must-Have until these six items land; Bug #6 (largest carried item) is
deliberately placed in Should-Have this cycle rather than repeating Sprint 8's pattern of scheduling
more than the branch has actually been able to absorb.

### Should Have (P2/P3)

| ID | Task | Agent/Owner | Est. Days | Dependencies | Acceptance Criteria |
|----|------|-------------|-----------|--------------|---------------------|
| S9-05 | Bug #6 / S7-11, 9th carry — write-through `NegativeReciver`→`PlayerData.currentHealth`, confirm `ON_PLAYER_DEATH` listener fires, EditMode test `TakeDamage_BelowZero_TriggersDeathState` | gameplay-programmer | 0.4 | S9-01, S9-02 | Test passes; single HP source of truth; `Reborn()` contract intact — do not close without the test |
| S9-04 | Fix BUG-044 — restore `PlayerDeathState.LogicUpdate()` body | gameplay-programmer | 0.15 | None | `PlayerDeathState` emits on death; no commented-out logic left |
| S9-03 | Fix BUG-043 — consolidate `EntityAttack.cs` and `EntityWeaponMelee.cs` into one enemy damage path | ai-programmer | 0.3 | S9-02 | One clear enemy-attack code path; cooldown gate functionally gates repeat attacks |
| S9-10 | Flip ADR-0002 (`EnemyManager` singleton) `Proposed → Accepted`, 5th carry | producer | 0.1 | None | ADR-0002 Status reads Accepted; sign-off note recorded |
| S9-11 | **Forced decision** — S4-05/S4-06 keep-or-cut, 9th carry | producer | 0.1 | None | Written decision recorded — make the call, do not re-carry a 10th time |
| S9-08 | Fix Bug #14 — add `return` after `Destroy(gameObject)` in `MazeController.Awake()` | lead-programmer | 0.1 | None | Duplicate `MazeController` instance no longer overwrites `Instance` |
| S9-D1 | Process change — file individual `production/qa/bugs/BUG-NNN.md` reports for BUG-041/042/043/044/053 | qa-lead | 0.2 | None | All five have individual files |

### Nice to Have

| ID | Task | Agent/Owner | Est. Days | Dependencies | Acceptance Criteria |
|----|------|-------------|-----------|--------------|---------------------|
| S9-09 | Fix Bug #15 — replace `File.ReadAllText(Application.dataPath...)` with `TextAsset` refs or StreamingAssets | lead-programmer | 0.5 | None | Room load works from a Player build |
| S9-N1 | First full playtest session — once S9-01/02/12 land | producer / owner | — | S9-01, S9-02, S9-12 | `/playtest-report` filed — 10th cycle attempt |
| S9-N2 | `/doc-sync` — `CLAUDE.md` Repository Layout is stale on filenames after the Sprint 8 merge (`WeaponMelee.cs`→`MeleeWeapon.cs`, `NewPlayer.cs`→`Player.cs`, etc.) | lead-programmer | 0.3 | None | Repository Layout section matches current filenames |

---

## Definition of Done for This Sprint

- [ ] S9-00 process gate adopted (artifact drafted at minimum; owner sign-off to formally adopt)
- [ ] Player attack deals damage to enemy in Play Mode, verified (BUG-041)
- [ ] Enemy `TakeDamage()` no longer throws; single `INegativeReceiver` implementer on enemy prefab; `EntityNegativeReciver.cs` deleted (BUG-042 + BUG-053)
- [ ] BUG-032, BUG-033 confirmed fixed in Play Mode
- [ ] S9-12 Play Mode gate actually confirmed by the owner
- [ ] QA Plan gate resolved (see below)

Everything else (Bug #6, BUG-043, BUG-044, ADR-0002, S4-05/S4-06) is Should-Have — carried but not
blocking sprint close, to avoid repeating Sprint 8's pattern of a Definition of Done wider than what
the branch demonstrably absorbs in a week.

---

## QA Plan

⚠️ **No QA plan exists** for Sprint 9 (`production/qa/qa-plan-sprint-09.md` not found) — **8th
consecutive sprint cycle** without one. This kickoff ran autonomously (no user present); per the QA
plan gate, the choice requiring judgment (full plan now vs. defer) is deferred to the owner rather
than decided unattended, consistent with every prior cycle's handling of this same gate.

> ⚠️ This sprint was started without a QA plan. Run `/qa-plan sprint` before the last story is
> implemented. The Production → Polish gate requires a QA sign-off report, which requires a QA plan.
> Given S9-05 is EditMode-test-gated and S9-12 is a Play Mode verification gate, a QA plan run early
> would meaningfully de-risk sign-off later — this recommendation has now repeated 8 times.

---

## Risks

| Risk | Probability | Impact | Mitigation |
|------|------------|--------|------------|
| Off-plan work recurs a 4th time now that the conversation-based countermeasure is retired | High | High | S9-00 replaces the conversation with an artifact (hook or written rule) that doesn't need synchronous presence to exist; still requires owner sign-off to become binding |
| BUG-041/042/053 fixes uncover further combat-path issues not caught by static review | Medium | High | S9-12 is an explicit Play-Mode verification gate on both attack directions |
| Bug #6 fails a 5th time if attempted this sprint (regressed twice historically) | Medium | Medium | Deliberately moved to Should-Have this cycle, not Must-Have, to avoid the same overcommitment pattern as Sprint 8 |
| No QA plan — 8th consecutive cycle | Confirmed | Medium | Flagged explicitly above; deferred to owner rather than silently dropped |
| No Unity CLI in this environment | Known constraint | Low | All Play Mode smoke checks (S9-12) are manual in-Editor by the owner |
| Weekend automation gap recurs (Sprint 8→9 transition itself overran by ~1 day/1 missed wrapup) | Medium | Low | Noted for scheduler review — not actionable by this run |

---

## Dependencies on External Factors

- No Unity CLI — Play Mode smoke checks require manual in-Editor confirmation by the owner.
- `gh` CLI unavailable — draft PR for `sprint-09` (base `sprint-08`) not auto-created; run manually:
  `gh pr create --draft --base sprint-08 --head sprint-09 --title "Sprint 9"`.

---

## Next Sprint Outlook (Sprint 10)

- If Must-Have closes clean: Bug #6 (if not done as Should-Have here), BUG-043 consolidation, first
  playtest (S9-N1, if not already run), `EnemyManager` lifecycle body, resume Pathfinding
  correctness/perf work — but only on a properly scoped/tracked branch per S9-00's rule, not drive-by.
- Bug #14/#15 and QA-plan process items, if not completed as Should-Have here.
- HUD health bar + between-room upgrade cards, once the death chain (both directions) is confirmed stable.
