## Retrospective: Sprint 15
Period: 2026-09-21 -- 2026-09-25. Wrap-up run 2026-09-28 (`pm-weekly-wrapup`, autonomous, **late** — the
Saturday 2026-09-26 slot did not fire; the Monday kickoff `5e1d81f` had already closed Sprint 15 and
opened Sprint 16 before this run. This retro complements, and does not overwrite, the closure block in
`sprint-15.md`).

### Metrics

| Metric | Planned | Actual | Delta |
|--------|---------|--------|-------|
| Must-Have (S15-01..S15-09) | 9 tasks, ~1.05d | 1 cut by owner decision (S15-01 / BUG-063 accepted-deferred), 1 code-done (S15-02 / BUG-064-7, unverified), 1 partial (S15-08 / BUG-071), 6 not started | ~-7 |
| Should-Have (S15-10..S15-14) | 5 | 2 done (S15-10 code-done, S15-14 `/doc-sync`), 1 blocked (S15-12 by BUG-084), 2 not started | -3 |
| Nice-to-Have | 3 | S15-N1 (BUG-074) fixed off-plan; N2/N3 not done | -2 |
| Planned-work delivered | 1.05d Must | ≈0.6d across all tiers | — |
| Commits in window (`15242e6..5b035b7`) | — | 40 (12 docs/standup/kickoff) | — |
| `.cs` files changed | — | 78 (+1350/-975) | — |
| Bugs closed | — | 12: BUG-043, 074, 075, 076, 077, 078, 080, 081, 082, 085, 088, 091 (+ BUG-089 closed by design) | — |
| Bugs introduced | — | 6: BUG-088, 089, 090, 091, 092 (all during the week) + BUG-093/094/095 from the Monday commit `5b035b7` | — |
| Open after triage | — | 14 open, 3 partial, 1 accepted (see `bug-triage-2026-09-28.md`) | — |

### Velocity Trend

| Sprint | Must-Have planned | Completed | Rate |
|--------|------|------|------|
| 12 | 1.0d | 0 clean | 0% |
| 13 | 0.75d | 0/6 | 0% |
| 14 | 0.9d | 0/9 (1 incidental) | 0% |
| 15 | 1.05d | 1/9 code-done + 1 partial + 1 cut by decision | ~15% |

Fifth sprint at or near zero on its own plan, but the first sprint in five where **the codebase health
improved net**: 12 bugs closed against 9 opened, one of which (BUG-092) is already half closed.

### What Went Well
- **BUG-088 compile break fixed within a day**, restoring Play Mode capability.
- **Abilities v2 damage path restored** (BUG-075, BUG-072 code) — the player's only ability path deals
  damage again by static analysis.
- **ADR-0005 unified character contract** (Amendments 1-3, ~20 commits): shared bases for input,
  movement, damage receiver, stats, vitals, weapon holder, ability holder. Enemies now use the player's
  `Weapon` and can cast Abilities v2. `EntityAttack` deleted (BUG-043). One `INegativeReceiver`
  implementer project-wide (BUG-081). BUG-066 and BUG-070 collapsed to one site.
- **Owner made real decisions**: BUG-063 accepted-deferred, BUG-089 closed by design with a reopen
  trigger, BUG-078 retracted as by-design. Three long-carried items stopped being noise.
- `/doc-sync` ran twice and `CLAUDE.md` matches source at `c0067f4`.
- BUG-064 item 7 finally got real DI (`[Inject] Construct` + inject-on-equip) — 6 carries.

### What Went Poorly
- **The opening block (S15-01..04) missed its own hard gate every day.** Only S15-01 moved, and it
  moved by being *cut*, not fixed. The "no feature merge until the opening block lands" rule was
  broken on day 1 and never enforced.
- **Two compile breaks committed in four days** (BUG-088, BUG-092). Nothing between a local save and a
  push compiles the code (TD-048).
- **The ranged weapon (`5b035b7`) landed non-functional**: its projectile prefab has no script, collider
  or rigidbody (BUG-095), aiming rotates the whole character and the camera (BUG-094), and the same
  commit commented out projectile-ability direction (BUG-093). All three are visible in the first
  seconds of Play Mode.
- **Play Mode session: 9th sprint without one.** S15-06 asked for a decision, not a session; neither
  was recorded.
- S15-07 (review gate) not decided while a ~20-commit architectural refactor landed mid-sprint.
- The Saturday wrap-up did not fire on time; the kickoff had to close the sprint without a triage or
  retro, and this run is out of order.

