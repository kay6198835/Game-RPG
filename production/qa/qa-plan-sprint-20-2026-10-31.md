# QA Plan — Sprint 20

**Date**: 2026-10-31
**Scope**: Sprint 20 (2026-10-26 to 2026-10-30), `production/sprints/sprint-20.md`, 13 stories (S20-01 to S20-12, S20-F1)
**Config**: qa.level standard, team.size individual, review_mode lean
**Strategy**: `production/qa/qa-strategy-sprint-20-2026-10-31.md`

## Story Classification

| Story | Type | Automated Required | Manual Required | Blocker? |
|---|---|---|---|---|
| S20-01 Playtest / Play Mode smoke | Integration | No | Yes | Yes — only QA gate |
| S20-02 BUG-108 `[FormerlySerializedAs]` on `CharacterData` | Config/Data (S1) | No | Yes | Yes (S1) |
| S20-03 BUG-096 zero-spawn room branch | Logic (S1) | Yes — impossible (BUG-084) | Yes | Yes (S1, evidence gap) |
| S20-04 Weapon trio BUG-106/107/110 | Integration | Yes for BUG-107 guard — impossible | Yes | Yes |
| S20-05 BUG-097 start/boss order | Logic or Config/Data (fix route) | Only if code route — impossible | Yes | No |
| S20-06 BUG-105 / BUG-095 / BUG-072 | Config/Data | No | Yes | No |
| S20-07 Process gate decision | Decision | N/A | N/A | Out of scope |
| S20-08 BUG-065 + BUG-086 death | Logic + Integration | Yes — impossible | Yes | No |
| S20-09 BUG-092 + BUG-066/070 verification | Logic | Yes — impossible | Yes | No |
| S20-10 BUG-115 runtime stat copy | Logic | Yes — impossible | Yes | No |
| S20-11 BUG-104/103/102/068 | Logic + UI (BUG-102) | Yes for 103/068 — impossible | Yes; screenshot for BUG-102 | BUG-102 screenshot BLOCKING |
| S20-12 BUG-073/090/112 missing scripts | Config/Data | No | Yes | No |
| S20-F1 Land `feature/add-effect-component` | Integration | No (compile check) | Yes | Yes |

## Automated Test Requirements

Required at qa.level standard for S20-03, S20-04 (BUG-107), S20-05 (code route), S20-08, S20-09,
S20-10, S20-11 (BUG-103, BUG-068). **None can be written this cycle:** no `.asmdef` exists under
`Assets/` and `tests/EditMode`, `tests/PlayMode` sit outside `Assets/` (BUG-084). Expected paths once
unblocked: `Assets/Tests/EditMode/<System>/` per `.claude/rules/test-standards.md` after its correction.
This is an **evidence gap**, not a waiver: qa.level is standard. Manual walkthroughs below are
recorded as substitute evidence; the sign-off reports the gap.

## Manual QA Scope

| Session | Stories | Validate |
|---|---|---|
| 1 — Smoke (Saturday) | S20-01, S20-F1 | `tests/smoke/critical-paths.md` via `MainGamePlay` → New Game → `LoadRandomMap` + `GameplayUI`; Archer / Mage abilities |
| 2 — Dungeon loop | S20-02, S20-03, S20-05 | Knight `Stats` non-null; Start / Rest / Shop / Buff / Boss rooms open; run starts in `StartRoom_Entrance`, ends in `BossRoom_ThroneArena` (3 runs) |
| 3 — Weapons | S20-04, S20-06 | Enemies attack; Mage confirm does not throw; picked-up weapon attacks; sword hit = 55 + PhysicalDamage; arrow hit registers; Lightning damages |
| 4 — Vitals / death | S20-08, S20-09, S20-10 | No movement during death; `ON_PLAYER_DEATH` once; Respawn works; DoT ticks N times; `git status` clean on `Assets/SO/Stat/*.asset` after Play Mode — **after S20-F1 merge** |
| 5 — Hygiene | S20-11, S20-12 | No unguarded state logs; RecoveryTime gate; Confirm panel labels (screenshot); Console 0 Missing Script |

Estimated owner Editor time ≈ 1.2d.

## Out of Scope

- S20-07 — a process decision, not testable code.
- S20-D1…D7 — deferred in the sprint file.
- Open bugs not targeted by a sprint-20 story (BUG-087, 071, 072 residual, 083, 084, 100, 109, 113, 114) — remain open and are reported in the sign-off.

## Entry Criteria

1. Smoke check PASS or PASS WITH WARNINGS report at `production/qa/smoke-*.md` — **not met** (no report exists; status UNKNOWN).
2. Build stable: Editor compiles with 0 `error CS`, menu → dungeon loads without crash.
3. Must Have stories S20-01…S20-06 are `in-progress`, `review` or `done` — `production/sprint-status.yaml` is stale (sprint 14), so this is read from the sprint file.
4. Working tree clean or the evidence branch named per session (14 modified `.cs` on `feature/add-effect-component` at plan time).
5. Session 4 runs only after S20-F1 is merged (it rewrites `VitalStatsBase`).

## Exit Criteria

- Every in-scope story has a recorded result: PASS, PASS WITH NOTES, FAIL (bug filed in `production/qa/bugs/`), or BLOCKED with reason.
- BUG-102 screenshot retained under `production/qa/evidence/s20-11/`.
- Sign-off written to `production/qa/qa-signoff-sprint-20-2026-10-31.md`.

## Acceptance-Criteria Issues (author to confirm rewrites)

S20-07 and S20-09 have no testable outcome; S20-04 ("picked-up weapon is the one that attacks"),
S20-06 (BUG-095, BUG-072 have none), S20-08 ("No slide"), S20-11 are vague. Proposed rewrites are in
the strategy file.
