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

## Status Verdict: 🔴 DAY 5 / FINAL (2026-08-07) — zero Must-Have tasks landed on `sprint-08` branch through the sprint's last scheduled day; BUG-041/BUG-042/BUG-053 confirmed still open, byte-identical to last night's post-merge check. S8-00 root-cause conversation still not held — 5th unaddressed cycle.

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

### Tue 2026-08-04 / Wed 2026-08-05 — no automated standup ran (gap)

No `chore(standup)` commit exists on `sprint-08` for either day — `git log sprint-08` shows only the
Mon 08-03 standup and the 08-06 module-quality-audit between kickoff and today. This run cannot
reconstruct what those sessions intended (no session state left behind); reconstructing **actual**
code activity from `git log --all` instead, since real work continued on `origin/feature/enemy-control`
(not `sprint-08`) both days:

- **Tue 2026-08-04**: `8f1cff0` "fix check range", `cfae933` "feat(pathfinding): add A* enemy chase
  system", `c80fc47` "update flee target" — all on `origin/feature/enemy-control`. Pathfinding/chase
  work, not on the sprint's Must-Have list (S8-01 through S8-07).
- **Wed 2026-08-05**: `f0bef93` "coding", `b3a52e0` "design: add attack speed system GDD" (new
  `design/gdd/attack-speed-system.md`, 247 lines), `2df0c04` "done logic flow idle<->move<->attack
  (need polish more)" — same branch. Touches `EntityAttack.cs`, `EntityFindTarget.cs`,
  `EntityInput.cs`, `EntityMovement.cs`, `EntityAttackState.cs`, `EntityIdleState.cs`,
  `EntityMoveState.cs` — enemy AI state-flow polish. **Verified directly: does not touch
  `EntityCore.TakeDamage()` (still `throw NotImplementedException`, BUG-042) or `WeaponMelee.Attack()`
  (still empty, BUG-041) even on this branch.**

This is the exact off-plan-work pattern S8-00 exists to stop, recurring for a 4th time this sprint
(3rd+4th days), on top of S8-00 itself never being held. No code from either day has been merged into
`sprint-08` — `git log sprint-08 ^sprint-07` still shows only kickoff + Mon standup + module-audit,
zero task-ID-linked commits.

### Thu 2026-08-06 10:00 — Day 4 standup (autonomous)

**Yesterday/this week so far, verified against code, not the carry-over table:**

| Bug/Task | File | Status on `sprint-08` | Status on `origin/feature/enemy-control` |
|----------|------|------------------------|-------------------------------------------|
| BUG-041 (S8-01) | `WeaponMelee.cs:26-34` | ⚠️ OPEN — `Attack()` still empty, `MakeDamage()` still fully commented out | ⚠️ OPEN — identical, unchanged |
| BUG-042 (S8-02) | `EntityCore.cs:9-12` | ⚠️ OPEN — still `throw new NotImplementedException()`; `EntityNegativeReciver.cs` duplicate still present | ⚠️ OPEN — identical, unchanged |
| BUG-043 (S8-03) | `EntityAttack.cs` vs `EntityWeaponMelee.cs` | ⚠️ OPEN — both paths still present; `nextAttackTime` still never advances after `Attack()` | Partial motion only — `EntityAttack.cs` gained a few lines this week, still two divergent paths |
| BUG-044 (S8-04) | `PlayerDeathState.cs:17-24` | ⚠️ OPEN, unchanged — body still fully commented out | not touched |
| BUG-032 (S8-06) | `EntityWeaponMelee.cs:26` | ⚠️ OPEN — `//Core.GetCoreComponent(out input);` still commented out | not touched |
| BUG-033 (S8-07) | `EnemySpawner.cs:62` | ⚠️ OPEN — still `set.Count == 0 \|\| set == null` (wrong order) | not checked |
| ADR-0002 (S8-10) | `docs/architecture/adr-0002-*.md` | ⚠️ Status still **Proposed** — not flipped | — |
| S8-00 root-cause conversation | — | ⚠️ Not held — 4th unaddressed cycle (3 in Sprint 7 lineage + this one) | — |

