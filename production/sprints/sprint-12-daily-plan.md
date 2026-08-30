# Sprint 12 — Daily Plan & Progress Tracker

> **Sprint**: 2026-08-31 (Mon) → 2026-09-04 (Fri)
> **Companion to**: `sprint-12.md` (formal plan) — this file is the day-by-day breakdown + live tracker
> **Maintained by**: PM assistant — updated each session/standup
> **Routines (Claude Code)**:
>   - **Mon–Fri 10:00** → `/daily-standup`
>   - **Sat 22:00** → `/weekly-wrapup`
>   - **Sun 22:00** → `/weekly-kickoff`
> **Opened**: 2026-08-30 (Sunday 22:00 kickoff, on-slot) — autonomous scheduled run, no owner present.
> Branch `sprint-12` created from `sprint-11` tip (`6348dc6`), after `git fetch origin sprint-11`
> confirmed the local ref was current. Sprint 11 closed **FAIL** — 2/5 Must-Have fully met, 1 partial,
> overridden by a build-breaking regression (BUG-064) found in code review — see `sprint-11.md` closure
> block and `retro-sprint-11-2026-08-30.md` for full detail.

---

## Status Verdict: 🔴 BLOCKED at open — project does not compile at `HEAD` (BUG-064). Every task below except S12-01 and the two pure-decision items (S12-02, S12-06/S12-07) is hard-blocked until the build is restored.

---

## Day-by-Day Plan

### Mon 2026-08-31 — BUG-064 first, no exceptions; clear the two trivial items alongside it

| Task | Est. | Status | Notes |
|------|------|--------|-------|
| S12-02 (BUG-063, `Stat.cs:63-65` `[SerializeField]` regression) | 0.05d | ❌ NOT DONE | 18th+ carry — retro explicitly recommends this be the literal first commit of Sprint 12, before BUG-064 or alongside it, because it has repeatedly lost every session to larger work |
| S12-01 (BUG-064, project does not compile) | 0.4d | ❌ NOT DONE | Blocking. Needs a 2-minute architecture confirmation first: does `EntityData` get a new `Stats` SO field, or does `Entity.cs` get its `data`/`Data` pattern back with the SO type updated — decide before patching, then sweep every dangling reference (`EntityData.cs:8`, `Entity.cs`, `EntityInput.cs:57,61,81,83`, `EntityAttack.cs:68`, `EntityEffectStats.cs:20`) |
| S12-05 (pre-push hook placeholder) | 0.15d | ❌ NOT DONE | Land even a bare `exit 0` + TODO — 14th carry, stop the silent count |

