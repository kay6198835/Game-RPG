## Retrospective: Sprint 14
Period: 2026-09-14 -- 2026-09-18, wrap-up run 2026-09-19 (Saturday `pm-weekly-wrapup`, autonomous)

### Metrics

| Metric | Planned | Actual | Delta |
|--------|---------|--------|-------|
| Must-Have (S14-01..S14-09) | 9 tasks, ~0.9d | 0 delivered as planned; S14-01 (BUG-067) fixed incidentally, unverified | -9 |
| Should-Have (S14-10..S14-14) | 5 | 0 planned; S14-14 (BUG-069) fixed incidentally, unverified | -5 |
| Nice-to-Have | 2 | 0 (blocked on S14-07) | -2 |
| Must-Have completion (verified) | - | 0% formal / ~0.05d incidental | - |
| Non-standup commits | - | 20 | - |
| `.cs` files changed | - | 46 (+570/-438) | - |
| New bugs | - | 4 (BUG-071..074) | +4 |
| Open bugs after triage | - | 13 | - |

### Velocity Trend

| Sprint | Must-Have Planned | Completed | Rate |
|--------|------|------|------|
| 11 | 0.75d | ~2/5 | ~40% |
| 12 | 1.0d | 0 clean | 0% verified |
| 13 | 0.75d | 0/6 | 0% |
| 14 | 0.9d | 0/9 (1 incidental) | 0% |

Four consecutive sprints at ~0% verified on Must-Have. Real output each sprint = unplanned Abilities v2 / Paladin work.

### What Went Well
- Paladin kit (Avatar of Light, Blessed Slash, Blessing, Consecrate) reached feature-complete: spawn-effect refactor into `SpawnEffectBase` / `SpawnProjectileEffect` / `SpawnSummonEffect`, controllers, VFX.
- `PlayerInputHandle` skill path rewritten around `TryDoAbility` with a cooldown check - fixes BUG-069 and makes BUG-068 dormant.
- BUG-067 heal/damage swap gone (interface renamed `Recovery`/`Reduction`).
- Daily standups verified the gate against source every day; the failure is fully documented.

### What Went Poorly
- **Hard gate (checked first each standup) held 0/5 days.** A checklist that only *reports* cannot block work. S14-02..S14-06 are byte-identical to sprint open (BUG-063 now 29th+ carry).
- All Must-Have process items again untouched: S14-07 Play Mode session (8th sprint), S14-08 review-gate decision, S14-09 pre-push hook (25th+), S14-10/11 sign-offs, S14-12 ADR.
- 4 new S2/S3 bugs from unreviewed work; BUG-073 (deleted scripts still referenced by live assets) means the original v2 proof ability `ShootSpirit` is broken.
- Branch `feature/fix-player-control` contains no player-control fix; naming no longer matches content.
- ~230 binary files in one merge; no size/LFS decision.
- A feature (`5b74575`) was added then reverted twice in one window - unplanned churn.

### Blockers
| Blocker | Duration | Resolution | Prevention |
|---|---|---|---|
| Work happens on a parallel branch outside the sprint plan | Whole sprint | None | Sprint plan must be the merge gate, or work must be planned to match the branch |
| No Unity CLI / `gh` | Ongoing | None | Structural |
| No owner Play Mode session | 8 sprints | None | Record as accepted risk |

### Estimation Accuracy
Same result as Sprint 11-13: sub-0.2d isolated fixes received 0% of scheduled time; large feature work absorbed all capacity. The estimates are not the problem; the allocation mechanism is.

### Carryover Analysis
| Task | Times carried | Action |
|---|---|---|
| BUG-063 | 29th+ | Fix as literal first commit or record explicit accepted risk - owner decision |
| BUG-064 item 7 | 5th | Sprint 15 opening block |
| BUG-065 | 5th | Sprint 15 opening block |
| BUG-066 + BUG-070 | 5th / 3rd | Sprint 15 opening block |
| BUG-073 (new) | 0 | Sprint 15 opening block (assets) |
| S14-07 Play Mode session | 8th | **Stop scheduling as a task; record accepted-risk decision** |
| S14-08 review gate | 3rd | Decide at Sprint 15 kickoff |
| S14-09 pre-push hook | 25th+ | Placeholder only |
| S14-10 / S14-11 | 19th+ / 18th+ | One owner sign-off pass |
| S14-12 TD-040 ADR | 2nd | Required before more v2 work |
| S14-13 first EditMode test | 30th+ | Pair with BUG-066/070 |
| BUG-069, BUG-067 | - | Verify only, need Play Mode |
| QA plan | 31st+ | Owner decision |

### Technical Debt
- Bug files: 14 on disk (13 open).
- `tests/EditMode/` still `.gitkeep`-only.
- Doc drift: `CLAUDE.md` still lists the deleted v2 effects (`DamageInFrontEffect`, `LungeForwardEffect`, `ShootObjectEffect`, `SpiritOrbProjectile`, ...) and `SkillState` (now `AbilityState`). Needs `/doc-sync`.
- Stale log strings ("ShootSpiritOrbEffect") in `SpawnEffectBase`; unused `using Unity.Mathematics` in `PlayerInputHandle.cs`; skill input hardcoded to `AbilitySlot.Utility`.

### Previous Action Items Follow-Up (Sprint 13 retro)
| # | Item | Status |
|---|---|---|
| 1 | Fix BUG-067 + BUG-068 first | Partial - BUG-067 fixed incidentally; BUG-068 dormant, not fixed |
| 2 | BUG-063/064-7/065/066 as isolated block | Not started (4th time) |
| 3 | Owner Play Mode session | Not started |
| 4 | Review gate decision for off-plan work | Not started |
| 5 | BUG-066 + BUG-070 + first EditMode test | Not started |

1 of 5 (partial).

### Action Items for Sprint 15
| # | Action | Owner | Priority |
|---|---|---|---|
| 1 | First commits: BUG-063, 064-7, 065, 066+070, 073 (~0.6d). No other diff merged until they exist. Enforce via a merge precondition, not a report | gameplay-programmer / ai-programmer | Blocking |
| 2 | Record S14-07 as accepted risk; stop re-scheduling | Owner (Kay) | Blocking |
| 3 | Decide S14-08: same-day `/code-review` for multi-file changes on Abilities v2 | producer | High |
| 4 | TD-040 ADR before any further v2 work | technical-director | High |
| 5 | One Editor session to verify BUG-067, 069, 072, 073 | Owner | High |
| 6 | Repo policy for large binary batches (LFS or not) | Owner / lead-programmer | Medium |
| 7 | Run `/doc-sync` after BUG-073 resolution | producer | Medium |

### Summary
Sprint 14 replaced a schedule note with a hard gate and got the same outcome: zero of the isolated fixes landed while a full Paladin ability kit did. Two bugs were fixed as side effects, four were introduced, and one (BUG-073) breaks a previously working ability through unswept asset references. The gate failed because it only observed. Sprint 15 needs an enforcing mechanism (merge precondition) or an explicit accepted-risk decision, not another checklist.
