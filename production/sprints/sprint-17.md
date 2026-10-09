# Sprint 17 — 2026-10-05 to 2026-10-09

**Status: CLOSED — SLIPPED (2026-10-09, `pm-weekly-kickoff`, autonomous).**

## Closure — 2026-10-09 (weekly kickoff, autonomous)

- **Must-Have:** 0 / 8 done. S17-01 (BUG-096) PARTIAL — start room opens on Champion confirm in
  `221d54be`, which is on feature branches only, not on `sprint-17`; no zero-spawn branch for
  Rest/Shop/Buff rooms.
- **Should-Have:** 0.2d / 0.95d (S17-14 `/doc-sync` only).
- **Planned velocity:** 0.2d / 4d = **0.05**. Off-plan owner output ≈2.6–3.1d (UIFlow real-game wiring,
  shop, start room / Champion select / confirm panel, CCGS tooling upgrade).
- **Evidence tier:** `COMPILED` + `LOG`, no `RUNTIME` — `playtest-2026-10-10.md` unfilled;
  **playtest done: no**.
- **Carry-over to Sprint 18:** S17-01 … S17-13. New confirmed bug at triage: BUG-102 (S3).
- **Recurring slippage:** Play Mode smoke carried for the 14th time; BUG-066+070 and BUG-065 for the
  9th time; BUG-092 residual and BUG-086 for the 4th time.
- Full trail: `sprint-17-daily-plan.md` (Daily Log, wrap-up entry).

*Original header:* **Status: OPEN.** Opened 2026-10-05 (`pm-weekly-kickoff`, autonomous run — no owner present; the run
fired on Monday, so the sprint starts today).
Branch `sprint-17`, created from `sprint-16` tip (`96c65fa`, "chore(wrapup): weekly wrap-up 2026-10-05").
`gh` CLI unavailable (`gh: command not found`) — draft PR **not** auto-created; run manually:

```bash
gh pr create --draft --base sprint-16 --head sprint-17 --title "Sprint 17"
```

**Review mode:** lean — producer feasibility gate (PR-SPRINT) skipped.
**Tracker:** `sprint-17-daily-plan.md`.
**Inputs:** `sprint-16.md` (Closure), `production/qa/bug-triage-2026-10-05.md`,
`production/retros/retro-sprint-16-2026-10-05.md`. No milestone file exists (`production/milestones/`
holds only `.gitkeep`); no risk register exists — risks are tracked in this file.

---

## Sprint Goal

**Unlock the dungeon loop and play it once.** Fix the two map regressions that seal the first room of
every run (BUG-096, BUG-097), then run the Play Mode smoke **before any feature commit** — the first
verified play session in 11 sprints — and land the small death/vitals fixes behind it.

---

## Capacity

- Total days: 5
- Buffer (20%): 1 day
- Available: 4 days

Must-Have load ≈ **1.25d**. Planned-work velocity has been ≈0–15% for four sprints while off-plan
output ran at ≈3.5d/week. The Must set therefore stays small, and is **ordered** rather than
parallel: Mon = map bugs + smoke. Remaining capacity is reserved for the owner's feature stream,
**after** the Mon block (retro action items 2 and 3).

---

## Carryover from Previous Sprint

| Task | Times carried | Reason | New Estimate |
|------|---------------|--------|--------------|
| Play Mode smoke (S16-07) | **11th** | Never scheduled ahead of feature work | 0.3d — Mon, first session |
| BUG-092 residual (S16-01) | 3rd | Not started | 0.05d |
| BUG-066 + BUG-070 (S16-02) | 8th / 6th | Not started; `e8d117e` added 4 more reads of the raw indexer | 0.15d |
| BUG-065 (S16-03) | 8th | Not started | 0.1d |
| BUG-086 (S16-04) | 3rd | Not started | 0.1d |
| BUG-064 item 7 verify (S16-05) | 7th | Code done; needs BUG-095 collider + smoke | 0.05d (inside smoke) |
| BUG-072 `layerMask` (S16-06) | 4th | Owner Inspector step | 0.05d |
| BUG-095 `Arrow.prefab` collider (S16-16) | 3rd | Owner Inspector step | 0.05d |
| TD-048 pre-push compile check (S16-08) | 28th+ | — | 0.1d — decide or drop explicitly |
| BUG-071 residual (S16-09) | 4th | Same file as S16-02 | 0.15d |
| BUG-073 + BUG-090 assets (S16-10) | 4th | Cleanup | 0.15d |
| BUG-068 `:28` (S16-11) | 2nd | — | 0.05d |
| Review-gate decision (S16-12) | **6th** | BUG-096 is now the concrete case for it | 0.1d |
| TD-040 ADR v1 vs v2 (S16-13) | 4th | `5b035b7` already trimmed v1 from weapons | 0.3d |
| BUG-087 design (S16-14) | 2nd | Needs BUG-086 first | 0.3d |
| BUG-084 decision (S16-N1) | — | Blocks every test story | 0.3d |
| BUG-083 (S16-N2) | — | Dormant | 0.2d |
| BUG-063 | — | **Not carried** — accepted/deferred to demo prep (owner, 2026-09-22) | — |