Goal: land the two cheapest, zero-technical-blocker items (S12-02, S12-05) in the first 15 minutes of the
session, before opening any Entity-side file — retro's Process Improvements section names this exact
failure mode ("trivial items never win a session because they're never urgent enough to interrupt
whatever larger item is mid-flight"). Then commit fully to S12-01 for the rest of the day; it is the
sprint's sole hard blocker.

### Tue 2026-09-01 — Finish BUG-064's sub-fixes + Play Mode smoke gate

| Task | Est. | Notes |
|------|------|-------|
| S12-01 continued (payload mismatch, `Resgister`/`UnResgister` typo, `RangeWeapon` DI wiring) | remaining | Items c/d/e from the bug-triage recommended assignment if not finished Monday |
| S12-01 smoke gate | — | **Gate** — owner confirms in-Editor: Unity Console zero errors, enter `LoadRandomMap`, kill one enemy, fire the ranged weapon once. Do not mark S12-01 done without this — it's an Integration story per `test-standards.md` |

Goal: a compiling, smoke-confirmed build by end of day Tuesday — everything else this sprint depends on it.

### Wed 2026-09-02 — Re-verification pass + enemy health routing

| Task | Est. | Notes |
|------|------|-------|
| S12-03 (BUG-053/BUG-054, enemy health routing) | 0.2d | Depends on S12-01. Confirm `EntityNegativeReciver` no longer resolves `PlayerInputHandler` off `EntityCore`, no NRE, no `ON_PLAYER_DEATH` from enemy code |
| S12-04 (re-verify BUG-042/043/044/046/033/NEW-1-4) | 0.2d | Depends on S12-01. Do not trust CLAUDE.md's "FIXED" tags — check each against current source + one Play Mode pass |

Goal: last week's claimed-fixed bugs actually confirmed against a build that compiles, not just against intent.

### Thu 2026-09-03 — Owner sign-off pass + doc-sync + debt cleanup

| Task | Est. | Notes |
|------|------|-------|
| S12-06 (S4-05/S4-06 forced decision) | 0.1d | 15th carry — owner-judgment-only, batch with S12-07 |
| S12-07 (ADR-0002 → Accepted) | 0.1d | 10th carry, trivial sign-off |
| S12-10 (doc-sync, CLAUDE.md BUG-053 reconciliation) | 0.1d | Depends on S12-04's findings |
| S12-08 (DI/VContainer ADR for `LifetimeScope/`) | 0.3d | 2nd carry, now escalated — a second `ObjectPoolManager` exists undocumented |
| S12-09 (batch-generate remaining `BUG-NNN.md` files) | 0.2d | 6th+ cycle, only 3 of 9+ open P1 items have files |
| Buffer / catch-up | — | Reserved for S12-01/S12-03/S12-04 slippage if the build took longer than Tuesday |

### Fri 2026-09-04 — Tests + stretch + wrap prep

| Task | Est. | Notes |
|------|------|-------|
| S12-11 (first EditMode/PlayMode tests, Entity damage chain) | 0.3d | Cheapest moment to break the TD-014 empty-tests streak — the Entity code is already being touched this sprint |
| S12-N1 (first playtest) | — | Only if S12-01/S12-03 confirmed stable — last log 2026-06-12 |
| S12-N2 (resolve duplicate `ObjectPoolManager`) | 0.2d | If S12-08 landed and decided which implementation is live |
| Friday wrap-up prep | — | Feeds into Sat 22:00 `/weekly-wrapup` |

---

## Standup Log

### Sun 2026-08-30 — Weekly Kickoff (autonomous, no owner present)

Sprint 11 closed FAIL (2/5 Must-Have fully met, 1 partial — overridden by a build-breaking regression
found in Saturday's code review) — already finalized in Saturday's `pm-weekly-wrapup` run (2026-08-30),
this kickoff only opened Sprint 12. `git fetch origin sprint-11` confirmed the local ref matched before
branching. Re-verified the carried-forward state directly against `retro-sprint-11-2026-08-30.md` and
`production/qa/bug-triage-2026-08-30.md` (both written same-day by the wrap-up run) rather than
re-deriving from source, since both were produced from a direct source read already:

- 🔴 **BUG-064 (new)** — project does not compile at `HEAD`. `EntityData.cs:8` references deleted
  `EntityStatsSO`; `Entity.cs` lost its `data` field/`Data` getter but `LoadEntity()`/`LoadState()`/
  `SetDataEntity()` and external callers (`EntityInput.cs:81,83`, `EntityAttack.cs:68`,
  `EntityEffectStats.cs:20`) still reference them; `EntityInput.cs:57,61` still references deleted
  `EntityFindTarget`. Filed as `production/qa/bugs/BUG-064.md` during Saturday's wrap-up. **This is the
  sprint's sole hard blocker — S12-01, must be the literal first commit.**
- ❌ **BUG-063** — `Stat.cs:63-65` still has `[SerializeField]` gated behind `#if UNITY_EDITOR`. 18th+
  consecutive carry, still cheapest item in the entire backlog. S12-02.
- 🟡 **BUG-053/BUG-054** — functional symptom (wrong `ON_PLAYER_DEATH` emission) fixed per Thursday's
  Sprint 11 session, but blocked from verification by BUG-064. S12-03.
- ❌ **S11-03 → S12-05** (pre-push hook) — still no `.git/hooks/pre-push`. 14th carry.
- ❌ **S11-06 → S12-06** (S4-05/S4-06 decision) — 15th carry, oldest unresolved item in the project.
- ❌ **S11-08 → S12-07** (ADR-0002) — still `Proposed`. 10th carry.

Also found: `feature/fix-player-control` (the branch this run started on, pre-`sprint-11` merge) carried
one uncommitted modification to a Knight combo-attack `.anim` file — outside this run's write scope
(assets), stashed rather than discarded (`git stash` on `feature/fix-player-control`, message "wip:
uncommitted anim change on feature/fix-player-control before sprint kickoff"). Flagging for the owner to
recover via `git stash pop` on that branch — not carried onto `sprint-12`.

`gh` CLI still unavailable — draft PR not auto-created, manual command left in `sprint-12.md`. No QA plan
exists for the 14th+ consecutive cycle — flagged, deferred to owner per every prior cycle's handling.

---

## Carry-Over Watch List (re-verify every standup)

- **BUG-064 — P0/S1, project does not compile.** New this cycle, found in Saturday's code review. Blocks
  every other item in this sprint except the two trivial/decision-only ones. Must be the literal first
  commit.
- **BUG-063 (`Stat.cs` `[SerializeField]` regression)** — 18th+ consecutive carry on a one-line fix with
  an explanatory comment already in the file. No technical blocker has ever existed for this item.
- **BUG-053/BUG-054 (enemy health routing)** — functional symptom looks fixed per last week's session,
  but cannot be confirmed until BUG-064 lands. Sequence directly after.
- **S12-05 process gate** — now 14th carry, same underlying pattern since Sprint 6/9. Its absence is
  exactly what let last week's build-breaking refactor land ungated onto `sprint-11`.
- **S12-06 (S4-05/S4-06)** — 15th carry, zero movement any cycle. Decision-avoidance, not an estimation
  problem.
- **S12-07 (ADR-0002 Accept)** — 10th carry, trivial sign-off-only change.
- **S12-08 (DI/VContainer ADR)** — 2nd carry, now escalated: a second `ObjectPoolManager` implementation
  exists undocumented in `LifetimeScope/Service/PoolableService/` alongside the original
  `Assets/Script/Poolable/`, actively compounding rather than just stale.
- **S12-09 (individual `BUG-NNN.md` files)** — 6th+ cycle, only 3 of 9+ open P1 items have files.
- QA plan — 14th+ consecutive cycle with none. Flagged in `sprint-12.md`, deferred to owner.
- **`feature/fix-player-control` stashed `.anim` change** — owner should recover via `git stash pop` on
  that branch; not related to sprint-12 work.
