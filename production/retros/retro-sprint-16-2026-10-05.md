## Retrospective: Sprint 16
Period: 2026-09-28 -- 2026-10-02 (+ weekend commits 10-02 evening and 10-04). Wrap-up run 2026-10-05
(`pm-weekly-wrapup`, autonomous, **late** — the Saturday 2026-10-03 slot did not fire, second sprint
running). HEAD `ac13ee4`.

### Metrics

| Metric | Planned | Actual | Delta |
|--------|---------|--------|-------|
| Must-Have (10 items incl. S16-15, S16-16) | ≈1.15d | 1 code-done (S16-15 / BUG-093, 0.05d), 1 code-done-unverified carried from S15 (S16-05), 8 not started | -8 |
| Should-Have (S16-10..14) | 0.9d | 0 | -5 |
| Nice-to-Have | 0.5d | 0 | -2 |
| Planned-work delivered | 1.15d Must | ≈0.05d (+0.05 S16-05 pre-done) | — |
| Commits in window (`5d2dc8a..ac13ee4`) | — | 22 (3 chore) | — |
| `.cs` files changed | — | 26 (+419/-181) | — |
| Bugs closed (code) | — | BUG-093, BUG-094, Bug #12/TD-023, Bug #13 (all Play Mode-unverified) | — |
| Bugs introduced / surfaced | — | BUG-095 (partial), BUG-096 (S1), BUG-097 (S2) | — |
| Play Mode sessions logged | 1 | 0 | -1 |

### Velocity Trend

| Sprint | Must-Have planned | Completed | Rate |
|--------|------|------|------|
| 13 | 0.75d | 0/6 | 0% |
| 14 | 0.9d | 0/9 | 0% |
| 15 | 1.05d | 1/9 + partial | ~15% |
| 16 | 1.15d | 1/10 (0.05d) | ~5% |

Sixth sprint near zero on its own plan. Planned velocity 0.05d / 4.0d ≈ **0.01**. Off-plan output ≈3.5d+.

### What Went Well
- **Shared projectile architecture** (`b7a0af5`, `7c637c0`): one `ProjectileBody` + `IProjectilePayload`
  for weapons and v2 abilities; BUG-093/094 fixed; `Pool.Release()` re-parent bug fixed in passing.
- **Map content**: 20-room set from Start to Boss with English names; minimap tracking re-enabled; player
  teleported into the start room (Bug #13).
- **`LevelManager` singleton removed** and injected through VContainer — Bug #12 / TD-023 closed in code
  after many sprints.
- **Four ability slots on keys 1-4** — the Paladin kit is now reachable from input.
- Combo-attack and Paladin 8-direction animation polish landed.

### What Went Poorly
- **Must-Have untouched for the whole week.** Four consecutive standups reported zero movement; the
  sprint goal ("make the project safe to play-test, then play-test it") was not attempted.
- **A same-day fix was reverted unnoticed** (`40d2c79` → `ac13ee4`, BUG-096). Together with BUG-097 the
  first room of every run is expected to be sealed — exactly what a 5-minute Play Mode run would show.
- **Play Mode smoke: 10th carry.** The "do it or sign it" rule was not honoured; no accepted-risk note.
- Reserved bucket (1.6d) overrun by ≈2d; asset work (≈66 MB `.fbx`, no LFS) displaced code Must-Haves.
- The Saturday wrap-up did not fire again; this retro runs on Monday.

### Blockers
| Blocker | Duration | Resolution | Prevention |
|---|---|---|---|
| No Play Mode verification | 10 sprints | None | Make the smoke the **first** Sprint 17 task, before any feature commit |
| No review / compile gate | Ongoing | None | S16-08 (TD-048) + S16-12 decision |
| Saturday scheduled run not firing | 2 sprints | Manual late run | Check the scheduled task trigger |
| No `gh` CLI | Ongoing | Manual PR command | Structural |

### Estimation Accuracy
Estimates are not the problem: every Must item is ≤0.15d and the total fits in a single day. The gap is
allocation — Must work never got a protected slot.

### Carryover Analysis
| Task | Times carried | Reason | Action |
|---|---|---|---|
| Play Mode smoke | 10 | Never scheduled ahead of features | Sprint 17 day 1, first task; or signed accepted-risk |
| BUG-065 / BUG-066+070 | 7 / 7 / 5 | Feature work first | Bundle with BUG-086 as one 0.4d block |
| BUG-092 residual | 2 | — | 2-line change |
| BUG-072 / BUG-095 Editor steps | 3 / 2 | Owner Editor time | Same session as the smoke |
| Pre-push compile check (TD-048) | 27+ | — | Decide or drop explicitly |
| Review-gate decision (S16-12) | 5 | — | BUG-096 is the case for it |

### Action Items for Sprint 17
| # | Action | Owner | Priority |
|---|---|---|---|
| 1 | Decide BUG-096 (restore `40d2c79` or record the intended rule) and reorder `Maze_Storage` (BUG-097) | Kay | P0 |
| 2 | First working session of the sprint = Play Mode smoke, no feature commit before it | Kay | P0 |
| 3 | Protected Must-Have block: no feature/asset commit until BUG-092 residual, BUG-066/070, BUG-065/086 land | Kay | P1 |
| 4 | Git LFS decision for `.fbx` before more source art lands | Kay | P1 |
| 5 | `/doc-sync`: input 1-4, `LevelManager` DI, runtime Player spawn, `ProjectileBody` | PM | P2 |

### Process Improvement
**Rule, not goal:** a commit that touches `Assets/Script/Map/` or reverts another commit must be
followed by one Play Mode load of `LoadRandomMap` before push.

### Summary
High off-plan throughput, near-zero planned throughput, and the first sprint where the lack of Play
Mode verification produced an S1 regression (BUG-096) by reverting a fix. Sprint 17 must start with the
smoke and the two map bugs.