**Net: 0 of the sprint's 8 Must-Have tasks (S8-01 through S8-07, S8-12) have landed on `sprint-08`.**
Zero task-ID-linked commits exist on the branch. Meanwhile ~920 lines changed on the unmerged feature
branch this week (enemy AI state-flow polish, a new A* pathfinding chase system, and a full new GDD
`design/gdd/attack-speed-system.md`) — real, non-trivial work, but entirely outside this sprint's
scoped Must-Have list and not merged anywhere the sprint can credit it.

**Today's plan (re-planned — original Thu row assumed Must-Have was done by now; it is not):**

| Task | Est. | Note |
|------|------|------|
| S8-01 — fix BUG-041 (`WeaponMelee.Attack()`/`MakeDamage()` wiring) | 0.2d | Unblocks all downstream verification; highest priority now, 4 days late |
| S8-02 — fix BUG-042 (`EntityCore.TakeDamage()` real impl; delete `EntityNegativeReciver.cs` duplicate) | 0.2d | Same priority tier as S8-01 |
| Reconcile `origin/feature/enemy-control` → `sprint-08` | 0.5d (new, unplanned) | Branch carries real, wanted enemy-AI/pathfinding progress but has drifted 8 commits / ~1300 lines from `sprint-08` unmerged; needs review + Play Mode check before merge, not a blind fast-forward — flagged as risk below, not started autonomously since it touches prefab/AI behavior |
| S8-06 / S8-07 — one-line fixes (BUG-032, BUG-033) | 0.1d each | Trivial, still open, pick up alongside S8-01/02 |
| S8-00 root-cause conversation | 0.1d | **Owner action required, 4th ask.** No code path substitutes for this. |

**Blockers:**
- S8-00 needs the owner in the room — unchanged blocker, now 4th cycle unheld.
- Merging `origin/feature/enemy-control` requires an in-Editor Play Mode check (no Unity CLI in this
  environment) before it can be folded into `sprint-08` — owner action needed.

**Emerging risks (new today):**
- **Sprint is 1 day from its Fri close with 0/8 Must-Have items landed on the sprint branch.** At the
  current rate, Sprint 8 is on track to close CONCERNS or worse, repeating Sprint 7's outcome despite
  being explicitly scoped as the recovery sprint.
- The off-plan-work pattern the sprint's #1 risk warned about (`High probability, High impact`) has
  now recurred through Tue and Wed with real, sizeable commits (~1300 lines across 4 commits) on a
  branch that isn't `sprint-08`. Per the sprint's own risk mitigation: **"If it recurs anyway, escalate
  to a hard process gate (e.g., branch protection / pre-push check) next cycle rather than a 4th
  conversation."** That condition has now been met — recommend Sprint 9 start with a hard gate instead
  of scheduling a 4th conversation.
- No standup was recorded Tue or Wed — tracker continuity gap, noted above; root cause not established
  by this autonomous run (no session state left to explain the gap).

---

### Thu 2026-08-06 22:11 — Post-merge update (autonomous, same-day follow-up)

**Context:** the 10:40 standup above ran before the day's real activity. Two commits landed on
`sprint-08` later the same evening: `b74a14f` "update flow equid weapon, need check flow change attack
statae" (22:08) and `c5f26b1` "Merge branch 'origin/feature/enemy-control' into sprint-08" (22:08) —
**the divergent-branch merge flagged as an emerging risk this morning has now happened**, resolving
that specific risk item. This run re-verified every Must-Have bug directly against the merged code
(`sprint-08` HEAD), not the carry-over table:

