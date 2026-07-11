# Retrospective: Interim (post-Sprint-4 wrap → pre-Sprint-5 execution)

Period: 2026-07-10 -- 2026-07-11 (Sprint 4 wrap-up already closed 07-10; Sprint 5 kickoff already ran
same day; this retro covers the one-day gap before Sprint 5's own execution window opens 07-14)
Generated: 2026-07-11 (automated `/weekly-wrapup`, Saturday 22:00 run)

> **Scope note**: This is a light interim check, not a full sprint retro. Sprint 5 has 0 days elapsed
> in its 07-14→07-18 execution window — there is no sprint velocity to measure yet. This retro instead
> follows up on Sprint 4 retro's action items and reviews what happened in the one-day gap.

---

### Metrics

| Metric | Value |
|--------|-------|
| Sprint 5 days elapsed | 0 of 5 |
| `.cs` files changed (committed) | 0 |
| `.cs` files changed (staged, uncommitted) | 1 — `EnemySpawner.cs` (S5-D1 WIP reapply) |
| Docs commits this window | 2 — `e600eb2` (GDD/epic/ADR re-sync), `89cad88` (scene padding fields + gh perms) |
| Bugs found (this triage) | 3 new (1 P1, 1 P2, 1 P3) — all in the touched `EnemySpawner.cs` WIP |
| Bugs fixed | 0 |
| Playtest sessions | 0 (still — see follow-up below) |

---

### Previous Action Items Follow-Up (from `retro-sprint-04-2026-07-10.md`)

| # | Action Item | Status | Notes |
|---|--------------|--------|-------|
| 1 | Fix BUG-ES-1 before further enemy-spawn integration work | Not Started | Still open, re-verified this triage; scoped as S5-C1 |
| 2 | Owner decision: fold enemy-spawn prototype into formal scope, or relocate to `prototypes/` | Decided | Sprint 5 renewal (`e600eb2`) formally adopted the prototype's direction (Option C) into scope — Track A of `sprint-05.md`. Not relocated to `prototypes/`; explicitly kept as production code under active refactor |
| 3 | Run first playtest since 2026-06-12 | Not Started | Still no session logged; latest file remains `playtest-2026-06-12-weekly-wrapup.md` |
| 4 | Write Skill Enhance vs `ActivateSkill` ADR | Not Started | No commit or doc found addressing this in the window checked |
| 5 | Add `ON_ENEMY_DEATH`/`ON_ROOM_CLEAR` to `EventID`, reconcile spawn drivers | Not Started | Scoped as S5-B1 / S5-C3, both Sprint 5 Day 1 / Day 4 per the daily plan |

**1 of 5 done (decided, not yet executed), 4 not started** — expected, since Sprint 5's execution
window hasn't opened. Worth watching whether item 3 (playtest) survives another sprint boundary
untouched — this is its second consecutive retro with zero progress.

---

### What Went Well

- **Plan renewal was handled correctly, not silently.** Rather than starting Sprint 5 execution on a
  stale kickoff draft, the owner re-synced the GDD/epic/ADR against actual code state and rewrote
  `sprint-05.md` same-day (`e600eb2`) before any Track A/B/C work began. This is the discipline Sprint
  4's retro asked for under item 2 — the ambiguous prototype status got resolved with a decision, not
  left to drift further.
- **No off-plan `.cs` work landed this window** — a reversal of the pattern flagged in the last 2
  retros (code appearing hours after a "no code" pivot). The only code touched (`EnemySpawner.cs`) is
  explicitly the carried WIP already named in `sprint-05.md`'s Carry-Over table (S5-D1), not new
  unplanned work.

---

### What Went Poorly

- **The carried WIP is still sitting uncommitted in the working tree.** `EnemySpawner.cs`'s padding
  fields are staged but not committed as of this triage — if this session ends without a commit, the
  work is one `git checkout`/accidental discard away from being lost, and the sprint's own S5-D1 task
  ("reapply `cb099ee` + stash... reconciled against S5-C1/C3") hasn't actually happened yet — S5-C1
  (BUG-ES-1's null-guard) doesn't exist in this diff, so the WIP was re-added without its companion
  safety fix.
- **The WIP inherits an un-flagged crash risk (BUG-ES-4)**, found only because this triage re-read the
  touched file line-by-line rather than trusting the commit message ("padding fields... WIP"). The
  `spawnPosition` list this WIP reads from has no null/empty guard, same shape of bug as BUG-ES-1 one
  line above it in the same method.
- **Playtest gap is now 2 consecutive retros with zero movement.** Sprint 4's retro named this
  explicitly as an action item with the blocker already cleared; nothing changed this window (though
  the window is only one day, this is worth carrying forward loudly into Sprint 5's actual execution).

---

### Carryover Analysis

| Task | Original Sprint | Times Carried | Reason | Action |
|------|----------------|---------------|--------|--------|
| BUG-06, BUG-05/07/08 (death loop) | Sprint 1 | 5 (S1→S2→S3→S4→S5) | Design-track dependency now resolved; Track B is the reason Sprint 5 exists | Sprint 5 Track B, Day 1-3 |
| BUG-ES-1/2/3 (spawn architecture) | Sprint 4 | 1 | Found last triage, not yet fixed | Sprint 5 Track C, Day 3-4 |
| BUG-ES-4/5/6 (NEW — found this triage, same file as ES-1) | Sprint 5 (new) | 0 | Surfaced by re-reading the carried WIP diff | Should land in the same S5-C1/S5-D1 pass, not carry further |
| S4-05 (`CancelInvoke` pairing), S4-06 (`TalentManager`→SO) | Sprint 2/3 | 3 | Deliberately pended twice already | Sprint 5 Should-Have (S5-D3/D4) — watch for a 4th carry if Must-Have work runs long |
| Skill Enhance vs `ActivateSkill` ADR | Sprint 3 | 2 | Still no commit/doc found | Unassigned this window — needs an explicit owner, not just a sprint slot |
| First playtest | — | 2 retros running | No blocker remains, simply hasn't happened | Flag again for Sprint 5 — recommend scheduling it as a fixed slot, not a "when convenient" task |

---

### Action Items for Next Iteration

| # | Action | Owner | Priority | Deadline |
|---|--------|-------|----------|----------|
| 1 | Commit or explicitly stash the `EnemySpawner.cs` WIP before Sprint 5 Day 1 starts — don't let it sit in the working tree across the sprint boundary | Owner (Kay) | High | Before 2026-07-14 |
| 2 | Land BUG-ES-1 and BUG-ES-4 together (same method, same shape of bug) when S5-C1 is picked up | gameplay-programmer | High | Sprint 5, S5-C1 |
| 3 | Decide on BUG-ES-6 (padding bounds clamp) — fix or document as known limitation — before calling S5-D1 done | gameplay-programmer | Medium | Sprint 5, S5-D1 |
| 4 | Schedule the playtest session as a fixed slot rather than a follow-up task — 2 retros running with no movement | Owner (Kay) | Medium | Sprint 5, before Day 3 |
| 5 | Assign an explicit owner to the Skill Enhance vs `ActivateSkill` ADR — it has carried 2 sprints with nobody named against it | Producer/Owner (Kay) | Medium | Sprint 5 |

---

### Process Improvements

- **Carried WIP should be committed (even behind a feature note) at the point it's reapplied, not left
  staged.** Staging without committing gives no `git log` trail if the wrap-up ritual needs to verify
  "was S5-D1 actually done" later — right now the only evidence is `git status`, which won't survive
  past this session.
- **When reapplying carried WIP, re-check it against bugs found on the same file since it was
  shelved** — BUG-ES-1 was found and scoped after this WIP was originally written; reapplying it
  without also reading the current triage report on that file is how BUG-ES-4 almost shipped unnoticed.

---

### Summary

Sprint 5 hasn't started its execution window yet, so this is a thin, mostly-process retro: the plan
renewal was handled well (a real fix to a pattern flagged twice before), but the one piece of code
touched this window is carried WIP that's still uncommitted and turned out to hide a second crash-risk
bug in the same method as the one already on the books. Nothing here blocks Sprint 5 Day 1, but the
WIP should not cross into the sprint week uncommitted, and S5-C1/S5-D1 should be treated as one
combined fix, not two.

---

### Reference Files

- Bug triage: `production/qa/bug-triage-2026-07-11.md`
- Sprint plan: `production/sprints/sprint-05.md`
- Daily plan tracker: `production/sprints/sprint-05-daily-plan.md`
- Prior retro: `production/retros/retro-sprint-04-2026-07-10.md`
- GDD: `design/gdd/enemy-spawn-system.md`
