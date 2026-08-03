# Sprint 8 — Daily Plan & Progress Tracker

> **Sprint**: 2026-08-03 (Mon) → 2026-08-07 (Fri)
> **Companion to**: `sprint-08.md` (formal plan) — this file is the day-by-day breakdown + live tracker
> **Maintained by**: PM assistant — updated each session/standup
> **Routines (Claude Code)**:
>   - **Mon–Fri 10:00** → `/daily-standup`
>   - **Sat 22:00** → `/weekly-wrapup`
>   - **Sun 22:00** → `/weekly-kickoff`
> **Opened**: 2026-08-02 (Sun 22:00 kickoff) — autonomous scheduled run, no user present. Branch
> `sprint-08` created from `sprint-07` tip (`7cc1f75`, includes the sprint-07 closure commit).

---

## Status Verdict: 🟡 DAY 1 IN PROGRESS (2026-08-03) — combat still fully broken both directions; root-cause conversation (S8-00) not yet held by owner.

Sprint 7 closed CONCERNS: component hub structurally sound, but combat confirmed non-functional in
both directions at close (BUG-041, BUG-042), and the off-plan-work root-cause conversation (S7-D4) was
scheduled twice and held zero times across Sprints 6 and 7. Sprint 8 is scoped as a recovery sprint:
restore combat function first (S8-01/02/03/12), and — for the 3rd time — schedule the root-cause
conversation, this time as Monday's literal first task rather than a late-week item.

---

## Day-by-Day Plan

### Mon 2026-08-03 — Root-cause conversation, then combat fixes batch 1

| Task | Est. | Notes |
|------|------|-------|
| S8-00 (root-cause conversation) | 0.1d | **Do this first, before any code task** — 3rd scheduling attempt, 0 held so far. Requires owner facilitation; this run cannot hold it autonomously. |
| S8-01 (BUG-041, player attack unwired) | 0.2d | P0 — player currently deals zero damage |
| S8-02 (BUG-042, enemy TakeDamage throws) | 0.2d | P0 — every player hit on an enemy currently crashes |
| S8-06 (BUG-032, one-line fix) | 0.1d | Trivial, 2nd carry — pick up early to close out quickly |
| S8-07 (BUG-033, one-line fix) | 0.1d | Trivial, 5th carry — pick up early to close out quickly |