| Bug/Task | File | Status after merge |
|----------|------|---------------------|
| BUG-041 (S8-01) | `Assets/Script/Weapons/MeleeWeapon/MeleeWeapon.cs` (renamed from `WeaponMelee.cs`) | ⚠️ OPEN, unchanged — `Attack()` still empty; `MakeDamage()` still fully commented out |
| BUG-042 (S8-02) | `EntityCore.cs:9` | ⚠️ OPEN, unchanged — still `throw new NotImplementedException()` |
| **BUG-053 (new)** | `EntityNegativeReciver.cs` | 🆕 **New S1** — the merge rewrote this duplicate receiver with player-copied logic: it now calls `Core.GetCoreComponent(out PlayerInputHandler input)` (wrong hub — `Core` is `EntityCore`, has no `PlayerInputHandler`) and emits `EventID.ON_PLAYER_DEATH` on enemy death. Filed as `production/qa/bugs/BUG-053.md`. Makes S8-02's "exactly one `INegativeReceiver` implementer" criterion harder, not easier, to close — the duplicate needs deleting, not patching. |
| BUG-043 (S8-03) | `EntityAttack.cs` vs `EntityWeaponMelee.cs` | ⚠️ OPEN — two paths still both present. Partial improvement: `EntityAttack.CallAttack()` now actually advances `nextAttackTime` (previously never did) — cooldown gate criterion is closer, but consolidation into one path still not done |
| BUG-044 (S8-04) | `PlayerDeathState.cs:17-24` | ⚠️ OPEN, unchanged — body still fully commented out |
| BUG-032 (S8-06) | `EntityWeaponMelee.cs:26` | ⚠️ OPEN, unchanged — still `//Core.GetCoreComponent(out input);` (comment now says "fix or remove it") |
| BUG-033 (S8-07) | `EnemySpawner.cs:62` | ⚠️ OPEN, unchanged — still `set.Count == 0 \|\| set == null` (wrong order) |
| ADR-0002 (S8-10) | `docs/architecture/adr-0002-*.md` | ⚠️ Status still **Proposed** |
| S8-00 root-cause conversation | — | ⚠️ Not held — 5th unaddressed cycle now |

**Net: still 0 of 8 Must-Have tasks (S8-01–S8-07, S8-12) landed, with 1 sprint day remaining
(Fri 2026-08-07).** The merge brought in real enemy-AI/pathfinding progress and resolved the
branch-divergence risk, but touched neither P0 bug and introduced one new S1 (BUG-053). Also notable
for `/doc-sync` (not actioned here — out of scope for this run): the merge renamed
`NewPlayer.cs`→`Player.cs`, `WeaponMelee.cs`→`MeleeWeapon.cs`, `WeaponMeleeStats.cs`→`MeleeWeaponStats.cs`,
`WeaponRangeStats.cs`→`RangeWeaponStats.cs`, and deleted `Shooting.cs`/`WeaponAttack.cs` — CLAUDE.md's
Repository Layout section now names stale filenames for all of these.

