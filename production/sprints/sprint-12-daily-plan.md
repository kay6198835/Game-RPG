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

## Status Verdict: 🟡 AT RISK (Day 4 of 5) — `origin/feature/fix-player-control` was merged into `sprint-12` today (`86b7ee0`, 2026-09-03 13:47) as a **fresh commit** (`6a56fe6`), not from the old stash. Re-verification against current source shows BUG-064 sub-items 1–4 and 6 now fixed; item 7 (`RangeWeapon` poolManager DI) is still open. Project likely compiles now but **has not been confirmed in the Unity Editor** — smoke gate still outstanding. One day of runway left before Friday wrap-up.

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

| Task | Est. | Status | Notes |
|------|------|--------|-------|
| S12-01 continued (payload mismatch, `Resgister`/`UnResgister` typo, `RangeWeapon` DI wiring) | remaining | ❌ NOT DONE on `sprint-12` HEAD — ⚠️ likely superseded by uncommitted WIP | Items c/d/e from the bug-triage recommended assignment if not finished Monday. See this standup's WIP finding below before re-deriving |
| S12-01 smoke gate | — | ❌ NOT DONE | **Gate** — owner confirms in-Editor: Unity Console zero errors, enter `LoadRandomMap`, kill one enemy, fire the ranged weapon once. Do not mark S12-01 done without this — it's an Integration story per `test-standards.md` |

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

### Tue 2026-09-01 02:00 — Daily Standup (autonomous, no owner present)

**Yesterday (Mon 2026-08-31):** Zero commits landed on `sprint-12` — `git log --all --since="yesterday 00:00"`
shows only the standup commit itself (`0c93917`) plus two dated `git stash`-style commits from the prior
session's branch switch, nothing touching gameplay code. Planned Monday work (S12-02, S12-05, S12-01) did
not land on this branch.

**Re-verified directly against current source at this standup:**
- 🔴 **BUG-064 / S12-01** — still broken on `sprint-12` `HEAD` (the last committed state): `EntityInput.cs:57`
  still references `EntityFindTarget`, `Entity.cs` still carries `LoadEntity()`/`LoadState()`/
  `SetDataEntity()`. No commit has restored the build.
- ❌ **BUG-063 / S12-02** — `Stat.cs:63-65` still wraps `modifiers` in `#if UNITY_EDITOR` / `[SerializeField]`.
  20th+ consecutive carry.
- ❌ **S12-05 (pre-push hook)** — `.git/hooks/pre-push` still does not exist. 15th+ carry.
- ❌ **S12-07 (ADR-0002)** — `docs/architecture/adr-0002-enemymanager-singleton-exception.md` Status line
  still reads `Proposed`. 11th+ carry.
- **S12-08** — second `ObjectPoolManager` still present and undocumented:
  `Assets/Script/LifetimeScope/Service/PoolableService/ObjectPoolManager.cs` alongside
  `Assets/Script/Poolable/`.
- **S12-09** — `production/qa/bugs/` still holds 4 files (BUG-052, BUG-053, BUG-063, BUG-064), unchanged
  from yesterday.

