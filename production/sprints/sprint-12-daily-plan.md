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
| S12-02 (BUG-063, `Stat.cs:63-65` `[SerializeField]` regression) | 0.05d | ❌ NOT DONE | 19th+ carry — verified directly against source at this standup, `#if UNITY_EDITOR` / `[SerializeField]` still wraps `modifiers` at `Stat.cs:63-65`, unchanged. Retro explicitly recommends this be the literal first commit of Sprint 12 |
| S12-01 (BUG-064, project does not compile) | 0.4d | ❌ NOT DONE (on `sprint-12`) — ⚠️ in-progress elsewhere, uncommitted | Verified directly against source: `EntityData.cs:8` still declares `EntityStatsSO statsSO`, `EntityInput.cs:57` still references deleted `EntityFindTarget`, `Entity.cs` still has `LoadEntity()`/`LoadState()`/`SetDataEntity()` — build still broken on `sprint-12` exactly as BUG-064 describes. **But**: `feature/fix-player-control` (a separate, non-sprint branch) carries a large uncommitted working-tree change touching this exact surface area — new `BaseStatsSO.cs`/`EnemyStatSO.cs`, new `EntityFindTarget.cs`, new `EntityUIController.cs`, deleted `StatsSO.cs`, `Weapon.OnActivate()` signature changed to take `finalDamage`, plus edits across `Entity.cs`/`EntityData.cs`/`EntityNegativeReciver.cs`/`WeaponHolder.cs`/`Player.cs`/`PlayerDeathState.cs`. This looks like an active attempt at the same architecture confirmation S12-01 calls for, but it is **not on `sprint-12` and not committed** — stashed only (`git stash` on `feature/fix-player-control`, entry "wip: uncommitted changes on feature/fix-player-control before standup checkout 2026-08-31"). Owner should decide whether to port/merge this WIP into `sprint-12` rather than re-deriving the fix from scratch |
| S12-05 (pre-push hook placeholder) | 0.15d | ❌ NOT DONE | `.git/hooks/pre-push` still absent, verified — 14th+ carry, stop the silent count |

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

### Mon 2026-08-31 02:00 — Daily Standup (autonomous, no owner present)

**Yesterday (Sun 2026-08-30):** Sprint 12 kickoff only — no commits landed on `sprint-12` since
`1dfc941` ("chore(kickoff): open sprint-12 2026-08-30"). `HEAD` on `sprint-12` is still that same
kickoff commit; zero work has landed on this branch yet.

**Re-verified directly against current source at this standup** (per BUG-064's own note not to trust
tags without a fresh check):
- 🔴 **BUG-064 / S12-01** — still broken exactly as filed. `EntityData.cs:8` still declares
  `EntityStatsSO statsSO` (type no longer exists), `EntityInput.cs:57` still declares
  `EntityFindTarget entityFind` (file no longer exists), `Entity.cs` still has `LoadEntity()` /
  `LoadState()` / `SetDataEntity()` referencing the removed `data` field. Project does not compile at
  `sprint-12` `HEAD`. Sole hard blocker, unchanged.
- ❌ **BUG-063 / S12-02** — `Stat.cs:63-65` still wraps `modifiers` in `#if UNITY_EDITOR` /
  `[SerializeField]`. 19th+ consecutive carry, still zero technical blocker.
- ❌ **S12-05 (pre-push hook)** — `.git/hooks/pre-push` still does not exist. 14th+ carry.
- ❌ **S12-07 (ADR-0002)** — `docs/architecture/adr-0002-enemymanager-singleton-exception.md` Status
  line still reads `Proposed`. 10th+ carry.
- **S12-09 (individual `BUG-NNN.md` files)** — `production/qa/bugs/` now holds 4 files (BUG-052,
  BUG-053, BUG-063, BUG-064) vs. 3 last cycle — BUG-064 got one at filing time, but the task itself
  (batch-generating the rest of the 9+ open P1 backlog) has not been run yet.

**⚠️ New finding this standup — active uncommitted WIP outside `sprint-12`:** the session that ran this
standup started on `feature/fix-player-control` (not a sprint branch) and found 36 uncommitted
working-tree changes there, later stashed to allow the branch switch (`git stash`, message "wip:
uncommitted changes on feature/fix-player-control before standup checkout 2026-08-31" — separate from
the earlier-stashed `.anim` change noted at kickoff). The changed-file set overlaps heavily with
BUG-064's exact surface: new `Assets/Script/StatSystem/BaseStatsSO.cs` and `EnemyStatSO.cs`, deleted
`Assets/Script/StatSystem/StatsSO.cs`, a new `Assets/Script/Character/Entity/CoreComponent/EntityFindTarget.cs`
(the very file BUG-064 says is missing), a new `EntityUIController.cs`, and edits to `Entity.cs`,
`EntityData.cs`, `EntityNegativeReciver.cs`, `EntityStatsHandler.cs`, `Player.cs`, `PlayerDeathState.cs`,
`WeaponHolder.cs`, `NegativeReciver.cs`, and `Weapon.OnActivate()`'s signature (now takes `finalDamage`).
This reads as an active, in-progress attempt at the same `Stats` SO architecture split S12-01 already
calls for — but it lives only in a stash on a non-sprint branch, not in `sprint-12`. **Recommend the
owner review this WIP before anyone starts BUG-064 from scratch on `sprint-12`** — there is real risk of
duplicated or conflicting work if both proceed independently. Not committed or ported by this standup
run per the hard constraint against touching `.cs`/asset files.

**Today (Mon 2026-08-31) — per the existing plan, unchanged:**
1. S12-02 (BUG-063 one-line fix) — Est. 0.05d — land first, zero excuse for a 19th carry
2. S12-05 (pre-push hook placeholder, `exit 0` + TODO) — Est. 0.15d — land alongside S12-02 before
   opening any Entity-side file
3. S12-01 (BUG-064) — Est. 0.4d remaining — the sprint's sole hard blocker; **first check the
   `feature/fix-player-control` stash above before starting fresh**, since it may already contain most
   of the needed sweep

**Blockers:**
- No owner-in-Editor session yet this sprint — S12-01's Play Mode smoke gate cannot be confirmed until
  one happens, same risk flagged in `sprint-12.md`.
- S12-06 (S4-05/S4-06 forced decision) needs owner judgment — cannot be resolved autonomously.

**Risks:**
- Full 5-day sprint remaining, but Day 1 opened with zero commits and a large relevant WIP sitting
  uncommitted on an unrelated branch — if that WIP is not surfaced to whoever picks up S12-01, duplicate
  effort is likely.
- `.git/hooks/pre-push` — 14th+ cycle absent, still nothing gates a repeat of last week's build-breaking
  merge.
- No QA plan — 14th+ consecutive cycle, deferred to owner per `sprint-12.md`.

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