Goal: root-cause conversation actually held (unlike Sprint 7's S7-D4), plus the two P0 combat bugs
landed or well underway.

### Tue 2026-08-04 — Combat fixes batch 2 + verification

| Task | Est. | Notes |
|------|------|-------|
| S8-03 (BUG-043, divergent attack paths) | 0.3d | Depends on S8-02 landing first |
| S8-04 (BUG-044, PlayerDeathState orphaned) | 0.15d | Independent, feeds S8-05 |
| S8-12 (Play Mode verify, both attack directions) | 0.2d | **Gate** — owner confirms in-Editor; do not treat as done without this, unlike S7-08 which was never confirmed all of Sprint 7 |

Goal: by end of Tue, both attack directions confirmed working in Play Mode, not just by static code read.

### Wed 2026-08-05 — Bug #6 (8th carry)

| Task | Est. | Notes |
|------|------|-------|
| S8-05 (Bug #6 / S7-11, write-through + listener + EditMode test) | 0.4d | Largest single item — 8th carry, regressed twice before. Do not close without the EditMode test. |

Goal: single HP source of truth for the player, `ON_PLAYER_DEATH` listener confirmed firing, EditMode
test passing.

### Thu 2026-08-06 — Should-Have + decisions

| Task | Est. | Notes |
|------|------|-------|
| S8-10 (ADR-0002 Accepted) | 0.1d | 4th carry, trivial — no complexity reason for continued delay |
| S8-11 (S4-05/S4-06 forced decision) | 0.1d | 8th carry — make the call, do not re-carry a 9th time |
| S8-08 (Bug #14, missing `return`) | 0.1d | Should-Have, quick |
| S8-D1 (individual BUG-NNN.md files) | 0.2d | Should-Have — start with BUG-041/042/043/044 |
| Buffer / catch-up | — | Reserved for Must-Have slippage |

### Fri 2026-08-07 — Should-Have stretch + wrap prep

| Task | Est. | Notes |
|------|------|-------|
| S8-09 (Bug #15, build-safe JSON load) | 0.5d | If Must-Have closed clean |
| S8-N1 (first playtest) | — | Only if S8-01/02/03/12 all confirmed stable |
| Friday wrap-up prep | — | Feeds into Sat 22:00 `/weekly-wrapup` |

---

## Standup Log

### Mon 2026-08-03 10:00 — Day 1 standup (autonomous)

**Yesterday (2026-08-02, Sun):** No sprint-08 work yet — the day was sprint-07 close (verdict
CONCERNS) + weekly wrap-up + sprint-08 kickoff. One pre-kickoff commit on the sprint-07 tip,
`5f0e58f "done flow attakc<->take damage"`, touched combat files but is mixed-result WIP, not a fix:
- `EntityAttack.cs` (enemy→player path) genuinely advanced — `hitColliders` is now a real
  `Collider2D[10]` (was `[]`), `Attack()` does `OverlapCircleNonAlloc` +
  `INegativeReceiver.TakeDamage()`. Partial progress toward BUG-043's hitBuffer criterion.
- `WeaponMelee.cs` (player→enemy path) regressed further — `Attack()` override is now an empty
  stub, and the previously-real damage code was moved into a **non-override** `MakeDamage()` method
  with its body **commented out**. Whatever previously called `MakeDamage()` (animation event or
  `AttackSO` hook) is now disconnected. BUG-041's fix surface may be slightly larger than scoped:
  the programmer picking up S8-01 should confirm how `MakeDamage()` used to be invoked before
  rewiring `Attack()`, not just fill in the empty override.

**Verified directly against current code (not just the carry-over table) — all Sprint 8 Must-Have
bugs confirmed still open, no drift from what `sprint-08.md` assumed:**

| Bug | File | Status |
|-----|------|--------|
| BUG-041 (player attack unwired) | `WeaponMelee.cs` | ⚠️ OPEN — worse than assumed, see above |
| BUG-042 (enemy TakeDamage throws) | `EntityCore.cs:11`, `EntityNegativeReciver.cs:7` | ⚠️ OPEN — confirmed duplicated in both classes exactly as sprint doc states |
| BUG-043 (divergent enemy attack paths) | `EntityAttack.cs` vs `EntityWeaponMelee.cs` | ⚠️ OPEN — two paths still both present; `EntityAttack.cs.nextAttackTime` still never advances after `Attack()`, cooldown gate still non-functional |
| BUG-044 (PlayerDeathState body commented out) | `PlayerDeathState.cs:17-24` | ⚠️ OPEN, unchanged |
| BUG-032 (EntityWeaponMelee.cs:26 input wiring) | `EntityWeaponMelee.cs:26` | ⚠️ OPEN, unchanged |
| BUG-033 (EnemySpawner.cs:62 null-check order) | `EnemySpawner.cs:62` | ⚠️ OPEN, unchanged (`set.Count == 0 || set == null`) |

**Today's plan (per Mon row of the day-by-day plan above):**

| Task | Est. | Note |
|------|------|------|
| S8-00 — root-cause conversation | 0.1d | **Owner action required.** Cannot be held autonomously. 3rd scheduling attempt, 0 held across Sprints 6-7. Escalating as top risk again below. |
| S8-01 — fix BUG-041 | 0.2d | Scope note above: check the old `MakeDamage()` call site before rewiring |
| S8-02 — fix BUG-042 | 0.2d | Implement `EntityCore.TakeDamage()` for real; delete `EntityNegativeReciver.cs` (duplicate) rather than fixing both |
| S8-06 — fix BUG-032 | 0.1d | Trivial one-line uncomment |
| S8-07 — fix BUG-033 | 0.1d | Trivial one-line reorder |

**Blockers:** S8-00 needs the owner in the room — no code path can substitute. This is the 3rd
scheduling attempt (0/2 held in Sprints 6-7). Per the sprint's own risk table, if this slips again
today the recommendation is to stop re-scheduling a conversation and move to a hard process gate
(pre-push compile+smoke check or branch protection) starting Sprint 9.

**Emerging risk (new today):** yesterday's WIP commit shows combat code is being edited directly on
the sprint-07→sprint-08 lineage outside any tracked S8 task ID before the sprint formally opened.
Small in scope here, but it's the same pattern (drive-by combat-file edits outside the sprint's own
task list) that S8-00 exists to address — worth raising explicitly in that conversation, not just as
a general reminder.

---

## Carry-Over Watch List (re-verify every standup)

- **S8-00 root-cause conversation — 3rd scheduling attempt.** Held zero times across Sprints 6 and 7.
  If this slips a 3rd time, the recommendation for Sprint 9 is a hard process gate (branch protection
  or a required pre-push compile+smoke check) rather than a 4th conversation.
- BUG-041/BUG-042 — P0, combat non-functional in both directions until fixed. Nothing else in the
  sprint can be meaningfully verified until these land.
- Bug #6 — 8th carry cycle, regressed twice. S8-05 is the third attempt scoped with a mandatory
  EditMode test.
- S8-11 (S4-05/S4-06) — 8th carry, zero movement any cycle. Decision-avoidance, not an estimation
  problem — recommend the owner just make the call.
- QA plan — 7 consecutive cycles with none. Flagged in `sprint-08.md`, deferred to owner.