---

## Tasks

### Must Have (Critical Path)

| ID | Task | Agent/Owner | Est. Days | Dependencies | Acceptance Criteria |
|----|------|-------------|-----------|--------------|---------------------|
| S17-01 | **BUG-096 (S1)** — owner decision on the `ac13ee4` revert, then restore a `spawnPositions.Count == 0` branch in `RoomGeneraterController.LoadRoom()` that builds the grid and opens doors (or record the intended non-combat door rule) | owner (Kay) + gameplay-programmer | 0.05 | None | Start / Rest / Shop / Buff / Boss rooms open their doors on load |
| S17-02 | **BUG-097 (S2)** — start cell gets Boss room: reorder `Maze_Storage.asset` (stop-gap) or select start/end by `RoomType` (closes Bug #16) | owner (Kay) | 0.05–0.2 | None | Run starts in `StartRoom_Entrance`, ends in `BossRoom_ThroneArena` |
| S17-03 | **Editor block** — BUG-072 `layerMask` on `Lightning.prefab` + BUG-095 `Collider2D` on `Arrow.prefab` | owner (Kay) | 0.1 | None | Both serialized; visible in `git diff` |
| S17-04 | **Play Mode smoke (11th carry — do it or sign it)** — one `LoadRandomMap` run: first room opens, 4 Paladin abilities (keys 1-4), melee, ranged weapon (closes BUG-064-7 verify), enemy weapon attack, BUG-093 aim direction, Bug #13 teleport, player death. Log in `production/qa/playtests/` | owner (Kay) | 0.3 | S17-01..03 | Written result per item, or a signed accepted-risk note in this file. **No feature commit before this** |
| S17-05 | **BUG-092 residual** — `[SerializeField]` `perTime` + `timeCount` on `RecoveryReductionPerTimeForDuration`; delete unused `duration` | gameplay-programmer | 0.05 | None | Fields in Inspector; a DoT asset ticks N times |
| S17-06 | **BUG-066 + BUG-070** — guard `currentStats[statType]` in `VitalStatsBase.cs` (all raw reads incl. `UpdateStatField()`); audit `Reborn()` seeding so a missing HP key is not read as death | ai-programmer | 0.15 | None | No `KeyNotFoundException`; missing key logged, not lethal |
| S17-07 | **BUG-065 + BUG-086** — `PlayerDeathState.Enter()` stops `PlayerMovement`; `LogicUpdate()` consumes `Status` so `ON_PLAYER_DEATH` fires once | gameplay-programmer | 0.2 | None | No slide on death; event emitted exactly once |
| S17-08 | **Process gate decision (S16-08 + S16-12 merged)** — write one rule: Map/revert commits need one `LoadRandomMap` load before push (retro rule), plus TD-048 hook or explicit drop | producer / owner | 0.1 | None | Decision recorded in this file |

Must-Have total ≈ **1.0–1.15d** (S17-02 range) + ≈0.1d slack → plan **1.25d**.

### Should Have

| ID | Task | Agent/Owner | Est. Days | Dependencies | Acceptance Criteria |
|----|------|-------------|-----------|--------------|---------------------|
| S17-09 | BUG-071 residual — keep a coroutine handle; stop HoT/DoT on death / disable | gameplay-programmer | 0.15 | S17-06 (same file) | DoT does not survive death or pooled respawn |
| S17-10 | BUG-068 `:28` — null-guard `CurrentActivationType` | gameplay-programmer | 0.05 | None | No NRE on freshly spawned enemy |
| S17-11 | BUG-073 + BUG-090 — remove/repoint missing-script assets + dead bullet / `SpawnBat` / `SpawnCrab` assets | owner (Editor) | 0.15 | None | No missing-script warnings |
| S17-12 | BUG-087 design — `GameManager` + `ON_PLAYER_DEATH` subscriber + player `Reborn()` (design note only) | game-designer / producer | 0.3 | S17-07 | Short design note / story |
| S17-13 | Git LFS decision for `.fbx` (pack 137.8 MiB, 12 `.fbx` last week) | owner / technical-director | 0.1 | None | Decision recorded; `.gitattributes` plan |
| S17-14 | `/doc-sync` — input keys 1-4, `LevelManager` DI (concrete type), runtime Player spawn, `ProjectileBody` / `IProjectilePayload` | PM | 0.2 | None | CLAUDE.md matches HEAD |

Should-Have total ≈ 0.95d. Must + Should ≈ 2.2d of 4.0d.

### Nice to Have

| ID | Task | Agent/Owner | Est. Days | Dependencies | Acceptance Criteria |
|----|------|-------------|-----------|--------------|---------------------|
| S17-N1 | TD-040 ADR — v1 vs v2 (re-scoped: v1 already trimmed from weapons in `5b035b7`) | technical-director | 0.3 | None | ADR under `docs/architecture/` |
| S17-N2 | BUG-084 decision — test location + runtime/test `.asmdef` | technical-director | 0.3 | None | Decision recorded |
| S17-N3 | BUG-083 — `HoldTime` / `HoldRatio` always `0f` | gameplay-programmer | 0.2 | None | Values update while held |
| S17-N4 | Remove `Debug.Log("Enter State: " …)` in `PlayerState.Enter()`; fix `Entity.cs:34-43` double Idle `Enter()` | gameplay-programmer | 0.05 | None | No per-state log; Idle entered once on spawn |

### Reserved

≈1.6d for owner-driven feature work — **opens only after S17-04 is logged or signed**. Record it in
the tracker as it lands so velocity reflects reality.

---

## Risks

| Risk | Probability | Impact | Mitigation |
|------|------------|--------|------------|
| Smoke slips an 11th time | High | High | S17-04 is Monday's first session; signed accepted-risk note is a valid close, a silent carry is not |
| Another fix silently reverted (BUG-096 pattern) | Medium | High | S17-08 rule: Map/revert commit ⇒ one Play Mode load before push |
| Must set loses to feature/asset work a 7th time | High | Medium | Reserved bucket gated behind S17-04 |
| Repo bloat from `.fbx` without LFS | Medium | Medium | S17-13 decision before more source art lands |
| Compile break with no pre-push check (3 in 3 weeks) | Medium | High | S17-08 |
| Saturday wrap-up not firing (2 sprints) | Known | Low | Owner checks the scheduled task trigger |
| No `gh` CLI | Known | Low | Manual PR command above |

## Dependencies on External Factors

- Owner Unity Editor time (S17-01..04, S17-11) — nothing can be verified without it.
- `gh` CLI install for automated PRs.

---

## Definition of Done for this Sprint

- [ ] S17-01, S17-02 committed (map loop unblocked)
- [ ] S17-03 Inspector steps committed
- [ ] S17-04 smoke logged in `production/qa/playtests/` or accepted-risk signed here
- [ ] S17-05..S17-07 committed
- [ ] S17-08 decision recorded
- [ ] No S1 bugs open on the dungeon loop
- [ ] QA plan gate resolved (below)

## QA Plan

> ⚠️ **No QA Plan**: This sprint was started without a QA plan
> (`production/qa/qa-plan-sprint-17.md` not found — 34th+ consecutive cycle). Autonomous run: option
> [B] recorded by default. S17-04 is the de facto QA gate. Run `/qa-plan sprint` before the last story
> is implemented. The Production → Polish gate requires a QA sign-off report, which requires a QA plan.

## Notes

- `production/sprint-status.yaml` is **not** updated by this run (still describes Sprint 14). The
  kickoff task may only write `.md` files; regenerate it with `/sprint-plan update` in an owner session.
- **Scope check:** no epic-scoped stories this sprint; `/scope-check` not required.
