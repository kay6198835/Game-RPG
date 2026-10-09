# Sprint 18 — 2026-10-12 to 2026-10-16

**Status: OPEN.** Opened 2026-10-09 (`pm-weekly-kickoff`, autonomous run — no owner present; the run
fired on Friday evening, so the sprint starts next Monday).
Branch `sprint-18`, created on the remote from the `sprint-17` tip (`d8962b8d`, "chore(kickoff): close
sprint 17 2026-10-09"). `gh` CLI unavailable (`gh: command not found`) — draft PR **not** auto-created;
run manually:

```bash
gh pr create --draft --base sprint-17 --head sprint-18 --title "Sprint 18"
```

**Review mode:** lean — producer feasibility gate skipped.
**Tracker:** `sprint-18-daily-plan.md`.
**Inputs:** `sprint-17.md` (Closure), `sprint-17-daily-plan.md` (wrap-up handoff),
`production/qa/bugs/BUG-096…BUG-102.md`. No milestone file and no risk register exist — risks are
tracked here.

> ⚠️ `.claude/docs/pm-routine-protocol.md` exists only on `origin/feature/update-bug-document`
> (`42a81260`), not on `sprint-17` / `sprint-18`. This run followed the version in that commit.
> Merge it into the sprint branch so routines can read it from the PM worktree.

---

## Sprint Goal

**Play the game once, then fix what blocks the loop.** The weekly playtest / smoke runs **first on
Monday, before any feature commit** (playtest done: no; 14 carries). Then land BUG-096 on the sprint
branch, the start/boss order, the two Inspector fields, and the vitals/death fixes.

---

## Capacity

- Total days: 5
- Buffer (20%): 1 day
- Available: 4 days

Planned-work velocity: Sprint 16 ≈0.01, Sprint 17 **0.05**. Off-plan owner output ≈2.6–3.1d/week.
Planned load is therefore kept at ≈1.3d Must and ≈1.0d Should. Retro action (Sprint 17): owner
feature work is planned as an **explicit story capped at 0.5d**, gated behind S18-01.

0.1d is reserved on **Saturday 2026-10-17** for the weekly playtest (`tests/smoke/critical-paths.md` — not present on `sprint-18`; use the `playtest-sheet.sh` sheet).

---

## Carryover from Previous Sprint

| Task | Times carried | Reason | New Estimate |
|------|---------------|--------|--------------|
| Play Mode smoke / playtest (S17-04) | **15th** | Never run ahead of feature work | 0.3d — Mon, first session |
| BUG-096 (S17-01) | 2nd | Partial on feature branch `221d54be` only | 0.1d |
| BUG-097 (S17-02) | 2nd | Not started | 0.05–0.2d |
| Editor block (S17-03) | 2nd (BUG-072 5th, BUG-095 4th) | Not started | 0.1d |
| BUG-092 residual (S17-05) | 4th | Not started | 0.05d |
| BUG-066 + BUG-070 (S17-06) | 9th / 7th | Not started | 0.15d |
| BUG-065 + BUG-086 (S17-07) | 9th / 4th | Not started | 0.2d |
| Process gate decision (S17-08) | 7th (TD-048 29th+) | Not started | 0.1d |
| BUG-071 residual (S17-09) | 5th | Same file as S18-06 | 0.15d |
| BUG-068 `:28` (S17-10) | 3rd | — | 0.05d |
| BUG-073 + BUG-090 (S17-11) | 5th | — | 0.15d |
| BUG-087 design note (S17-12) | 3rd | Blocked by S18-07 | 0.3d |
| Git LFS decision (S17-13) | 2nd | — | 0.1d |

---

## Tasks

### Must Have (Critical Path)

| ID | Task | Agent/Owner | Est. Days | Dependencies | Acceptance Criteria |
|----|------|-------------|-----------|--------------|---------------------|
| S18-01 | **Weekly playtest / Play Mode smoke — first task Monday (15th carry).** Fill `production/qa/playtests/playtest-2026-10-10.md` (or a fresh sheet) through the menu: `MainGamePlay` → New Game → `LoadRandomMap` + `GameplayUI`; per `tests/smoke/critical-paths.md` — not present on `sprint-18`; use the `playtest-sheet.sh` sheet | owner (Kay) | 0.3 | None | Sheet filled with PASS/FAIL per row, or a signed accepted-risk note here. **No feature commit before this** |
| S18-02 | **BUG-096 (S1)** — merge `221d54be` (start room on Champion confirm) into `sprint-18`, then add the zero-spawn branch in `RoomGeneraterController.LoadRoom()` so Rest / Shop / Buff rooms open their doors | owner (Kay) + gameplay-programmer | 0.1 | S18-01 | Every non-combat room opens on load; fix on `sprint-18` |
| S18-03 | **BUG-097 (S2)** — start/boss room order: reorder `Maze_Storage.asset` or select by `RoomType` (closes Bug #16) | owner (Kay) | 0.05–0.2 | None | Run starts in `StartRoom_Entrance`, ends in `BossRoom_ThroneArena` |
| S18-04 | **Editor block** — BUG-072 `layerMask` on `Lightning.prefab` + BUG-095 `Collider2D` on `Arrow.prefab` | owner (Kay) | 0.1 | None | Both serialized; visible in `git diff` |
| S18-05 | **BUG-092 residual** — `[SerializeField]` on `perTime` + `timeCount`; delete unused `duration` | gameplay-programmer | 0.05 | None | A DoT asset ticks N times |
| S18-06 | **BUG-066 + BUG-070** — guard `currentStats[statType]` in `VitalStatsBase.cs`; audit `Reborn()` seeding | ai-programmer | 0.15 | None | No `KeyNotFoundException`; missing key logged, not lethal |
| S18-07 | **BUG-065 + BUG-086** — stop movement on death; emit `ON_PLAYER_DEATH` once; keep `RealPlayerDataProvider` respawn working | gameplay-programmer | 0.2 | None | No slide; event once; Respawn still works |
| S18-08 | **Process gate decision** — Map/revert commits need one menu → dungeon load before push; TD-048 hook or explicit drop | producer / owner | 0.1 | None | Decision recorded in this file |

Must-Have total ≈ **1.05–1.2d** → plan **1.3d** (incl. Saturday playtest 0.1d).

### Should Have

| ID | Task | Agent/Owner | Est. Days | Dependencies | Acceptance Criteria |
|----|------|-------------|-----------|--------------|---------------------|
| S18-09 | **BUG-100 (S2)** — timed buff expiry strips every modifier (shared source `this` in `VitalStatsBase`) | gameplay-programmer | 0.2 | S18-06 (same file) | Item buff survives a timed-buff expiry |
| S18-10 | BUG-071 residual — keep a coroutine handle; stop HoT/DoT on death / disable | gameplay-programmer | 0.15 | S18-06 | DoT does not survive death or respawn |
| S18-11 | BUG-068 `:28` + **BUG-102** (confirm-panel `?.Invoke()` + `SetConfirm()` labels) | gameplay / ui-programmer | 0.1 | None | No NRE on pooled enemy; no NRE without `GameplayUI` |
| S18-12 | BUG-073 + BUG-090 — remove/repoint missing-script assets | owner (Editor) | 0.15 | None | No missing-script warnings |
| S18-13 | Git LFS decision for `.fbx` | owner / technical-director | 0.1 | None | Decision recorded |
| S18-14 | BUG-087 design note — `GameManager` + player `Reborn()` | game-designer / producer | 0.3 | S18-07 | Short design note / story |

Should-Have total ≈ **1.0d**. Must + Should ≈ 2.3d of 4.0d.

### Feature story (owner, capped)

| ID | Task | Agent/Owner | Est. Days | Dependencies | Acceptance Criteria |
|----|------|-------------|-----------|--------------|---------------------|
| S18-F1 | Start room / Champion select / shop — finish the in-flight feature work on `sprint-18` | owner (Kay) | **0.5 cap** | **S18-01** | Merged to `sprint-18`; covered by the Saturday playtest rows |

### Nice to Have

| ID | Task | Agent/Owner | Est. Days | Dependencies | Acceptance Criteria |
|----|------|-------------|-----------|--------------|---------------------|
| S18-N1 | TD-040 ADR — v1 vs v2 | technical-director | 0.3 | None | ADR under `docs/architecture/` |
| S18-N2 | BUG-084 decision — test location + `.asmdef` | technical-director | 0.3 | None | Decision recorded |
| S18-N3 | BUG-083 — `HoldTime` / `HoldRatio` | gameplay-programmer | 0.2 | None | Values update while held |
| S18-N4 | BUG-094 + `Entity.cs` double Idle `Enter()` | gameplay-programmer | 0.05 | None | No per-state log |
| S18-N5 | BUG-098 / BUG-099 / BUG-101 doc drift | PM (`/doc-sync`) | 0.2 | None | Docs match source |

---

## Risks

| Risk | Probability | Impact | Mitigation |
|------|------------|--------|------------|
| Smoke slips a 15th time | High | High | S18-01 is Monday's first session and gates S18-F1 |
| BUG-096 fix stays on a feature branch | Medium | High | S18-02 AC requires it on `sprint-18` |
| Must set loses to feature work an 8th time | High | Medium | Feature work is an explicit 0.5d story, gated |
| `VitalStatsBase.cs` touched by S18-05/06/09/10 at once | Medium | Medium | One session, in order 06 → 09 → 10 |
| Compile break with no pre-push check | Medium | High | S18-08 |
| PM protocol doc not on sprint branch | Known | Low | Merge `42a81260` content into `sprint-18` |
| No `gh` CLI | Known | Low | Manual PR command above |

## Dependencies on External Factors

- Owner Unity Editor time (S18-01..04, S18-12) — nothing can be verified without it.
- `gh` CLI install for automated PRs.

---

## Definition of Done for this Sprint

- [ ] S18-01 playtest sheet filled (or accepted-risk signed) — Monday
- [ ] S18-02, S18-03 on `sprint-18` (dungeon loop unblocked)
- [ ] S18-04 Inspector steps committed
- [ ] S18-05..S18-07 committed
- [ ] S18-08 decision recorded
- [ ] Saturday 2026-10-17 playtest sheet filled
- [ ] No S1 bugs open on the dungeon loop

## QA Plan

> ⚠️ **No QA Plan** (`production/qa/qa-plan-sprint-18.md` not found — 35th+ consecutive cycle).
> Autonomous run: option [B] recorded by default. S18-01 and the Saturday playtest are the de facto QA
> gate. Run `/qa-plan sprint` in an owner session.

## Notes

- Open bug-inbox notes (e.g. NOTE-20261009-2, NOTE-20261009-6) are **not** tasks; only bugs confirmed
  at Saturday triage are. They stay in the inbox for the next triage.
- `production/sprint-status.yaml` not updated (`.md`-only run).