### Blockers
| Blocker | Duration | Resolution | Prevention |
|---|---|---|---|
| Project did not compile (BUG-088, then BUG-092) | ~1 day + ~3 days | Fixed `723fab1`/`2a83469`, `5b035b7` | Pre-push compile check (TD-048 / S16-08) |
| No Play Mode verification | 9 sprints | None | S16-07 accepts a signed accepted-risk note as a valid close |
| No `.asmdef` (BUG-084) | Ongoing | None | Needs a decision (S16-N1) |
| No `gh` CLI in scheduled runs | Ongoing | Manual PR command | Structural |

### Estimation Accuracy
The sub-0.2d fixes were not under-estimated; they were not scheduled against the owner's real work.
The one opening-block item that *did* land (BUG-064-7) landed because the owner's own feature (ranged
weapon) needed it. **Lesson: bugs that sit on the path of the owner's next feature get fixed; bugs that
don't, don't.** Sprint 16 already applies this by reserving ~1.6d for the owner stream.

### Carryover Analysis
| Task | Times carried | Action (Sprint 16 ID) |
|---|---|---|
| BUG-066+070 | 6th / 4th | S16-02 |
| BUG-065 | 6th | S16-03 |
| BUG-064 item 7 | 6th | S16-05 — verify only, but **needs BUG-094 + BUG-095 first** |
| BUG-071 residual | 2nd | S16-09 |
| BUG-072 Inspector step | 2nd | S16-06 |
| BUG-073 + BUG-090 | 2nd | S16-10 |
| Play Mode smoke | 9th | S16-07 (do it or sign it) |
| Review-gate decision | 4th | S16-12 |
| Pre-push hook / TD-048 | 26th+ | S16-08 |
| TD-040 ADR | 3rd | S16-13 (re-scoped by ADR-0005) |
| First EditMode test | 31st+ | Blocked by BUG-084 (S16-N1) |
| QA plan | 33rd+ | Owner decision |

### Technical Debt
- 35 bug files on disk; 14 open, 3 partial, 1 accepted.
- `tests/` still `.gitkeep`-only; 0 `.asmdef`.
- Commented-out code now changes behaviour in three places (`SpawnEffectBase.cs:19-22`, `Weapon.cs:7,70`,
  `AbilityInstance.Exit()`).
- `RangeWeaponStats.projectileCount` duplicates `RangeAttackSO.ProjectileCount` and is never read.
- `PlayerTest.prefab` now serializes `AbilityHolder.currentAbility.AbilityContext` (runtime state).
- v1 ability links removed from the weapon path without an ADR — TD-040 is being decided in code.

### Previous Action Items Follow-Up (Sprint 14 retro)
| # | Item | Status |
|---|---|---|
| 1 | First commits BUG-063, 064-7, 065, 066+070, 073 | Partial — 063 cut by decision, 064-7 code done; 065, 066+070, 073 not done |
| 2 | Record Play Mode as accepted risk | Not done |
| 3 | Decide review gate | Not done |
| 4 | TD-040 ADR before more v2 work | Not done (ADR-0005 landed instead; partly overtakes it) |
| 5 | One Editor session to verify 067/069/072/073 | Not done |
| 6 | Repo policy for binary batches | Not done |
| 7 | `/doc-sync` | ✅ Done (`6ca82f3`) |

1 of 7 done, 1 partial.

### Action Items for Sprint 16
| # | Action | Owner | Priority |
|---|---|---|---|
| 1 | Fix BUG-094 + BUG-095 **before** S16-05; otherwise the ranged-weapon verification fails for a known reason | gameplay-programmer / owner | Blocking |
| 2 | First commits: BUG-093 (S16-15) and BUG-092 residual (S16-01), ≤0.05d each | gameplay-programmer | High |
| 3 | Open the Editor and confirm a clean compile before every push until TD-048 exists (S16-08) | Owner (Kay) | High |
| 4 | Stop using comment-out as a change mechanism — delete or restore | Owner / all | Medium |
| 5 | Run the S16-07 smoke, or sign the accepted-risk note — a 10th silent carry is not an option | Owner (Kay) | High |
| 6 | Fix the wrap-up schedule so Saturday runs before Sunday kickoff | Owner | Low |

### Summary
Sprint 15 again missed its own plan (~15% of Must-Have), but it was the first sprint in five where the
project got measurably healthier: a major architecture refactor landed, twelve bugs closed, and the owner
converted three long-running items into explicit decisions. The recurring cost is verification: two
compile breaks and a ranged weapon that cannot work were committed because nothing between an edit and a
push runs the game. Sprint 16's plan is right to centre on a single Play Mode session; it needs BUG-094
and BUG-095 added in front of it.