**Plan for Fri 2026-08-07 (sprint's last scheduled day):**

| Task | Est. | Note |
|------|------|------|
| S8-01 — fix BUG-041 | 0.2d | Still the single highest-priority item — nothing else in the sprint is verifiable until this and S8-02 land |
| S8-02 — fix BUG-042 + BUG-053 together | 0.3d (revised up from 0.2d) | Now explicitly two files to reconcile, not one — delete `EntityNegativeReciver.cs`, implement `EntityCore.TakeDamage()` for real |
| S8-06 / S8-07 | 0.1d each | Still trivial one-line fixes, still open — cheapest path to any Must-Have credit before sprint close |
| S8-00 root-cause conversation | 0.1d | **Owner action, 5th ask.** Escalation condition from the sprint's own risk table was already met as of yesterday's standup |

**Realistic sprint-close assessment:** with 1 day left and 0/8 Must-Have landed, Sprint 8 — explicitly
scoped as a *recovery* sprint — is very unlikely to close clean. Recommend the Friday close treat
S8-01/S8-02/BUG-053 as the only realistic Must-Have target (a partial recovery: at least one attack
direction verified working), formally re-carry the rest into Sprint 9, and use the close to finally
decide on the hard process gate (branch protection / pre-push compile+smoke check) the risk table has
been recommending since yesterday.

**Blockers:** unchanged — S8-00 needs the owner in the room; no code path substitutes.

**Emerging risks (new tonight):**
- BUG-053 — see above, filed as a new S1. Worth surfacing at sprint close as evidence that off-plan
  branch work, even once merged, still needs a Play-Mode/code review pass before being trusted as
  progress — the branch divergence risk is resolved, but it shipped a new bug in the process.
- CLAUDE.md Repository Layout is now stale on filenames (see rename list above) — flag for `/doc-sync`,
  not actioned by this run per its .cs/asset edit restriction.

---

### Fri 2026-08-07 10:00 — Day 5 / final scheduled-day standup (autonomous)

**Yesterday (2026-08-06 evening, since the last standup):** no further commits — HEAD on `sprint-08`
is still `5ec8045` (last night's standup commit), same as when the 22:11 post-merge update was written.
Re-verified all three open Must-Have blockers directly against current code, byte-identical to last
night:

| Bug/Task | File | Status |
|----------|------|--------|
| BUG-041 (S8-01) | `Assets/Script/Weapons/MeleeWeapon/MeleeWeapon.cs:17-25` | ⚠️ OPEN, unchanged — `Attack()` still an empty override; `MakeDamage()` still a non-override method with body fully commented out |
| BUG-042 (S8-02) | `Assets/Script/Character/Entity/Core/EntityCore.cs:9-12` | ⚠️ OPEN, unchanged — still `throw new System.NotImplementedException()` |
| BUG-053 | `Assets/Script/Character/Entity/CoreComponent/EntityNegativeReciver.cs` | ⚠️ OPEN, unchanged — still calls `Core.GetCoreComponent(out PlayerInputHandler input)` (wrong hub) and emits `ON_PLAYER_DEATH` on enemy death |

**Analysis:** 0d burned since last night (no commits). Planned burn for the sprint's Must-Have set was
1.75d; actual landed = 0d across all 5 sprint days. S8-00 (root-cause conversation) never held —
now a 5th unaddressed cycle, exactly as flagged last night.

**Verdict: BLOCKED** — biggest reason: no owner-authored commit since the merge; the two P0 fixes
(S8-01/S8-02) still require a programmer session that has not happened, and S8-00 still requires the
owner in the room, which no autonomous run can substitute for.

**Today's plan (last scheduled sprint day):**

| Task | Est. | Why now |
|------|------|---------|
| S8-01 — fix BUG-041 (`MeleeWeapon.Attack()`/`MakeDamage()` wiring) | 0.2d | Still the single highest-leverage fix — nothing else verifiable until this lands |
| S8-02 — fix BUG-042 + BUG-053 together | 0.3d | Delete/rewrite `EntityNegativeReciver.cs` duplicate, implement `EntityCore.TakeDamage()` for real |
| S8-06 / S8-07 — one-line fixes (BUG-032, BUG-033) | 0.1d each | Cheapest possible Must-Have credit before close, still untouched |
| S8-00 root-cause conversation | 0.1d | 5th ask — owner action, no code path substitutes |

💡 **Focus:** S8-01 first (smallest, unblocks player→enemy verification), then S8-02+BUG-053 together
since they're now one reconciliation, not two separate fixes.

**Sprint close assessment:** with the last scheduled day starting at 0/8 Must-Have landed, Sprint 8 —
explicitly scoped as a recovery sprint — is going into its Friday close very unlikely to land clean.
Recommend today's close formally re-carry S8-01 through S8-07/S8-12 into Sprint 9, and use the close
to decide the hard process gate (branch protection / pre-push compile+smoke check) the risk table has
recommended since Thursday, rather than a 6th root-cause conversation attempt.

**Blockers:** unchanged — S8-00 needs the owner in the room; S8-01/S8-02 need an actual coding session
on `sprint-08` (none held since the 22:08 merge last night).

---

## Carry-Over Watch List (re-verify every standup)

- **S8-00 root-cause conversation — held zero times across 5 cycles now (Sprint 6, Sprint 7, and three
  times this sprint's own window).** The sprint's own risk table condition for escalation ("if it
  recurs anyway") was already met as of the 2026-08-06 morning standup — recommend a hard process gate
  (branch protection or required pre-push compile+smoke check) for Sprint 9 instead of a 6th
  conversation attempt.
- BUG-041/BUG-042/BUG-053 — P0/S1, combat non-functional in both directions until fixed, plus a new
  merge-introduced bug in the enemy damage-receiver duplicate. Still 0% landed on `sprint-08` with 1
  sprint day remaining. Nothing else in the sprint can be meaningfully verified until these land.
- `origin/feature/enemy-control` divergence — **resolved** 2026-08-06 22:08 (`c5f26b1`), but the merge
  itself introduced BUG-053; recommend a Play-Mode check on the merged enemy-control code before
  Sprint 9 kickoff, not just a compile check.
- Bug #6 — 8th carry cycle, regressed twice. S8-05 not started this sprint at all (was scheduled for
  Wed, no standup recorded that day, and Thu/Fri are now committed to BUG-041/042/053 recovery).
- S8-11 (S4-05/S4-06) — 8th carry, zero movement any cycle. Decision-avoidance, not an estimation
  problem — recommend the owner just make the call.
- QA plan — 7 consecutive cycles with none. Flagged in `sprint-08.md`, deferred to owner.