**⚠️ WIP state changed since yesterday — now sitting directly in the working tree, not stashed.** This
run started on `feature/fix-player-control` with the same ~30-file uncommitted change yesterday's standup
found (new `BaseStatsSO.cs`/`EnemyStatSO.cs`, deleted `StatsSO.cs`, new `EntityFindTarget.cs`, new
`EntityUIController.cs`, edits across `Entity.cs`/`EntityData.cs`/`EntityNegativeReciver.cs`/
`WeaponHolder.cs`/`Player.cs`/`PlayerDeathState.cs`/`Weapon.cs` and others) — but this time it was **not**
stashed; `git checkout sprint-12` carried the uncommitted working-tree changes across branches directly
(no conflict, so git allowed it silently). Net effect: those ~30 files now show as uncommitted
modifications with `sprint-12` checked out, in this same working directory. Read-only spot check: the new
`EntityFindTarget.cs` now defines the class `EntityInput.cs:57` references, and `Entity.cs` still defines
`LoadEntity()`/`LoadState()`/`SetDataEntity()` as real methods (not dangling calls) — text-level, this
WIP looks consistent with a working BUG-064 fix, but **no Unity compile or Play Mode check was run** (out
of this run's scope — read-only on code). Not staged, not committed, not discarded, per the hard
constraint against touching `.cs`/asset files. **Owner action needed**: open the project in the Unity
Editor from this exact working tree, confirm the Console is clean, and if so, commit this WIP directly to
`sprint-12` as the S12-01 fix rather than re-deriving it — re-doing this work from scratch would be pure
waste.

**Today (Tue 2026-09-01) — per the existing plan, adjusted for the WIP finding:**
1. Owner-in-Editor check of the current working tree (0.1d) — confirm the uncommitted WIP compiles clean
   and passes the S12-01 smoke gate (`LoadRandomMap`, kill one enemy, fire the ranged weapon once) —
   **do this first**, it very likely collapses the rest of today's plan into "commit and verify" instead
   of "implement from scratch"
2. S12-01 remainder (0.2–0.4d, contingent on #1) — if the WIP does *not* fully close BUG-064, finish the
   remaining payload-mismatch / `Resgister`/`UnResgister` / `RangeWeapon` DI items on top of it
3. S12-02 (BUG-063 one-line fix, 0.05d) — still zero excuse for a 20th carry, land alongside #1/#2
4. S12-05 (pre-push hook placeholder, 0.15d) — same, cheap and unblocked
   💡 Focus: get an owner into the Unity Editor against the current working tree before anything else —
   the sprint's sole hard blocker may already be solved and unverified, which is a worse waste than not
   having started it

**Blockers:**
- No owner-in-Editor session yet this sprint (Day 2) — S12-01's Play Mode smoke gate still unconfirmed,
  and now gates a WIP that may already be complete.
- S12-06 (S4-05/S4-06 forced decision) still needs owner judgment — cannot be resolved autonomously.

**Risks:**
- Two full days into a five-day sprint with zero commits on `sprint-12` and the sprint's sole hard blocker
  still open — Wed/Thu's plan (S12-03, S12-04, doc-sync, ADR work) has no runway left if Tuesday also
  closes without a compiling build landing.
- The uncommitted WIP now lives only in this session's working tree — if the next session starts from a
  clean `git fetch`/fresh clone instead of this exact working directory, the WIP is invisible and BUG-064
  looks completely unstarted. Recommend the owner commit or explicitly stash-and-note it before ending
  this session.
- `.git/hooks/pre-push` — 15th+ cycle absent.
- No QA plan — 15th+ consecutive cycle, deferred to owner.

---

### Thu 2026-09-03 02:00 — Daily Standup (autonomous, no owner present)

**Note:** No Wed 2026-09-02 standup entry exists in this file — gap, likely a missed/failed scheduled
run. Nothing between Tue's entry and this one to cross-check against, so this entry re-verifies
everything directly against current source rather than diffing against a Wed snapshot.

**Yesterday/since Tue (2026-08-31→09-02):** Zero commits on `sprint-12` since `9a38ab8` (Tue's standup
commit). `git log --since="2026-09-02 00:00" sprint-12` returns nothing — no dev work, no standup, no
kickoff activity landed on this branch in the last ~40 hours.

**Re-verified directly against current source at this standup (build still broken, confirmed line-by-line):**
- 🔴 **BUG-064 / S12-01 — still open, all 7 sub-items unresolved:**
  - (1) `EntityData.cs:8,31` still declares/exposes `EntityStatsSO statsSO` — type still deleted, still dangling.
  - (2)/(3)/(4) `EntityInput.cs:57` still declares `EntityFindTarget entityFind` — class still does not exist in `Assets/Script/`.
  - (6) `EnemySpawner.cs:32` (`OnDisable`) still calls `EventManager.Resgister` instead of `UnResgister` for `ON_ENEMY_DEATH` — duplicate-subscription bug still live.
  - (7) `RangeWeapon.cs:7` still declares `[SerializeField] private IObjecPoolService poolManager` with no `[Inject]` Construct — ranged weapon `poolManager` still always null.
  - Project does not compile at `sprint-12` `HEAD`. Zero progress since Sunday's kickoff — 4 days into a 5-day sprint with the sole Must-Have blocker completely untouched on this branch.
- ❌ **BUG-063 / S12-02** — `Stat.cs:63-66` still wraps `modifiers` in `#if UNITY_EDITOR` / `[SerializeField]`. 21st+ consecutive carry, zero technical blocker, zero excuse.
- ❌ **S12-05 (pre-push hook)** — `.git/hooks/pre-push` confirmed still absent. 17th+ carry.
- ❌ **S12-07 (ADR-0002)** — `docs/architecture/adr-0002-enemymanager-singleton-exception.md:4` Status still reads `Proposed`. 13th+ carry.
- **S12-08** — second `ObjectPoolManager` still present, still undocumented: `Assets/Script/LifetimeScope/Service/PoolableService/ObjectPoolManager.cs` alongside `Assets/Script/Poolable/`.
- **S12-09** — `production/qa/bugs/` still holds exactly 4 files (BUG-052, BUG-053, BUG-063, BUG-064), unchanged since Mon.
- **WIP status** — the ~30-file candidate fix from Tue is no longer sitting loose in a working tree; it is stashed on `feature/fix-player-control` as `stash@{0}` ("wip: uncommitted changes on feature/fix-player-control before standup checkout 2026-08-31"). Nobody has popped, reviewed, or ported it in the two days since. It remains the fastest path to closing S12-01 — re-deriving from scratch is strictly worse.

**Today (Thu 2026-09-03) — plan, adjusted for lost runway (only 1 day left after this):**
1. **Owner pops `stash@{0}` on `feature/fix-player-control`, opens Unity, confirms Console is clean** — Est. 0.15d. This is now the single highest-leverage action in the sprint: it may already contain the fix for BUG-064 items 1–4, and possibly more.
2. If clean: port/merge that WIP onto `sprint-12`, then confirm items 6 and 7 (the `Resgister`/`UnResgister` typo and `RangeWeapon` DI wiring) are covered — the WIP's file list did not obviously touch `EnemySpawner.cs` or `RangeWeapon.cs`, so these two may need a small follow-up patch even after the WIP lands — Est. 0.3d combined.
3. S12-02 (BUG-063 one-line fix) — Est. 0.05d — no reason this has not landed in 21 cycles; land it same session as #1.
4. S12-05 (pre-push hook placeholder) — Est. 0.15d — same.
5. Play Mode smoke gate (kill one enemy, fire ranged weapon) — cannot be skipped per `test-standards.md`, Integration-type evidence required.

**Blockers:**
- No owner-in-Editor session across 4 consecutive sprint days — S12-01's smoke gate has never been attempted, let alone confirmed. This is now the sprint's critical risk, not just a note.
- S12-06 (S4-05/S4-06 forced decision) still needs owner judgment.

**Risks:**
- Only 1 working day left (Fri) after today. If BUG-064 does not land today, Sprint 12 closes FAIL for the second consecutive sprint on the exact same blocker, and Sprint 13 opens with the same P0 still unresolved plus a stale stash that has now sat for 4+ days.
- Missed Wed standup means a full day passed with no re-verification checkpoint — if this pattern repeats, the tracker stops being reliable as a source of truth.
- `.git/hooks/pre-push` — 17th+ cycle absent, still nothing gates a repeat of the Sprint 11 build-breaking merge.
- No QA plan — 17th+ consecutive cycle, deferred to owner.

---

### Thu 2026-09-03 13:50 — Mid-day check-in (post-standup, autonomous)

**Trigger:** `feature/fix-player-control` merged into `sprint-12` at 13:47 (`86b7ee0`, merging `6a56fe6`
"coding") — landed ~2 minutes after this morning's 02:00 standup commit (`003fc20`). This is a **fresh
commit on `feature/fix-player-control`**, not the two stashes noted in every prior entry — `stash@{0}`
("wip: uncommitted changes … before standup checkout 2026-08-31") and `stash@{1}` (the `.anim` change)
are both still sitting untouched, unpopped, unrelated to this merge.

**BUG-064 re-verified line-by-line against post-merge source:**
- ✅ (1) `EntityData.cs` — no `EntityStatsSO` reference found anywhere in `Assets/Script/`; type fully
  removed, `StatsSO.cs` deleted, replaced by new `BaseStatsSO.cs` / `EnemyStatSO.cs`.
- ✅ (2)/(3)/(4) `EntityFindTarget.cs` now exists (`Assets/Script/Character/Entity/CoreComponent/`) and
  defines the class `EntityInput.cs:57`, `EntityAttack.cs:8`, `EntityMovement.cs:22`,
  `EntityBasicState.cs:11` all reference. No dangling type.
- ✅ `Entity.cs` — `data` field (`EntityData`) and `Data` getter are both present again;
  `LoadEntity()`/`LoadState()`/`SetDataEntity()` all resolve against a real field, not a removed one.
  This was the exact break the retro flagged — looks closed.
- ✅ (6) `EnemySpawner.cs:30-32` `OnDisable` now correctly calls `UnResgister` for all three events
  including `ON_ENEMY_DEATH` — the duplicate-subscription bug is gone.
- ❌ (7) `RangeWeapon.cs:7` **still** declares `[SerializeField] private IObjecPoolService poolManager;`
  with no `[Inject]`/constructor wiring anywhere in the file. Unity cannot serialize an interface field via
  the Inspector, so this stays null at runtime regardless of merge — ranged weapon fire will silently
  no-op (`CanAttack()` gates on `poolManager != null`). **Sole remaining BUG-064 sub-item.**
- Also landed in the same commit, not previously tracked: `GameLifetimeScope` now registers `ItemSpawner`;
  `IPlayerStatService` gained `GetLevel()`; new `EntityUIController.cs`. Text-level read only — not
  exhaustively diffed against every consumer.

**S12-08 (duplicate `ObjectPoolManager`) — improved, not closed:** `Assets/Script/Poolable/ObjectPoolManager.cs`
no longer exists on disk; only `Assets/Script/LifetimeScope/Service/PoolableService/ObjectPoolManager.cs`
remains. Single implementation now, which resolves the *duplication* — but the DI/VContainer ADR this
item calls for (documenting the `LifetimeScope/` pattern itself) still does not exist. Downgrading
urgency, not closing S12-08.

**Unchanged (re-verified, no movement):**
- ❌ BUG-063 / S12-02 (`Stat.cs:63-64` `[SerializeField]` under `#if UNITY_EDITOR`) — 22nd+ carry.
- ❌ S12-05 (pre-push hook) — `.git/hooks/pre-push` still absent, 18th+ carry.
- ❌ S12-07 (ADR-0002) — Status still `Proposed`, 14th+ carry.
- S12-09 — still exactly 4 `BUG-NNN.md` files (BUG-052/053/063/064) of 9+ open P1s.

**Owner action needed (highest leverage, in order):**
1. Open the project in Unity Editor **from this exact `sprint-12` HEAD** (`86b7ee0`) and confirm the
   Console is clean — this has never been done this sprint and is the only way to actually close BUG-064,
   text-level verification is not a substitute.
2. Run the smoke gate (`LoadRandomMap`, kill one enemy, fire the ranged weapon once) — expect the ranged
   weapon to visibly fail here due to item 7 above; that failure is expected and pinpoints the one
   remaining line to fix, not a regression.
3. Wire `RangeWeapon.poolManager` via `[Inject]` (matching whatever pattern `GameLifetimeScope` already
   uses for its other services) — this is now the single smallest item standing between the sprint and a
   closed BUG-064.
4. Land BUG-063 (`Stat.cs`) and the pre-push hook placeholder alongside #3 — both remain zero-blocker,
   sub-15-minute items with no reason left to carry into Sprint 13.

**Risk, updated:** the sprint's sole hard blocker went from "completely unstarted on `sprint-12`" to
"one small DI wiring fix + an Editor confirmation" within the same day — genuine progress, but it is
**still unconfirmed in-Editor**, and Friday is the last working day. If item 7 and the Editor check don't
happen tomorrow, Sprint 12 still risks closing on an unverified build.

---

## Carry-Over Watch List (re-verify every standup)

- **BUG-064 — P0/S1, project does not compile.** Found in Saturday's code review. **2026-09-03 13:50
  update**: `feature/fix-player-control` merged into `sprint-12` (`86b7ee0`) — sub-items 1/2/3/4/6
  re-verified fixed against source; only sub-item 7 (`RangeWeapon.cs:7` `poolManager` needs `[Inject]`)
  remains open. Still **not confirmed in the Unity Editor** — Console-clean + smoke gate outstanding.
  Highest-leverage single action remaining in the sprint.
- **BUG-063 (`Stat.cs` `[SerializeField]` regression)** — 21st+ consecutive carry on a one-line fix with
  an explanatory comment already in the file. No technical blocker has ever existed for this item.
- **BUG-053/BUG-054 (enemy health routing)** — functional symptom looks fixed per Sprint 11's session,
  but cannot be confirmed until BUG-064 lands. Sequence directly after.
- **S12-05 process gate** — now 17th carry, same underlying pattern since Sprint 6/9. Its absence is
  exactly what let last week's build-breaking refactor land ungated onto `sprint-11`.
- **S12-06 (S4-05/S4-06)** — 17th+ carry, zero movement any cycle. Decision-avoidance, not an estimation
  problem.
- **S12-07 (ADR-0002 Accept)** — 13th+ carry, trivial sign-off-only change.
- **S12-08 (DI/VContainer ADR)** — 2nd+ carry: a second `ObjectPoolManager` implementation still exists
  undocumented in `LifetimeScope/Service/PoolableService/` alongside the original `Assets/Script/Poolable/`.
- **S12-09 (individual `BUG-NNN.md` files)** — 6th+ cycle, only 4 of 9+ open P1 items have files.
- QA plan — 17th+ consecutive cycle with none. Flagged in `sprint-12.md`, deferred to owner.
- **Candidate BUG-064 fix stashed on `feature/fix-player-control` (`stash@{0}`)** — not committed, not
  ported, not reviewed in the Editor, sitting untouched since Mon 08-31. Highest-priority item to resolve
  before Friday wrap-up, or the sprint closes FAIL on the same blocker as Sprint 11.
- **`feature/fix-player-control` stashed `.anim` change (`stash@{1}`)** — owner should still recover via
  `git stash pop` on that branch; not related to sprint-12 work. Unchanged since kickoff.
